﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

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

	//public enum ImplementationLifetime {
	//	NewPerResolution,
	//	NewPerContainerInstance
	//}

	public class DependencyContainer : IDependencyContainer {
		private static Int64 count;
		private static void IncrementCount() => Interlocked.Increment(ref count);
		private static void DecrementCount() => Interlocked.Decrement(ref count);
		private static Int64 GetCount() => Interlocked.Read(ref count);

		private DependencyContainer() { Console.WriteLine(nameof(DependencyContainer)); }
		~DependencyContainer() { DecrementCount(); }

		public static IDependencyContainer Create() {
			var dc = GetCount() == 0 ? (IDependencyContainer) new StaticDependencyContainerWrapper() : new DependencyContainer();
			IncrementCount();
			return dc;
		}

		private static readonly Lazy<IDependencyContainer> instance = new Lazy<IDependencyContainer>(Create);
		public static IDependencyContainer Instance => instance.Value;
		
		internal static void Register<TImpl>(Func<TImpl> factory, MethodInfo registerMethodInfo, IDependencyContainer instance = null)
		where TImpl : class {
			if (factory == null)
				throw new ArgumentNullException(nameof(factory));

			var type = typeof(TImpl);
			var interfaces = type.GetInterfaces();
			var abstractBases = new List<Type>();
			while (type.BaseType != null && type.BaseType.IsAbstract)
				abstractBases.Add(type = type.BaseType);

			foreach (var @interface in abstractBases.Concat(interfaces))
				registerMethodInfo.MakeGenericMethod(@interface, type).Invoke(instance, new Object[] { factory });
		}

		private readonly ConcurrentDictionary<Type, Func<Object>> registrations = new ConcurrentDictionary<Type, Func<Object>>();

		private static void Register<TInterface, TImpl>(DependencyContainer dc, Func<TImpl> factory)
		where TImpl : class, TInterface
		where TInterface : class {
			if (factory == null)
				throw new ArgumentNullException(nameof(factory));

			dc.registrations.AddOrUpdate(typeof(TInterface), t => factory, (t, f) => factory);
		}
		private static readonly MethodInfo RegisterMethod = new Action<DependencyContainer, Func<Object>>(Register<Object, Object>).Method.GetGenericMethodDefinition();

		void IDependencyContainer.Register<TInterface, TImpl>(Func<TImpl> factory) => Register<TInterface, TImpl>(this, factory);
		void IDependencyContainer.Register<TInterface, TImpl>() => Register<TInterface, TImpl>(this, Factory<TImpl>.Create);
		void IDependencyContainer.Register<TImpl>(Func<TImpl> factory) => Register(factory, RegisterMethod);
		void IDependencyContainer.Register<TImpl>() => Register(Factory<TImpl>.Create, RegisterMethod);

		private static void RegisterSingleton<TInterface, TImpl>(DependencyContainer dc, Func<TImpl> factory)
		where TImpl : class, TInterface
		where TInterface : class {
			var lazySingleton = new Lazy<TImpl>(factory);
			Register<TInterface, TImpl>(dc, () => lazySingleton.Value);
		}
		private static readonly MethodInfo RegisterSingletonMethod = new Action<DependencyContainer, Func<Object>>(RegisterSingleton<Object, Object>).Method.GetGenericMethodDefinition();

		void IDependencyContainer.RegisterSingleton<TInterface, TImpl>(Func<TImpl> factory) => RegisterSingleton<TInterface, TImpl>(this, factory);
		void IDependencyContainer.RegisterSingleton<TInterface, TImpl>() => RegisterSingleton<TInterface, TImpl>(this, Factory<TImpl>.Create);
		void IDependencyContainer.RegisterSingleton<TImpl>(Func<TImpl> factory) => Register(factory, RegisterSingletonMethod);
		void IDependencyContainer.RegisterSingleton<TImpl>() => Register(Factory<TImpl>.Create, RegisterSingletonMethod);

		TInterface IDependencyContainer.Resolve<TInterface>() {
			Func<Object> factory;
			if (!registrations.TryGetValue(typeof(TInterface), out factory))
				throw new InvalidOperationException("No implementation registered for this interface.");
			return (TInterface) factory();
		}

		//public void Register<TInterface, TImplementation>(ImplementationLifetime implementationLifetime = ImplementationLifetime.NewPerResolution) where TImplementation : class, TInterface, new() {
		//	Register<TInterface, TImplementation>(() => Factory<TImplementation>.Create(), implementationLifetime);
		//}

		//public void Register<TInterface, TImplementation>(Func<TImplementation> factory, ImplementationLifetime implementationLifetime = ImplementationLifetime.NewPerResolution) where TImplementation : class, TInterface {
		//	if (factory == null)
		//		throw new ArgumentNullException(nameof(factory));
		//	if (!registrations.TryAdd(typeof(TInterface), GetFactory(factory, implementationLifetime)))
		//		throw new InvalidOperationException("Implementation type already registered for this interface.");
		//}

		//public TInterface Resolve<TInterface>() {
		//	IImplementationFactory implementationFactory;
		//	if (!registrations.TryGetValue(typeof(TInterface), out implementationFactory))
		//		throw new InvalidOperationException("Implementation type not yet registered for this interface.");
		//	return (TInterface) implementationFactory.Instance;
		//}



		//private interface IImplementationFactory {
		//	Object Instance { get; }
		//}

		//private class PerContainer : IImplementationFactory {
		//	private readonly Lazy<Object> factory;
		//	public PerContainer(Func<Object> factory) { this.factory = new Lazy<Object>(factory); }
		//	Object IImplementationFactory.Instance => factory.Value;
		//}

		//private class PerResolution : IImplementationFactory {
		//	private readonly Func<Object> factory;
		//	public PerResolution(Func<Object> factory) { this.factory = factory; }
		//	Object IImplementationFactory.Instance => factory();
		//}

		//private static IImplementationFactory GetFactory(Func<Object> factory, ImplementationLifetime implementationLifetime) {
		//	switch (implementationLifetime) {
		//		case ImplementationLifetime.NewPerContainerInstance:
		//			return new PerContainer(factory);
		//		case ImplementationLifetime.NewPerResolution:
		//			return new PerResolution(factory);
		//		default:
		//			throw new InvalidEnumArgumentException(nameof(implementationLifetime), (Int32) implementationLifetime, typeof(ImplementationLifetime));
		//	}
		//}
	}
}