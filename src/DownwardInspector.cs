using System;
using System.IO;

namespace asmref
{
    class DownwardInspector : Inspector
    {
        public DownwardInspector(TextWriter writer, bool isVerboseOutput)
            : base(writer, isVerboseOutput)
        {

        }

        public override void InspectReferences(string rootPath, string assemblyOrFileName)
        {
            throw new NotImplementedException();
        }
    }
}
