using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.Parsing
{
    public class ParsingEventArgs : EventArgs
    {
        public readonly ParsingContext Context;
        public ParsingEventArgs(ParsingContext context)
        {
            this.Context = context;
        }
    }

    public class ReducedEventArgs : ParsingEventArgs
    {
        public readonly Production ReducedProduction;
        public readonly ParseTreeNode ResultNode;
        public ReducedEventArgs(ParsingContext context, Production reducedProduction, ParseTreeNode resultNode)
            : base(context)
        {
            this.ReducedProduction = reducedProduction;
            this.ResultNode = resultNode;
        }
    }

    public class ValidateTokenEventArgs : ParsingEventArgs
    {
        public ValidateTokenEventArgs(ParsingContext context)
            : base(context)
        { }

        public Token Token
        {
            get { return this.Context.CurrentToken; }
        }//Token

        public void ReplaceToken(Token token)
        {
            this.Context.CurrentToken = token;
        }
        public void SetError(string errorMessage, params object[] messageArgs)
        {
            this.Context.CurrentToken = this.Context.CreateErrorToken(errorMessage, messageArgs);
        }
        //Rejects the token; use it when there's more than one terminal that can be used to scan the input and ValidateToken is used
        // to help Scanner make the decision. Once the token is rejected, the scanner will move to the next Terminal (with lower priority)
        // and will try to produce token.
        public void RejectToken()
        {
            this.Context.CurrentToken = null;
        }
    }//class
}//namespace
