using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace DependencyInjection.Container {
	public class DependencyContainer : IDependencyContainer {
		private readonly ConcurrentDictionary<Type, Lazy<Object>> _typeRegistrations = new ConcurrentDictionary<Type, Lazy<Object>>();

		public void Register<TInterface, TImplementation>() where TImplementation : class, TInterface, new() {
			Register<TInterface, TImplementation>(() => Resolver<TImplementation>.Resolve());
		}

		public void Register<TInterface, TImplementation>(Func<TImplementation> factory) where TImplementation : class, TInterface {
			if (factory == null)
				throw new ArgumentNullException("factory");
			if (!_typeRegistrations.TryAdd(typeof(TInterface), new Lazy<Object>(factory)))
				throw new InvalidOperationException("Implementation type already registered for this interface.");
		}

		public TInterface Resolve<TInterface>() {
			Lazy<Object> factory;
			if (!_typeRegistrations.TryGetValue(typeof(TInterface), out factory))
				throw new InvalidOperationException("Implementation type not yet registered for this interface.");
			return (TInterface)factory.Value;
		}

		static class Resolver<TImplementation> where TImplementation : class, new() {
			public static readonly Func<TImplementation> Resolve = Expression.Lambda<Func<TImplementation>>(Expression.New(typeof(TImplementation))).Compile();
		}
	}
}