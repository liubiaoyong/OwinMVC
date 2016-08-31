using Microsoft.Owin;
using OwinMvc.Web;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OwinMvc
{
    public class MyHttpRequest : HttpRequestBase
    {
        private readonly IOwinContext owinContext;
        private readonly IOwinRequest req;

        public MyHttpRequest(IOwinContext oldContext)
        {
            this.owinContext = oldContext;
            this.req = this.owinContext.Request;
        }


        public override System.IO.Stream InputStream
        {
            get
            {
                return this.req.Body;
            }
        }



        public override string this[string key]
        {
            get
            {
                return string.Join(",", this.req.Query[key]);
            }
        }

        private NameValueCollection _form; // = new NameValueCollection();

        public override NameValueCollection Form
        {
            get
            {
                if (_form == null)
                {
                    var form1  = Nito.AsyncEx.AsyncContext.Run<IFormCollection>(new Func<Task<IFormCollection>>(this.req.ReadFormAsync));
                    var enu = form1.GetEnumerator();
                    this._form = new NameValueCollection();
                    while(enu.MoveNext())
                    {
                        this._form.Add(enu.Current.Key, string.Join(",", enu.Current.Value));
                    }
                }

                return this._form;
            }
        }



        public override string AppRelativeCurrentExecutionFilePath
        {
            get
            {
                return "~" + this.req.Path.Value;
            }
        }

        public override string ApplicationPath
        {
            get
            {
                return MyVirtualPathProvider.AppRoot;
            }
        }


        public override string PathInfo
        {
            get
            {
                return string.Empty;
            }
        }

        public override string Path
        {
            get
            {
                return this.req.Path.Value;
            }
        }

        public override string RawUrl
        {
            get
            {
                if (req.QueryString.HasValue)
                {
                    return this.req.Path.Value + "?" + req.QueryString.Value;
                }
                else
                {
                    return this.req.Path.Value;
                }
            }
        }


        public override System.Web.Routing.RequestContext RequestContext
        {
            get;
            set;
        }


        private HeaderNameValueCollection _headers;

        public override System.Collections.Specialized.NameValueCollection Headers
        {
            get
            {
                if (this._headers == null)
                {
                    this._headers = new HeaderNameValueCollection(this.req.Headers);
                }
                return this._headers;

            }
        }


        public override void ValidateInput()
        {
            //base.ValidateInput();
        }


        public override string PhysicalApplicationPath
        {
            get
            {
                return string.Empty;
            }
        }


        public override string PhysicalPath
        {
            get
            {
                return string.Empty;
            }
        }


        public override string RequestType
        {
            get
            {
                return this.owinContext.Request.Method;
            }
            set
            {
                this.owinContext.Request.Method = value;
            }
        }

        private HttpCookieCollection _cookies;

        public override HttpCookieCollection Cookies
        {
            get
            {
                if (this._cookies == null)
                {
                    this._cookies = new HttpCookieCollection();
                    foreach(var ck in this.req.Cookies)
                    {
                        this._cookies.Add(new HttpCookie(ck.Key, ck.Value));
                    }
                }
                return this._cookies;
            }
        }


        public override string UserAgent
        {
            get
            {
                return this.req.Headers.Get("User-Agent");
            }
        }

        public override string[] AcceptTypes
        {
            get
            {
                return this.req.Headers.GetValues("Accept").ToArray();
            }
        }

        private MyHttpBrowserCapabilities _brower;

        public override HttpBrowserCapabilitiesBase Browser
        {
            get
            {
                if (_brower == null)
                {
                    _brower = new MyHttpBrowserCapabilities();
                }
                return _brower;
            }
        }


        public override bool IsLocal
        {
            get
            {
                // return base.IsLocal;

                var remoteAddress = this.req.LocalIpAddress;                
                return !string.IsNullOrEmpty(remoteAddress) && (remoteAddress == "127.0.0.1" || remoteAddress == "::1" );

            }
        }


    }
}
