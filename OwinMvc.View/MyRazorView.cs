using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OwinMvc.View
{
    internal class MyRazorView : System.Web.Mvc.RazorView
    {
        private IViewPageActivator ViewPageActivator;
        private ControllerContext _controllerContext;

        public MyRazorView(ControllerContext controllerContext, string viewPath, string layoutPath, bool runViewStartPages, IEnumerable<string> viewStartFileExtensions)
            : base(controllerContext, viewPath, layoutPath, runViewStartPages, viewStartFileExtensions, null)
        {
            this._controllerContext = controllerContext;
            this.ViewPageActivator = new MyViewPageActivator();
            
        }


        public MyRazorView(ControllerContext controllerContext, string viewPath, string layoutPath, bool runViewStartPages, IEnumerable<string> viewStartFileExtensions, IViewPageActivator viewPageActivator)
            : base(controllerContext, viewPath, layoutPath, runViewStartPages, viewStartFileExtensions, viewPageActivator)
        {
            this._controllerContext = controllerContext;
            this.ViewPageActivator = viewPageActivator;
        }


        public override void Render(ViewContext viewContext, System.IO.TextWriter writer)
        {
            // base.Render(viewContext, writer);


            if (viewContext == null)
            {
                throw new ArgumentNullException("viewContext");
            }

            object instance = null;

            // Type type = BuildManager.GetCompiledType(ViewPath);

            Type type = TemplateManager.ResolveType(ViewPath);

            if (type != null)
            {
                instance = ViewPageActivator.Create(_controllerContext, type);
            }

            if (instance == null)
            {
                throw new InvalidOperationException(String.Format("{0}: CshtmlView_ViewCouldNotBeCreated", ViewPath));
            }

            RenderView(viewContext, writer, instance);
        }


        

        protected override void RenderView(ViewContext viewContext, System.IO.TextWriter writer, object instance)
        {
            base.RenderView(viewContext, writer, instance);            
        }
    }

}
