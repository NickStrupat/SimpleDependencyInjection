namespace DependencyInjection.Examples
{
    public class AnimalService : IAnimalService {
	    public string GetNewAnimal()
        {
            return "Monkey"; //it's not a very good zoo
        }
    }
}