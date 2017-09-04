namespace Recipes.ObjectModel
{
	public interface IChatsInfoStorage
	{
		ChatInfo GetChatInfoById(long chatId);
		void Add(long chatId, ChatInfo info);
	}
}