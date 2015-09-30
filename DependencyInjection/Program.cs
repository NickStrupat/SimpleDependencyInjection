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
			Dependencies.Register(() => new Zoo(Dependencies.Resolve<IAnimalService>(), Dependencies.Resolve<IVisitorService>()));
			Dependencies.Register(() => new VisitorService(Dependencies.Resolve<IPersonNameGenerator>()));
			Dependencies.RegisterSingleton<AnimalService>();
			Dependencies.Register<Bar>();

			//External Types
			Dependencies.Register<IPersonNameGenerator, PersonNameGenerator>();
		}

		static void Main(String[] args) {
			var foo = Dependencies.Resolve<Foo>();
			var zoo = Dependencies.Resolve<IZoo>();

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
