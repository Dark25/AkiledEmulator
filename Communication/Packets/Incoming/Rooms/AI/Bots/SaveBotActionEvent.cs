
using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.RoomBots;
using System;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    internal class SaveBotActionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            if (currentRoom == null || !currentRoom.CheckRights(Session, true))
                return;
            int BotId = Packet.PopInt();
            int num = Packet.PopInt();
            string str1 = Packet.PopString();
            if (BotId <= 0 || (num < 1 || num > 5))
                return;
            RoomUser Bot = (RoomUser)null;
            if (!currentRoom.GetRoomUserManager().TryGetBot(BotId, out Bot))
                return;
            RoomBot botData = Bot.BotData;
            if (botData == null)
                return;
            switch (num)
            {
                case 1:
                    Bot.BotData.Look = Session.GetHabbo().Look;
                    Bot.BotData.Gender = Session.GetHabbo().Gender;
                    currentRoom.SendPacket((IServerPacket)new UserChangeComposer(Bot));
                    using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        queryReactor.SetQuery("UPDATE `bots` SET `look` = @look, `gender` = '" + Session.GetHabbo().Gender + "' WHERE `id` = '" + Bot.BotData.Id.ToString() + "' LIMIT 1");
                        queryReactor.AddParameter("look", Session.GetHabbo().Look);
                        queryReactor.RunQuery();
                        break;
                    }
                case 2:
                    string[] strArray1 = str1.Split(new string[1]
                    {
            ";#;"
                    }, StringSplitOptions.None);
                    string[] strArray2 = strArray1[0].Split(new char[2]
                    {
            '\r',
            '\n'
                    }, StringSplitOptions.RemoveEmptyEntries);
                    string str2 = Convert.ToString(strArray1[1]);
                    string str3 = Convert.ToString(strArray1[2]);
                    string str4 = Convert.ToString(strArray1[3]);
                    if (string.IsNullOrEmpty(str3) || Convert.ToInt32(str3) <= 0 || Convert.ToInt32(str3) < 7)
                        str3 = "7";
                    botData.AutomaticChat = Convert.ToBoolean(str2);
                    botData.SpeakingInterval = Convert.ToInt32(str3);
                    botData.MixSentences = Convert.ToBoolean(str4);
                    string Text = "";
                    for (int index = 0; index <= strArray2.Length - 1; ++index)
                    {
                        string str5 = strArray2[index];
                        if (str5.Length > 150)
                            str5.Substring(0, 150);
                        Text = Text + strArray2[index] + "\r";
                    }
                    botData.ChatText = Text;
                    botData.LoadRandomSpeech(Text);
                    using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        queryReactor.SetQuery("UPDATE `bots` SET `chat_enabled` = @AutomaticChat, `chat_seconds` = @SpeakingInterval, `is_mixchat` = @MixChat, `chat_text` = @ChatText WHERE `id` = @id LIMIT 1");
                        queryReactor.AddParameter("id", BotId);
                        queryReactor.AddParameter("AutomaticChat", AkiledEnvironment.BoolToEnum(Convert.ToBoolean(str2)));
                        queryReactor.AddParameter("SpeakingInterval", Convert.ToInt32(str3));
                        queryReactor.AddParameter("MixChat", AkiledEnvironment.BoolToEnum(Convert.ToBoolean(str4)));
                        queryReactor.AddParameter("ChatText", Text);
                        queryReactor.RunQuery();
                        break;
                    }
                case 3:
                    Bot.BotData.WalkingEnabled = !Bot.BotData.WalkingEnabled;
                    using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        queryReactor.RunQuery("UPDATE bots SET walk_enabled = '" + AkiledEnvironment.BoolToEnum(Bot.BotData.WalkingEnabled) + "' WHERE id = " + Bot.BotData.Id.ToString());
                        break;
                    }
                case 4:
                    if (Bot.DanceId > 0)
                    {
                        Bot.DanceId = 0;
                        Bot.BotData.IsDancing = false;
                    }
                    else
                    {
                        Random random = new Random();
                        Bot.DanceId = random.Next(1, 4);
                        Bot.BotData.IsDancing = true;
                    }
                    currentRoom.SendPacket((IServerPacket)new DanceComposer(Bot, Bot.DanceId));
                    using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        queryReactor.RunQuery("UPDATE bots SET is_dancing = '" + AkiledEnvironment.BoolToEnum(Bot.BotData.IsDancing) + "' WHERE id = " + Bot.BotData.Id.ToString());
                        break;
                    }
                case 5:
                    if (str1.Length == 0 || str1.Length >= 16 || (str1.Contains("<img src") || str1.Contains("<font ") || (str1.Contains("</font>") || str1.Contains("</a>")) || str1.Contains("<i>")))
                        break;
                    Bot.BotData.Name = str1;
                    using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        queryReactor.SetQuery("UPDATE `bots` SET `name` = @name WHERE `id` = '" + Bot.BotData.Id.ToString() + "' LIMIT 1");
                        queryReactor.AddParameter("name", str1);
                        queryReactor.RunQuery();
                    }
                    currentRoom.SendPacket((IServerPacket)new UserNameChangeMessageComposer(0, Bot.VirtualId, Bot.BotData.Name));
                    break;
            }
        }
    }
}
