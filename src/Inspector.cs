using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace asmref
{
    abstract class Inspector
    {
        protected static readonly Regex AssemblyExtensionRegex = new Regex(@".(dll)|(exe)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected TextWriter Writer { get; set; }

        protected bool IsVerboseOutput { get; set; }

        public Inspector(TextWriter writer, bool isVerboseOutput)
        {
            Writer = writer;
            IsVerboseOutput = IsVerboseOutput;
        }

        public abstract void InspectReferences(string rootPath, string assemblyOrFileName);

        protected Assembly Load(string fullAssemblyPath)
        {
            try
            {
                return Assembly.ReflectionOnlyLoadFrom(fullAssemblyPath);
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected bool IsSameName(AssemblyName assemblyName, string shortAssemblyName)
        {
            return string.Equals(assemblyName.Name, shortAssemblyName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
