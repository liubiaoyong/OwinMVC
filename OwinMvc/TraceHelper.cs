using System.Diagnostics;

namespace OwinMvc
{
   
    internal static class TraceHelper
    {
        

        private static readonly TraceSource ts = new TraceSource("OwinMvc");
       
        static TraceHelper()
        {
            

        }


       

        public static void Error(string format, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                try
                {
                    format = string.Format(format, args);
                }
                catch
                {
                }
            }
            ts.TraceEvent(TraceEventType.Error, 0, format);
        }


        public static void Warning(string format, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                try
                {
                    format = string.Format(format, args);
                }
                catch
                {
                }
            }
            ts.TraceEvent(TraceEventType.Warning, 0, format);
        }


       


        public static void Information(string format, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                try
                {
                    format = string.Format(format, args);
                }
                catch
                {
                }
            }
            ts.TraceEvent(TraceEventType.Information, 0, format);
        }

        public static void Debug(string format, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                try
                {
                    format = string.Format(format, args);
                }
                catch
                {
                }
            }
            ts.TraceEvent(TraceEventType.Verbose, 0, format);
        }



    }
}
