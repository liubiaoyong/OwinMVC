using OwinMvc.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace OwinMvc.View
{
    internal class TypeContext
    {
        //private static readonly string TemplateFileExtension = ".cshtml";
        private static readonly System.Type _templateType = typeof(System.Web.Mvc.WebViewPage<>);

        private static long _classIndex = 0;

        internal string VirtualPath { get; private set; }

        internal string ClassName { get; private set; }


        public string Namespace
        {
            get { return CompilerServiceBase.DynamicTemplateNamespace; }
        }

        public string FullClassName { get; private set; }


        public string TemplateSourceCode { get; private set; }

        /// <summary>
        /// Gets or sets the base template type.
        /// </summary>
        public Type TemplateType { get { return _templateType; } }


        ///// <summary>
        ///// Gets or sets the model type.
        ///// </summary>
        //public Type ModelType { get; set; }

        /// <summary>
        /// Gets the set of namespace imports.
        /// </summary>
        public ISet<string> UsingNamespaces { get; private set; }



        /// <summary>
        /// Initialises a new instance of <see cref="TypeContext"/>.
        /// </summary>
        internal TypeContext(string virtualPath)
        {
            this.VirtualPath = virtualPath;
            this.ClassName = EncodeFilePathToFileName(this.VirtualPath);
            this.FullClassName = this.Namespace + "." + this.ClassName;
            this.UsingNamespaces = new HashSet<string>()
                                {
                                    "System",
                                    "System.Collections.Generic",
                                    "System.Linq",                                   
                                };

            var physicalPath = MyVirtualPathProvider.MapPath(virtualPath);
            using (StreamReader reader = new StreamReader(physicalPath))
            {
                this.TemplateSourceCode = reader.ReadToEnd();
            }
        }





        /// <summary>
        /// 将文件路径替换为可行的文件名
        /// 将所有非大小写字母，还有数组替换为数字和大写字母的结合
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        private static string EncodeFilePathToFileName(string virtualPath)
        {
            if (String.IsNullOrWhiteSpace(virtualPath))
            {
                throw new ArgumentException("url can't be empty");
            }

            var sb = new StringBuilder();
            foreach (var c in virtualPath)
            {
                // only accept alphanumeric chars
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                }
                // otherwise encode them in UTF8
                else
                {
                    sb.Append("_");
                    foreach (var b in Encoding.UTF8.GetBytes(new[] { c }))
                    {
                        sb.Append(b.ToString("X"));
                    }
                }
            }
            sb.Append("_").Append(Interlocked.Increment(ref _classIndex));

            return sb.ToString();
        }



        //public static string GetClassNameByVirtualPath(string fullPath)
        //{
        //    string virtualPath;
        //    if (fullPath.StartsWith(MapPathBasedVirtualPathProvider.AppRoot, StringComparison.OrdinalIgnoreCase))
        //    {
        //        virtualPath = fullPath.Substring(MapPathBasedVirtualPathProvider.AppRoot.Length);
        //        if (virtualPath != null & virtualPath.Length > 0)
        //        {
        //            virtualPath = virtualPath.TrimStart(new char[] { '/', '\\', '~' });
        //        }
        //    }
        //    else
        //    {
        //        virtualPath = fullPath;
        //    }
        //    //virtualPath = virtualPath.Substring(0, virtualPath.Length - 6);//".chtml".Length==6

        //    if (virtualPath.EndsWith(TemplateFileExtension, StringComparison.OrdinalIgnoreCase))
        //    {
        //        virtualPath = virtualPath.Substring(0, virtualPath.Length - TemplateFileExtension.Length);
        //    }

        //    //string modelName = ModelType == null ? "null" : CompilerServicesUtility.GetTypeName(ModelType);
        //    return CompilerServicesUtility.EncodeFilePathToFileName(virtualPath);
        //}





        ////public static string GetClassNameFromFullPath(string fullPath)
        ////{
        ////    string virtualPath;
        ////    if (fullPath.StartsWith(MapPathBasedVirtualPathProvider.AppRoot, StringComparison.OrdinalIgnoreCase))
        ////    {
        ////        virtualPath = fullPath.Substring(MapPathBasedVirtualPathProvider.AppRoot.Length);
        ////        if (virtualPath != null & virtualPath.Length > 0)
        ////        {
        ////            virtualPath = virtualPath.TrimStart(new char[] { '/', '\\', '~' });
        ////        }
        ////    }
        ////    else
        ////    {
        ////        virtualPath = fullPath;
        ////    }
        ////    //virtualPath = virtualPath.Substring(0, virtualPath.Length - 6);//".chtml".Length==6

        ////    if (virtualPath.EndsWith(TemplateFileExtension, StringComparison.OrdinalIgnoreCase))
        ////    {
        ////        virtualPath = virtualPath.Substring(0, virtualPath.Length - TemplateFileExtension.Length);
        ////    }

        ////    //string modelName = ModelType == null ? "null" : CompilerServicesUtility.GetTypeName(ModelType);
        ////    return CompilerServicesUtility.EncodeFilePathToFileName(virtualPath);
        ////}

        //public string AssemblyName { get; set; }


        ///// <summary>
        ///// Gets or sets the template content.
        ///// </summary>
        //public ITemplateSource TemplateContent { get; set; }

        ///// <summary>
        ///// Gets or sets the base template type.
        ///// </summary>
        //public Type TemplateType { get; set; }
        //#endregion
    }
}
