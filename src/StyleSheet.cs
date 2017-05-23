using System;
using System.Collections.Generic;

namespace asmref
{
    internal sealed class StyleSheet
    {
        private readonly Dictionary<Style, ConsoleColor> _styleDefinitions = new Dictionary<Style, ConsoleColor>
        {
            { Style.Normal, ConsoleColor.Gray },
            { Style.Emphasis, ConsoleColor.White },
            { Style.Error, ConsoleColor.Red },
            { Style.Command, ConsoleColor.Cyan },
        };

        public ConsoleColor GetTextColor(Style style)
        {
            return _styleDefinitions[style];
        }
    }
}
