using OwinMvc.View;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace OwinMvc
{
    internal static class MyAreaRegistrationUtility
    {

        /// <summary>
        /// Registers all areas in an ASP.NET MVC application.
        /// 这个方法不需要用到BuilderManager.
        /// </summary>
        public static void RegisterAllAreas()
        {
            MyAreaRegistrationUtility.RegisterAllAreas(RouteTable.Routes, null);
        }



        private static bool IsAreaRegistrationType(Type type)
        {
            return typeof(AreaRegistration).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null;
        }



        


        internal static void RegisterAllAreas(RouteCollection routes, object state)
        {

            List<Type> areaRegTypes = new List<Type>(256);
            var assemblies = DefaultReferencedAssemblyProvider.GetReferencedAssemblies();
            foreach(var assem in assemblies)
            {
                var types = AssemblyUtility.SafeGetAvaliableTypes(assem);

                foreach(var type in types)
                {
                    if (IsAreaRegistrationType(type))
                    {
                        AreaRegistration areaRegistration = (AreaRegistration)Activator.CreateInstance(type);
                        CreateContextAndRegister(areaRegistration, routes, state);
                    }
                }

            }

           
        }

        internal static void CreateContextAndRegister(AreaRegistration areaRegistration,  RouteCollection routes, object state)
        {
            AreaRegistrationContext areaRegistrationContext = new AreaRegistrationContext(areaRegistration.AreaName, routes, state);
            string @namespace = areaRegistration.GetType().Namespace;
            if (@namespace != null)
            {
                areaRegistrationContext.Namespaces.Add(@namespace + ".*");
            }
            areaRegistration.RegisterArea(areaRegistrationContext);
        }


    }
}
