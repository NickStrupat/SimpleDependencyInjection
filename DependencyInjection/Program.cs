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
			StaticContainer.Register(() => new Zoo(StaticContainer.Resolve<IAnimalService>(), StaticContainer.Resolve<IVisitorService>()));
			StaticContainer.Register(() => new VisitorService(StaticContainer.Resolve<IPersonNameGenerator>()));
			StaticContainer.RegisterSingleton<AnimalService>();
			StaticContainer.Register<Bar>();

			//External Types
			StaticContainer.Register<IPersonNameGenerator, PersonNameGenerator>();
		}

		static void Main(String[] args) {
			var foo = StaticContainer.Resolve<Foo>();
			var zoo = StaticContainer.Resolve<IZoo>();

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
