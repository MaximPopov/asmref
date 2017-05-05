using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace asmref
{
    internal sealed class AssemblyLoader
    {
        public List<Assembly> LoadAssemblies(string rootPath)
        {
            var assemblies = new List<Assembly>();

            foreach (var fullAssemblyPath in Directory.EnumerateFiles(rootPath, "*.*", SearchOption.TopDirectoryOnly))
            {
                if (fullAssemblyPath.LooksLikeAssemblyFile())
                {
                    var assembly = LoadAssembly(fullAssemblyPath);
                    if (assembly != null)
                    {
                        assemblies.Add(assembly);
                    }
                }
            }

            return assemblies;
        }

        private static Assembly LoadAssembly(string fullAssemblyPath)
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
    }
}
