using System;
using System.Collections.Generic;

namespace Eventing.Api
{
    public interface IIocContainer
    {
        IEnumerable<T> GetAll<T>() where T : class;

        object GetService(Type serviceType);

        IEnumerable<object> GetServices(Type serviceType);

        IEnumerable<TTypeToResolve> GetServices<TTypeToResolve>();

        void Register<TBaseTypeToRegister, TTypeToReturn>(IocLifecycle lifecycle)
            where TTypeToReturn : class, TBaseTypeToRegister
            where TBaseTypeToRegister : class;

        void Register<TBaseTypeToRegister, TTypeToReturn>(TTypeToReturn instanceToRegister)
            where TTypeToReturn : class, TBaseTypeToRegister
            where TBaseTypeToRegister : class;

        void Register<TBaseTypeToRegister>(object instanceToRegister)
            where TBaseTypeToRegister : class;

        void RegisterMultiple<TBaseTypeToRegister, TTypeToReturn>(IocLifecycle lifecycle)
            where TTypeToReturn : class, TBaseTypeToRegister
            where TBaseTypeToRegister : class;
    }

    public enum IocLifecycle
    {
        PerRequest,
        Singleton
    }
}
