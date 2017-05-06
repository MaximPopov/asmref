using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace asmref
{
    internal class DownwardInspector : Inspector
    {
        public DownwardInspector(IWriter writer, bool isVerboseOutput)
            : base(writer, isVerboseOutput)
        {
        }

        public override int InspectReferences(IReadOnlyList<Assembly> assemblies, string assemblyOrFileName)
        {
            var baseAssembly = assemblies.FirstOrDefault(asm => asm.HasShortName(assemblyOrFileName) || asm.IsLocatedIn(assemblyOrFileName));
            if (baseAssembly == null)
            {
                Writer.WriteLine($@"Cannot find ""{assemblyOrFileName}"" assembly.", Style.Error);
                return 1;
            }

            var referencedAssemblyNames = baseAssembly.GetReferencedAssemblies();
            Array.Sort(referencedAssemblyNames, (n1, n2) => string.CompareOrdinal(n1.FullName, n2.FullName));

            Writer.WriteLine(baseAssembly.GetName().Format(), Style.Emphasis);
            for (int index = 0; index < referencedAssemblyNames.Length; index++)
            {
                var referencedAssemblyName = referencedAssemblyNames[index];
                Writer.WriteLine($"   {(index < referencedAssemblyNames.Length - 1 ? Arrow : LastArrow)} {referencedAssemblyName.Format()}");
            }

            return 0;
        }
    }
}
