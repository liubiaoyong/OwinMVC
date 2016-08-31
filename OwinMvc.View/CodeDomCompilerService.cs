using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.WebPages;

namespace OwinMvc.View
{
    /// <summary>
    /// Base compiler service class for roslyn compilers
    /// </summary>
    internal sealed class CodeDomCompilerService : CompilerServiceBase
    {

        /// <summary>
        /// Creates a new instance of the <see cref="CodeDomCompilerService"/> class.
        /// </summary>
        /// <param name="codeLanguage"></param>
        /// <param name="markupParserFactory"></param>
        public CodeDomCompilerService()
            : base()
        {
            this.ReferenceResolver = new DefaultReferencedAssemblyProvider();
        }


        private readonly IReferenceResolver ReferenceResolver;

        ///// <summary>
        ///// Returns "cs".
        ///// </summary>
        //public override string SourceFileExtension
        //{
        //    get { return "cs"; }
        //}




        /// <summary>
        /// Check for mono runtime as Roslyn doesn't support generating debug symbols on mono/unix
        /// </summary>
        /// <returns></returns>
        private static bool IsMono()
        {
            return Type.GetType("Mono.Runtime") != null;
            //return System.Environment.OSVersion.Platform == PlatformID.Unix;
        }


        /// <summary>
        /// Configures and runs the compiler.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override System.Type CompileType(CompileContext context)
        {
            var sourceCode = context.SourceCode;

            var assemblyName = GetAssemblyName(context);

            //(new PermissionSet(PermissionState.Unrestricted)).Assert();
            var tempDir = GetTemporaryDirectory();

            var sourceCodeFile = Path.Combine(tempDir, String.Format("{0}.{1}", assemblyName, SourceFileExtension));

            // 写文件出错时忽略，因为这时调试用的，实际运行不依赖于这个文件。
            try
            {
                //File.WriteAllText(sourceCodeFile, sourceCode);
                using (StreamWriter writer = new StreamWriter(sourceCodeFile, false, Encoding.UTF8))
                {
                    writer.Write(sourceCode);
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                TraceHelper.Error(ex.ToString());
            }

            using (var codeDomProvider = new CSharpCodeProvider())
            {
                CompilerParameters cp = new CompilerParameters();
                //cp.ReferencedAssemblies.Add();
                cp.GenerateExecutable = false;
                cp.GenerateInMemory = true;
                cp.IncludeDebugInformation = true;
                cp.OutputAssembly = assemblyName;
                cp.ReferencedAssemblies.AddRange(this.GetAllReferences(context).ToArray());

                var compileResult = codeDomProvider.CompileAssemblyFromSource(cp, sourceCode);

                if (compileResult.Errors != null && compileResult.Errors.Count > 0)
                {
                    var errorBuilder = new StringBuilder(4096);
                    int errorCount = 0;
                    for (int i = 0; i < compileResult.Errors.Count; i++)
                    {
                        var error = compileResult.Errors[i];
                        if (!error.IsWarning)
                        {
                            errorCount += 1;
                            errorBuilder.AppendLine(error.ErrorText);
                            errorBuilder.Append(error.FileName)
                                .Append("line : ").Append(error.Line)
                                .Append("col :").Append(error.Column).AppendLine().AppendLine();
                        }
                    }

                    if (errorCount > 0)
                    {
                        throw new ApplicationException(errorBuilder.ToString());
                    }
                }

                Assembly assembly = compileResult.CompiledAssembly;
                //var type = assembly.GetTypes()[0];
                // var type = assembly.GetType(context.FullClassName);

                var type = GetWebPageType(assembly, context);

                if (type == null)
                {
                    throw new ApplicationException("type==null");
                }
                return type;
            }


        }


        private static System.Type GetWebPageType(Assembly assembly, CompileContext context)
        {
            var type = assembly.GetTypes().FirstOrDefault(p => typeof(WebPageBase).IsAssignableFrom(p));
            if (type == null)
            {
                throw new ApplicationException("编译后的程序程序集中没有视图类型.");
            }
            return type;
        }

        internal IEnumerable<string> GetAllReferences(CompileContext compileContext)
        {
            return this.ReferenceResolver.GetReferencedAssemblyLocations(compileContext);
        }


    }
}
