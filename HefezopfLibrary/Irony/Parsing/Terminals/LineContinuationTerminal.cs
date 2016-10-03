using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Irony.Parsing
{
    public class LineContinuationTerminal : Terminal
    {
        public LineContinuationTerminal(string name, params string[] startSymbols)
            : base(name, TokenCategory.Outline)
        {
            var symbols = startSymbols.Where(s => !IsNullOrWhiteSpace(s)).ToArray();
            this.StartSymbols = new StringList(symbols);
            if (this.StartSymbols.Count == 0)
            {
                this.StartSymbols.AddRange(_defaultStartSymbols);
            }

            this.Priority = TerminalPriority.High;
        }

        public StringList StartSymbols;
        private string _startSymbolsFirsts = String.Concat(_defaultStartSymbols);
        private static string[] _defaultStartSymbols = new[] { "\\", "_" };
        public string LineTerminators = "\n\r\v";

        #region overrides

        public override void Init(GrammarData grammarData)
        {
            base.Init(grammarData);

            // initialize string of start characters for fast lookup
            this._startSymbolsFirsts = new String(this.StartSymbols.Select(s => s.First()).ToArray());

            if (this.EditorInfo == null)
            {
                this.EditorInfo = new TokenEditorInfo(TokenType.Delimiter, TokenColor.Comment, TokenTriggers.None);
            }
        }

        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            // Quick check
            var lookAhead = source.PreviewChar;
            var startIndex = this._startSymbolsFirsts.IndexOf(lookAhead);
            if (startIndex < 0)
            {
                return null;
            }

            // Match start symbols
            if (!this.BeginMatch(source, startIndex, lookAhead))
            {
                return null;
            }

            // Match NewLine
            var result = this.CompleteMatch(source);
            if (result != null)
            {
                return result;
            }

            // Report an error
            return context.CreateErrorToken(Resources.ErrNewLineExpected);
        }

        private bool BeginMatch(ISourceStream source, int startFrom, char lookAhead)
        {
            foreach (var startSymbol in this.StartSymbols.Skip(startFrom))
            {
                if (startSymbol[0] != lookAhead)
                {
                    continue;
                }

                if (source.MatchSymbol(startSymbol))
                {
                    source.PreviewPosition += startSymbol.Length;
                    return true;
                }
            }
            return false;
        }

        private Token CompleteMatch(ISourceStream source)
        {
            if (source.EOF())
            {
                return null;
            }

            do
            {
                // Match NewLine
                var lookAhead = source.PreviewChar;
                if (this.LineTerminators.IndexOf(lookAhead) >= 0)
                {
                    source.PreviewPosition++;
                    // Treat \r\n as single NewLine
                    if (!source.EOF() && lookAhead == '\r' && source.PreviewChar == '\n')
                    {
                        source.PreviewPosition++;
                    }

                    break;
                }

                // Eat up whitespace
                if (this.Grammar.IsWhitespaceOrDelimiter(lookAhead))
                {
                    source.PreviewPosition++;
                    continue;
                }

                // Fail on anything else
                return null;
            }
            while (!source.EOF());

            // Create output token
            return source.CreateToken(this.OutputTerminal);
        }

        public override IList<string> GetFirsts()
        {
            return this.StartSymbols;
        }

        #endregion

        private static bool IsNullOrWhiteSpace(string s)
        {
#if VS2008
      if (String.IsNullOrEmpty(s))
        return true;
      return s.Trim().Length == 0;
#else
            return String.IsNullOrWhiteSpace(s);
#endif
        }
    } // LineContinuationTerminal class
}
