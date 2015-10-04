using System;
using System.Collections.Generic;
using System.Reflection;

namespace DependencyInjection.Container {
	internal static class StaticContainer {
		private static readonly List<Action> resetActions = new List<Action>();
		private static readonly Object syncRoot = new Object();

		internal static void Reset() {
			lock (syncRoot) {
				foreach (var resetAction in resetActions)
					resetAction();
				resetActions.Clear();
			}
		}

		private static class Registration<TInterface>
		where TInterface : class {
			public static Func<TInterface> Implementation;
		}

		public static void Register<TInterface, TImplementation>(Func<TImplementation> factory)
		where TImplementation : class, TInterface
		where TInterface : class {
			if (factory == null)
				throw new ArgumentNullException(nameof(factory));

			lock (syncRoot) {
				Registration<TInterface>.Implementation = factory;
				resetActions.Add(() => Registration<TInterface>.Implementation = null);
			}
		}

		public static void Register<TInterface, TImplementation>()
		where TImplementation : class, TInterface, new()
		where TInterface : class {
			Register<TInterface, TImplementation>(Factory<TImplementation>.Create);
		}

		private static readonly MethodInfo RegisterMethod = new Action<Func<Object>>(Register<Object, Object>).Method.GetGenericMethodDefinition();

		public static void Register<TImplementation>(Func<TImplementation> factory)
		where TImplementation : class {
			DependencyContainer.Register(factory, RegisterMethod);
		}

		public static void Register<TImplementation>()
		where TImplementation : class, new() {
			Register(Factory<TImplementation>.Create);
		}



		private static class SingletonRegistration<TInterface>
		where TInterface : class {
			public static Lazy<TInterface> Implementation;
		}

		public static void RegisterSingleton<TInterface, TImplementation>(Func<TImplementation> factory)
		where TImplementation : class, TInterface
		where TInterface : class {
			if (factory == null)
				throw new ArgumentNullException(nameof(factory));

			SingletonRegistration<TInterface>.Implementation = new Lazy<TInterface>(factory);
			if (typeof(IDisposable).IsAssignableFrom(typeof(TImplementation)))
				lock (syncRoot)
					resetActions.Add(() => {
						if (SingletonRegistration<TInterface>.Implementation.IsValueCreated)
							((IDisposable) (TImplementation) SingletonRegistration<TInterface>.Implementation.Value).Dispose();
					});
			Register<TInterface, TImplementation>(() => (TImplementation) SingletonRegistration<TInterface>.Implementation.Value);
		}

		public static void RegisterSingleton<TInterface, TImplementation>()
		where TImplementation : class, TInterface, new()
		where TInterface : class {
			RegisterSingleton<TInterface, TImplementation>(Factory<TImplementation>.Create);
		}

		private static readonly MethodInfo RegisterSingletonMethod = new Action<Func<Object>>(RegisterSingleton<Object, Object>).Method.GetGenericMethodDefinition();

		public static void RegisterSingleton<TImplementation>(Func<TImplementation> factory)
		where TImplementation : class {
			DependencyContainer.Register(factory, RegisterSingletonMethod);
		}

		public static void RegisterSingleton<TImplementation>()
		where TImplementation : class, new() {
			RegisterSingleton(Factory<TImplementation>.Create);
		}



		public static TInterface Resolve<TInterface>()
		where TInterface : class {
			if (Registration<TInterface>.Implementation == null)
				throw new InvalidOperationException("No implementation registered for this interface.");
			return Registration<TInterface>.Implementation();
		}
	}
}