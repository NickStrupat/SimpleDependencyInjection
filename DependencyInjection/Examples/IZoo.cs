using System;

namespace DependencyInjection.Examples
{
    public interface IZoo
    {
        void GetNewAnimal();
        void LetVisitorIn(int num);
        void ListVisitors();
        void ListAnimals();
    }
}