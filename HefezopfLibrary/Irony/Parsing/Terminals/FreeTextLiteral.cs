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
using System.Linq;
using System.Text;

namespace Irony.Parsing
{
    // Sometimes language definition includes tokens that have no specific format, but are just "all text until some terminator character(s)";
    // FreeTextTerminal allows easy implementation of such language element.

    [Flags]
    public enum FreeTextOptions
    {
        None = 0x0,
        ConsumeTerminator = 0x01, //move source pointer beyond terminator (so token "consumes" it from input), but don't include it in token text
        IncludeTerminator = 0x02, // include terminator into token text/value
        AllowEof = 0x04, // treat EOF as legitimate terminator
        AllowEmpty = 0x08,
    }

    public class FreeTextLiteral : Terminal
    {
        public StringSet Terminators = new StringSet();
        public StringSet Firsts = new StringSet();
        public StringDictionary Escapes = new StringDictionary();
        public FreeTextOptions FreeTextOptions;
        private char[] _stopChars;
        private bool _isSimple; //True if we have a single Terminator and no escapes
        private string _singleTerminator;

        public FreeTextLiteral(string name, params string[] terminators)
            : this(name, FreeTextOptions.None, terminators)
        { }

        public FreeTextLiteral(string name, FreeTextOptions freeTextOptions, params string[] terminators)
            : base(name)
        {
            this.FreeTextOptions = freeTextOptions;
            this.Terminators.UnionWith(terminators);
            base.SetFlag(TermFlags.IsLiteral);
        }//constructor

        public override IList<string> GetFirsts()
        {
            var result = new StringList();
            result.AddRange(this.Firsts);
            return result;
        }
        public override void Init(GrammarData grammarData)
        {
            base.Init(grammarData);
            this._isSimple = this.Terminators.Count == 1 && this.Escapes.Count == 0;
            if (this._isSimple)
            {
                this._singleTerminator = this.Terminators.First();
                return;
            }
            var stopChars = new CharHashSet();
            foreach (var key in this.Escapes.Keys)
            {
                stopChars.Add(key[0]);
            }

            foreach (var t in this.Terminators)
            {
                stopChars.Add(t[0]);
            }

            this._stopChars = stopChars.ToArray();
        }

        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            if (!this.TryMatchPrefixes(context, source))
            {
                return null;
            }

            return this._isSimple ? this.TryMatchContentSimple(context, source) : this.TryMatchContentExtended(context, source);
        }

        private bool TryMatchPrefixes(ParsingContext context, ISourceStream source)
        {
            if (this.Firsts.Count == 0)
            {
                return true;
            }

            foreach (var first in this.Firsts)
            {
                if (source.MatchSymbol(first))
                {
                    source.PreviewPosition += first.Length;
                    return true;
                }
            }

            return false;
        }

        private Token TryMatchContentSimple(ParsingContext context, ISourceStream source)
        {
            var startPos = source.PreviewPosition;
            var termLen = this._singleTerminator.Length;
            var stringComp = this.Grammar.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
            int termPos = source.Text.IndexOf(this._singleTerminator, startPos, stringComp);
            if (termPos < 0 && this.IsSet(FreeTextOptions.AllowEof))
            {
                termPos = source.Text.Length;
            }

            if (termPos < 0)
            {
                return context.CreateErrorToken(Resources.ErrFreeTextNoEndTag, this._singleTerminator);
            }

            var textEnd = termPos;
            if (this.IsSet(FreeTextOptions.IncludeTerminator))
            {
                textEnd += termLen;
            }

            var tokenText = source.Text.Substring(startPos, textEnd - startPos);
            if (string.IsNullOrEmpty(tokenText) && (this.FreeTextOptions & Parsing.FreeTextOptions.AllowEmpty) == 0)
            {
                return null;
            }
            // The following line is a fix submitted by user rmcase
            source.PreviewPosition = this.IsSet(FreeTextOptions.ConsumeTerminator) ? termPos + termLen : termPos;
            return source.CreateToken(this.OutputTerminal, tokenText);
        }

        private Token TryMatchContentExtended(ParsingContext context, ISourceStream source)
        {
            StringBuilder tokenText = new StringBuilder();
            while (true)
            {
                //Find next position of one of stop chars
                var nextPos = source.Text.IndexOfAny(this._stopChars, source.PreviewPosition);
                if (nextPos == -1)
                {
                    if (this.IsSet(FreeTextOptions.AllowEof))
                    {
                        source.PreviewPosition = source.Text.Length;
                        return source.CreateToken(this.OutputTerminal);
                    }
                    else
                        return null;
                }
                var newText = source.Text.Substring(source.PreviewPosition, nextPos - source.PreviewPosition);
                tokenText.Append(newText);
                source.PreviewPosition = nextPos;
                //if it is escape, add escaped text and continue search
                if (this.CheckEscape(source, tokenText))
                {
                    continue;
                }
                //check terminators
                if (this.CheckTerminators(source, tokenText))
                {
                    break; //from while (true); we reached
                }
                //The current stop is not at escape or terminator; add this char to token text and move on
                tokenText.Append(source.PreviewChar);
                source.PreviewPosition++;
            }//while
            var text = tokenText.ToString();
            if (string.IsNullOrEmpty(text) && (this.FreeTextOptions & Parsing.FreeTextOptions.AllowEmpty) == 0)
            {
                return null;
            }

            return source.CreateToken(this.OutputTerminal, text);
        }

        private bool CheckEscape(ISourceStream source, StringBuilder tokenText)
        {
            foreach (var dictEntry in this.Escapes)
            {
                if (source.MatchSymbol(dictEntry.Key))
                {
                    source.PreviewPosition += dictEntry.Key.Length;
                    tokenText.Append(dictEntry.Value);
                    return true;
                }
            }//foreach
            return false;
        }

        private bool CheckTerminators(ISourceStream source, StringBuilder tokenText)
        {
            foreach (var term in this.Terminators)
            {
                if (source.MatchSymbol(term))
                {
                    if (this.IsSet(FreeTextOptions.IncludeTerminator))
                    {
                        tokenText.Append(term);
                    }

                    if (this.IsSet(FreeTextOptions.ConsumeTerminator | FreeTextOptions.IncludeTerminator))
                    {
                        source.PreviewPosition += term.Length;
                    }

                    return true;
                }
            }

            return false;
        }

        private bool IsSet(FreeTextOptions option)
        {
            return (this.FreeTextOptions & option) != 0;
        }
    }//class
}//namespace
