using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace asmref
{
    class Program
    {
        static int Main(string[] args)
        {
            var writer = Console.Out;

            var arguments = Arguments.Parse(args);
            if (arguments.ErrorMessage != null)
            {
                writer.WriteLine();
                writer.WriteLine(arguments.ErrorMessage);
                writer.WriteLine(Arguments.BuildHelpMessage());
                return -1;
            }

            Inspector inspector;
            if (arguments.IsUpward)
            {
                inspector = new UpwardInspector(writer, arguments.IsVerboseOutput);
            }
            else
            {
                inspector = new DownwardInspector(writer, arguments.IsVerboseOutput);
            }

            string rootPath = Directory.GetCurrentDirectory();
            inspector.InspectReferences(rootPath, arguments.AssemblyOrFileName);

            return 0;
        }
    }
}
