using OwinMvc.Web;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Razor;

namespace OwinMvc.View
{
    internal static class SourceGenerator
    {

        internal static readonly string TempDir = @"c:\temp_razorGen";

        static SourceGenerator()
        {
            try
            {
                if (!System.IO.Directory.Exists(TempDir))
                {
                    System.IO.Directory.CreateDirectory(TempDir);
                }
            }
            catch
            {

            }
        }


        private static string GetSourceFile(string virtualPath)
        {
            StringBuilder sb = new StringBuilder(255);            
            foreach(char c   in virtualPath)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                }
                else if (c == '.')
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append("_");
                }
            }
            // return sb.ToString();

            return System.IO.Path.Combine(TempDir, sb.ToString());
        }


        internal static CodeCompileUnit GetCompileUnit(string virtualPath)
        {
            var physicalPath = MyVirtualPathProvider.MapPath(virtualPath);
            var host = new System.Web.Mvc.Razor.MvcWebPageRazorHost(virtualPath, physicalPath);

            // host.DefaultPageBaseClass = typeof(System.Web.Mvc.WebViewPage).FullName;
            host.DefaultPageBaseClass = typeof(MyViewPage).FullName;
            
            host.NamespaceImports.Remove(typeof(System.Web.WebPages.HelperPage).Namespace);
            host.NamespaceImports.Remove(typeof(System.Web.WebPages.Html.HtmlHelper).Namespace);

            host.NamespaceImports.Add(typeof(System.Web.Mvc.HtmlHelper).Namespace);
            host.NamespaceImports.Add(typeof(System.Web.Mvc.Html.PartialExtensions).Namespace);
            host.NamespaceImports.Add(typeof(System.Web.Mvc.Ajax.AjaxExtensions).Namespace);


            var sourceFile = GetSourceFile(virtualPath);

            var engine = new RazorTemplateEngine(host);
            GeneratorResults result;
            using (var reader = new System.IO.StreamReader(physicalPath, System.Text.Encoding.UTF8))
            {
                result = engine.GenerateCode(reader, null, null, sourceFile);
            }


            if (result.Success)
            {
                //// 写文件出错时忽略，因为这时调试用的，实际运行不依赖于这个文件。
                //try
                //{
                //    var tempDir = this.GetTemporaryDirectory();
                //    var sourceCodeFile = Path.Combine(tempDir, context.ClassName + ".cs");
                //    var sourceCode = this.InspectSource(result.GeneratedCode);
                //    //File.WriteAllText(sourceCodeFile, sourceCode);
                //    using (StreamWriter writer = new StreamWriter(sourceCodeFile, false, Encoding.UTF8))
                //    {
                //        writer.Write(sourceCode);
                //        writer.Flush();
                //    }
                //}
                //catch (Exception ex)
                //{
                //    TraceHelper.Error(ex.ToString());
                //}

                return result.GeneratedCode;
            }

            StringBuilder errorBuilder = new StringBuilder(1024);
            errorBuilder.Append("razor翻译错误: \r\n");

            if (result.ParserErrors != null)
            {
                foreach (var error in result.ParserErrors)
                {
                    errorBuilder.Append("Location=").Append(error.Location)
                        .Append("; Length=").Append(error.Length)
                        .Append("; Message=").Append(error.Message)
                        .Append("\r\n");
                }
            }

            throw new ApplicationException(errorBuilder.ToString());            
        }



        /// <summary>
        /// Inspects the GeneratorResults and returns the source code.
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static string InspectSource(CodeCompileUnit compileUnit)
        {
            //string generatedCode;
            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder, CultureInfo.InvariantCulture))
            {
                using (var codeDomProvider = new Microsoft.CSharp.CSharpCodeProvider())
                {
                    codeDomProvider.GenerateCodeFromCompileUnit(compileUnit, writer, new CodeGeneratorOptions());
                }
                //generatedCode = builder.ToString();
            }
            return builder.ToString();
        }


    }
}
