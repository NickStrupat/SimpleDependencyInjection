using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyInjection.Container {
	public static class Dependencies {
		private static class Registration<TInterface>
			where TInterface : class {
			public static Func<TInterface> Implementation;
		}

		public static void Register<TInterface, TImplementation>(Func<TImplementation> factory)
			where TImplementation : class, TInterface
			where TInterface : class {
			if (factory == null)
				throw new ArgumentNullException(nameof(factory));
			Registration<TInterface>.Implementation = factory;
		}

		public static void Register<TInterface, TImplementation>()
			where TImplementation : class, TInterface, new()
			where TInterface : class {
			Register<TInterface, TImplementation>(Factory<TImplementation>.Create);
		}

		public static void Register<TImplementation>(Func<TImplementation> factory)
			where TImplementation : class {
			Register(factory, RegisterMethod);
		}

		public static void Register<TImplementation>()
			where TImplementation : class, new() {
			Register(Factory<TImplementation>.Create);
		}

		private static void Register<TImplementation>(Func<TImplementation> factory, MethodInfo registerMethodInfo)
			where TImplementation : class {
			if (factory == null)
				throw new ArgumentNullException(nameof(factory));

			var type = typeof(TImplementation);
			var interfaces = type.GetInterfaces();
			var abstractBases = new List<Type>();
			while (type.BaseType != null && type.BaseType.IsAbstract)
				abstractBases.Add(type = type.BaseType);
			
			foreach (var @interface in abstractBases.Concat(interfaces))
				registerMethodInfo.MakeGenericMethod(@interface, type).Invoke(null, new Object[] { factory });
		}

		private static readonly MethodInfo RegisterMethod = typeof(Dependencies).GetMethods().Single(x => x.Name == nameof(Register) && x.GetGenericArguments().Count() == 2 && x.GetParameters().Count() == 1);



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
			Registration<TInterface>.Implementation = () => SingletonRegistration<TInterface>.Implementation.Value;
		}

		public static void RegisterSingleton<TInterface, TImplementation>()
			where TImplementation : class, TInterface, new()
			where TInterface : class {
			RegisterSingleton<TInterface, TImplementation>(Factory<TImplementation>.Create);
		}
		
		private static readonly MethodInfo RegisterSingletonMethod = typeof(Dependencies).GetMethods().Single(x => x.Name == nameof(RegisterSingleton) && x.GetGenericArguments().Count() == 2 && x.GetParameters().Count() == 1);

		public static void RegisterSingleton<TImplementation>(Func<TImplementation> factory)
			where TImplementation : class {
			Register(factory, RegisterSingletonMethod);
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