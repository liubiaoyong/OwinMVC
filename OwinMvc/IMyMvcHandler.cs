using System.Web;

namespace OwinMvc
{
    internal interface IMyMvcHandler
    {
        void ProcessRequest(HttpContextBase context);
    }
}
