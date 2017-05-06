namespace asmref
{
    internal interface IWriter
    {
        void Write(string text, Style style = Style.Normal);

        void WriteLine(string text, Style style = Style.Normal);

        void WriteLine();
    }
}
