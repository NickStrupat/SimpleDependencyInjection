using DependencyInjection.Container;
using DependencyInjection.Examples;

using RandomNameGeneratorLibrary;

namespace DependencyInjection {
	public static class ApplicationContainer {
		private static readonly IDependencyContainer Container;

		static ApplicationContainer() {
			Container = DependencyContainer.Create();

			//In House Types
			Container.RegisterSingleton<IZoo, Zoo>(() => new Zoo(Container.Resolve<IAnimalService>(), Container.Resolve<IVisitorService>()));
			Container.Register<IVisitorService, VisitorService>(() => new VisitorService(Container.Resolve<IPersonNameGenerator>()));
			Container.Register<IAnimalService, AnimalService>();

			//External Types
			Container.Register<IPersonNameGenerator, PersonNameGenerator>();
		}

		public static T Resolve<T>() where T : class {
			return Container.Resolve<T>();
		}
	}
}