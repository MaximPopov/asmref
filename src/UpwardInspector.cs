using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace asmref
{
    sealed class UpwardInspector : Inspector
    {
        public UpwardInspector(TextWriter Writer, bool isVerboseOutput)
            : base(Writer, isVerboseOutput)
        {
        }

        public override void InspectReferences(string rootPath, string assemblyOrFileName)
        {
            int scannedCount = 0;
            int foundCount = 0;

            var results = new Dictionary<string, List<AssemblyName>>();
            string shortAssemblyName = assemblyOrFileName;
            Assembly baseAssembly = null;

            if (AssemblyExtensionRegex.IsMatch(assemblyOrFileName))
            {
                baseAssembly = Load(Path.Combine(rootPath, assemblyOrFileName));
                if (baseAssembly != null)
                {
                    shortAssemblyName = baseAssembly.GetName().Name;
                }
                else
                {
                    shortAssemblyName = Path.GetFileNameWithoutExtension(assemblyOrFileName);
                }
            }

            foreach (var fullAssemblyPath in Directory.EnumerateFiles(rootPath, "*.*", SearchOption.TopDirectoryOnly))
            {
                var extension = Path.GetExtension(fullAssemblyPath);
                if (extension == null || !AssemblyExtensionRegex.IsMatch(extension))
                {
                    continue;
                }

                var assembly = Load(fullAssemblyPath);
                if (assembly == null)
                {
                    if (IsVerboseOutput)
                    {
                        Writer.WriteLine($"\t!!! {Path.GetFileName(fullAssemblyPath)} failed to load into reflection context (perhaps it's not an assembly?)");
                    }
                    continue;
                }

                scannedCount++;

                // Check if this is the base assembly.
                if (IsSameName(assembly.GetName(), shortAssemblyName))
                {
                    if (baseAssembly == null)
                    {
                        baseAssembly = assembly;
                    }
                    // Base assembly can't reference itself, so skip.
                    continue;
                }

                foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
                {
                    if (IsSameName(referencedAssemblyName, shortAssemblyName))
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

            if (baseAssembly != null)
            {
                Writer.WriteLine();
                Writer.WriteLine($"Found assembly {baseAssembly.GetName()}");
            }

            var referencedAssemblyNames = results.Keys.ToArray();
            Array.Sort(referencedAssemblyNames);

            foreach (var referencedAssemblyName in referencedAssemblyNames)
            {
                var foundAssemblyNames = results[referencedAssemblyName];

                foundAssemblyNames.Sort((n1, n2) => string.CompareOrdinal(n1.FullName, n2.FullName));

                Writer.WriteLine();
                foreach (var foundAssemblyName in foundAssemblyNames)
                {
                    Writer.WriteLine(foundAssemblyName.FullName);
                }
                Writer.WriteLine($"\t=> {referencedAssemblyName}");
            }

            Writer.WriteLine();
            Writer.WriteLine($"Number of assemblies that reference '{shortAssemblyName ?? assemblyOrFileName}': {foundCount}");
            Writer.WriteLine($"Number of assemblies scanned: {scannedCount}");
        }
    }
}
