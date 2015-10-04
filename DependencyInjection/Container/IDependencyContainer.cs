using System;

namespace DependencyInjection.Container {
	public interface IDependencyContainer : IDisposable {
		void Register<TInterface, TImplementation>(Func<TImplementation> factory) where TImplementation : class, TInterface where TInterface : class;
		void Register<TInterface, TImplementation>() where TImplementation : class, TInterface, new() where TInterface : class;

		void Register<TImplementation>(Func<TImplementation> factory) where TImplementation : class;
		void Register<TImplementation>() where TImplementation : class, new();

		void RegisterSingleton<TInterface, TImplementation>(Func<TImplementation> factory) where TImplementation : class, TInterface where TInterface : class;
		void RegisterSingleton<TInterface, TImplementation>() where TImplementation : class, TInterface, new() where TInterface : class;

		void RegisterSingleton<TImplementation>(Func<TImplementation> factory) where TImplementation : class;
		void RegisterSingleton<TImplementation>() where TImplementation : class, new();

		TInterface Resolve<TInterface>() where TInterface : class;
	}
}