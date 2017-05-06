using System.Collections.Generic;
using System.Reflection;

namespace asmref
{
    internal abstract class Inspector
    {
        protected const string Arrow = "├─>";
        protected const string LastArrow = "└─>";

        protected IWriter Writer { get; set; }

        protected bool IsVerboseOutput { get; set; }

        protected Inspector(IWriter writer, bool isVerboseOutput)
        {
            Writer = writer;
            IsVerboseOutput = isVerboseOutput;
        }

        public abstract int InspectReferences(IReadOnlyList<Assembly> assemblies, string assemblyOrFileName);

    }
}
