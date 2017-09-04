using System;
using System.Collections.Generic;
using System.Linq;
using Neo4jClient;
using Neo4jClient.Cypher;
using Recipes.ObjectModel;

namespace Recipes.DataLayer
{
    public class Neo4JDataLayer : IDataLayer
    {
        private readonly string _connectionString;
        private readonly string _userName;
        private readonly string _password;
        
        public Neo4JDataLayer(string connectionString, string userName, string password)
        {
            _connectionString = connectionString;
            _userName = userName;
            _password = password;
        }

        public IEnumerable<Recipe> GetRecipe(string[] ingredients)
        {
            using (var graphClient = new GraphClient(new Uri(_connectionString), _userName, _password))
            {
                graphClient.Connect();

                var resultRecipes = graphClient.Cypher.Match("(recipe:Recipe)-[contains:Contains]->(ingredientExists:Ingredient)")
                    .Where("any(name in {ingredientNames} WHERE ingredientExists.Name =~ name)")
                    .WithParam("ingredientNames", ingredients.Select(ing => string.Format("{0}.*",ing)))
                    .Match("(recipe:Recipe)-[contains:Contains]->(ingredientExists:Ingredient)")
                    .With("recipe, count(ingredientExists) AS countExists")
                    .Match("(recipe:Recipe)-[contains2:Contains]->(ingredient:Ingredient)")
                    .With("recipe, countExists, count(contains2) AS allCount, collect(distinct ingredient.Name) AS ingredientsList")
                    .Return((recipe, ingredientsList) => new Recipe
                    {
                        Name = Return.As<string>("recipe.Name"),
                        Ingredients = ingredientsList.As<IEnumerable<string>>()
                    })  
                    .OrderBy("countExists DESC, allCount - countExists")
                    .Limit(100)
                    .Results.ToList();

                return resultRecipes;
            }
        }
    }
}