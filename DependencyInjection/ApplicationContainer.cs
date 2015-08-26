using System;

using DependencyInjection.Container;
using DependencyInjection.Examples;

using RandomNameGeneratorLibrary;

namespace DependencyInjection
{
    public static class ApplicationContainer
    {
        private static readonly DependencyContainer Container;

        static ApplicationContainer()
        {
            Container = new DependencyContainer();

            //In House Types
            Container.RegisterType<IZoo, Zoo>();
            Container.RegisterType<IVisitorService, VisitorService>();
            Container.RegisterType<IAnimalService, AnimalService>();

            //External Types
            Container.RegisterType<IPersonNameGenerator, PersonNameGenerator>();
        }

        public static T Resolve<T>()
        {
            return Container.CreateInstance<T>();
        }
    }
}