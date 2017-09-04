using System.Collections.Generic;
using Recipes.ObjectModel;

namespace Recipes.DataLayer
{
    public interface IDataLayer
    {
        IEnumerable<Recipe> GetRecipe(string[] ingredients);
    }
}