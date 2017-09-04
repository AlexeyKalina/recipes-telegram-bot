using System.Collections.Concurrent;

namespace Recipes.ObjectModel
{
	public class InMemoryChatStorage : IChatsInfoStorage
	{
		private readonly ConcurrentDictionary<long, ChatInfo> _chats = new ConcurrentDictionary<long, ChatInfo>();

		public ChatInfo GetChatInfoById(long chatId)
		{
			ChatInfo result;
			_chats.TryGetValue(chatId, out result);
			return result;
		}

		public void Add(long chatId, ChatInfo info)
		{
			_chats[chatId] = info;
		}
	}
}