using System;
using System.Collections.Generic;
using TinyIoC;

namespace Euventing.Api
{
    public class TinyIocContainerImplementation : IIocContainer
    {
        private readonly TinyIoCContainer tinyIoCContainer;

        public TinyIocContainerImplementation(TinyIoCContainer container)
        {
            this.tinyIoCContainer = container;

            this.RegisterMyselfInMyOwnContainer();
        }

        private void RegisterMyselfInMyOwnContainer()
        {
            this.tinyIoCContainer.Register<IIocContainer, TinyIocContainerImplementation>(this);
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            return this.tinyIoCContainer.ResolveAll<T>();
        }

        public object GetService(Type serviceType)
        {
            return this.tinyIoCContainer.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.tinyIoCContainer.ResolveAll(serviceType);
        }

        public IEnumerable<TTypeToResolve> GetServices<TTypeToResolve>()
        {
            throw new NotImplementedException();
        }

        public void Register<TBaseTypeToRegister, TTypeToReturn>(IocLifecycle lifecycle)
            where TTypeToReturn : class, TBaseTypeToRegister
            where TBaseTypeToRegister : class
        {
            if (lifecycle == IocLifecycle.PerRequest)
                this.tinyIoCContainer.Register<TBaseTypeToRegister, TTypeToReturn>();
            else
                this.tinyIoCContainer.Register<TBaseTypeToRegister, TTypeToReturn>().AsSingleton();
        }


        public void Register<TBaseTypeToRegister, TTypeToReturn>(TTypeToReturn instanceToRegister)
            where TTypeToReturn : class, TBaseTypeToRegister
            where TBaseTypeToRegister : class
        {
            this.tinyIoCContainer.Register<TBaseTypeToRegister, TTypeToReturn>(instanceToRegister);
        }

        public void Register<TBaseTypeToRegister>(object instanceToRegister) where TBaseTypeToRegister : class
        {
            this.tinyIoCContainer.Register<TBaseTypeToRegister>((TBaseTypeToRegister)instanceToRegister);
        }

        public void RegisterMultiple<TBaseTypeToRegister, TTypeToReturn>(IocLifecycle lifecycle)
            where TBaseTypeToRegister : class
            where TTypeToReturn : class, TBaseTypeToRegister
        {
            if (lifecycle == IocLifecycle.PerRequest)
                this.tinyIoCContainer.RegisterMultiple<TBaseTypeToRegister>(new[] { typeof(TTypeToReturn) });
            else
                this.tinyIoCContainer.RegisterMultiple<TBaseTypeToRegister>(new[] { typeof(TTypeToReturn) }).AsSingleton();
        }
    }
}
