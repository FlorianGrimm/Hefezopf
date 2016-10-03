using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.Parsing
{
    [Flags]
    public enum OutlineOptions
    {
        None = 0,
        ProduceIndents = 0x01,
        CheckBraces = 0x02,
        CheckOperator = 0x04, //to implement, auto line joining if line ends with operator
    }

    public class CodeOutlineFilter : TokenFilter
    {
        public readonly OutlineOptions Options;
        public readonly KeyTerm ContinuationTerminal; //Terminal

        public Stack<int> Indents = new Stack<int>();
        public Token CurrentToken;
        public Token PreviousToken;
        public SourceLocation PreviousTokenLocation;
        public TokenStack OutputTokens = new TokenStack();

        private GrammarData _grammarData;
        private Grammar _grammar;
        private ParsingContext _context;
        private bool _produceIndents;
        private bool _checkBraces;
        private bool _checkOperator;

        private bool _isContinuation;
        private bool _prevIsContinuation;
        private bool _isOperator;
        private bool _prevIsOperator;
        private bool _doubleEof;

        #region constructor
        public CodeOutlineFilter(GrammarData grammarData, OutlineOptions options, KeyTerm continuationTerminal)
        {
            this._grammarData = grammarData;
            this._grammar = grammarData.Grammar;
            this._grammar.LanguageFlags |= LanguageFlags.EmitLineStartToken;
            this.Options = options;
            this.ContinuationTerminal = continuationTerminal;
            if (this.ContinuationTerminal != null)
            {
                if (!this._grammar.NonGrammarTerminals.Contains(this.ContinuationTerminal))
                {
                    this._grammarData.Language.Errors.Add(GrammarErrorLevel.Warning, null, Resources.ErrOutlineFilterContSymbol, this.ContinuationTerminal.Name);
                }
            }
            //"CodeOutlineFilter: line continuation symbol '{0}' should be added to Grammar.NonGrammarTerminals list.",
            this._produceIndents = this.OptionIsSet(OutlineOptions.ProduceIndents);
            this._checkBraces = this.OptionIsSet(OutlineOptions.CheckBraces);
            this._checkOperator = this.OptionIsSet(OutlineOptions.CheckOperator);
            this.Reset();
        }
        #endregion

        public override void Reset()
        {
            base.Reset();
            this.Indents.Clear();
            this.Indents.Push(0);
            this.OutputTokens.Clear();
            this.PreviousToken = null;
            this.CurrentToken = null;
            this.PreviousTokenLocation = new SourceLocation();
        }

        public bool OptionIsSet(OutlineOptions option)
        {
            return (this.Options & option) != 0;
        }

        public override IEnumerable<Token> BeginFiltering(ParsingContext context, IEnumerable<Token> tokens)
        {
            this._context = context;
            foreach (Token token in tokens)
            {
                this.ProcessToken(token);
                while (this.OutputTokens.Count > 0)
                {
                    yield return this.OutputTokens.Pop();
                }
            }//foreach
        }//method

        public void ProcessToken(Token token)
        {
            this.SetCurrentToken(token);
            //Quick checks
            if (this._isContinuation)
            {
                return;
            }

            var tokenTerm = token.Terminal;

            //check EOF
            if (tokenTerm == this._grammar.Eof)
            {
                this.ProcessEofToken();
                return;
            }

            if (tokenTerm != this._grammar.LineStartTerminal)
            {
                return;
            }
            //if we are here, we have LineStart token on new line; first remove it from stream, it should not go to parser
            this.OutputTokens.Pop();

            if (this.PreviousToken == null)
            {
                return;
            }

            // first check if there was continuation symbol before
            // or - if checkBraces flag is set - check if there were open braces
            if (this._prevIsContinuation || this._checkBraces && this._context.OpenBraces.Count > 0)
            {
                return; //no Eos token in this case
            }

            if (this._prevIsOperator && this._checkOperator)
            {
                return; //no Eos token in this case
            }

            //We need to produce Eos token and indents (if _produceIndents is set).
            // First check indents - they go first into OutputTokens stack, so they will be popped out last
            if (this._produceIndents)
            {
                var currIndent = token.Location.Column;
                var prevIndent = this.Indents.Peek();
                if (currIndent > prevIndent)
                {
                    this.Indents.Push(currIndent);
                    this.PushOutlineToken(this._grammar.Indent, token.Location);
                }
                else if (currIndent < prevIndent)
                {
                    this.PushDedents(currIndent);
                    //check that current indent exactly matches the previous indent
                    if (this.Indents.Peek() != currIndent)
                    {
                        //fire error
                        this.OutputTokens.Push(new Token(this._grammar.SyntaxError, token.Location, string.Empty, Resources.ErrInvDedent));
                        // "Invalid dedent level, no previous matching indent found."
                    }
                }
            }//if _produceIndents
             //Finally produce Eos token, but not in command line mode. In command line mode the Eos was already produced
             // when we encountered Eof on previous line
            if (this._context.Mode != ParseMode.CommandLine)
            {
                var eosLocation = this.ComputeEosLocation();
                this.PushOutlineToken(this._grammar.Eos, eosLocation);
            }
        }//method

        private void SetCurrentToken(Token token)
        {
            this._doubleEof = this.CurrentToken != null && this.CurrentToken.Terminal == this._grammar.Eof
                            && token.Terminal == this._grammar.Eof;
            //Copy CurrentToken to PreviousToken
            if (this.CurrentToken != null && this.CurrentToken.Category == TokenCategory.Content)
            { //remember only content tokens
                this.PreviousToken = this.CurrentToken;
                this._prevIsContinuation = this._isContinuation;
                this._prevIsOperator = this._isOperator;
                if (this.PreviousToken != null)
                {
                    this.PreviousTokenLocation = this.PreviousToken.Location;
                }
            }
            this.CurrentToken = token;
            this._isContinuation = (token.Terminal == this.ContinuationTerminal && this.ContinuationTerminal != null);
            this._isOperator = token.Terminal.Flags.IsSet(TermFlags.IsOperator);
            if (!this._isContinuation)
            {
                this.OutputTokens.Push(token); //by default input token goes to output, except continuation symbol
            }
        }

        //Processes Eof token. We should take into account the special case of processing command line input.
        // In this case we should not automatically dedent all stacked indents if we get EOF.
        // Note that tokens will be popped from the OutputTokens stack and sent to parser in the reverse order compared to
        // the order we pushed them into OutputTokens stack. We have Eof already in stack; we first push dedents, then Eos
        // They will come out to parser in the following order: Eos, Dedents, Eof.
        private void ProcessEofToken()
        {
            //First decide whether we need to produce dedents and Eos symbol
            bool pushDedents = false;
            bool pushEos = true;
            switch (this._context.Mode)
            {
                case ParseMode.File:
                    pushDedents = this._produceIndents; //Do dedents if token filter tracks indents
                    break;
                case ParseMode.CommandLine:
                    //only if user entered empty line, we dedent all
                    pushDedents = this._produceIndents && this._doubleEof;
                    pushEos = !this._prevIsContinuation && !this._doubleEof; //if previous symbol is continuation symbol then don't push Eos
                    break;
                case ParseMode.VsLineScan:
                    pushDedents = false; //Do not dedent at all on every line end
                    break;
            }
            //unindent all buffered indents;
            if (pushDedents)
            {
                this.PushDedents(0);
            }
            //now push Eos token - it will be popped first, then dedents, then EOF token
            if (pushEos)
            {
                var eosLocation = this.ComputeEosLocation();
                this.PushOutlineToken(this._grammar.Eos, eosLocation);
            }
        }

        private void PushDedents(int untilPosition)
        {
            while (this.Indents.Peek() > untilPosition)
            {
                this.Indents.Pop();
                this.PushOutlineToken(this._grammar.Dedent, this.CurrentToken.Location);
            }
        }

        private SourceLocation ComputeEosLocation()
        {
            if (this.PreviousToken == null)
            {
                return new SourceLocation();
            }
            //Return position at the end of previous token
            var loc = this.PreviousToken.Location;
            var len = this.PreviousToken.Length;
            return new SourceLocation(loc.Position + len, loc.Line, loc.Column + len);
        }

        private void PushOutlineToken(Terminal term, SourceLocation location)
        {
            this.OutputTokens.Push(new Token(term, location, string.Empty, null));
        }
    }//class
}//namespace
