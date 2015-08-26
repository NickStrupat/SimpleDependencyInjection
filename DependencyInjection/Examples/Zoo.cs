using System;
using System.Collections.Generic;

namespace DependencyInjection.Examples
{
    public class Zoo : IZoo
    {
        private IAnimalService _animalService;
        private IVisitorService _visitorService;

        private List<string> _visitors;
        private List<string> _animals; 

        public Zoo(IAnimalService animalService, IVisitorService visitorService)
        {
            _animalService = animalService;
            _visitorService = visitorService;
            _visitors = new List<string>();
            _animals = new List<string>();
        }

        public void GetNewAnimal()
        {
            _animals.Add(_animalService.GetNewAnimal());
        }

        public void LetVisitorIn(int num)
        {
            _visitors.AddRange(_visitorService.GetNewVisitor(num));
        }

        public void ListVisitors()
        {
            _visitors.ForEach(Console.WriteLine);
        }

        public void ListAnimals()
        {
            _animals.ForEach(Console.WriteLine);
        }
    }
}