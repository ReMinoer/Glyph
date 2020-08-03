using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Glyph.Export
{
    static public class TypeImporter
    {
        static public List<Type> GetTypesFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            var importedTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
                foreach (AssemblyContainsAttribute attribute in assembly.GetCustomAttributes<AssemblyContainsAttribute>())
                    importedTypes.AddRange(assembly.GetExportedTypes().Where(t => attribute.Type.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract));
            
            return importedTypes;
        }

        static public List<Type> GetTypesFromLocalAssemblies()
        {
            string assemblyLocation = Assembly.GetAssembly(typeof(TypeImporter)).Location;
            string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
            if (assemblyDirectory == null)
                throw new InvalidOperationException();

            Assembly CurrentDomainOnReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs e) => Assembly.ReflectionOnlyLoad(e.Name);
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomainOnReflectionOnlyAssemblyResolve;

            IEnumerable<Assembly> assemblies = Directory.GetFiles(assemblyDirectory, "*.dll", SearchOption.TopDirectoryOnly)
                                                        .Where(x => !IgnoredDlls.Contains(Path.GetFileName(x)))
                                                        .Select(Assembly.ReflectionOnlyLoadFrom)
                                                        .Where(HasAssemblyContainsAttribute)
                                                        .ToArray();

            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomainOnReflectionOnlyAssemblyResolve;
            
            return GetTypesFromAssemblies(assemblies.Select(a => Assembly.Load(a.GetName())));
        }

        static private bool HasAssemblyContainsAttribute(Assembly a)
        {
            try
            {
                return a.CustomAttributes.Any(x => x.AttributeType.Name == typeof(AssemblyContainsAttribute).Name);
            }
            catch (IOException)
            {
                return false;
            }
        }

        static private string[] IgnoredDlls =
        {
            "netstandard.dll",
            // "Module was expected to contain an assembly manifest."
            "assimp.dll",
            "FreeImage.dll",
            "freetype6.dll",
            "libmojoshader_64.dll",
            "nvtt.dll",
            "PVRTexLibWrapper.dll"
        };
    }
}