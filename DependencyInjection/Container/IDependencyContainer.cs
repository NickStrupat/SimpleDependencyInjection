using System;

namespace DependencyInjection.Container
{
    public interface IDependencyContainer
    {
        void RegisterType<T>(Type classType);
        T CreateInstance<T>();
    }
}