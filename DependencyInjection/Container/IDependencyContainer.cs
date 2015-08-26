using System;

namespace DependencyInjection.Container
{
    public interface IDependencyContainer
    {
        void RegisterType<TInterface, TImplementation>();
        TInterface ResolveInstance<TInterface>();
    }
}