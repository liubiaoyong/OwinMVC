using OwinMvc.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace OwinMvc.Web
{
    class MyVirtualPathFactory : IVirtualPathFactory
    {
        public object CreateInstance(string virtualPath)
        {
            var type = TemplateManager.ResolveType(virtualPath);

            return Activator.CreateInstance(type);
        }

        public bool Exists(string virtualPath)
        {
            var path = MyVirtualPathProvider.MapPath(virtualPath);
            return System.IO.File.Exists(path);
        }
    }
}
