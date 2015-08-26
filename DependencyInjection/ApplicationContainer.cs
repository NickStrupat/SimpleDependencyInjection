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
            Container.RegisterType<IZoo>(typeof(Zoo));
            Container.RegisterType<IVisitorService>(typeof(VisitorService));
            Container.RegisterType<IAnimalService>(typeof(AnimalService));

            //External Types
            Container.RegisterType<IPersonNameGenerator>(typeof (PersonNameGenerator));
        }

        public static T Resolve<T>()
        {
            return Container.CreateInstance<T>();
        }
    }
}