using System;
using System.Web.Mvc;

namespace OwinMvc
{
    internal sealed class MyMvcHandler : MvcHandler, IMyMvcHandler
    {
        private readonly MvcHandler _handler;

        internal MyMvcHandler(MvcHandler handler) : base(handler.RequestContext)
        {
            this._handler = handler;
        }

        protected override void ProcessRequest(System.Web.HttpContext httpContext)
        {
            throw new NotImplementedException();
        }
        

        void IMyMvcHandler.ProcessRequest(System.Web.HttpContextBase context)
        {
            base.ProcessRequest(context);
        }
    }
}
