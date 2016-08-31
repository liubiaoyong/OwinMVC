using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OwinMvc.View
{
    /// <summary>
    /// Resolves the assemblies by using all currently loaded assemblies. See <see cref="IReferenceResolver"/>
    /// </summary>
    internal sealed class DefaultReferencedAssemblyProvider : IReferenceResolver
    {


        private static ICollection<Assembly> _referencedAssemblies;
        private static readonly object stateLocker = new object();
        


        /// <summary>
        /// Gets an enumerable of all assemblies loaded in the current domain.
        /// </summary>
        /// <returns>An enumerable of loaded assemblies.</returns>
        internal static IEnumerable<Assembly> GetLoadedAssemblies()
        {
            var domain = AppDomain.CurrentDomain;
            return domain.GetAssemblies();
        }


        /// <summary>
        /// See <see cref="IReferenceResolver.GetReferencedAssemblyLocations"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="includeAssemblies"></param>
        /// <returns></returns>
        public static ICollection<Assembly> GetReferencedAssemblies()
        {
            if (_referencedAssemblies == null)
            {
                lock(stateLocker)
                {
                    if (_referencedAssemblies == null)
                    {
                        _referencedAssemblies =  GetLoadedAssemblies()
                       .Where(a => !a.IsDynamic && File.Exists(a.Location))
                       .GroupBy(a => a.GetName().Name)
                       .Select(grp => grp.First(y => y.GetName().Version == grp.Max(x => x.GetName().Version))) // only select distinct assemblies based on FullName to avoid loading duplicate assemblies
                       .ToList();
                    }
                    return _referencedAssemblies;
                }
            }
            return _referencedAssemblies;
        }
        ////    return CompilerServicesUtility
        ////           .GetLoadedAssemblies()
        ////           .Where(a => !a.IsDynamic && File.Exists(a.Location) && !a.Location.Contains(RazorEngine.DynamicTemplateNamespace))
        ////           .GroupBy(a => a.GetName().Name).Select(grp => grp.First(y => y.GetName().Version == grp.Max(x => x.GetName().Version))) // only select distinct assemblies based on FullName to avoid loading duplicate assemblies
        ////           .Select(a => CompilerReference.From(a))
        ////           .Concat(includeAssemblies ?? Enumerable.Empty<CompilerReference>());
        ////}


        /// <summary>
        /// See <see cref="IReferenceResolver.GetReferencedAssemblyLocations"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="includeAssemblies"></param>
        /// <returns></returns>
        public IEnumerable<string> GetReferencedAssemblyLocations(CompileContext compileContext)
        {
            ////return GetReferencedAssemblies()
            ////       .Where(a => !a.IsDynamic && File.Exists(a.Location) && !a.Location.Contains(CompilerServiceBase.DynamicTemplateNamespace))
            ////       .GroupBy(a => a.GetName().Name)
            ////       .Select(grp => grp.First(y => y.GetName().Version == grp.Max(x => x.GetName().Version))) // only select distinct assemblies based on FullName to avoid loading duplicate assemblies
            ////       .Select(a => a.Location);

            return GetReferencedAssemblies().Select(a => a.Location);
        }


        //public IEnumerable

    }
}
