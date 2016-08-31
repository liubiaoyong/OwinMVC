using OwinMvc.Web;
using System.Web.WebPages;

namespace OwinMvc.View
{
    public abstract class MyViewPage :System.Web.Mvc.WebViewPage
    {

        private IVirtualPathFactory _virtualPathFactory;

        public override System.Web.WebPages.IVirtualPathFactory VirtualPathFactory
        {
            get
            {
                if (this._virtualPathFactory == null)
                {
                    this._virtualPathFactory = new MyVirtualPathFactory();
                }

                return this._virtualPathFactory;
            }
            set
            {
                this._virtualPathFactory = value;
            }
        }


        public override string VirtualPath
        {
            get
            {
                return base.VirtualPath;
            }
            set
            {
                base.VirtualPath = value;
            }
        }

        /// <summary>
        /// Normalizes path relative to the current virtual path and throws if a file does not exist at the location.
        /// Layout的路径问题是在这里解决的，现在只能使用绝对路径。
        /// </summary>
        /// <param name="layoutPagePath"></param>
        /// <returns></returns>
        protected override string NormalizeLayoutPagePath(string layoutPagePath)
        {
            return base.NormalizeLayoutPagePath(layoutPagePath);
        }
    }


    public abstract class MyViewPage<T> : System.Web.Mvc.WebViewPage<T>
    {

        private IVirtualPathFactory _virtualPathFactory;

        public override System.Web.WebPages.IVirtualPathFactory VirtualPathFactory
        {
            get
            {
                if (this._virtualPathFactory == null)
                {
                    this._virtualPathFactory = new MyVirtualPathFactory();
                }

                return this._virtualPathFactory;
            }
            set
            {
                this._virtualPathFactory = value;
            }
        }


        public override string VirtualPath
        {
            get
            {
                return base.VirtualPath;
            }
            set
            {
                base.VirtualPath = value;
            }
        }


        /// <summary>
        /// Normalizes path relative to the current virtual path and throws if a file does not exist at the location.
        /// Layout的路径问题是在这里解决的，现在只能使用绝对路径。
        /// </summary>
        /// </summary>
        /// <param name="layoutPagePath"></param>
        /// <returns></returns>
        protected override string NormalizeLayoutPagePath(string layoutPagePath)
        {
            return base.NormalizeLayoutPagePath(layoutPagePath);
        }


    }
}
