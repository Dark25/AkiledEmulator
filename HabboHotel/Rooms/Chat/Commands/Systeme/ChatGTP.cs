namespace AkiledEmulator.HabboHotel.Rooms.Chat.Commands.Systeme;

using Akiled;
using Akiled.Database.Daos.Bot;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.Chat.Commands;
using Akiled.HabboHotel.Rooms.RoomBots;

internal sealed class ChatGTP : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var bot = room.GetRoomUserManager.GetBotOrPetByName(parameters[1]);
        if (bot == null || bot.BotData.AiType == AIType.ChatGPT)
        {
            return;
        }

        bot.BotData.AiType = AIType.ChatGPT;
        bot.BotData.ChatText = AkiledEnvironment.GetSettingsManager().TryGetValue("openia.prompt");
        bot.BotData.LoadRandomSpeech(bot.BotData.ChatText);

        bot.BotAI = bot.BotData.GenerateBotAI(bot.VirtualId);
        bot.BotAI.Init(bot.BotData.Id, bot, room);

        var dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor();
        BotUserDao.UpdateChatGPT(dbClient, bot.BotData.Id, bot.BotData.ChatText);

        userRoom.SendWhisperChat("ChatGPT vient d'être activé !");
    }
}
