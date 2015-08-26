using System;

using DependencyInjection.Examples;

namespace DependencyInjection
{
    class Program
    {
        static void Main(string[] args)
        {
            var zoo = ApplicationContainer.Resolve<IZoo>();

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
