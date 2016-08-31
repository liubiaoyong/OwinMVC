using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace OwinMvc.Web
{


    internal class MyVirtualPathProvider : VirtualPathProvider
    {
        private static readonly MyVirtualPathProvider _instalce;

        /// <summary>
        /// 应用程序的根目录,主要用于判断视图的路径，相当于"~"或者"~/"代表的位置。
        /// </summary>
        private static readonly string _AppRoot;

        /// <summary>
        /// 应用程序的程序集所在路径,对于web项目来讲，就相当于/bin目录。
        /// 对于自托管项目来讲，就相当于 _AppRoot.
        /// </summary>
        private static readonly string _binDir;


        static MyVirtualPathProvider()
        {
            _instalce = new MyVirtualPathProvider();
            _binDir = AppDomain.CurrentDomain.BaseDirectory;
            _AppRoot = GetAppRoot();
        }

        private MyVirtualPathProvider()
        {

        }

        public static VirtualPathProvider Instance
        {
            get { return _instalce;  }
        }

        private static string GetAppRoot()
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;

            //2016-6-13 zhumin 还是要考虑  \bin 的情况
            root = root.Replace('\\', '/').ToLower();

            if (root.EndsWith("/") || root.EndsWith(@"\"))
            {
                root = root.Substring(0, root.Length - 1);
            }

            if (root.EndsWith("/bin", StringComparison.OrdinalIgnoreCase))
            {                
                root = root.Substring(0, root.Length - 4);                
            }
            else if (root.EndsWith("/bin/debug", StringComparison.OrdinalIgnoreCase))
            {
                root = root.Substring(0, root.Length - 10);
            }
            else if (root.EndsWith("/bin/release", StringComparison.OrdinalIgnoreCase))
            {
                root = root.Substring(0, root.Length - 12);
            }

            return root;
        }


        private static readonly object _rootLocker = new object();

        /// <summary>
        /// 根目录对应的物理路径
        /// </summary>
        public static string AppRoot
        {
            get
            {
                return _AppRoot;

                //string text = MyVirtualPathProvider._AppRoot;
                //if (text == null)
                //{
                //    lock (_rootLocker)
                //    {
                //        if (_AppRoot == null)
                //        {
                //            //InternalSecurityPermissions.AppPathDiscovery.Assert();
                //            //text = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
                //            text = GetAppRoot();
                //            text = FileUtil.RemoveTrailingDirectoryBackSlash(text);
                //            _AppRoot = text;
                //        }
                //    }
                //}
                //return text;
            }
        }


        public static string BinDirectory
        {
            get { return _binDir;  }
        }

        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            throw new NotImplementedException();
            //HashCodeCombiner hashCodeCombiner = new HashCodeCombiner();
            //IEnumerator enumerator = virtualPathDependencies.GetEnumerator();
            //try
            //{
            //    while (enumerator.MoveNext())
            //    {
            //        string fileName = HostingEnvironment.MapPathInternal((string)enumerator.Current);
            //        hashCodeCombiner.AddFile(fileName);
            //    }
            //}
            //finally
            //{
            //    IDisposable disposable = enumerator as IDisposable;
            //    if (disposable != null)
            //    {
            //        disposable.Dispose();
            //    }
            //}
            //return hashCodeCombiner.CombinedHashString;
        }


        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            throw new NotImplementedException();
            //if (virtualPathDependencies == null)
            //{
            //    return null;
            //}
            //StringCollection stringCollection = null;
            //IEnumerator enumerator = virtualPathDependencies.GetEnumerator();
            //try
            //{
            //    while (enumerator.MoveNext())
            //    {
            //        string value = HostingEnvironment.MapPathInternal((string)enumerator.Current);
            //        if (stringCollection == null)
            //        {
            //            stringCollection = new StringCollection();
            //        }
            //        stringCollection.Add(value);
            //    }
            //}
            //finally
            //{
            //    IDisposable disposable = enumerator as IDisposable;
            //    if (disposable != null)
            //    {
            //        disposable.Dispose();
            //    }
            //}
            //if (stringCollection == null)
            //{
            //    return null;
            //}
            //string[] array = new string[stringCollection.Count];
            //stringCollection.CopyTo(array, 0);
            //return new CacheDependency(0, array, utcStart);
        }
        private string CreateCacheKey(bool isFile, string physicalPath)
        {
            if (isFile)
            {
                return "Bf" + physicalPath;
            }
            return "Bd" + physicalPath;
        }
        
        public override bool FileExists(string virtualPath)
        {
            var path = MapPath(virtualPath);
            return System.IO.File.Exists(path);
        }
        public override bool DirectoryExists(string virtualDir)
        {
            var path = MapPath(virtualDir);
            return System.IO.Directory.Exists(path);
        }
        public override VirtualFile GetFile(string virtualPath)
        {
            return new MapPathBasedVirtualFile(virtualPath);
        }

        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            //return new MapPathBasedVirtualDirectory(virtualDir);
            //return new MapPathBasedVirtualFile

            throw new NotImplementedException();
        }


        /// <summary>
        /// 将虚拟路径转换成物理路径
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static string MapPath(string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath))
            {
                virtualPath = "/";
            }
            if (virtualPath.StartsWith("~"))
            {
                virtualPath = virtualPath.Substring(1);
            }
            if (virtualPath.StartsWith("/"))
            {
                virtualPath = virtualPath.Substring(1);
            }
            //virtualPath = virtualPath.Replace('/', '\\');
            var path = Path.Combine(AppRoot, virtualPath);

            // TODO: 这里要判断是否是Mono.
            return path.Replace('/', '\\');
        }
    }
}
