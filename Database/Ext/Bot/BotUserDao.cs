namespace Akiled.Database.Daos.Bot;

using System;
using System.Collections.Generic;
using System.Data;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.RoomBots;
using Akiled.Utilities;

internal sealed class BotUserDao
{
    internal static void SaveBots(IQueryAdapter dbClient, List<RoomUser> botList)
    {
        var queryChunk = new QueryChunk();

        foreach (var bot in botList)
        {
            var botData = bot.BotData;
            if (botData.AiType == AIType.RolePlayBot)
            {
                continue;
            }

            if (bot.X != botData.X || bot.Y != botData.Y || bot.Z != botData.Z || bot.RotBody != botData.Rot)
            {
                queryChunk.AddQuery("UPDATE `bot_user` SET `x` = '" + bot.X + "', `y` = '" + bot.Y + "', `z` = '" + bot.Z + "', `rotation` = '" + bot.RotBody + "' WHERE `id` = " + bot.BotData.Id);
            }
        }

        queryChunk.Execute(dbClient);
        queryChunk.Dispose();
    }

    internal static void UpdateRoomId(IQueryAdapter dbClient, int botId)
    {
        dbClient.SetQuery("UPDATE `bot_user` SET `room_id` = '0' WHERE `id` = @id LIMIT 1");
        dbClient.AddParameter("id", botId);
        dbClient.RunQuery();
    }

    internal static void UpdatePosition(IQueryAdapter dbClient, int botId, int roomId, int x, int y) => dbClient.RunQuery("UPDATE `bot_user` SET room_id = '" + roomId + "', x = '" + x + "', y = '" + y + "' WHERE id = '" + botId + "'");

    internal static void UpdateLookGender(IQueryAdapter dbClient, int botId, string gender, string look)
    {
        dbClient.SetQuery("UPDATE `bot_user` SET look = @look, gender = '" + gender + "' WHERE id = '" + botId + "' LIMIT 1");
        dbClient.AddParameter("look", look);
        dbClient.RunQuery();
    }

    internal static void UpdateChat(IQueryAdapter dbClient, int botId, bool automaticChat, int speakingInterval, bool mixChat, string chatText)
    {
        dbClient.SetQuery("UPDATE `bot_user` SET chat_enabled = @AutomaticChat, chat_seconds = @SpeakingInterval, is_mixchat = @MixChat, chat_text = @ChatText WHERE id = @id LIMIT 1");
        dbClient.AddParameter("id", botId);
        dbClient.AddParameter("AutomaticChat", automaticChat ? "1" : "0");
        dbClient.AddParameter("SpeakingInterval", speakingInterval);
        dbClient.AddParameter("MixChat", mixChat ? "1" : "0");
        dbClient.AddParameter("ChatText", chatText);
        dbClient.RunQuery();
    }

    internal static void UpdateChatGPT(IQueryAdapter dbClient, int botId, string chatText)
    {
        dbClient.SetQuery("UPDATE `bot_user` SET chat_text = @ChatText, ai_type = 'chatgpt' WHERE id = @id LIMIT 1");
        dbClient.AddParameter("id", botId);
        dbClient.AddParameter("ChatText", chatText);
        dbClient.RunQuery();
    }

    internal static void UpdateWalkEnabled(IQueryAdapter dbClient, int botId, bool balkingEnabled) => dbClient.RunQuery("UPDATE `bot_user` SET walk_enabled = '" + (balkingEnabled ? "1" : "0") + "' WHERE id = '" + botId + "'");

    internal static void UpdateIsDancing(IQueryAdapter dbClient, int botId, bool isDancing) => dbClient.RunQuery("UPDATE `bot_user` SET is_dancing = '" + (isDancing ? "1" : "0") + "' WHERE id = '" + botId + "'");

    internal static void UpdateName(IQueryAdapter dbClient, int botId, string name)
    {
        dbClient.SetQuery("UPDATE `bot_user` SET name = @name WHERE id = '" + botId + "' LIMIT 1");
        dbClient.AddParameter("name", name);
        dbClient.RunQuery();
    }

