using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System.Collections.Generic;
using System.Text;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class SaveRoomSettingsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int RoomId = Packet.PopInt();

            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);
            if (room == null)
                return;

            if (!room.CheckRights(Session, true) && !Session.GetHabbo().HasFuse("fuse_settings_room"))
                return;


            string Name = Packet.PopString();
            string Description = Packet.PopString();
            int State = Packet.PopInt();
            string Password = Packet.PopString();
            int MaxUsers = Packet.PopInt();
            int CategoryId = Packet.PopInt();
            int TagCount = Packet.PopInt();
            List<string> tags = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < TagCount; ++index)
            {
                if (index > 0)
                    stringBuilder.Append(",");
                string tag = Packet.PopString().ToLower();
                tags.Add(tag);
                stringBuilder.Append(tag);
            }
            int TrocStatus = Packet.PopInt(); // trade status (0 = disabled, 1 = dueños de la sala, 2 = todos)
            bool AllowPets = Packet.PopBoolean();
            bool AllowPetsEat = Packet.PopBoolean();
            bool AllowWalkthrough = Packet.PopBoolean();
            bool Hidewall = Packet.PopBoolean();
            int WallThickness = Packet.PopInt();
            int FloorThickness = Packet.PopInt();
            int mutefuse = Packet.PopInt();
            int kickfuse = Packet.PopInt();
            int banfuse = Packet.PopInt();
            int ChatType = Packet.PopInt();
            int ChatBalloon = Packet.PopInt();
            int ChatSpeed = Packet.PopInt();
            int ChatMaxDistance = Packet.PopInt();
            int ChatFloodProtection = Packet.PopInt();

            if (WallThickness < -2 || WallThickness > 1)
                WallThickness = 0;

            if (FloorThickness < -2 || FloorThickness > 1)
                FloorThickness = 0;
            if (Name.Length < 1 || Name.Length > 100)
                return;
            if (State < 0 || State > 3)
                return;
            if (MaxUsers < 10 || MaxUsers > 75)
                MaxUsers = 25;

            if (TrocStatus < 0 || TrocStatus > 2)
                TrocStatus = 0;

            if (TagCount > 2 || mutefuse != 0 && mutefuse != 1 || kickfuse != 0 && kickfuse != 1 && kickfuse != 2 || banfuse != 0 && banfuse != 1)
                return;

            if (ChatMaxDistance > 99)
                ChatMaxDistance = 99;

            room.RoomData.AllowPets = AllowPets;
            room.RoomData.AllowPetsEating = AllowPetsEat;
            room.RoomData.AllowWalkthrough = AllowWalkthrough;
            room.RoomData.Hidewall = Hidewall;
            room.RoomData.Name = Name;
            room.RoomData.State = State;
            room.RoomData.Description = Description;
            room.RoomData.Category = CategoryId;
            if (!string.IsNullOrEmpty(Password))
                room.RoomData.Password = Password;
            room.ClearTags();
            room.AddTagRange(tags);
            room.RoomData.Tags.Clear();
            room.RoomData.Tags.AddRange((IEnumerable<string>)tags);
            room.RoomData.UsersMax = MaxUsers;
            room.RoomData.WallThickness = WallThickness;
            room.RoomData.FloorThickness = FloorThickness;
            room.RoomData.MuteFuse = mutefuse;
            room.RoomData.WhoCanKick = kickfuse;
            room.RoomData.BanFuse = banfuse;

            room.RoomData.ChatType = ChatType;
            room.RoomData.ChatBalloon = ChatBalloon;
            room.RoomData.ChatSpeed = ChatSpeed;
            room.RoomData.ChatMaxDistance = ChatMaxDistance;
            room.RoomData.ChatFloodProtection = ChatFloodProtection;

            room.RoomData.TrocStatus = TrocStatus;
            string str5 = "open";
            if (room.RoomData.State == 1)
                str5 = "locked";
            else if (room.RoomData.State == 2)
                str5 = "password";
            else if (room.RoomData.State == 3)
                str5 = "hide";

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("UPDATE rooms SET caption = @caption, description = @description, password = @password, category = '" + CategoryId + "', state = '" + str5 + "', tags = @tags, users_max = '" + MaxUsers + "', allow_pets = '" + (AllowPets ? 1 : 0) + "', allow_pets_eat = '" + (AllowPetsEat ? 1 : 0) + "', allow_walkthrough = '" + (AllowWalkthrough ? 1 : 0) + "', allow_hidewall = '" + (room.RoomData.Hidewall ? 1 : 0) + "', floorthick = '" + room.RoomData.FloorThickness + "', wallthick = '" + room.RoomData.WallThickness + "', moderation_mute_fuse = '" + mutefuse + "', moderation_kick_fuse = '" + kickfuse + "', moderation_ban_fuse = '" + banfuse + "', chat_type = '" + ChatType + "', chat_balloon = '" + ChatBalloon + "', chat_speed = '" + ChatSpeed + "', chat_max_distance = '" + ChatMaxDistance + "', chat_flood_protection = '" + ChatFloodProtection + "', TrocStatus = '" + TrocStatus + "' WHERE id = " + room.Id);
                queryreactor.AddParameter("caption", room.RoomData.Name);
                queryreactor.AddParameter("description", room.RoomData.Description);
                queryreactor.AddParameter("password", room.RoomData.Password);
                queryreactor.AddParameter("tags", (stringBuilder).ToString());
                queryreactor.RunQuery();
            }

            ServerPacket Response = new ServerPacket(ServerPacketHeader.RoomSettingsSavedMessageComposer);
            Response.WriteInteger(room.Id);
            Session.SendPacket(Response);

            ServerPacket Response2 = new ServerPacket(ServerPacketHeader.RoomVisualizationSettingsMessageComposer);
            Response2.WriteBoolean(room.RoomData.Hidewall);
            Response2.WriteInteger(room.RoomData.WallThickness);
            Response2.WriteInteger(room.RoomData.FloorThickness);
            Session.GetHabbo().CurrentRoom.SendPacket(Response2);

            ServerPacket m = new ServerPacket(ServerPacketHeader.RoomChatSettingsComposer);
            Response.WriteInteger(room.RoomData.ChatType);
            Response.WriteInteger(room.RoomData.ChatBalloon);
            Response.WriteInteger(room.RoomData.ChatSpeed);
            Response.WriteInteger(room.RoomData.ChatMaxDistance);
            Response.WriteInteger(room.RoomData.ChatFloodProtection);


            Session.GetHabbo().CurrentRoom.SendPacket(Response);


            Session.SendPacket(new GetGuestRoomResultComposer(Session, room.RoomData, true, false));
        }
    }
}
