using Microsoft.Owin;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Routing;

namespace OwinMvc.Owin
{
    internal class MyMvcOwinMiddleware :OwinMiddleware
    {
        
        public MyMvcOwinMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {            
            try
            {
                UrlRoutingModule module = new UrlRoutingModule();
                using (var httpContext = new MyHttpContext(context))
                {
                    module.PostResolveRequestCache(httpContext);

                    var handler1 = httpContext.Handler as IMyMvcHandler;
                    if (handler1 != null)
                    {
                        handler1.ProcessRequest(httpContext);
                    }
                    else
                    {
                        throw new NotImplementedException("can not conver to IMyMvcHandler");
                    }
                }                
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("error:" + ex.ToString());

                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                context.Response.Write(ex.ToString());
                //throw;
            }


            // 这里this.Next不为null, 但调用之后显示两遍内容。
            //if (this.Next != null)
            //{
            //    await this.Next.Invoke(context);
            //}

            return Task.FromResult<object>(null);
        }
        
    }
}
