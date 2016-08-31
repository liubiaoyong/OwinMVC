using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwinMvc.View
{
    internal interface IReferenceResolver
    {
       

        /// <summary>
        /// 主要是用来获取
        /// </summary>
        /// <param name="context">gives context about the compilation process.</param>
        /// <param name="includeAssemblies">The references that should be included (requested by the compiler itself)</param>
        /// <returns>the references which will be used in the compilation process.</returns>
        IEnumerable<string> GetReferencedAssemblyLocations(CompileContext context);
    }
}
