
namespace OwinMvc.View
{
    internal static class TemplateBuilder
    {

        //private static long _DynamicClassIndex = DateTime.Now.Ticks;


        //private static string GetClassName(string viewPath)
        //{
        //    StringBuilder sb = new StringBuilder(128);
        //    sb.Append("DynamicClass_")
        //        .Append(Interlocked.Increment(ref _DynamicClassIndex))
        //        .Append("_")
        //        .Append(TypeContext.GetClassNameFromFullPath(viewPath));
        //    return sb.ToString();
        //}



        internal static System.Type BuilderType(string viewPath)
        {
            var typeContext = new TypeContext(viewPath);
            var result = SourceGenerator.GetCompileUnit(viewPath);
            var sourceCode = SourceGenerator.InspectSource(result);



            var compileContext = new CompileContext(sourceCode, null, null);
            //CSharpRoslynCompilerService service = new CSharpRoslynCompilerService();
            //t = service.CompileType(compileContext);

            var service = CompilerServiceFactory.CreateService();
            var t = service.CompileType(compileContext);
            return t;
        }
    }
}
