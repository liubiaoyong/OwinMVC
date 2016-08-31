using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;

namespace OwinMvc
{
    internal sealed class MyControllerFactory : IControllerFactory
    {

        private static readonly HashSet<string> _defaultNamespaces = new HashSet<string>();
        private static readonly List<Type> _controllerTypes = new List<Type>(4096);//同时包含mvc和APIController
        //private static readonly Lazy<ConcurrentDictionary<string, ControllerDescriptor>> _controllerInfoCache;
        //只包含APIController


       

        /// <summary>
        /// 获取Controller类的搜索范围[Assembly]
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<Assembly> GetSearchAssemblies()
        {
            Func<Assembly,bool> where = asm => !(
                    asm.FullName.StartsWith("System", StringComparison.OrdinalIgnoreCase) ||
                    asm.FullName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) // ||
                    //asm.FullName.StartsWith("System.Web", StringComparison.OrdinalIgnoreCase) ||
                    //asm.FullName.StartsWith("RazorEngine", StringComparison.OrdinalIgnoreCase)
                );

            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var allAssemblies = new HashSet<Assembly>(loadedAssemblies.Where(where));
            HashSet<string> loadedLocations = new HashSet<string>();
            
            foreach (var asm in allAssemblies)
            {
                try
                {
                    string location = asm.Location;
                    loadedLocations.Add(location.ToLower());
                }
                catch (NotSupportedException ex) //2016-6-13 zhumin 这里可能会有动态程序集，不支持Location操作，暂时忽略
                {
                    TraceHelper.Error(ex.ToString());
                }
            }
            
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            

            var domainFriendlyName = AppDomain.CurrentDomain.FriendlyName;

            if (!string.IsNullOrEmpty(domainFriendlyName) && domainFriendlyName.StartsWith("/LM/W3SVC/", StringComparison.OrdinalIgnoreCase))
            {
                // host with IIS
                dir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
            }
            else 
            {
                dir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }


            // TODO: 这里有漏洞.
            var refDlls = System.IO.Directory.GetFiles(dir, "*.dll", System.IO.SearchOption.AllDirectories);


            Assembly assem1;
            foreach (var dll_path in refDlls)
            {
                if (!loadedLocations.Contains(dll_path.ToLower()))
                {
                    assem1 = Assembly.LoadFrom(dll_path);
                    if (assem1 != null && where(assem1))
                    {
                        allAssemblies.Add(assem1);
                    }
                }
            }

            return allAssemblies;
        }


        static MyControllerFactory()
        {
            Type interfaceType = typeof(IController);
            var searchAssbemblies = GetSearchAssemblies();

            Type[] assemblyTypes;
            foreach (Assembly assembly in searchAssbemblies)
            {                
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
                    assemblyTypes = null;
                    TraceHelper.Error(ex.ToString());
                }

                if (assemblyTypes != null && assemblyTypes.Length > 0)
                {
                    //_controllerTypes.AddRange(ex.Types);

                    //foreach (Type type in assemblyTypes.Where(type => type.IsPublic && interfaceType.IsAssignableFrom(type) && type.IsClass && (!type.IsAbstract)))
                    //{
                    //    _controllerTypes.Add(type);
                    //}

                    _controllerTypes.AddRange(assemblyTypes.Where(type => type.IsPublic && type.IsClass && interfaceType.IsAssignableFrom(type) && (!type.IsAbstract)));
                }                
            }

            //_controllerInfoCache = new Lazy<ConcurrentDictionary<string, ControllerDescriptor>>(InitializeControllerInfoCache);

            //////将命名空间最短的Controller作为默认命名空间
            ////if (controllerTypes.Count >= 1)
            ////{
            ////    string minLengthNamespaces = controllerTypes.Select(c => c.Namespace).ToList().OrderBy(s => s.Length).First();
            ////    _defaultNamespaces.Clear();
            ////    _defaultNamespaces.Add(minLengthNamespaces + ".*");
            ////}

