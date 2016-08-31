using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwinMvc
{
    internal static class AssemblyUtility
    {
        internal static IEnumerable<System.Type> SafeGetAvaliableTypes(this System.Reflection.Assembly assembly)
        {
             Type[] assemblyTypes;
             try
             {
                 assemblyTypes = assembly.GetTypes();
             }
             catch (System.Reflection.ReflectionTypeLoadException ex)
             {
                 assemblyTypes = ex.Types;

                 TraceHelper.Error(ex.ToString());
                 if (ex.LoaderExceptions != null && ex.LoaderExceptions.Length > 0)
                 {
                     foreach (var ex2 in ex.LoaderExceptions)
                     {
                         TraceHelper.Error(ex.ToString());
                     }
                 }
             }
             catch (Exception ex)
             {
                 assemblyTypes = new Type[0];
                 TraceHelper.Error(ex.ToString());
             }

             return assemblyTypes;
                
        }
    }
}
