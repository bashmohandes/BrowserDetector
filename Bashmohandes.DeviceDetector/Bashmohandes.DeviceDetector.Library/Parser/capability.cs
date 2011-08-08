using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bashmohandes.DeviceDetector.Library.Parser
{
    partial class capability : IMatchable
    {
        private Regex regexMatch, regexNonMatch;

        public Regex MatchExpression
        {
            get
            {
                if (!String.IsNullOrEmpty(this.match))
                {
                    if (this.regexMatch != null)
                        return this.regexMatch;

                    this.regexMatch = new Regex(this.match);
                }
                return this.regexMatch;
            }
        }

        public Regex NonMatchExpression
        {
            get
            {
                if (!String.IsNullOrEmpty(this.nonMatch))
                {
                    if (this.regexNonMatch != null)
                        return this.regexNonMatch;

                    this.regexNonMatch = new Regex(this.nonMatch);
                }
                return this.regexNonMatch;
            }
        }
    }
}
