using System;
using System.Web.Mvc;

namespace OwinMvc.View
{
    class MyViewPageActivator : IViewPageActivator
    {
        private Func<IDependencyResolver> _resolverThunk;

        public MyViewPageActivator()
            : this(null)
        {
        }

        public MyViewPageActivator(IDependencyResolver resolver)
        {
            if (resolver == null)
            {
                _resolverThunk = () => DependencyResolver.Current;
            }
            else
            {
                _resolverThunk = () => resolver;
            }
        }

        public object Create(ControllerContext controllerContext, Type type)
        {
            try
            {
                return _resolverThunk().GetService(type) ?? Activator.CreateInstance(type);
            }
            catch (MissingMethodException exception)
            {
                // Ensure thrown exception contains the type name.  Might be down a few levels.
                MissingMethodException replacementException =
                    TypeHelpers.EnsureDebuggableException(exception, type.FullName);
                if (replacementException != null)
                {
                    throw replacementException;
                }

                throw;
            }
        }
    }

}
