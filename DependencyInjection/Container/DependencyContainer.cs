using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DependencyInjection.Container {
	public class DependencyContainer : IDependencyContainer {
		private readonly Dictionary<Type, Lazy<Object>> _typeRegistrations = new Dictionary<Type, Lazy<Object>>();

		public void Register<TInterface, TImplementation>() where TImplementation : class, TInterface, new() {
			Register<TInterface, TImplementation>(() => Resolver<TImplementation>.Resolve());
		}

		public void Register<TInterface, TImplementation>(Func<TImplementation> factory) where TImplementation : class, TInterface {
			if (_typeRegistrations.ContainsKey(typeof(TInterface)))
				throw new InvalidOperationException("Implementation type already registered for this interface.");
			_typeRegistrations.Add(typeof(TInterface), new Lazy<Object>(factory));
		}

		public TInterface Resolve<TInterface>() {
			if (!_typeRegistrations.ContainsKey(typeof(TInterface)))
				throw new InvalidOperationException("Implementation type not yet registered for this interface.");
			return (TInterface)_typeRegistrations[typeof(TInterface)].Value;
		}

		static class Resolver<TInterface> where TInterface : class, new() {
			public static readonly Func<TInterface> Resolve = Expression.Lambda<Func<TInterface>>(Expression.New(typeof(TInterface))).Compile();
		}
	}
}