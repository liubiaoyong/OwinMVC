using OwinMvc.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace OwinMvc.View
{
    internal class CompileContext
    {
         /// <summary>
        /// Creates a new TypeContext instance with the given classname and the given namespaces.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="namespaces"></param>
        internal CompileContext(string sourceCode, string className, ISet<string> namespaces)
        {
            if (string.IsNullOrEmpty(sourceCode))
            {

            }
            this.SourceCode = sourceCode;
            ClassName = className;
            UsingNamespaces = new HashSet<string>();
            if (namespaces != null)
            {
                foreach (var s in namespaces)
                {
                    UsingNamespaces.Add(s);
                }
            }

        }

        public string SourceCode { get; private set; }

        private string _className;
        private static readonly string TemplateFileExtension = ".cshtml";
        /// <summary>
        /// Gets the set of namespace imports.
        /// </summary>
        public ISet<string> UsingNamespaces { get; set; }

        public static string GetClassNameFromFullPath(string fullPath)
        {
            string virtualPath;
            if (fullPath.StartsWith(MyVirtualPathProvider.AppRoot, StringComparison.OrdinalIgnoreCase))
            {
                virtualPath = fullPath.Substring(MyVirtualPathProvider.AppRoot.Length);
                if (virtualPath != null & virtualPath.Length > 0)
                {
                    virtualPath = virtualPath.TrimStart(new char[] { '/', '\\', '~' });
                }
            }
            else
            {
                virtualPath = fullPath;
            }
            //virtualPath = virtualPath.Substring(0, virtualPath.Length - 6);//".chtml".Length==6

            if (virtualPath.EndsWith(TemplateFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                virtualPath = virtualPath.Substring(0, virtualPath.Length - TemplateFileExtension.Length);
            }

            //string modelName = ModelType == null ? "null" : CompilerServicesUtility.GetTypeName(ModelType);
            return EncodeFilePathToFileName(virtualPath);
        }

        //public string AssemblyName { get; set; }

        public string Namespace
        {
            get { return CompilerServiceBase.DynamicTemplateNamespace; }
        }

        public string FullClassName
        {
            get { return this.Namespace + "." + this.ClassName; }
        }

        /// <summary>
        /// Gets the class name.
        /// </summary>
        public string ClassName
        {
            get
            {
                //if (string.IsNullOrEmpty(_className))
                //{
                //    if (TemplateContent != null && !string.IsNullOrEmpty(TemplateContent.TemplateFile))
                //    {
                //        string virtualPath = TemplateContent.TemplateFile.Substring(TemplateFullPathResolver.RootPath.Length);
                //        virtualPath = virtualPath.Substring(0, virtualPath.Length - 6);//".chtml".Length==6
                //        string modelName= ModelType == null ? "null" :CompilerServicesUtility.GetTypeName(ModelType);
                //        _className = CompilerServicesUtility.EncodeFilePathToFileName(virtualPath+modelName);
                //    }
                //    else
                //    {
                //        _className = CompilerServicesUtility.GenerateClassName();
                //    }
                //}
                return _className;
            }
            internal set
            {
                _className = value;
            }
        }


         /// <summary>
        /// 将文件路径替换为可行的文件名
        /// 将所有非大小写字母，还有数组替换为数字和大写字母的结合
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string EncodeFilePathToFileName(string url)
        {
            if (String.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("url can't be empty");
            }

            var sb = new StringBuilder();
            foreach (var c in url.ToLowerInvariant())
            {
                // only accept alphanumeric chars
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
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

            return sb.ToString();
        }

    }
}
