using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyInjection.Container
{
    public class DependencyContainer : IDependencyContainer
    {
        private readonly Dictionary<Type, Type> _container;

        public DependencyContainer()
        {
            _container = new Dictionary<Type, Type>();
        }

        public void RegisterType<T>(Type classType)
        {
            _container.Add(typeof(T), classType);
        }

        public T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        public object CreateInstance(Type interfaceType)
        {
            var resolvedType = _container[interfaceType];
            var constructors = resolvedType.GetConstructors();
            if (constructors.Any())
            {
                var constructorInfo = constructors.First();
                var resolvedParameters = ResolveConstructorParameters(constructorInfo).ToArray();
                return Activator.CreateInstance(resolvedType, resolvedParameters);
            }
            return Activator.CreateInstance(resolvedType);
        }

        private IEnumerable<object> ResolveConstructorParameters(ConstructorInfo constructorInfo)
        {
            //if you have circular dependencies you will run into trouble here
            foreach (var parameter in constructorInfo.GetParameters())
            {
                yield return CreateInstance(parameter.ParameterType);
            }
        }
    }
}