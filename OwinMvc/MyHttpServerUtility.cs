using System.Web;

namespace OwinMvc
{
    class MyHttpServerUtility :HttpServerUtilityBase
    {
        private readonly MyHttpContext _context;

        public MyHttpServerUtility(MyHttpContext context) :base()
        {
            this._context = context;
        }

        public override void Execute(string path)
        {
            base.Execute(path);
        }

        public override void Execute(string path, System.IO.TextWriter writer)
        {
            base.Execute(path, writer);
        }


        public override void Execute(IHttpHandler handler, System.IO.TextWriter writer, bool preserveForm)
        {
            //base.Execute(handler, writer, preserveForm);
            //handler.ProcessRequest(this._context)

            var handler1 = handler as IMyMvcHandler;
            if (handler1 != null)
            {
                handler1.ProcessRequest(this._context);
            }


        }
    }
}
