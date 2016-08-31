using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace OwinMvc.Web
{
    internal class MapPathBasedVirtualFile : VirtualFile
    {
        private string _physicalPath;
        //private FindFileData _ffd;
        public override string Name
        {
            get
            {
                return System.IO.Path.GetFileName(_physicalPath);
            }
        }
        internal string PhysicalPath
        {
            get
            {
                return this._physicalPath;
            }
        }
        internal MapPathBasedVirtualFile(string virtualPath)
            : base(virtualPath)
        {
        }
        //internal MapPathBasedVirtualFile(string virtualPath, string physicalPath, FindFileData ffd)
        //    : base(virtualPath)
        //{
        //    this._physicalPath = physicalPath;
        //    this._ffd = ffd;
        //}
        //private void EnsureFileInfoObtained()
        //{
        //    if (this._physicalPath == null)
        //    {
        //        this._physicalPath = HostingEnvironment.MapPathInternal(base.VirtualPath);
        //        FindFileData.FindFile(this._physicalPath, out this._ffd);
        //    }
        //}
        public override Stream Open()
        {
            //this.EnsureFileInfoObtained();
            //TimeStampChecker.AddFile(base.VirtualPath, this._physicalPath);
            return new FileStream(this._physicalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}
