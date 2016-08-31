using OwinMvc.Owin;
using OwinMvc.Web;
using System;
using System.Threading.Tasks;

namespace Owin
{
    public static class MyMvcExtenstions
    {
        public static void UserCompactMvc(this IAppBuilder app)
        {
            MvcPreAppStartCode.Start();
            System.Threading.Thread.GetDomain().UnhandledException += CompactMvcOwinMiddleware_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            app.Use<MyMvcOwinMiddleware>();
        }


        static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine("task Unobserved exception :" + e.ToString());
        }

        static void CompactMvcOwinMiddleware_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("domain unhandle exception :" + e.ToString());

        }

    }
}
