using Microsoft.Owin;
using System;
using System.Web;

namespace OwinMvc
{
    public class MyHttpResponse : HttpResponseBase, IDisposable
    {
        private readonly IOwinContext owinContext;
        private readonly IOwinResponse resp;

        public MyHttpResponse(IOwinContext oldContext)
        {
            this.owinContext = oldContext;
            this.resp = this.owinContext.Response;

            if (string.IsNullOrEmpty(resp.ContentType))
            {
                resp.ContentType = "text/html; charset=UTF-8";
            }
        }


        public override System.IO.Stream OutputStream
        {
            get
            {
                return this.resp.Body;
            }
        }

        private System.IO.TextWriter _output;

        public override System.IO.TextWriter Output
        {
            get
            {
                if (this._output == null)
                {
                    this._output = new System.IO.StreamWriter(this.resp.Body, System.Text.Encoding.UTF8, 1024, true);
                }
                return this._output;
            }
            set
            {
                base.Output = value;
            }
        }

        public override int StatusCode
        {
            get
            {
                return this.resp.StatusCode;
            }
            set
            {
                this.resp.StatusCode = value;
            }
        }


        public override void Write(string s)
        {
            this.resp.Write(s);
        }


        public override string ContentType
        {
            get
            {
                return this.resp.ContentType;
            }
            set
            {
                this.resp.ContentType = value;
            }
        }


        private readonly HttpCookieCollection _cookies = new HttpCookieCollection();

        public override HttpCookieCollection Cookies
        {
            get
            {
                return this._cookies;
            }
        }


        //private readonly System.Collections.Specialized.NameValueCollection _headers = new System.Collections.Specialized.NameValueCollection();

        private HeaderNameValueCollection _headers;

        public override System.Collections.Specialized.NameValueCollection Headers
        {
            get
            {
                if (this._headers == null)
                {
                    this._headers = new HeaderNameValueCollection(this.resp.Headers);
                }
                return this._headers;

            }
        }


        public override void AppendHeader(string name, string value)
        {
            this.resp.Headers.Set(name, value);
        }

        public override void AppendCookie(HttpCookie cookie)
        {
            this.resp.Cookies.Append(cookie.Name, cookie.Value, new CookieOptions()
            {
                Domain = cookie.Domain,
                Expires = cookie.Expires,
                HttpOnly = cookie.HttpOnly,
                Path = cookie.Path,
                Secure = cookie.Secure,
            });
        }


        public override string ApplyAppPathModifier(string virtualPath)
        {
            //return base.ApplyAppPathModifier(virtualPath);
            return virtualPath;
        }

        


        public void Dispose()
        {
            var obj = this._output;
            if (obj != null)
            {
                try
                {
                    obj.Dispose();
                }
                catch
                {
                    // TODO: 将cookies写入到Headers中。
                }
            }
        }
    }
}
