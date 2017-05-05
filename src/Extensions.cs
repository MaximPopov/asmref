using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace asmref
{
    internal static class Extensions
    {
        private static readonly Regex AssemblyExtensionRegex = new Regex(@".(dll)|(exe)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool LooksLikeAssemblyFile(this string path)
        {
            var extension = Path.GetExtension(path);
            return extension != null && AssemblyExtensionRegex.IsMatch(extension);
        }

        public static bool HasShortName(this Assembly assembly, string shortAssemblyName)
        {
            return HasShortName(assembly.GetName(), shortAssemblyName);
        }

        public static bool HasShortName(this AssemblyName assemblyName, string shortAssemblyName)
        {
            return string.Equals(assemblyName.Name, shortAssemblyName, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsLocatedIn(this Assembly assembly, string fileName)
        {
            return string.Equals(Path.GetFileName(assembly.CodeBase), fileName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
