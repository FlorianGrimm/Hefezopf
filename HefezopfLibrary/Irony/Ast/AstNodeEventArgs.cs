using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace Irony.Ast
{
    public class AstNodeEventArgs : EventArgs
    {
        public readonly ParseTreeNode ParseTreeNode;

        public AstNodeEventArgs(ParseTreeNode parseTreeNode)
        {
            this.ParseTreeNode = parseTreeNode;
        }

        public object AstNode => this.ParseTreeNode.AstNode;
    }
}
