using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace asmref
{
    internal sealed class UpwardInspector : Inspector
    {
        public UpwardInspector(IWriter writer, bool isVerboseOutput)
            : base(writer, isVerboseOutput)
        {
        }

        public override int InspectReferences(IReadOnlyList<Assembly> assemblies, string assemblyOrFileName)
        {
            string shortAssemblyName;
            var baseAssembly = assemblies.FirstOrDefault(asm => asm.HasShortName(assemblyOrFileName) || asm.IsLocatedIn(assemblyOrFileName));

            if (baseAssembly != null)
            {
                // This is to set the correct case.
                shortAssemblyName = baseAssembly.GetName().Name;
            }
            else if (assemblyOrFileName.LooksLikeAssemblyFile())
            {
                shortAssemblyName = Path.GetFileNameWithoutExtension(assemblyOrFileName);
            }
            else
            {
                shortAssemblyName = assemblyOrFileName;
            }

            var results = new Dictionary<AssemblyName, List<AssemblyName>>();
            var foundCount = 0;

            foreach (var assembly in assemblies)
            {
                foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
                {
                    if (referencedAssemblyName.HasShortName(shortAssemblyName))
                    {
                        // This is to set the correct case if it hasn't been done earlier.
                        shortAssemblyName = referencedAssemblyName.Name;

                        List<AssemblyName> assemblyNames;
                        if (!results.TryGetValue(referencedAssemblyName, out assemblyNames))
                        {
                            assemblyNames = new List<AssemblyName>();
                            results.Add(referencedAssemblyName, assemblyNames);
                        }
                        assemblyNames.Add(assembly.GetName());

                        foundCount++;
                    }
                }
            }

            if (baseAssembly != null)
            {
                Writer.Write("Found assembly ");
                Writer.WriteLine(baseAssembly.GetName().Format(), Style.Emphasis);
            }

            var referencedAssemblyNames = results.Keys.ToArray();
            Array.Sort(referencedAssemblyNames, (n1, n2) => string.CompareOrdinal(n1.FullName, n2.FullName));

            foreach (var referencedAssemblyName in referencedAssemblyNames)
            {
                var foundAssemblyNames = results[referencedAssemblyName];

                foundAssemblyNames.Sort((n1, n2) => string.CompareOrdinal(n1.FullName, n2.FullName));

                Writer.WriteLine();
                foreach (var foundAssemblyName in foundAssemblyNames)
                {
                    Writer.WriteLine(foundAssemblyName.Format());
                }
                Writer.WriteLine($"\t└─> {referencedAssemblyName.Format()}", Style.Emphasis);
            }

            Writer.WriteLine();
            Writer.WriteLine($"Number of assemblies that reference '{shortAssemblyName ?? assemblyOrFileName}': {foundCount}");

            return 0;
        }
    }
}
