using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

	public sealed class DependencyContainer : IDependencyContainer {
		private static Int64 containerCount;

		private DependencyContainer() { }
		~DependencyContainer() { Dispose(); }
		private readonly List<Action> disposers = new List<Action>();
		private readonly Object syncRoot = new Object();
		public void Dispose() {
			lock (syncRoot)
				foreach (var disposer in disposers)
					disposer();
			Interlocked.Decrement(ref containerCount);
		}

		public static IDependencyContainer Create() {
			Interlocked.Increment(ref containerCount);
			var dc = Interlocked.Read(ref containerCount) == 1 ? (IDependencyContainer) new StaticDependencyContainerWrapper() : new DependencyContainer();
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

		    var parameters = registerMethodInfo.GetParameters().Length == 2 ? new Object[] {instance, factory} : new Object[] {factory};
            foreach (var @interface in abstractBases.Concat(interfaces))
				registerMethodInfo.MakeGenericMethod(@interface, type).Invoke(null, parameters);
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
		void IDependencyContainer.Register<TImpl>() => Register(Factory<TImpl>.Create, RegisterMethod, this);

		private static void RegisterSingleton<TInterface, TImpl>(DependencyContainer dc, Func<TImpl> factory)
		where TImpl : class, TInterface
		where TInterface : class {
			var lazySingleton = new Lazy<TImpl>(factory);
			if (typeof(IDisposable).IsAssignableFrom(typeof(TImpl)))
				lock (dc.syncRoot)
					dc.disposers.Add(() => {
						if (lazySingleton.IsValueCreated)
							((IDisposable) lazySingleton.Value).Dispose();
					});
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
	}
}