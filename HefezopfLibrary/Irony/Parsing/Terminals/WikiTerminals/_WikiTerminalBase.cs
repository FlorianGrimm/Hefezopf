#pragma warning disable SA1649
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.Parsing
{
    public enum WikiTermType
    {
        Text,
        Element,
        Format,
        Heading,
        List,
        Block,
        Table
    }

    public abstract class WikiTerminalBase : Terminal
    {
        public readonly WikiTermType TermType;
        public readonly string OpenTag;
        public readonly string CloseTag;
        public string HtmlElementName;
        public string ContainerHtmlElementName;
        public string OpenHtmlTag;
        public string CloseHtmlTag;
        public string ContainerOpenHtmlTag;
        public string ContainerCloseHtmlTag;

        public WikiTerminalBase(string name, WikiTermType termType, string openTag, string closeTag, string htmlElementName)
            : base(name)
        {
            this.TermType = termType;
            this.OpenTag = openTag;
            this.CloseTag = closeTag;
            this.HtmlElementName = htmlElementName;
            this.Priority = TerminalPriority.Normal + this.OpenTag.Length; //longer tags have higher priority
        }

        public override IList<string> GetFirsts()
        {
            return new string[] { this.OpenTag };
        }
        public override void Init(GrammarData grammarData)
        {
            base.Init(grammarData);
            if (!string.IsNullOrEmpty(this.HtmlElementName))
            {
                if (string.IsNullOrEmpty(this.OpenHtmlTag))
                {
                    this.OpenHtmlTag = "<" + this.HtmlElementName + ">";
                }

                if (string.IsNullOrEmpty(this.CloseHtmlTag))
                {
                    this.CloseHtmlTag = "</" + this.HtmlElementName + ">";
                }
            }
            if (!string.IsNullOrEmpty(this.ContainerHtmlElementName))
            {
                if (string.IsNullOrEmpty(this.ContainerOpenHtmlTag))
                {
                    this.ContainerOpenHtmlTag = "<" + this.ContainerHtmlElementName + ">";
                }

                if (string.IsNullOrEmpty(this.ContainerCloseHtmlTag))
                {
                    this.ContainerCloseHtmlTag = "</" + this.ContainerHtmlElementName + ">";
                }
            }
        }
    }//class
}//namespace
