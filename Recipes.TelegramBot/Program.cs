using System;
using System.Configuration;
using Recipes.DataLayer;
using Recipes.ObjectModel;
using Recipes.TelegramBot.Configuration;

namespace Recipes.TelegramBot
{
    internal class Program
    {
        private static void Main()
        {
            var botSettings = (BotSettings)ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).Sections["BotSettings"];
            var neo4JSettings = (Neo4JSettings)ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).Sections["Neo4JSettings"];
            
            IDataLayer dataLayer = new Neo4JDataLayer(neo4JSettings.ConnectionString, neo4JSettings.Username, neo4JSettings.Password);
            IChatsInfoStorage chatsInfoStorage = new InMemoryChatStorage();
            
            var bot = new Bot(botSettings.ApiKey, dataLayer, chatsInfoStorage);
            bot.Start();
            Console.Read();
        }
    }
}