#region License
/* **********************************************************************************
 * Copyright (c) Roman Ivantsov
 * This source code is subject to terms and conditions of the MIT License
 * for Irony. A copy of the license can be found in the License.txt file
 * at the root of this distribution.
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;

namespace Irony.Parsing
{
    [Flags]
    public enum ParseOptions
    {
        Reserved = 0x01,
        AnalyzeCode = 0x10,   //run code analysis; effective only in Module mode
    }

    public enum ParseMode
    {
        File,       //default, continuous input file
        VsLineScan,   // line-by-line scanning in VS integration for syntax highlighting
        CommandLine, //line-by-line from console
    }

    public enum ParserStatus
    {
        Init, //initial state
        Parsing,
        Previewing, //previewing tokens
        Recovering, //recovering from error
        Accepted,
        AcceptedPartial,
        Error,
    }

    // The purpose of this class is to provide a container for information shared
    // between parser, scanner and token filters.
    public partial class ParsingContext
    {
        public readonly Parser Parser;
        public readonly LanguageData Language;

        //Parser settings
        public ParseOptions Options;
        public bool TracingEnabled;
        public ParseMode Mode = ParseMode.File;
        public int MaxErrors = 20; //maximum error count to report
        public CultureInfo Culture; //defaults to Grammar.DefaultCulture, might be changed by app code

        #region properties and fields
        //Parser fields
        public ParseTree CurrentParseTree { get; internal set; }
        public readonly TokenStack OpenBraces = new TokenStack();
        public ParserTrace ParserTrace = new ParserTrace();
        internal readonly ParserStack ParserStack = new ParserStack();

        public ParserState CurrentParserState { get; internal set; }
        public ParseTreeNode CurrentParserInput { get; internal set; }
        public Token CurrentToken; //The token just scanned by Scanner
        public TokenList CurrentCommentTokens = new TokenList(); //accumulated comment tokens
        public Token PreviousToken;
        public SourceLocation PreviousLineStart; //Location of last line start

        //list for terminals - for current parser state and current input char
        public TerminalList CurrentTerminals = new TerminalList();

        public ISourceStream Source;

        //Internal fields
        internal TokenFilterList TokenFilters = new TokenFilterList();
        internal TokenStack BufferedTokens = new TokenStack();
        internal IEnumerator<Token> FilteredTokens; //stream of tokens after filter
        internal TokenStack PreviewTokens = new TokenStack();
        internal ParsingEventArgs SharedParsingEventArgs;
        internal ValidateTokenEventArgs SharedValidateTokenEventArgs;

        public VsScannerStateMap VsLineScanState; //State variable used in line scanning mode for VS integration

        public ParserStatus Status { get; internal set; }
        public bool HasErrors; //error flag, once set remains set

        //values dictionary to use by custom language implementations to save some temporary values during parsing
        public readonly Dictionary<string, object> Values = new Dictionary<string, object>();

        public int TabWidth = 8;

        #endregion

        #region constructors
        public ParsingContext(Parser parser)
        {
            this.Parser = parser;
            this.Language = this.Parser.Language;
            this.Culture = this.Language.Grammar.DefaultCulture;
            //This might be a problem for multi-threading - if we have several contexts on parallel threads with different culture.
            //Resources.Culture is static property (this is not Irony's fault, this is auto-generated file).
            Resources.Culture = this.Culture;
            this.SharedParsingEventArgs = new ParsingEventArgs(this);
            this.SharedValidateTokenEventArgs = new ValidateTokenEventArgs(this);
        }
        #endregion

        #region Events: TokenCreated
        public event EventHandler<ParsingEventArgs> TokenCreated;

        internal void OnTokenCreated()
        {
            if (this.TokenCreated != null)
            {
                this.TokenCreated(this, this.SharedParsingEventArgs);
            }
        }
        #endregion

        #region Error handling and tracing

        public Token CreateErrorToken(string message, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }

            return this.Source.CreateToken(this.Language.Grammar.SyntaxError, message);
        }

        public void AddParserError(string message, params object[] args)
        {
            var location = this.CurrentParserInput == null ? this.Source.Location : this.CurrentParserInput.Span.Location;
            this.HasErrors = true;
            this.AddParserMessage(ErrorLevel.Error, location, message, args);
        }
        public void AddParserMessage(ErrorLevel level, SourceLocation location, string message, params object[] args)
        {
            if (this.CurrentParseTree == null)
            {
                return;
            }

            if (this.CurrentParseTree.ParserMessages.Count >= this.MaxErrors)
            {
                return;
            }

            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }

            this.CurrentParseTree.ParserMessages.Add(new LogMessage(level, location, message, this.CurrentParserState));
            if (this.TracingEnabled)
            {
                this.AddTrace(true, message);
            }
        }

        public void AddTrace(string message, params object[] args)
        {
            this.AddTrace(false, message, args);
        }
        public void AddTrace(bool asError, string message, params object[] args)
        {
            if (!this.TracingEnabled)
            {
                return;
            }

            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }

            this.ParserTrace.Add(new ParserTraceEntry(this.CurrentParserState, this.ParserStack.Top, this.CurrentParserInput, message, asError));
        }

        #region comments
        // Computes set of expected terms in a parser state. While there may be extended list of symbols expected at some point,
        // we want to reorganize and reduce it. For example, if the current state expects all arithmetic operators as an input,
        // it would be better to not list all operators (+, -, *, /, etc) but simply put "operator" covering them all.
        // To achieve this grammar writer can group operators (or any other terminals) into named groups using Grammar's methods
        // AddTermReportGroup, AddNoReportGroup etc. Then instead of reporting each operator separately, Irony would include
        // a single "group name" to represent them all.
        // The "expected report set" is not computed during parser construction (it would take considerable time),
        // but does it on demand during parsing, when error is detected and the expected set is actually needed for error message.
        // Multi-threading concerns. When used in multi-threaded environment (web server), the LanguageData would be shared in
        // application-wide cache to avoid rebuilding the parser data on every request. The LanguageData is immutable, except
        // this one case - the expected sets are constructed late by CoreParser on the when-needed basis.
        // We don't do any locking here, just compute the set and on return from this function the state field is assigned.
        // We assume that this field assignment is an atomic, concurrency-safe operation. The worst thing that might happen
        // is "double-effort" when two threads start computing the same set around the same time, and the last one to finish would
        // leave its result in the state field.
        #endregion
        internal static StringSet ComputeGroupedExpectedSetForState(Grammar grammar, ParserState state)
        {
            var terms = new TerminalSet();
            terms.UnionWith(state.ExpectedTerminals);
            var result = new StringSet();
            //Eliminate no-report terminals
            foreach (var group in grammar.TermReportGroups)
            {
                if (group.GroupType == TermReportGroupType.DoNotReport)
                {
                    terms.ExceptWith(group.Terminals);
                }
            }
            //Add normal and operator groups
            foreach (var group in grammar.TermReportGroups)
            {
                if ((group.GroupType == TermReportGroupType.Normal || group.GroupType == TermReportGroupType.Operator) &&
                      terms.Overlaps(group.Terminals))
                {
                    result.Add(group.Alias);
                    terms.ExceptWith(group.Terminals);
                }
            }
            //Add remaining terminals "as is"
            foreach (var terminal in terms)
            {
                result.Add(terminal.ErrorAlias);
            }

            return result;
        }

        #endregion

        internal void Reset()
        {
            this.CurrentParserState = this.Parser.InitialState;
            this.CurrentParserInput = null;
            this.CurrentCommentTokens = new TokenList();
            this.ParserStack.Clear();
            this.HasErrors = false;
            this.ParserStack.Push(new ParseTreeNode(this.CurrentParserState));
            this.CurrentParseTree = null;
            this.OpenBraces.Clear();
            this.ParserTrace.Clear();
            this.CurrentTerminals.Clear();
            this.CurrentToken = null;
            this.PreviousToken = null;
            this.PreviousLineStart = new SourceLocation(0, -1, 0);
            this.BufferedTokens.Clear();
            this.PreviewTokens.Clear();
            this.Values.Clear();
            foreach (var filter in this.TokenFilters)
            {
                filter.Reset();
            }
        }

        public void SetSourceLocation(SourceLocation location)
        {
            foreach (var filter in this.TokenFilters)
            {
                filter.OnSetSourceLocation(location);
            }

            this.Source.Location = location;
        }

        public SourceSpan ComputeStackRangeSpan(int nodeCount)
        {
            if (nodeCount == 0)
            {
                return new SourceSpan(this.CurrentParserInput.Span.Location, 0);
            }

            var first = this.ParserStack[this.ParserStack.Count - nodeCount];
            var last = this.ParserStack.Top;
            return new SourceSpan(first.Span.Location, last.Span.EndPosition - first.Span.Location.Position);
        }

        #region Expected term set computations
        public StringSet GetExpectedTermSet()
        {
            if (this.CurrentParserState == null)
            {
                return new StringSet();
            }
            //See note about multi-threading issues in ComputeReportedExpectedSet comments.
            if (this.CurrentParserState.ReportedExpectedSet == null)
            {
                this.CurrentParserState.ReportedExpectedSet = Construction.ParserDataBuilder.ComputeGroupedExpectedSetForState(this.Language.Grammar, this.CurrentParserState);
            }
            //Filter out closing braces which are not expected based on previous input.
            // While the closing parenthesis ")" might be expected term in a state in general,
            // if there was no opening parenthesis in preceding input then we would not
            //  expect a closing one.
            var expectedSet = this.FilterBracesInExpectedSet(this.CurrentParserState.ReportedExpectedSet);
            return expectedSet;
        }

        private StringSet FilterBracesInExpectedSet(StringSet stateExpectedSet)
        {
            var result = new StringSet();
            result.UnionWith(stateExpectedSet);
            //Find what brace we expect
            var nextClosingBrace = string.Empty;
            if (this.OpenBraces.Count > 0)
            {
                var lastOpenBraceTerm = this.OpenBraces.Peek().KeyTerm;
                var nextClosingBraceTerm = lastOpenBraceTerm.IsPairFor as KeyTerm;
                if (nextClosingBraceTerm != null)
                {
                    nextClosingBrace = nextClosingBraceTerm.Text;
                }
            }
            //Now check all closing braces in result set, and leave only nextClosingBrace
            foreach (var term in this.Language.Grammar.KeyTerms.Values)
            {
                if (term.Flags.IsSet(TermFlags.IsCloseBrace))
                {
                    var brace = term.Text;
                    if (result.Contains(brace) && brace != nextClosingBrace)
                    {
                        result.Remove(brace);
                    }
                }
            }//foreach term
            return result;
        }

        #endregion

    }//class

    // A struct used for packing/unpacking ScannerState int value; used for VS integration.
    // When Terminal produces incomplete token, it sets
    // this state to non-zero value; this value identifies this terminal as the one who will continue scanning when
    // it resumes, and the terminal's internal state when there may be several types of multi-line tokens for one terminal.
    // For ex., there maybe several types of string literal like in Python.
    [StructLayout(LayoutKind.Explicit)]
    public struct VsScannerStateMap
    {
        [FieldOffset(0)]
        public int Value;
        [FieldOffset(0)]
        public byte TerminalIndex;   //1-based index of active multiline term in MultilineTerminals
        [FieldOffset(1)]
        public byte TokenSubType;         //terminal subtype (used in StringLiteral to identify string kind)
        [FieldOffset(2)]
        public short TerminalFlags;  //Terminal flags
    }//struct
}
