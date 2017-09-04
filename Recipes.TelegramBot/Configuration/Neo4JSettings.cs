using System.Configuration;

namespace Recipes.TelegramBot.Configuration
{
	public class Neo4JSettings : ConfigurationSection
	{
		[ConfigurationProperty("ConnectionString")]
		public string ConnectionString => (string)this["ConnectionString"];

		[ConfigurationProperty("Username")]
		public string Username => (string)this["Username"];

		[ConfigurationProperty("Password")]
		public string Password => (string)this["Password"];
	}
}