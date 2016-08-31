using OwinMvc.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OwinMvc.View
{
    class MyRazorViewEngine : RazorViewEngine
    {

        internal MyRazorViewEngine() :base()
        {
            this.VirtualPathProvider = MyVirtualPathProvider.Instance;
            //this.DisplayModeProvider = 
        }

        protected override bool IsPrecompiledNonUpdateableSite
        {
            get
            {
                return false;
            }
        }


        
        



        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            //return base.FileExists(controllerContext, virtualPath);

            //var path = MapPathBasedVirtualPathProvider.MapPath(virtualPath);
            bool exists = MyVirtualPathProvider.Instance.FileExists(virtualPath);

            return exists;
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return base.FindView(controllerContext, viewName, masterName, useCache);

            //if (controllerContext == null)
            //{
            //    throw new ArgumentNullException("controllerContext");
            //}
            //if (string.IsNullOrEmpty(viewName))
            //{
            //    throw new ArgumentNullException("viewName");
            //}
            //string controllerName = controllerContext.RouteData.GetRequiredString("controller");
            //string[] viewLocationsSearched;
            //string viewPath = MyVirtualPathProvider.MapPath(viewName);
            //string[] masterLocationsSearched;
            //string masterPath = this.GetPath(controllerContext, this.MasterLocationFormats, this.AreaMasterLocationFormats, "MasterLocationFormats", masterName, controllerName, "Master", useCache, out masterLocationsSearched);
            //ViewEngineResult result;
            //if (string.IsNullOrEmpty(viewPath) || (string.IsNullOrEmpty(masterPath) && !string.IsNullOrEmpty(masterName)))
            //{
            //    result = new ViewEngineResult(viewLocationsSearched.Union(masterLocationsSearched));
            //}
            //else
            //{
            //    result = new ViewEngineResult(this.CreateView(controllerContext, viewPath, masterPath), this);
            //}
            //return result;


        }


        //// System.Web.Mvc.VirtualPathProviderViewEngine
        //private string GetPathFromGeneralName(ControllerContext controllerContext, List<ViewLocation> locations, string name, string controllerName, string areaName, string cacheKey, ref string[] searchedLocations)
        //{
        //    string result = string.Empty;
        //    //searchedLocations = new string[locations.Count];
        //    //for (int i = 0; i < locations.Count; i++)
        //    //{
        //    //    ViewLocation location = locations[i];
        //    //    string virtualPath = location.Format(name, controllerName, areaName);
        //    //    DisplayInfo virtualPathDisplayInfo = this.DisplayModeProvider.GetDisplayInfoForVirtualPath(virtualPath, controllerContext.HttpContext, (string path) => this.FileExists(controllerContext, path), controllerContext.DisplayMode);
        //    //    if (virtualPathDisplayInfo != null)
        //    //    {
        //    //        string resolvedVirtualPath = virtualPathDisplayInfo.FilePath;
        //    //        searchedLocations = VirtualPathProviderViewEngine._emptyLocations;
        //    //        result = resolvedVirtualPath;
        //    //        this.ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, VirtualPathProviderViewEngine.AppendDisplayModeToCacheKey(cacheKey, virtualPathDisplayInfo.DisplayMode.DisplayModeId), result);
        //    //        if (controllerContext.DisplayMode == null)
        //    //        {
        //    //            controllerContext.DisplayMode = virtualPathDisplayInfo.DisplayMode;
        //    //        }
        //    //        IEnumerable<IDisplayMode> allDisplayModes = this.DisplayModeProvider.Modes;
        //    //        foreach (IDisplayMode displayMode in allDisplayModes)
        //    //        {
        //    //            if (displayMode.DisplayModeId != virtualPathDisplayInfo.DisplayMode.DisplayModeId)
        //    //            {
        //    //                DisplayInfo displayInfoToCache = displayMode.GetDisplayInfo(controllerContext.HttpContext, virtualPath, (string path) => this.FileExists(controllerContext, path));
        //    //                string cacheValue = string.Empty;
        //    //                if (displayInfoToCache != null && displayInfoToCache.FilePath != null)
        //    //                {
        //    //                    cacheValue = displayInfoToCache.FilePath;
        //    //                }
        //    //                this.ViewLocationCache.InsertViewLocation(controllerContext.HttpContext, VirtualPathProviderViewEngine.AppendDisplayModeToCacheKey(cacheKey, displayMode.DisplayModeId), cacheValue);
        //    //            }
        //    //        }
        //    //        break;
        //    //    }
        //    //    searchedLocations[i] = virtualPath;
        //    //}
        //    return result;
        //}




        // System.Web.Mvc.VirtualPathProviderViewEngine
        private bool IsSpecificPath(string name)
        {
            char c = name[0];
            return c == '~' || c == '/';
        }




        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return base.FindPartialView(controllerContext, partialViewName, useCache);


            

        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            //return base.CreateView(controllerContext, viewPath, masterPath);

            var view = new MyRazorView(controllerContext, viewPath,
                                     layoutPath: masterPath, runViewStartPages: true, viewStartFileExtensions: FileExtensions, viewPageActivator: ViewPageActivator)
            {
                //DisplayModeProvider = DisplayModeProvider
            };
            return view;

        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            // return base.CreatePartialView(controllerContext, partialPath);


            return new MyRazorView(controllerContext, partialPath,
                                 layoutPath: null, runViewStartPages: false, viewStartFileExtensions: FileExtensions, viewPageActivator: ViewPageActivator)
            {
                //DisplayModeProvider = DisplayModeProvider
            };


        }



        public override void ReleaseView(ControllerContext controllerContext, IView view)
        {
            base.ReleaseView(controllerContext, view);
            
        }

        

        public void Test()
        {
           


        }
        
    }
}
