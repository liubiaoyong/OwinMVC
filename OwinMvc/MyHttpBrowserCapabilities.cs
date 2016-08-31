using System.Web;

namespace OwinMvc
{
    class MyHttpBrowserCapabilities : HttpBrowserCapabilitiesBase
    {

        public override bool IsMobileDevice
        {
            get
            {
                return false;
            }
        }

        public override bool ActiveXControls
        {
            get
            {
                return false;
            }
        }

        
    }
}
