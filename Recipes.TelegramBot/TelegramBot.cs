using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recipes.DataLayer;
using Recipes.ObjectModel;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;

namespace Recipes.TelegramBot
{
    public class Bot
    {
        private readonly TelegramBotClient _bot;
        private readonly IDataLayer _dataLayer;
        private InlineKeyboardMarkup _keyboard;
        private IChatsInfoStorage _chatsInfoStorage;


        public Bot(string token, IDataLayer dataLayer, IChatsInfoStorage chatsInfoStorage)
        {
            _bot = new TelegramBotClient(token);
            _bot.OnMessage += BotOnMessageReceived;
            _bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            _keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    new InlineKeyboardCallbackButton("Показать еще", "next"),
                    new InlineKeyboardCallbackButton("Ингредиенты", "ingredients"),
                }
            });

            _dataLayer = dataLayer;
            _chatsInfoStorage = chatsInfoStorage;
        }

        public void Start()
        {
            _bot.StartReceiving();
        }

        private async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.TextMessage)
                return;

            var text = message.Text;
            var ingredients = text.Split(' ');

            var chatInfo = _chatsInfoStorage.GetChatInfoById(message.Chat.Id);
            if (chatInfo == null)
            {
                chatInfo = new ChatInfo(message.Chat.Id);
                _chatsInfoStorage.Add(message.Chat.Id, chatInfo);
            }

            chatInfo.Recipes = _dataLayer.GetRecipe(ingredients).ToArray();
            chatInfo.Index = -1;

            if (chatInfo.Recipes == null || !chatInfo.Recipes.Any())
                await _bot.SendTextMessageAsync(message.Chat.Id, "Сорян, ничего не найдено");
            else
            {
                chatInfo.Index++;
                await _bot.SendTextMessageAsync(message.Chat.Id, chatInfo.Recipes[chatInfo.Index].Name, 
                    parseMode: ParseMode.Html, replyMarkup: _keyboard);
            }
        }

        private async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var chatInfo = _chatsInfoStorage.GetChatInfoById(callbackQueryEventArgs.CallbackQuery.Message.Chat.Id);

            if (chatInfo == null)
                return;

            if (callbackQueryEventArgs.CallbackQuery.Data.StartsWith("next"))
            {
                if (chatInfo.Recipes == null)
                    return;

                if (chatInfo.Index == chatInfo.Recipes.Length)
                    chatInfo.Index = 0;

                chatInfo.Index++;
                await _bot.SendTextMessageAsync(callbackQueryEventArgs.CallbackQuery.Message.Chat.Id, 
                chatInfo.Recipes[chatInfo.Index].Name, parseMode: ParseMode.Html, replyMarkup: _keyboard);
            }

            else if (callbackQueryEventArgs.CallbackQuery.Data.StartsWith("ingredients"))
            {
                if (chatInfo.Recipes == null)
                    return;

                var body = new StringBuilder();
                body.AppendLine(string.Format("<b>{0}</b>", chatInfo.Recipes[chatInfo.Index].Name));

                int ingredientCounter = 1;
                foreach (var ingredient in chatInfo.Recipes[chatInfo.Index].Ingredients)
                {
                    body.AppendLine(string.Format("{0}. {1}", ingredientCounter, ingredient));
                    ingredientCounter++;
                }

                await _bot.SendTextMessageAsync(callbackQueryEventArgs.CallbackQuery.Message.Chat.Id, 
                body.ToString(), parseMode: ParseMode.Html, replyMarkup: _keyboard);
            }
        }
    }
}