            var default_namespaces = CollectDefaultlNamespaces(_controllerTypes);
            default_namespaces = default_namespaces.OrderBy(p => p.Length).ToList();
            _defaultNamespaces.Clear();
            foreach (var item in default_namespaces)
            {
                _defaultNamespaces.Add(item);
            }
        }

        /// <summary>
        /// 添加默认的Namespace.
        /// </summary>
        /// <param name="defaultNamespaces"></param>
        public static void AddDefaultNamespace(IEnumerable<string> defaultNamespaces)
        {
            foreach (var item in defaultNamespaces)
            {
                _defaultNamespaces.Add(item);
            }
        }

        /// <summary>
        /// 重置默认的Namespace为设定的值。
        /// </summary>
        /// <param name="defaultNamespaces"></param>
        public static void ResetDefaultNamespace(IEnumerable<string> defaultNamespaces)
        {
            _defaultNamespaces.Clear();
            foreach (var item in defaultNamespaces)
            {
                _defaultNamespaces.Add(item);
            }
        }

        private static IEnumerable<string> CollectDefaultlNamespaces(IEnumerable<Type> allControllerTypes)
        {
            // 强制注册
            //AreaRegistration.RegisterAllAreas();
            MyAreaRegistrationUtility.RegisterAllAreas();


            var allAreaUsedNamespaces = CollectRegisterdNamespaces(RouteTable.Routes);
            //
            var allControllerNamepaces = allControllerTypes.Select(p => p.Namespace).Distinct();

            List<string> default_namespaces = new List<string>();
            foreach (var name_pace in allControllerNamepaces)
            {
                if (!IsContains(allAreaUsedNamespaces, name_pace))
                {
                    default_namespaces.Add(name_pace);
                }
            }
            return default_namespaces;
        }


        private static bool IsContains(HashSet<string> all, string s)
        {
            // return all.Contains(s);
            foreach (var item in all)
            {
                if (item.EndsWith(".*"))
                {
                    if (s.StartsWith(item.Substring(0, item.Length - 2))) return true;
                }
                else
                {
                    if (s.StartsWith(item)) return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 收集所有用到的Namespaces.
        /// </summary>
        /// <param name="routes"></param>
        private static HashSet<string> CollectRegisterdNamespaces(RouteCollection routes)
        {
            HashSet<string> allUsedNamespaces = new HashSet<string>();

            var temp_routes = routes.Where(p => p.GetType() == typeof(Route)).Cast<Route>();
            foreach (var temp_route in temp_routes)
            {
                object namespace_list;
                if (temp_route.DataTokens.TryGetValue(ConstVars.Namespaces, out namespace_list))
                {
                    var temp_list = namespace_list as IEnumerable<string>;
                    if (temp_list != null)
                    {
                        allUsedNamespaces.UnionWith(temp_list);
                    }
                }
            }
            return allUsedNamespaces;
        }





        /// <summary>
        /// 获取所有的ControllerTypes
        /// </summary>
        /// <returns></returns>
        internal static List<Type> GetAllControllerTypes()
        {
            return _controllerTypes.ToList();
        }


        ///// <summary>
        ///// 用于返回所有WebApi的控制器
        ///// </summary>
        ///// <returns></returns>
        //public IDictionary<string, ControllerDescriptor> GetAPIControllerMapping()
        //{
        //    return InitializeControllerInfoCache().ToDictionary(c => c.Key, c => c.Value, StringComparer.OrdinalIgnoreCase);
        //    //return _controllerInfoCache.Value.ToDictionary(c => c.Key, c => c.Value, StringComparer.OrdinalIgnoreCase);
        //}

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException("requestContext");
            }

            if (String.IsNullOrEmpty(controllerName))
            {
                throw new ArgumentException("controllerName can not be null!");
            }

            Type controllerType = GetControllerType(requestContext, controllerName);
            IController controller = GetControllerInstance(requestContext, controllerType);

            var ctrlBase = controller as ControllerBase;
            if (ctrlBase != null)
            {
                // 在这里设置DisplayModel 或者 DisplayModeProvider.Instance.RequireConsistentDisplayMode = false;
                // ctrlBase.ControllerContext.DisplayMode = DisplayModeProvider.Instance.Modes.First(p => p.DisplayModeId == DisplayModeProvider.DefaultDisplayModeId);
                DisplayModeProvider.Instance.RequireConsistentDisplayMode = false;
            }


            return controller;
        }


        public void ReleaseController(IController controller)
        {
            IDisposable disposable = controller as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
        /// <summary>
        /// 获取控制器的类型,自定义MapRoute等方法时需要将当前AreaRestration的命名空间加进去
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public Type GetControllerType(RequestContext requestContext, string controllerName)
        {

            if (requestContext == null)
            {
                throw new ArgumentNullException("requestContext");
            }

            if (String.IsNullOrEmpty(controllerName) &&
                (requestContext.RouteData == null ))
            {
                throw new ArgumentException("controllerName can not be null!");
            }

            RouteData routeData = requestContext.RouteData;

            Type match;
            object routeNamespacesObj;
            if (routeData.DataTokens.TryGetValue(ConstVars.Namespaces, out routeNamespacesObj))
            {
                //只有两种情况上面判断通过
                //1.执行的是Area的route
                //2.在调用MapRoute的时候设置了命名空间
                IEnumerable<string> routeNamespaces = routeNamespacesObj as IEnumerable<string>;
                //其实下面这个不用判断的因为只有是Area里面的这个在注册的时候默认加上控制器的命名空间的
                if (routeNamespaces != null && routeNamespaces.Any())
                {
                    HashSet<string> namespaceHash = new HashSet<string>(routeNamespaces, StringComparer.OrdinalIgnoreCase);
                    match = GetControllerTypeWithinNamespaces(routeData.Route, controllerName, namespaceHash);

                    return match;
                }
            }

            object UseNamespaceFallback;
            if (routeData.DataTokens.TryGetValue(ConstVars.UseNamespaceFallback, out UseNamespaceFallback) && false.Equals(UseNamespaceFallback))
            {
                //return null;
                throw new ApplicationException("UseNamespaceFallback = false");
            }

            match = GetControllerTypeWithinNamespaces(routeData.Route, controllerName, new HashSet<string>(_defaultNamespaces, StringComparer.OrdinalIgnoreCase));
            if (match == null)
            {
                throw new ApplicationException("Can not find the Controller! _defaultNamespaces = " + string.Join(",", _defaultNamespaces));
            }

            return match;
        }

        public IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                if (requestContext != null)
                {
                    
                    //throw new Exception("Can not find the Controller! path=" + requestContext.OwinContext.Request.Path.ToString());
                    throw new Exception("Can not find the Controller! path=" + requestContext.HttpContext.Request.Path);
                }
                else
                {
                    throw new Exception("Can not find the Controller!");
                }
            }

            return (IController)DependencyResolver.Current.GetService(controllerType);
        }


        /// <summary>
        /// 从参数namespaces中查找是否存在对应的控制器
        /// </summary>
        /// <param name="route"></param>
        /// <param name="controllerName"></param>
        /// <param name="namespaces"></param>
        /// <returns></returns>
        private Type GetControllerTypeWithinNamespaces(RouteBase route, string controllerName, HashSet<string> namespaces)
        {
            IList<Type> matchingTypes = new List<Type>();
            IList<Type> allNameMatchedTypes = _controllerTypes.Where(t => t.Name.ToLower() == (controllerName + ConstVars.Controller).ToLower()).ToList();
            if (allNameMatchedTypes != null && allNameMatchedTypes.Count >= 1)
            {
                foreach (var requestedNamespace in namespaces)
                {
                    foreach (var type in allNameMatchedTypes)
                    {
                        if (IsNamespaceMatch(requestedNamespace, type.Namespace))
                        {
                            if (matchingTypes.FirstOrDefault(tt => tt.Namespace == type.Namespace) == null)
                            matchingTypes.Add(type);
                        }
                    }
                }

            }
            if (matchingTypes.Count == 1)
                return matchingTypes.First();
            else if(matchingTypes.Count ==0)
            {
                //return null;
                System.Text.StringBuilder err_builder = new System.Text.StringBuilder(512);
                err_builder.Append("can not find controllertype : controllerName = ").Append(controllerName)
                    .Append("\r\n namespaces-count:").Append(namespaces != null ? namespaces.Count : 0);

                if (namespaces != null && namespaces.Count > 0)
                {
                    foreach (var name_space in namespaces)
                    {
                        err_builder.Append("\r\n namespace:").Append(name_space);
                    }
                }

                throw new ApplicationException(err_builder.ToString());
            }    
            else
                // multiple matching types
                throw new Exception("The request matches multiple controllers!");

        }
        /// <summary>
        /// 判定两个命名空间是否相同
        /// </summary>
        /// <param name="requestedNamespace"></param>
        /// <param name="targetNamespace"></param>
        /// <returns></returns>
        internal static bool IsNamespaceMatch(string requestedNamespace, string targetNamespace)
        {
            // degenerate cases
            if (requestedNamespace == null)
            {
                return false;
            }
            else if (requestedNamespace.Length == 0)
            {
                return true;
            }

            if (!requestedNamespace.EndsWith(".*", StringComparison.OrdinalIgnoreCase))
            {
                // looking for exact namespace match
                return String.Equals(requestedNamespace, targetNamespace, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                // looking for exact or sub-namespace match
                requestedNamespace = requestedNamespace.Substring(0, requestedNamespace.Length - ".*".Length);
                if (!targetNamespace.StartsWith(requestedNamespace, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (requestedNamespace.Length == targetNamespace.Length)
                {
                    // exact match
                    return true;
                }
                else if (targetNamespace[requestedNamespace.Length] == '.')
                {
                    // good prefix match, e.g. requestedNamespace = "Foo.Bar" and targetNamespace = "Foo.Bar.Baz"
                    return true;
                }
                else
                {
                    // bad prefix match, e.g. requestedNamespace = "Foo.Bar" and targetNamespace = "Foo.Bar2"
                    return false;
                }
            }
        }


        ///// <summary>
        ///// 这个静态方法主要用于获取项目中所有存在的APIController
        ///// </summary>
        ///// <returns></returns>
        //private static ConcurrentDictionary<string, ControllerDescriptor> InitializeControllerInfoCache()
        //{
        //    var result = new ConcurrentDictionary<string, ControllerDescriptor>(StringComparer.OrdinalIgnoreCase);
        //    var apiControllerTypes = controllerTypes.Where(p => OwinContextExtensions.apiControllerType.IsAssignableFrom(p));

        //    var duplicateControllers = new HashSet<string>();

        //    var groupedByName = apiControllerTypes.GroupBy(
        //        t => t.Name.Substring(0, t.Name.Length - ConstVars.Controller.Length),
        //        StringComparer.OrdinalIgnoreCase);

        //    Dictionary<string, ILookup<string, Type>> controllerTypeGroups = groupedByName.ToDictionary(
        //        g => g.Key,
        //        g => g.ToLookup(t => t.Namespace ?? String.Empty, StringComparer.OrdinalIgnoreCase),
        //        StringComparer.OrdinalIgnoreCase);

        //    foreach (KeyValuePair<string, ILookup<string, Type>> controllerTypeGroup in controllerTypeGroups)
        //    {
        //        string controllerName = controllerTypeGroup.Key;

        //        foreach (IGrouping<string, Type> controllerTypesGroupedByNs in controllerTypeGroup.Value)
        //        {
        //            foreach (Type controllerType in controllerTypesGroupedByNs)
        //            {
        //                if (result.Keys.Contains(controllerName))
        //                {
        //                    duplicateControllers.Add(controllerName);
        //                    break;
        //                }
        //                else
        //                {
        //                    result.TryAdd(controllerName, new ReflectedControllerDescriptor(controllerType));
        //                }
        //            }
        //        }
        //    }

        //    foreach (string duplicateController in duplicateControllers)
        //    {
        //        ControllerDescriptor descriptor;
        //        result.TryRemove(duplicateController, out descriptor);
        //    }

        //    return result;
        //}




        IController IControllerFactory.CreateController(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            //return base.CreateController(requestContext, controllerName);
            return this.CreateController(requestContext, controllerName);
        }        

        void IControllerFactory.ReleaseController(IController controller)
        {
            this.ReleaseController(controller);
        }


        public IReadOnlyList<Type> GetControllerTypes()
        {
            //var obj = typeof(DefaultControllerFactory).InvokeMember("GetControllerTypes", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod, null, this, null);
            //return obj as IReadOnlyList<Type>;
            return _controllerTypes;
        }        
        

        System.Web.SessionState.SessionStateBehavior IControllerFactory.GetControllerSessionBehavior(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException("requestContext");
            }
            if (string.IsNullOrEmpty(controllerName))
            {
                throw new ArgumentException("controllerName");
            }
            //Type controllerType = this.GetControllerType(requestContext, controllerName);
            //return this.GetControllerSessionBehavior(requestContext, controllerType);

            return System.Web.SessionState.SessionStateBehavior.Disabled;
        }
    }
}
