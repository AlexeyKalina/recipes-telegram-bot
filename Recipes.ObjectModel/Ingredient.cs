namespace Recipes.ObjectModel
{
    public class Ingredient
    {
        public Ingredient(string name)
        {
            Name = name;
        }
        
        public string Name { get; }
    }
}