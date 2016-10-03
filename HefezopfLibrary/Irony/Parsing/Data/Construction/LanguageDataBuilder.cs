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
using System.Diagnostics;

namespace Irony.Parsing.Construction
{
    internal class LanguageDataBuilder
    {
        internal LanguageData Language;
        private Grammar _grammar;

        public LanguageDataBuilder(LanguageData language)
        {
            this.Language = language;
            this._grammar = this.Language.Grammar;
        }

        public bool Build()
        {
            var sw = new Stopwatch();
            try
            {
                if (this._grammar.Root == null)
                {
                    this.Language.Errors.AddAndThrow(GrammarErrorLevel.Error, null, Resources.ErrRootNotSet);
                }

                sw.Start();
                var gbld = new GrammarDataBuilder(this.Language);
                gbld.Build();
                //Just in case grammar author wants to customize something...
                this._grammar.OnGrammarDataConstructed(this.Language);
                var sbld = new ScannerDataBuilder(this.Language);
                sbld.Build();
                var pbld = new ParserDataBuilder(this.Language);
                pbld.Build();
                this.Validate();
                //call grammar method, a chance to tweak the automaton
                this._grammar.OnLanguageDataConstructed(this.Language);
                return true;
            }
            catch (GrammarErrorException)
            {
                return false; //grammar error should be already added to Language.Errors collection
            }
            finally
            {
                this.Language.ErrorLevel = this.Language.Errors.GetMaxLevel();
                sw.Stop();
                this.Language.ConstructionTime = sw.ElapsedMilliseconds;
            }
        }

        #region Language Data Validation
        private void Validate()
        {
        }//method
        #endregion
    }//class
}
