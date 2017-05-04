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
        private static readonly Regex AssemblyExtensionRegex = new Regex(@".(dll)|(exe)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
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

            int scannedCount = 0;
            int foundCount = 0;

            var results = new Dictionary<string, List<AssemblyName>>();
            string shortAssemblyName = arguments.AssemblyOrFileName;

            string rootPath = Directory.GetCurrentDirectory();

            if (AssemblyExtensionRegex.IsMatch(arguments.AssemblyOrFileName))
            {
                var assembly = Load(Path.Combine(rootPath, arguments.AssemblyOrFileName), rootPath, writer, arguments.IsVerboseOutput);
                if (assembly != null)
                {
                    shortAssemblyName = assembly.GetName().Name;
                }
            }

            foreach (var fullAssemblyPath in Directory.EnumerateFiles(rootPath, "*.*", SearchOption.TopDirectoryOnly))
            {
                var extension = Path.GetExtension(fullAssemblyPath);
                if (extension == null || !AssemblyExtensionRegex.IsMatch(extension))
                {
                    continue;
                }

                var relativeAssemblyPath = Path.GetFileName(fullAssemblyPath);
                var assembly = Load(fullAssemblyPath, relativeAssemblyPath, writer, arguments.IsVerboseOutput);

                if (assembly == null)
                {
                    continue;
                }

                scannedCount++;

                foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
                {
                    if (string.Equals(referencedAssemblyName.Name, shortAssemblyName, StringComparison.OrdinalIgnoreCase))
                    {
                        shortAssemblyName = referencedAssemblyName.Name;

                        List<AssemblyName> assemblyNames;
                        if (!results.TryGetValue(referencedAssemblyName.FullName, out assemblyNames))
                        {
                            assemblyNames = new List<AssemblyName>();
                            results.Add(referencedAssemblyName.FullName, assemblyNames);
                        }
                        assemblyNames.Add(assembly.GetName());

                        foundCount++;
                    }
                }
            }

            var referencedAssemblyNames = results.Keys.ToArray();
            Array.Sort(referencedAssemblyNames);

            foreach (var referencedAssemblyName in referencedAssemblyNames)
            {
                var foundAssemblyNames = results[referencedAssemblyName];

                foundAssemblyNames.Sort((n1, n2) => string.CompareOrdinal(n1.FullName, n2.FullName));

                writer.WriteLine();
                foreach (var foundAssemblyName in foundAssemblyNames)
                {
                    writer.WriteLine(foundAssemblyName.FullName);
                }
                writer.WriteLine($"\t=> {referencedAssemblyName}");
            }

            writer.WriteLine();
            writer.WriteLine($"Number of assemblies that reference '{shortAssemblyName ?? arguments.AssemblyOrFileName}': {foundCount}");
            writer.WriteLine($"Number of assemblies scanned: {scannedCount}");

            return 0;
        }

        private static Assembly Load(string fullAssemblyPath, string relativeAssemblyPath, TextWriter writer, bool isVerboseOutput)
        {
            try
            {
                return Assembly.ReflectionOnlyLoadFrom(fullAssemblyPath);
            }
            catch (Exception)
            {
                if (isVerboseOutput)
                {
                    writer.WriteLine($"\t!!! {relativeAssemblyPath} failed to load into reflection context (perhaps it's not an assembly?)");
                }
                return null;
            }
        }
    }
}
