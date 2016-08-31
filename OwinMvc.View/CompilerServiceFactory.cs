
namespace OwinMvc.View
{
    internal static class CompilerServiceFactory
    {
        public static CompilerServiceBase CreateService()
        {
            return new CodeDomCompilerService();
        }
    }
}
