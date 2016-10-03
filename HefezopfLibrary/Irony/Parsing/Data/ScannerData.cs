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
#pragma warning disable SA1649

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.Parsing
{
    public class TerminalLookupTable : Dictionary<char, TerminalList> { }

    // ScannerData is a container for all detailed info needed by scanner to read input.
    public class ScannerData
    {
        public readonly LanguageData Language;
        //hash table for fast terminal lookup by input char
        public readonly TerminalLookupTable TerminalsLookup = new TerminalLookupTable();
        public readonly TerminalList MultilineTerminals = new TerminalList();
        public readonly TerminalLookupTable NonGrammarTerminalsLookup = new TerminalLookupTable();

        //Terminals with no limited set of prefixes, copied from GrammarData
        //hash table for fast lookup of non-grammar terminals by input char
        public TerminalList NoPrefixTerminals = new TerminalList();

        public ScannerData(LanguageData language)
        {
            this.Language = language;
        }
    }//class
}//namespace
