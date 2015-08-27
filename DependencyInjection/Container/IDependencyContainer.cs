using System;

namespace DependencyInjection.Container {
	public interface IDependencyContainer {
		void Register<TInterface, TImplementation>() where TImplementation : class, TInterface, new();
		void Register<TInterface, TImplementation>(Func<TImplementation> factory) where TImplementation : class, TInterface;
		TInterface Resolve<TInterface>();
	}
}