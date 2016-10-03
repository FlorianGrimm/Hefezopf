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
using System.Text;

namespace Irony.Parsing
{
    public class CommentTerminal : Terminal
    {
        public string StartSymbol;
        public StringList EndSymbols;
        private char[] _endSymbolsFirsts;
        private bool _isLineComment; //true if NewLine is one of EndSymbols; if yes, EOF is also considered a valid end symbol

        public CommentTerminal(string name, string startSymbol, params string[] endSymbols)
            : base(name, TokenCategory.Comment)
        {
            this.StartSymbol = startSymbol;
            this.EndSymbols = new StringList();
            this.EndSymbols.AddRange(endSymbols);
            this.Priority = TerminalPriority.High; //assign max priority
        }

        #region overrides
        public override void Init(GrammarData grammarData)
        {
            base.Init(grammarData);
            //_endSymbolsFirsts char array is used for fast search for end symbols using String's method IndexOfAny(...)
            this._endSymbolsFirsts = new char[this.EndSymbols.Count];
            for (int i = 0; i < this.EndSymbols.Count; i++)
            {
                string sym = this.EndSymbols[i];
                this._endSymbolsFirsts[i] = sym[0];
                this._isLineComment |= sym.Contains("\n");
                if (!this._isLineComment)
                {
                    this.SetFlag(TermFlags.IsMultiline);
                }
            }
            if (this.EditorInfo == null)
            {
                TokenType ttype = this._isLineComment ? TokenType.LineComment : TokenType.Comment;
                this.EditorInfo = new TokenEditorInfo(ttype, TokenColor.Comment, TokenTriggers.None);
            }
        }

        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            Token result;
            if (context.VsLineScanState.Value != 0)
            {
                // we are continuing in line mode - restore internal env (none in this case)
                context.VsLineScanState.Value = 0;
            }
            else
            {
                //we are starting from scratch
                if (!this.BeginMatch(context, source))
                {
                    return null;
                }
            }
            result = this.CompleteMatch(context, source);
            if (result != null)
            {
                return result;
            }
            //if it is LineComment, it is ok to hit EOF without final line-break; just return all until end.
            if (this._isLineComment)
            {
                return source.CreateToken(this.OutputTerminal);
            }

            if (context.Mode == ParseMode.VsLineScan)
            {
                return this.CreateIncompleteToken(context, source);
            }

            return context.CreateErrorToken(Resources.ErrUnclosedComment);
        }

        private Token CreateIncompleteToken(ParsingContext context, ISourceStream source)
        {
            source.PreviewPosition = source.Text.Length;
            Token result = source.CreateToken(this.OutputTerminal);
            result.Flags |= TokenFlags.IsIncomplete;
            context.VsLineScanState.TerminalIndex = this.MultilineIndex;
            return result;
        }

        private bool BeginMatch(ParsingContext context, ISourceStream source)
        {
            //Check starting symbol
            if (!source.MatchSymbol(this.StartSymbol))
            {
                return false;
            }

            source.PreviewPosition += this.StartSymbol.Length;
            return true;
        }
        private Token CompleteMatch(ParsingContext context, ISourceStream source)
        {
            //Find end symbol
            while (!source.EOF())
            {
                int firstCharPos;
                if (this.EndSymbols.Count == 1)
                {
                    firstCharPos = source.Text.IndexOf(this.EndSymbols[0], source.PreviewPosition);
                }
                else
                {
                    firstCharPos = source.Text.IndexOfAny(this._endSymbolsFirsts, source.PreviewPosition);
                }

                if (firstCharPos < 0)
                {
                    source.PreviewPosition = source.Text.Length;
                    return null; //indicating error
                }
                //We found a character that might start an end symbol; let's see if it is true.
                source.PreviewPosition = firstCharPos;
                foreach (string endSymbol in this.EndSymbols)
                {
                    if (source.MatchSymbol(endSymbol))
                    {
                        //We found end symbol; eat end symbol only if it is not line comment.
                        // For line comment, leave LF symbol there, it might be important to have a separate LF token
                        if (!this._isLineComment)
                        {
                            source.PreviewPosition += endSymbol.Length;
                        }

                        return source.CreateToken(this.OutputTerminal);
                    }//if
                }//foreach endSymbol
                source.PreviewPosition++; //move to the next char and try again
            }//while
            return null; //might happen if we found a start char of end symbol, but not the full endSymbol
        }//method

        public override IList<string> GetFirsts()
        {
            return new string[] { this.StartSymbol };
        }
        #endregion
    }//CommentTerminal class
}
