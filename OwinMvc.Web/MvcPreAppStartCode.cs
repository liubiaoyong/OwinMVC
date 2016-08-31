using OwinMvc.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
//using System.Web.Compilation;
//using System.Web.Compilation;
//using System.Web.Configuration;
using System.Web.WebPages.Razor;

namespace OwinMvc.Web
{
   internal static class MvcPreAppStartCode
    {
       //private static readonly System.Web.Hosting.HostingEnvironment _hostEnv;
        private static bool _startWasCalled;


       static MvcPreAppStartCode()
        {
            //_hostEnv = new System.Web.Hosting.HostingEnvironment();
           //_hostEnv = HostingEnvironment.
        }
        public static void Start()
        {
            if (!MvcPreAppStartCode._startWasCalled)
            {


                MvcPreAppStartCode._startWasCalled = true;

                var domain = System.Threading.Thread.GetDomain();
                domain.SetData(".appPath", MyVirtualPathProvider.AppRoot);
                domain.SetData(".appVPath", "/");

                

                domain.SetData(".appDomain", "OwinMvc_" + Guid.NewGuid().ToString("N"));
                domain.SetData(".appId", "OwinMvc_" + Guid.NewGuid().ToString("N"));
                domain.SetData(".domainId", "OwinMvc_" + Guid.NewGuid().ToString("N"));


                ////// 某些地方好象有依赖。
                ////var f1 = typeof(System.Web.Compilation.BuildManager).GetField("_theBuildManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.Static);
                ////var t = f1.GetValue(null);
                ////var buildMgr = (System.Web.Compilation.BuildManager)t;

                ////var f2 = typeof(System.Web.Compilation.BuildManager).GetField("_topLevelFilesCompiledCompleted", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetField);
                ////f2.SetValue(buildMgr, true);
                ////var f3 = typeof(System.Web.Compilation.BuildManager).GetField("_skipTopLevelCompilationExceptions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetField);
                ////f3.SetValue(buildMgr, true);


                ////ClassLibrary1.Class1 c = new ClassLibrary1.Class1();

                ////System.Web.Compilation.BuildManager.GetReferencedAssemblies()


                ////// 以下语句主要的目的：BuildManager.PreStartInitStage = PreStartInitStage.AfterPreStartInit;
                ////// AreaRegistration.RegisterAllAreas()需要依赖于这个变量。其它的暂时没发现依赖。
                ////// 看后面能否将这个依赖给去掉。
                ////typeof(System.Web.Compilation.BuildManager).InvokeMember("InvokePreStartInitMethods",
                ////    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod, null, null,
                ////    new object[] { new System.Collections.Generic.List<System.Reflection.MethodInfo>() });


                //AreaRegistration.RegisterAllAreas();
                MyAreaRegistrationUtility.RegisterAllAreas();

                var factory = new MyControllerFactory();
                ControllerBuilder.Current.SetControllerFactory(factory);
                //DependencyResolver.SetResolver(new dep)
                //factory.EnsureCacheInitialized();


                //System.Web.Compilation.BuildProvider.RegisterBuildProvider(".cshtml", typeof(RazorBuildProvider));
                //System.Web.Compilation.BuildProvider.RegisterBuildProvider(".vbhtml", typeof(RazorBuildProvider));

                //System.Web.WebPages.WebPageHttpHandler.RegisterExtension("cshtml");
                //System.Web.WebPages.WebPageHttpHandler.RegisterExtension("vbhtml");
                //System.Web.UI.PageParser.EnableLongStringsAsResources = false;

                //DynamicModuleUtility.RegisterModule(typeof(WebPageHttpModule));
                //System.Web.WebPages.Scope.ScopeStorage.CurrentProvider = new System.Web.WebPages.Scope.AspNetRequestScopeStorageProvider();

                //System.Web.Mvc.ViewContext.GlobalScopeThunk = (() => System.Web.WebPages.Scope.ScopeStorage.CurrentScope);


                
                //System.Web.Mvc.PreApplicationStartCode.Start();



               


                //System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(MyVirtualPathProvider.Instance);

               

                // 因为webform viewengine在findview时会出错。因此先拿掉webformviewengine.
                //var webform_viewengine = ViewEngines.Engines.FirstOrDefault(p => p.GetType() == typeof(WebFormViewEngine));
                //if (webform_viewengine != null)
                //{
                //    ViewEngines.Engines.Remove(webform_viewengine);
                //}
                //if (ViewEngines.Engines.Count == 0)
                //{
                //    ViewEngines.Engines.Add(new RazorViewEngine());
                //}

                ViewEngines.Engines.Clear();
                ViewEngines.Engines.Add(new MyRazorViewEngine());

                // 创建隐藏类的实体即可。



                // System.Runtime.Remoting.Messaging.CallContext.SetData("__TemporaryVirtualPathProvider__", 
                //var vpp = System.Runtime.Remoting.Messaging.CallContext.GetData("__TemporaryVirtualPathProvider__");
                //Console.WriteLine(vpp.ToString());



                ////var controlTypes = factory.GetControllerTypes();


                ////CompactBuildManager.GetCompilationAppConfig();

                ////var assems2 = System.Threading.Thread.GetDomain().GetAssemblies();

                ////Console.WriteLine(assems2.Length.ToString());

                ////var assemblies = System.Web.Compilation.BuildManager.GetReferencedAssemblies();
                ////System.Diagnostics.Trace.WriteLine(assemblies.Count.ToString());


                ////var dict = new Dictionary<string, string>();

                ////var runtime_dir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
                ////System.Diagnostics.Trace.WriteLine("dir = " + runtime_dir);
                ////dict.Add("runtimedir", runtime_dir);


                ////var system_config_file = System.Runtime.InteropServices.RuntimeEnvironment.SystemConfigurationFile;
                ////dict.Add("system_config_file", system_config_file);



                ////var bindir = HttpRuntime.BinDirectory;
                ////dict.Add("bindir", bindir);


                ////var ApplicationPhysicalPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                ////dict.Add("ApplicationPhysicalPath", ApplicationPhysicalPath);

                ////var ApplicationVirtualPath = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
                ////dict.Add("ApplicationVirtualPath", ApplicationVirtualPath);

                ////var p2 = System.Web.Hosting.HostingEnvironment.VirtualPathProvider;
                ////dict.Add("System.Web.Hosting.HostingEnvironment.VirtualPathProvider", (p2 == null) ? string.Empty : p2.ToString());

                //var GetPhysicalPath = System.Web.Hosting.HostingEnvironment.ApplicationHost.GetPhysicalPath();
                //dict.Add("System.Web.Hosting.HostingEnvironment.ApplicationHost.GetPhysicalPath()", GetPhysicalPath);

                //var GetVirtualPath = System.Web.Hosting.HostingEnvironment.ApplicationHost.GetVirtualPath();
                //dict.Add("GetVirtualPath", GetVirtualPath);


            }
        }
    }
}
