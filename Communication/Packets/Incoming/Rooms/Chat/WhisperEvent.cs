using Akiled.Communication.Packets.Outgoing;using Akiled.Communication.Packets.Outgoing.Structure;using Akiled.HabboHotel.GameClients;using Akiled.HabboHotel.Rooms;using Akiled.HabboHotel.Rooms.Chat.Styles;using Akiled.Utilities;using System;using System.Collections.Generic;namespace Akiled.Communication.Packets.Incoming.Structure{    class WhisperEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {            if (Session == null || Session.GetHabbo() == null)                return;            Room Room = Session.GetHabbo().CurrentRoom;            if (Room == null)                return;

            RoomUser Userroom = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);

            if (Room.UserIsMuted(Session.GetHabbo().Id))
            {
                if (!Room.HasMuteExpired(Session.GetHabbo().Id))
                {
                    Userroom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("user.muted", Session.Langue));
                    return;
                }
                else
                    Room.RemoveMute(Session.GetHabbo().Id);
            }

            if (Session.GetHabbo().Rank < 5U && Room.RoomMuted && !Userroom.IsOwner() && !Session.GetHabbo().CurrentRoom.CheckRights(Session))
            {
                Userroom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("room.muted", Session.Langue));
                return;
            }            string Params = StringCharFilter.Escape(Packet.PopString());            if (string.IsNullOrEmpty(Params) || Params.Length > 100 || !Params.Contains(" "))                return;            string ToUser = Params.Split(new char[1] { ' ' })[0];            if (ToUser.Length + 1 > Params.Length)                return;            string Message = Params.Substring(ToUser.Length + 1);            int Color = Packet.PopInt();

            ChatStyle Style = null;            if (!AkiledEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Color, out Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().HasFuse(Style.RequiredRight)))                Color = 0;

            if (Session.Antipub(Message, "<MP>"))
                return;            if (!Session.GetHabbo().HasFuse("word_filter_override"))                Message = AkiledEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Message);            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);

            if (User == null) return;            if (User.IsSpectator) return;

            TimeSpan timeSpan = DateTime.Now - Session.GetHabbo().spamFloodTime;
            if (timeSpan.TotalSeconds > (double)Session.GetHabbo().spamProtectionTime && Session.GetHabbo().spamEnable)
            {
                User.FloodCount = 0;
                Session.GetHabbo().spamEnable = false;
            }
            else if (timeSpan.TotalSeconds > 4.0)
                User.FloodCount = 0;

            if (timeSpan.TotalSeconds < (double)Session.GetHabbo().spamProtectionTime && Session.GetHabbo().spamEnable)
            {
                int i = Session.GetHabbo().spamProtectionTime - timeSpan.Seconds;
                User.GetClient().SendPacket(new FloodControlComposer(i));
                return;
            }
            else if (timeSpan.TotalSeconds < 4.0 && User.FloodCount > 5 && !Session.GetHabbo().HasFuse("fuse_mod"))
            {
                Session.GetHabbo().spamProtectionTime = (Room.IsRoleplay) ? 5 : 30;
                Session.GetHabbo().spamEnable = true;

                User.GetClient().SendPacket(new FloodControlComposer(Session.GetHabbo().spamProtectionTime - timeSpan.Seconds));

                return;
            }
            else if (Message.Length > 40 && Message == User.LastMessage && User.LastMessageCount == 1)
            {
                User.LastMessageCount = 0;
                User.LastMessage = "";

                Session.GetHabbo().spamProtectionTime = (Room.IsRoleplay) ? 5 : 30;
                Session.GetHabbo().spamEnable = true;
                User.GetClient().SendPacket(new FloodControlComposer(Session.GetHabbo().spamProtectionTime - timeSpan.Seconds));
                return;
            }
            else
            {
                if (Message == User.LastMessage && Message.Length > 40)
                    User.LastMessageCount++;                User.LastMessage = Message;                Session.GetHabbo().spamFloodTime = DateTime.Now;                User.FloodCount++;                if (Message.StartsWith("@red@"))
                    User.ChatTextColor = "@red@";
                if (Message.StartsWith("@cyan@"))
                    User.ChatTextColor = "@cyan@";
                if (Message.StartsWith("@blue@"))
                    User.ChatTextColor = "@blue@";
                if (Message.StartsWith("@green@"))
                    User.ChatTextColor = "@green@";
                if (Message.StartsWith("@purple@"))
                    User.ChatTextColor = "@purple@";
                if (Message.StartsWith("@black@"))
                    User.ChatTextColor = "";

                if (!string.IsNullOrEmpty(User.ChatTextColor))
                    Message = User.ChatTextColor + " " + Message;

                User.Unidle();

                if (ToUser == "Grupo")
                {
                    if (User.WhiperGroupUsers.Count <= 0)
                        return;

                    string GroupUsername = String.Join(", ", User.WhiperGroupUsers);

                    Message = "(" + GroupUsername + ") " + Message;

                    ServerPacket Message1 = new ServerPacket(ServerPacketHeader.WhisperMessageComposer);
                    Message1.WriteInteger(User.VirtualId);
                    Message1.WriteString(Message);
                    Message1.WriteInteger(RoomUser.GetSpeechEmotion(Message));
                    Message1.WriteInteger(Color);
                    Message1.WriteInteger(0);
                    Message1.WriteInteger(-1);
                    User.GetClient().SendPacket(Message1);

                    if (Session.GetHabbo().IgnoreAll)
                        return;

                    foreach (string Username in User.WhiperGroupUsers.ToArray())
                    {
                        RoomUser UserWhiper = Room.GetRoomUserManager().GetRoomUserByHabbo(Username);

                        if (UserWhiper == null || UserWhiper.GetClient() == null || UserWhiper.GetClient().GetHabbo() == null)
                        {
                            User.WhiperGroupUsers.Remove(Username);
                            continue;
                        }

                        if (UserWhiper.IsSpectator || UserWhiper.IsBot || UserWhiper.UserId == User.UserId || UserWhiper.GetClient().GetHabbo().MutedUsers.Contains(Session.GetHabbo().Id))
                        {
                            User.WhiperGroupUsers.Remove(Username);
                            continue;
                        }

                        UserWhiper.GetClient().SendPacket(Message1);
                    }

                    List<RoomUser> roomUserByRank = Room.GetRoomUserManager().GetStaffRoomUser();
                    if (roomUserByRank.Count <= 0)
                        return;

                    ServerPacket Message2 = new ServerPacket(ServerPacketHeader.WhisperMessageComposer);
                    Message2.WriteInteger(User.VirtualId);
                    Message2.WriteString(AkiledEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message);
                    Message2.WriteInteger(RoomUser.GetSpeechEmotion(Message));
                    Message2.WriteInteger(Color);
                    Message2.WriteInteger(0);
                    Message2.WriteInteger(-1);
                    foreach (RoomUser roomUser in roomUserByRank)
                    {
                        if (roomUser != null && roomUser.HabboId != User.HabboId && roomUser.GetClient() != null && roomUser.GetClient().GetHabbo().ViewMurmur && !User.WhiperGroupUsers.Contains(roomUser.GetUsername()))
                            roomUser.GetClient().SendPacket(Message2);
                    }
                }
                else
                {
                    ServerPacket Message1 = new ServerPacket(ServerPacketHeader.WhisperMessageComposer);
                    Message1.WriteInteger(User.VirtualId);
                    Message1.WriteString(Message);
                    Message1.WriteInteger(RoomUser.GetSpeechEmotion(Message));
                    Message1.WriteInteger(Color);
                    Message1.WriteInteger(0);
                    Message1.WriteInteger(-1);
                    User.GetClient().SendPacket(Message1);

                    if (Session.GetHabbo().IgnoreAll)
                        return;

                    RoomUser UserWhiper = Room.GetRoomUserManager().GetRoomUserByName(ToUser);

                    if (UserWhiper == null || UserWhiper.GetClient() == null || UserWhiper.GetClient().GetHabbo() == null)
                    {
                        return;
                    }

                    if (UserWhiper.IsSpectator || UserWhiper.IsBot || UserWhiper.UserId == User.UserId || UserWhiper.GetClient().GetHabbo().MutedUsers.Contains(Session.GetHabbo().Id))
                    {
                        return;
                    }

                    UserWhiper.GetClient().SendPacket(Message1);

                    List<RoomUser> roomUserByRank = Room.GetRoomUserManager().GetStaffRoomUser();
                    if (roomUserByRank.Count <= 0)
                        return;

                    ServerPacket Message2 = new ServerPacket(ServerPacketHeader.WhisperMessageComposer);
                    Message2.WriteInteger(User.VirtualId);
                    Message2.WriteString(AkiledEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message);
                    Message2.WriteInteger(RoomUser.GetSpeechEmotion(Message));
                    Message2.WriteInteger(Color);
                    Message2.WriteInteger(0);
                    Message2.WriteInteger(-1);
                    foreach (RoomUser roomUser in roomUserByRank)
                    {
                        if (roomUser != null && roomUser.HabboId != User.HabboId && roomUser.GetClient() != null && roomUser.GetClient().GetHabbo().ViewMurmur && UserWhiper.UserId != roomUser.UserId)
                            roomUser.GetClient().SendPacket(Message2);
                    }
                }

                Session.GetHabbo().GetChatMessageManager().AddMessage(User.UserId, User.GetUsername(), User.RoomId, AkiledEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message);
                Room.GetChatMessageManager().AddMessage(User.UserId, User.GetUsername(), User.RoomId, AkiledEnvironment.GetLanguageManager().TryGetValue("moderation.whisper", Session.Langue) + ToUser + ": " + Message);            }        }    }}