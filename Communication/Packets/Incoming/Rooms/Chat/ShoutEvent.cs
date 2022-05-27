using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Quests;
using Akiled.HabboHotel.Roleplay.Player;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.Chat.Styles;
using Akiled.Utilities;
using System;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class ShoutEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (User == null)
                return;

            if(Room.IsRoleplay)
            {
                    RolePlayer Rp = User.Roleplayer;
                    if (Rp != null && Rp.Dead)
                        return;
            }

            string Message = StringCharFilter.Escape(Packet.PopString());
            if (Message.Length > 100)
                Message = Message.Substring(0, 100);

            int Colour = Packet.PopInt();

            ChatStyle Style = null;
            if (!AkiledEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Colour, out Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().HasFuse(Style.RequiredRight)))
                Colour = 0;

            User.Unidle();

            if (Session.GetHabbo().Rank < 5U && Room.RoomMuted && !User.IsOwner() && !Session.GetHabbo().CurrentRoom.CheckRights(Session))
            {
                User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("room.muted", Session.Langue));
                return;
            }

            if (Room.GetJanken().PlayerStarted(User))
            {
                if (!Room.GetJanken().PickChoix(User, Message))
                    User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("janken.choice", Session.Langue));

                return;
            }

            if (Session.GetHabbo().TimeMuted > 0.0)
            {
                Session.SendMessage(new MutedComposer(Session.GetHabbo().TimeMuted));
                Session.SendNotification("Oops, estas muteado - no puedes enviar mensajes.");
                return;
            }

            if (Room.UserIsMuted(Session.GetHabbo().Id))
            {
                if (!Room.HasMuteExpired(Session.GetHabbo().Id))
                {
                    User.GetClient().SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("user.muted", Session.Langue));
                    return;
                }
                else
                    Room.RemoveMute(Session.GetHabbo().Id);
            }

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
            else if (timeSpan.TotalSeconds < 4.0 && User.FloodCount > 15 && !Session.GetHabbo().HasFuse("fuse_mod"))
            {
                Session.GetHabbo().spamProtectionTime = 30;
                Session.GetHabbo().spamEnable = true;
                
                User.GetClient().SendPacket(new FloodControlComposer(Session.GetHabbo().spamProtectionTime - timeSpan.Seconds));

                return;
            }
            else if (Message.Length > 40 && Message == User.LastMessage && User.LastMessageCount == 1)
            {
                User.LastMessageCount = 0;
                User.LastMessage = "";
                
                Session.GetHabbo().spamProtectionTime = 10;
                Session.GetHabbo().spamEnable = true;
                User.GetClient().SendPacket(new FloodControlComposer(Session.GetHabbo().spamProtectionTime - timeSpan.Seconds));
                return;
            }
            else
            {
                if (Message == User.LastMessage && Message.Length > 40)
                    User.LastMessageCount++;

                User.LastMessage = Message;

                Session.GetHabbo().spamFloodTime = DateTime.Now;
                User.FloodCount++;

                if (Session != null)
                {
                    if (Message.StartsWith("@red@"))
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
                }

                if (Message.StartsWith(":", StringComparison.CurrentCulture) && AkiledEnvironment.GetGame().GetChatManager().GetCommands().Parse(Session, User, Room, Message))
                {
                    Room.GetChatMessageManager().AddMessage(Session.GetHabbo().Id, Session.GetHabbo().Username, Room.Id, string.Format("{0} a utiliser la commande {1}", Session.GetHabbo().Username, Message));
                    return;
                }

                if (Session != null && !User.IsBot)
                {
                    if (Session.Antipub(Message, "<TCHAT>"))
                    {
                        AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("publicidad", "El usuario: " + Session.GetHabbo().Username + ", palabra:" + Message + ", pulsa aquí para ir a mirar.", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
                        return;
                    }

                    AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_CHAT, 0);
                    Session.GetHabbo().GetChatMessageManager().AddMessage(Session.GetHabbo().Id, Session.GetHabbo().Username, Room.Id, Message);
                    Room.GetChatMessageManager().AddMessage(Session.GetHabbo().Id, Session.GetHabbo().Username, Room.Id, Message);

                    if (User.transfbot)
                        Colour = 2;
                }
            }

            if (!Session.GetHabbo().HasFuse("word_filter_override"))
                Message = AkiledEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Message);

            if (Room.AllowsShous(User, Message))
            {
                User.SendWhisperChat(Message, true);
                return;
            }
            Room.OnUserSay(User, Message, true);

            if (User.IsSpectator)
                return;

            if (!string.IsNullOrEmpty(User.ChatTextColor))
                Message = User.ChatTextColor + " " + Message;

            User.OnChat(Message, Colour, true);

        }
    }
}
