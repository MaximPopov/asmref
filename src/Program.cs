using System;
using System.Collections.Generic;
using System.IO;

namespace asmref
{
    class Program
    {
        static int Main(string[] args)
        {
            IWriter writer = new ConsoleWriter();

            var arguments = Arguments.Parse(args);
            if (arguments.ErrorMessage != null)
            {
                writer.WriteLine();
                writer.WriteLine(arguments.ErrorMessage, Style.Error);
                writer.WriteLine(Arguments.BuildHelpMessage());
                return -1;
            }

            var rootPath = Directory.GetCurrentDirectory();
            var loader = new AssemblyLoader();
            var assemblies = loader.LoadAssemblies(rootPath);

            writer.WriteLine($"Number of assemblies found in the current folder: {assemblies.Count}");
            writer.WriteLine();
            if (assemblies.Count == 0)
            {
                return -1;
            }

            Inspector inspector;
            if (arguments.IsUpward)
            {
                inspector = new UpwardInspector(writer);
            }
            else
            {
                inspector = new DownwardInspector(writer);
            }

            if (!arguments.IsInteractiveMode)
            {
                return inspector.InspectReferences(assemblies, arguments.AssemblyOrFileName, null);
            }

            var foundAssemblyNames = new HashSet<string>();
            var assemblyNameStack = new Stack<string>();
            var assemblyName = arguments.AssemblyOrFileName;

            while (true)
            {
                var returnCode = inspector.InspectReferences(assemblies, assemblyName, foundAssemblyNames);
                if (returnCode > 0)
                {
                    return returnCode;
                }

                assemblyName = PickAssemblyName(foundAssemblyNames, assemblyNameStack, assemblyName, writer);
                if (assemblyName == null)
                {
                    return 0;
                }

                foundAssemblyNames.Clear();
            }
        }

        private static string PickAssemblyName(HashSet<string> foundAssemblyNames, Stack<string> assemblyNameStack, string currentAssemblyName, IWriter writer)
        {
            var nameMap = new Dictionary<char, string>();

            writer.WriteLine();
            char indexChar = '1';
            foreach (var assemblyName in foundAssemblyNames)
            {
                writer.WriteLine($"{char.ToUpper(indexChar)}) {assemblyName}", Style.Command);

                nameMap[indexChar] = assemblyName;
                if (indexChar == '9')
                {
                    indexChar = 'a';
                }
                else if (indexChar == 'z')
                {
                    break;
                }
                else
                {
                    indexChar++;
                }
            }

            if (assemblyNameStack.Count > 0)
            {
                writer.WriteLine($"BACKSPACE) Back to {assemblyNameStack.Peek()}", Style.Command);
            }
            writer.WriteLine("ESC) Quit", Style.Command);
            writer.WriteLine();

            while (true)
            {
                var keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    return null;
                }
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (assemblyNameStack.Count > 0)
                    {
                        return assemblyNameStack.Pop();
                    }
                }
                else
                {
                    string assemblyName;
                    if (nameMap.TryGetValue(keyInfo.KeyChar, out assemblyName))
                    {
                        assemblyNameStack.Push(currentAssemblyName);
                        return assemblyName;
                    }
                }
            }
        }
    }
}
