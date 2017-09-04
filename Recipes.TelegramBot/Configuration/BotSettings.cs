using System.Configuration;

namespace Recipes.TelegramBot.Configuration
{
	public class BotSettings : ConfigurationSection
	{
        [ConfigurationProperty("ApiKey")]
		public string ApiKey => (string)this["ApiKey"];
	}
}