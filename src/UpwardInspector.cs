using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace asmref
{
    internal sealed class UpwardInspector : Inspector
    {
        public UpwardInspector(IWriter writer)
            : base(writer)
        {
        }

        public override int InspectReferences(IReadOnlyList<Assembly> assemblies, string assemblyOrFileName, ISet<string> foundAssemblyNames)
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

            var results = new Dictionary<string, List<AssemblyName>>(StringComparer.OrdinalIgnoreCase);
            var foundCount = 0;

            foreach (var assembly in assemblies)
            {
                foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
                {
                    if (referencedAssemblyName.HasShortName(shortAssemblyName))
                    {
                        // This is to set the correct case if it hasn't been done earlier.
                        shortAssemblyName = referencedAssemblyName.Name;

                        string formattedReferencedAssemblyName = referencedAssemblyName.Format();
                        List<AssemblyName> assemblyNames;
                        if (!results.TryGetValue(formattedReferencedAssemblyName, out assemblyNames))
                        {
                            assemblyNames = new List<AssemblyName>();
                            results.Add(formattedReferencedAssemblyName, assemblyNames);
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
            Array.Sort(referencedAssemblyNames);

            foreach (var referencedAssemblyName in referencedAssemblyNames)
            {
                var assemblyNames = results[referencedAssemblyName];

                assemblyNames.Sort((n1, n2) => string.CompareOrdinal(n1.FullName, n2.FullName));

                Writer.WriteLine();
                foreach (var assemblyName in assemblyNames)
                {
                    Writer.WriteLine(assemblyName.Format());
                }
                Writer.WriteLine($"   {LastArrow} {referencedAssemblyName}", Style.Emphasis);
            }

            Writer.WriteLine();
            Writer.WriteLine($"Number of assemblies that reference '{shortAssemblyName ?? assemblyOrFileName}': {foundCount}");

            if (foundAssemblyNames != null)
            {
                foreach (var assemblyName in results.Values.SelectMany(n => n))
                {
                    foundAssemblyNames.Add(assemblyName.Name);
                }
            }

            return 0;
        }
    }
}
