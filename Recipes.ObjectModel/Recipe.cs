using System.Collections.Generic;

namespace Recipes.ObjectModel
{
    public class Recipe
    {        
        public string Name { get; set; }
        public IEnumerable<string> Ingredients { get; set; }
    }
}