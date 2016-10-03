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
    //Terminal based on custom method; allows creating custom match without creating new class derived from Terminal
    public delegate Token MatchHandler(Terminal terminal, ParsingContext context, ISourceStream source);

    public class CustomTerminal : Terminal
    {
        public CustomTerminal(string name, MatchHandler handler, params string[] prefixes)
            : base(name)
        {
            this._handler = handler;
            if (prefixes != null)
            {
                this.Prefixes.AddRange(prefixes);
            }

            this.EditorInfo = new TokenEditorInfo(TokenType.Unknown, TokenColor.Text, TokenTriggers.None);
        }

        public readonly StringList Prefixes = new StringList();

        public MatchHandler Handler
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this._handler; }
        }
        private MatchHandler _handler;

        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            return this._handler(this, context, source);
        }
        [System.Diagnostics.DebuggerStepThrough]
        public override IList<string> GetFirsts()
        {
            return this.Prefixes;
        }
    }//class
}
