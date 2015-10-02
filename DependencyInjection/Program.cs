using System;

using DependencyInjection.Container;
using DependencyInjection.Examples;

using RandomNameGeneratorLibrary;

namespace DependencyInjection {
	abstract class Foo {
		protected Foo() {
			Console.WriteLine(GetType().Name);
		}
	}
	internal class Bar : Foo {}

	class Program {
		static Program() {
			DependencyContainer.Instance.Register(() => new Zoo(StaticContainer.Resolve<IAnimalService>(), StaticContainer.Resolve<IVisitorService>()));
			DependencyContainer.Instance.Register(() => new VisitorService(StaticContainer.Resolve<IPersonNameGenerator>()));
			DependencyContainer.Instance.RegisterSingleton<AnimalService>();
			DependencyContainer.Instance.Register<Bar>();

			//External Types
			DependencyContainer.Instance.Register<IPersonNameGenerator, PersonNameGenerator>();
		}

		static void Main(String[] args) {
			var foo = DependencyContainer.Instance.Resolve<Foo>();
			var zoo = DependencyContainer.Instance.Resolve<IZoo>();

			zoo.GetNewAnimal();
			zoo.GetNewAnimal();
			zoo.GetNewAnimal();
			zoo.GetNewAnimal();

			zoo.LetVisitorIn(5);

			Console.WriteLine("Visitors are:");
			zoo.ListVisitors();
			Console.WriteLine();

			Console.WriteLine("Animals are:");
			zoo.ListAnimals();

			Console.ReadKey();
		}
	}
}
