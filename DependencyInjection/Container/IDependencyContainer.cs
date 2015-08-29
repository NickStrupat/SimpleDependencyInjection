using System;

namespace DependencyInjection.Container {
	public interface IDependencyContainer {
		void Register<TInterface, TImplementation>(ImplementationLifetime implementationLifetime = ImplementationLifetime.NewPerResolution) where TImplementation : class, TInterface, new();
		void Register<TInterface, TImplementation>(Func<TImplementation> factory, ImplementationLifetime implementationLifetime = ImplementationLifetime.NewPerResolution) where TImplementation : class, TInterface;
		TInterface Resolve<TInterface>();
	}
}