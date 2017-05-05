using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace asmref
{
    internal class DownwardInspector : Inspector
    {
        public DownwardInspector(TextWriter writer, bool isVerboseOutput)
            : base(writer, isVerboseOutput)
        {

        }

        public override int InspectReferences(IReadOnlyList<Assembly> assemblies, string assemblyOrFileName)
        {
            var baseAssembly = assemblies.FirstOrDefault(asm => asm.HasShortName(assemblyOrFileName) || asm.IsLocatedIn(assemblyOrFileName));
            if (baseAssembly == null)
            {
                Writer.WriteLine($@"Cannot find ""{assemblyOrFileName}"" assembly.");
                return 1;
            }

            var referencedAssemblyNames = baseAssembly.GetReferencedAssemblies();
            Array.Sort(referencedAssemblyNames, (n1, n2) => string.CompareOrdinal(n1.FullName, n2.FullName));

            Writer.WriteLine(baseAssembly.GetName());
            foreach (var referencedAssemblyName in referencedAssemblyNames)
            {
                Writer.WriteLine($" └─> {referencedAssemblyName}");
            }
            
            return 0;
        }
    }
}
