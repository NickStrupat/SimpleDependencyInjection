using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;

namespace DependencyInjection.Container {
	internal static class Factory<TImplementation> where TImplementation : class, new() {
		public static readonly Func<TImplementation> Create = GetConstructor();

		private static Func<TImplementation> GetConstructor() {
			var dynamicMethod = new DynamicMethod(typeof(TImplementation).Name + "Ctor", typeof(TImplementation), Type.EmptyTypes, typeof(TImplementation), true);
			var ilGen = dynamicMethod.GetILGenerator();
			ilGen.Emit(OpCodes.Newobj, typeof(TImplementation).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, new ParameterModifier[0]));
			ilGen.Emit(OpCodes.Ret);
			return (Func<TImplementation>) dynamicMethod.CreateDelegate(typeof(Func<TImplementation>));
		}
	}

	public enum ImplementationLifetime {
		NewPerResolution,
		NewPerContainerInstance
	}

	public class DependencyContainer : IDependencyContainer {

		private readonly ConcurrentDictionary<Type, IImplementationFactory> registrations = new ConcurrentDictionary<Type, IImplementationFactory>();

		public void Register<TInterface, TImplementation>(ImplementationLifetime implementationLifetime = ImplementationLifetime.NewPerResolution) where TImplementation : class, TInterface, new() {
			Register<TInterface, TImplementation>(() => Factory<TImplementation>.Create(), implementationLifetime);
		}

		public void Register<TInterface, TImplementation>(Func<TImplementation> factory, ImplementationLifetime implementationLifetime = ImplementationLifetime.NewPerResolution) where TImplementation : class, TInterface {
			if (factory == null)
				throw new ArgumentNullException(nameof(factory));
			if (!registrations.TryAdd(typeof(TInterface), GetFactory(factory, implementationLifetime)))
				throw new InvalidOperationException("Implementation type already registered for this interface.");
		}

		public TInterface Resolve<TInterface>() {
			IImplementationFactory implementationFactory;
			if (!registrations.TryGetValue(typeof(TInterface), out implementationFactory))
				throw new InvalidOperationException("Implementation type not yet registered for this interface.");
			return (TInterface) implementationFactory.Instance;
		}

		private interface IImplementationFactory {
			Object Instance { get; }
		}

		private class PerContainer : IImplementationFactory {
			private readonly Lazy<Object> factory;
			public PerContainer(Func<Object> factory) { this.factory = new Lazy<Object>(factory); }
			Object IImplementationFactory.Instance => factory.Value;
		}

		private class PerResolution : IImplementationFactory {
			private readonly Func<Object> factory;
			public PerResolution(Func<Object> factory) { this.factory = factory; }
			Object IImplementationFactory.Instance => factory();
		}

		private static IImplementationFactory GetFactory(Func<Object> factory, ImplementationLifetime implementationLifetime) {
			switch (implementationLifetime) {
				case ImplementationLifetime.NewPerContainerInstance:
					return new PerContainer(factory);
				case ImplementationLifetime.NewPerResolution:
					return new PerResolution(factory);
				default:
					throw new InvalidEnumArgumentException(nameof(implementationLifetime), (Int32) implementationLifetime, typeof(ImplementationLifetime));
			}
		}
	}
}