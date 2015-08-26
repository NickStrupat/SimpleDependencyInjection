using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DependencyInjection.Container
{
    public class DependencyContainer : IDependencyContainer
    {
        private readonly Dictionary<Type, Lazy<Object>> _typeRegistrations = new Dictionary<Type, Lazy<Object>>();
        
        public void RegisterType<TInterface, TImplementation>()
        {
            if (_typeRegistrations.ContainsKey(typeof(TInterface)))
                throw new InvalidOperationException("Implementation type already registered for this interface.");
            _typeRegistrations.Add(typeof (TInterface), new Lazy<Object>(() => Resolver<TImplementation>.Resolve()));
        }

        public TInterface ResolveInstance<TInterface>()
        {
            if (!_typeRegistrations.ContainsKey(typeof(TInterface)))
                throw new InvalidOperationException("Implementation type not yet registered for this interface.");
            return (TInterface) _typeRegistrations[typeof(TInterface)].Value;
        }

        static class Resolver<T>
        {
            public static readonly Func<T> Resolve = GetResolver();

            private static Func<T> GetResolver()
            {
                var constructors = typeof(T).GetConstructors();
                if (constructors.Count() == 0)
                    throw new InvalidOperationException("Implementation type has no public constructor.");
                if (constructors.Count() > 1)
                    throw new InvalidOperationException("Implementation type has more than one public constructor.");

                var constructor = constructors.Single();
                var parameters = constructor.GetParameters();
                var resolveTypeInfo = typeof(DependencyContainer).getmeth
                var parameterResolveExpressions = new List<MethodCallExpression>();
                foreach (var parameterInfo in parameters)
                {
                    parameterResolveExpressions.Add(Expression.Call());
                }

                return Expression.Lambda<Func<T>>(Expression.New(typeof (T))).Compile();
            }
        }
    }
}