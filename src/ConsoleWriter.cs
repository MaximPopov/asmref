using System;

namespace asmref
{
    internal sealed class ConsoleWriter : IWriter
    {
        public StyleSheet StyleSheet { get; set; } = new StyleSheet();

        public void Write(string text, Style style)
        {
            WriteInStyle(() => Console.Write(text), style);
        }

        public void WriteLine(string text, Style style)
        {
            WriteInStyle(() => Console.WriteLine(text), style);
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }

        private void WriteInStyle(Action action, Style style)
        {
            if (style != Style.Normal)
            {
                Console.ForegroundColor = StyleSheet.GetTextColor(style);
            }

            action();

            if (style != Style.Normal)
            {
                Console.ResetColor();
            }
        }
    }
}
