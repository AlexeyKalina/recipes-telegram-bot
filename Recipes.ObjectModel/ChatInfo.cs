namespace Recipes.ObjectModel
{
    public class ChatInfo
    {
        public ChatInfo(long chatId)
        {
            ChatId = chatId;
        }
        public long ChatId { get; set; }
        public Recipe[] Recipes { get; set; }
        public int Index { get; set; }
    }
}