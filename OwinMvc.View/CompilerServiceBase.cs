using System;
using System.IO;
using System.Security;
using System.Threading;

namespace OwinMvc.View
{
    internal abstract class CompilerServiceBase
    {
        internal static readonly string DynamicTemplateNamespace = "TempMVC";

        [SecurityCritical]
        protected CompilerServiceBase()
        {

        }


        /// <summary>
        /// Extension of a source file without dot ("cs" for C# files or "vb" for VB.NET files).
        /// </summary>
        public virtual string SourceFileExtension { get { return "cs"; } }


        /// <summary>
        /// Tries to create and return a unique temporary directory.
        /// </summary>
        /// <returns>the (already created) temporary directory</returns>
        protected static string GetDefaultTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), "OwinMvc");
            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }
            return tempDirectory;
        }

        /// <summary>
        /// Returns a new temporary directory ready to be used.
        /// This can be overwritten in subclases to change the created directories.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetTemporaryDirectory()
        {
            try
            {
                return SourceGenerator.TempDir;
                //var tempPath = System.Configuration.ConfigurationManager.AppSettings["CompileTempPath"];
                //if (!string.IsNullOrEmpty(tempPath))
                //{
                //    if (!Directory.Exists(tempPath))
                //    {
                //        Directory.CreateDirectory(tempPath);
                //    }
                //    return tempPath;
                //}
                //else
                //{
                //    return GetDefaultTemporaryDirectory();
                //}
            }
            catch
            {
                return GetDefaultTemporaryDirectory();
            }
        }



        /// <summary>
        /// Compiles the type defined in the specified type context.
        /// </summary>
        /// <param name="context">The type context which defines the type to compile.</param>
        /// <returns>The compiled type.</returns>
        [SecurityCritical]
        public abstract System.Type CompileType(CompileContext context);


        private static long _DynamicAssemblyIndex = 1;

        /// <summary>
        /// Helper method to generate the prefered assembly name.
        /// </summary>
        /// <param name="context">the context of the current compilation.</param>
        /// <returns></returns>
        protected string GetAssemblyName(CompileContext context)
        {
            try
            {
                return String.Format("{0}_{1}.{2}", DynamicTemplateNamespace,
                    Interlocked.Increment(ref _DynamicAssemblyIndex),
                    context.ClassName);
            }
            catch
            {
                //return a + Guid.NewGuid().ToString("N");
                return "DynamicAssembly_" + Interlocked.Increment(ref _DynamicAssemblyIndex);
            }
        }


    }
}
