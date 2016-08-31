using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwinMvc
{
    internal static class ConstVars
    {
        /// <summary>
        /// 决定控制器的命名空间
        /// </summary>
        public const string Namespaces = "Namespaces";
        public const string Area = "area";
        public const string Controller = "controller";
        public const string Action = "action";

        public const string UseNamespaceFallback = "UseNamespaceFallback";
        public const string AsyncPostfix = "Async";

    }
}
