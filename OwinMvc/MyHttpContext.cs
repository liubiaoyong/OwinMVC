using Microsoft.Owin;
using System;
using System.Web;

namespace OwinMvc
{
    public class MyHttpContext : HttpContextBase, IDisposable
    {
        private readonly IOwinContext owinContext;
        private readonly MyHttpRequest req;
        private readonly MyHttpResponse resp;

        public MyHttpContext(IOwinContext oldContext)
        {
            this.owinContext = oldContext;
            this.req = new MyHttpRequest(this.owinContext);
            this.resp = new MyHttpResponse(this.owinContext);
        }

        public override HttpRequestBase Request
        {
            get
            {
                return this.req;
            }
        }


        public override HttpResponseBase Response
        {
            get
            {
                return this.resp;
            }
        }


        private System.Web.SessionState.SessionStateBehavior _SessionStateBehavior = System.Web.SessionState.SessionStateBehavior.Disabled;

        public override void SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior sessionStateBehavior)
        {
            this._SessionStateBehavior = sessionStateBehavior;
        }

        public override IHttpHandler Handler
        {
            get;
            set;
        }

        public override IHttpHandler CurrentHandler
        {
            get
            {
                return this.Handler;
            }
        }

        //private IHttpHandler _remapHandler;

        public override void RemapHandler(IHttpHandler handler)
        {
            var handler1 = handler as System.Web.Mvc.MvcHandler;
            if (handler1 != null)
            {
                var controllerName = handler1.RequestContext.RouteData.GetRequiredString("controller");
                System.Diagnostics.Trace.WriteLine("controller: " + controllerName);
                this.Handler = new MyMvcHandler(handler1);
            }
            else
            {
                this.Handler = handler;
            }
        }

        public override HttpSessionStateBase Session
        {
            get
            {
                //return base.Session;
                return null;
            }
        }

        public override System.Security.Principal.IPrincipal User
        {
            get
            {
                return this.owinContext.Request.User;
            }
            set
            {
                this.owinContext.Request.User = value;
            }
        }


        private System.Collections.IDictionary _items;

        public override System.Collections.IDictionary Items
        {
            get
            {
                if (this._items == null)
                {
                    this._items = new System.Collections.Hashtable();
                }
                return this._items;
            }
        }


        private MyHttpServerUtility _server;

        public override HttpServerUtilityBase Server
        {
            get
            {
                if (this._server == null)
                {
                    this._server = new MyHttpServerUtility(this);
                }
                return this._server;
            }
        }

        public void Dispose()
        {
            this.resp.Dispose();
        }
    }
}
