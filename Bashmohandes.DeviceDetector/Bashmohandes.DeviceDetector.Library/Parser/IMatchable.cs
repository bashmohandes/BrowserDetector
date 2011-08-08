using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bashmohandes.DeviceDetector.Library.Parser
{
    interface IMatchable
    {
        Regex MatchExpression { get; }

        Regex NonMatchExpression { get; }
    }
}
