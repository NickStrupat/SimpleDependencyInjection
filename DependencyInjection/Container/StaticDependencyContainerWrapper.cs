using System;

namespace DependencyInjection.Container {
	internal class StaticDependencyContainerWrapper : IDependencyContainer {
		internal StaticDependencyContainerWrapper() { StaticContainer.Reset(); Console.WriteLine(nameof(StaticDependencyContainerWrapper)); }
		~StaticDependencyContainerWrapper() { StaticContainer.Reset(); }
		void IDependencyContainer.Register<TInterface, TImpl>(Func<TImpl> factory) => StaticContainer.Register<TInterface, TImpl>(factory);
		void IDependencyContainer.Register<TInterface, TImpl>() => StaticContainer.Register<TInterface, TImpl>();
		void IDependencyContainer.Register<TImpl>(Func<TImpl> factory) => StaticContainer.Register<TImpl>(factory);
		void IDependencyContainer.Register<TImpl>() => StaticContainer.Register<TImpl>();
		void IDependencyContainer.RegisterSingleton<TInterface, TImpl>(Func<TImpl> factory) => StaticContainer.RegisterSingleton<TInterface, TImpl>(factory);
		void IDependencyContainer.RegisterSingleton<TInterface, TImpl>() => StaticContainer.RegisterSingleton<TInterface, TImpl>();
		void IDependencyContainer.RegisterSingleton<TImpl>(Func<TImpl> factory) => StaticContainer.RegisterSingleton<TImpl>(factory);
		void IDependencyContainer.RegisterSingleton<TImpl>() => StaticContainer.RegisterSingleton<TImpl>();
		TInterface IDependencyContainer.Resolve<TInterface>() => StaticContainer.Resolve<TInterface>();
	}
}