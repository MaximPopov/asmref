using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace asmref
{
    internal abstract class Inspector
    {

        protected TextWriter Writer { get; set; }

        protected bool IsVerboseOutput { get; set; }

        protected Inspector(TextWriter writer, bool isVerboseOutput)
        {
            Writer = writer;
            IsVerboseOutput = isVerboseOutput;
        }

        public abstract int InspectReferences(IReadOnlyList<Assembly> assemblies, string assemblyOrFileName);

    }
}
