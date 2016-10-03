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
    public class SourceStream : ISourceStream
    {
        private StringComparison _stringComparison;
        private int _tabWidth;
        private char[] _chars;
        private int _textLength;

        public SourceStream(string text, bool caseSensitive, int tabWidth)
            : this(text, caseSensitive, tabWidth, new SourceLocation())
        { }

        public SourceStream(string text, bool caseSensitive, int tabWidth, SourceLocation initialLocation)
        {
            this._text = text;
            this._textLength = this._text.Length;
            this._chars = this.Text.ToCharArray();
            this._stringComparison = caseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;
            this._tabWidth = tabWidth;
            this._location = initialLocation;
            this._previewPosition = this._location.Position;
            if (this._tabWidth <= 1)
            {
                this._tabWidth = 8;
            }
        }

        #region ISourceStream Members
        public string Text
        {
            get { return this._text; }
        }
        private string _text;

        public int Position
        {
            get
            {
                return this._location.Position;
            }
            set
            {
                if (this._location.Position != value)
                {
                    this.SetNewPosition(value);
                }
            }
        }

        public SourceLocation Location
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return this._location;
            }
            set
            {
                this._location = value;
            }
        }
        private SourceLocation _location;

        public int PreviewPosition
        {
            get
            {
                return this._previewPosition;
            }
            set
            {
                this._previewPosition = value;
            }
        }
        private int _previewPosition;

        public char PreviewChar
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if (this._previewPosition >= this._textLength)
                {
                    return '\0';
                }

                return this._chars[this._previewPosition];
            }
        }

        public char NextPreviewChar
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                if (this._previewPosition + 1 >= this._textLength)
                {
                    return '\0';
                }

                return this._chars[this._previewPosition + 1];
            }
        }

        public bool MatchSymbol(string symbol)
        {
            try
            {
                int cmp = string.Compare(this._text, this.PreviewPosition, symbol, 0, symbol.Length, this._stringComparison);
                return cmp == 0;
            }
            catch
            {
                //exception may be thrown if Position + symbol.length > text.Length;
                // this happens not often, only at the very end of the file, so we don't check this explicitly
                //but simply catch the exception and return false. Again, try/catch block has no overhead
                // if exception is not thrown.
                return false;
            }
        }

        public Token CreateToken(Terminal terminal)
        {
            var tokenText = this.GetPreviewText();
            return new Token(terminal, this.Location, tokenText, tokenText);
        }
        public Token CreateToken(Terminal terminal, object value)
        {
            var tokenText = this.GetPreviewText();
            return new Token(terminal, this.Location, tokenText, value);
        }

        [System.Diagnostics.DebuggerStepThrough]
        public bool EOF()
        {
            return this._previewPosition >= this._textLength;
        }
        #endregion

        //returns substring from Location.Position till (PreviewPosition - 1)
        private string GetPreviewText()
        {
            var until = this._previewPosition;
            if (until > this._textLength)
            {
                until = this._textLength;
            }

            var p = this._location.Position;
            string text = this.Text.Substring(p, until - p);
            return text;
        }

        // To make debugging easier: show 20 chars from current position
        public override string ToString()
        {
            string result;
            try
            {
                var p = this.Location.Position;
                if (p + 20 < this._textLength)
                {
                    result = this._text.Substring(p, 20) + Resources.LabelSrcHaveMore;// " ..."
                }
                else
                {
                    result = this._text.Substring(p) + Resources.LabelEofMark; //"(EOF)"
                }
            }
            catch (Exception)
            {
                result = this.PreviewChar + Resources.LabelSrcHaveMore;
            }
            return string.Format(Resources.MsgSrcPosToString, result, this.Location); //"[{0}], at {1}"
        }

        //Computes the Location info (line, col) for a new source position.
        private void SetNewPosition(int newPosition)
        {
            if (newPosition < this.Position)
            {
                throw new Exception(Resources.ErrCannotMoveBackInSource);
            }

            int p = this.Position;
            int col = this.Location.Column;
            int line = this.Location.Line;
            while (p < newPosition)
            {
                if (p >= this._textLength)
                {
                    break;
                }

                var curr = this._chars[p];
                switch (curr)
                {
                    case '\n': line++; col = 0; break;
                    case '\r': break;
                    case '\t': col = (col / this._tabWidth + 1) * this._tabWidth; break;
                    default: col++; break;
                } //switch
                p++;
            }
            this.Location = new SourceLocation(p, line, col);
        }
    }//class
}//namespace
