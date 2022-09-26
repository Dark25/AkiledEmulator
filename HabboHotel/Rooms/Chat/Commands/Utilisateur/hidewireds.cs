    using Akiled.Communication.Packets.Outgoing;
    using Akiled.Communication.Packets.Outgoing.Structure;
    using Akiled.Database.Interfaces;
    using Akiled.HabboHotel.GameClients;
    using System.Collections.Generic;

    namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
    {
        class hidewireds : IChatCommand
        {
            public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
            {
                Room currentRoom = Session.GetHabbo().CurrentRoom;
                if (currentRoom == null)
                    return;

                currentRoom.HideWired = !currentRoom.HideWired;

            using (IQueryAdapter con = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                con.SetQuery("UPDATE `rooms` SET `allow_hidewireds` = @enum WHERE `id` = @id LIMIT 1");
                con.AddParameter("enum", AkiledEnvironment.BoolToEnum(Room.HideWired));
                con.AddParameter("id", Room.Id);
                con.RunQuery();
            }

            if (currentRoom.HideWired)
                {
                    UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.hidewireds.true", Session.Langue));
               
                }
                else
                {
                    UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.hidewireds.false", Session.Langue));
               
                }
                List<ServerPacket> list = new List<ServerPacket>();

                list = Room.HideWiredMessages(currentRoom.HideWired);

                Room.SendMessage(list);
            }
        }
    }