    internal static void UpdateRoomBot(IQueryAdapter dbClient, int roomId) => dbClient.RunQuery("UPDATE `bot_user` SET room_id = '0' WHERE room_id = '" + roomId + "'");

    internal static int InsertAndGetId(IQueryAdapter dbClient, int ownerId, string name, string motto, string figure, string gender)
    {
        dbClient.SetQuery("INSERT INTO `bot_user` (user_id,name,motto,look,gender,chat_text) VALUES ('" + ownerId + "', '" + name + "', '" + motto + "', '" + figure + "', '" + gender + "', '')");

        return Convert.ToInt32(dbClient.InsertQuery());
    }

    internal static DataRow GetOne(IQueryAdapter dbClient, int ownerId, int id)
    {
        dbClient.SetQuery("SELECT id,user_id,name,motto,look,gender FROM `bot_user` WHERE user_id = '" + ownerId + "' AND id = '" + id + "' LIMIT 1");

        return dbClient.GetRow();
    }

    internal static void DupliqueAllBotInRoomId(IQueryAdapter dbClient, int userId, int roomId, int oldRoomId) => dbClient.RunQuery("INSERT INTO `bot_user` (user_id, name, motto, gender, look, room_id, walk_enabled, x, y, z, rotation, chat_enabled, chat_text, chat_seconds, is_dancing, is_mixchat) " +
        "SELECT '" + userId + "', name, motto, gender, look, '" + roomId + "', walk_enabled, x, y, z, rotation, chat_enabled, chat_text, chat_seconds, is_dancing, is_mixchat FROM `bot_user` WHERE room_id = '" + oldRoomId + "'");

    internal static void UpdateEnable(IQueryAdapter dbClient, int botId, int enableId) => dbClient.RunQuery("UPDATE `bot_user` SET enable = '" + enableId + "' WHERE id = '" + botId + "'");

    internal static void UpdateHanditem(IQueryAdapter dbClient, int botId, int handItem) => dbClient.RunQuery("UPDATE `bot_user` SET handitem = '" + handItem + "' WHERE id = '" + botId + "'");

    internal static void UpdateRotation(IQueryAdapter dbClient, int botId, int rotBody) => dbClient.RunQuery("UPDATE `bot_user` SET rotation = '" + rotBody + "' WHERE id = '" + botId + "'");

    internal static void UpdateStatus1(IQueryAdapter dbClient, int botId) => dbClient.RunQuery("UPDATE `bot_user` SET status = '1' WHERE id = '" + botId + "'");

    internal static void UpdateStatus0(IQueryAdapter dbClient, int botId) => dbClient.RunQuery("UPDATE `bot_user` SET status = '0' WHERE id = '" + botId + "'");

    internal static void UpdateStatus2(IQueryAdapter dbClient, int botId) => dbClient.RunQuery("UPDATE `bot_user` SET status = '2' WHERE id = '" + botId + "'");

    internal static DataTable GetOneByRoomId(IQueryAdapter dbClient, int roomId)
    {
        dbClient.SetQuery("SELECT `id`, `user_id`, `name`, `motto`, `gender`, `look`, `room_id`, `walk_enabled`, `x`, `y`, `z`, `rotation`, `chat_enabled`, `chat_text`, `chat_seconds`, `is_dancing`, `is_mixchat`, `status`, `enable`, `handitem`, `ai_type` FROM `bot_user` WHERE room_id = '" + roomId + "'");

        return dbClient.GetTable();
    }

    internal static void Delete(IQueryAdapter dbClient, int userId) => dbClient.RunQuery("DELETE FROM `bot_user` WHERE room_id = '0' AND user_id = '" + userId + "'");

    internal static DataTable GetAllByUserId(IQueryAdapter dbClient, int userId, int limit)
    {
        dbClient.SetQuery("SELECT `id`, `user_id`, `name`, `motto`, `gender`, `look`, `room_id`, `walk_enabled`, `x`, `y`, `z`, `rotation`, `chat_enabled`, `chat_text`, `chat_seconds`, `is_dancing`, `is_mixchat`, `status`, `enable`, `handitem`, `ai_type` FROM `bot_user` WHERE user_id = '" + userId + "' AND room_id = '0' LIMIT " + limit);

        return dbClient.GetTable();
    }
}
