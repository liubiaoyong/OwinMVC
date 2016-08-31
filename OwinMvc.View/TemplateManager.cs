using System;
using System.Collections.Concurrent;

namespace OwinMvc.View
{
    internal static class TemplateManager
    {

        private static readonly ConcurrentDictionary<string, System.Type> typeCache = new ConcurrentDictionary<string, Type>(4, 8192);


        internal static System.Type ResolveType(string virtualPath)
        {

            if (string.IsNullOrEmpty(virtualPath))
            {
                return null;
            }

            string pathKey = virtualPath.ToLower();

            System.Type type;
            if (typeCache.TryGetValue(pathKey, out type))
            {
                return type;
            }

            type = TemplateBuilder.BuilderType(virtualPath);

            var type2 = typeCache.GetOrAdd(pathKey, type);
            //throw new NotImplementedException();

            return type2;

        }


        //internal static void 

    }
}
