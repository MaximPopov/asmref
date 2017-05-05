using System;
using System.IO;

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

            var rootPath = Directory.GetCurrentDirectory();
            var loader = new AssemblyLoader();
            var assemblies = loader.LoadAssemblies(rootPath);

            writer.WriteLine($"Number of assemblies found in the current folder: {assemblies.Count}");
            if (assemblies.Count == 0)
            {
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

            return inspector.InspectReferences(assemblies, arguments.AssemblyOrFileName);
        }

    }
}
