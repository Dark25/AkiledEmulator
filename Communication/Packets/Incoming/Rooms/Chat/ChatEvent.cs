using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Quests;
using Akiled.HabboHotel.Roleplay.Player;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.Chat.Styles;
using Akiled.HabboHotel.Rooms.Map.Movement;
using Akiled.HabboHotel.Users;
using Akiled.Utilities;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class ChatEvent : IPacketEvent
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

            if (Room.IsRoleplay)
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
                    User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("user.muted", Session.Langue));
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
                Session.GetHabbo().spamProtectionTime = (Room.IsRoleplay) ? 15 : 4;
                Session.GetHabbo().spamEnable = true;

                User.GetClient().SendPacket(new FloodControlComposer(Session.GetHabbo().spamProtectionTime - timeSpan.Seconds));

                return;
            }
            else if (Message.Length > 40 && Message == User.LastMessage && User.LastMessageCount == 1)
            {
                User.LastMessageCount = 0;
                User.LastMessage = "";

                Session.GetHabbo().spamProtectionTime = (Room.IsRoleplay) ? 15 : 4;
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
                    if (Message.StartsWith("@red@") || Message.StartsWith("@rouge@"))
                        User.ChatTextColor = "@red@";
                    if (Message.StartsWith("@cyan@"))
                        User.ChatTextColor = "@cyan@";
                    if (Message.StartsWith("@blue@") || Message.StartsWith("@bleu@"))
                        User.ChatTextColor = "@blue@";
                    if (Message.StartsWith("@green@") || Message.StartsWith("@vert@"))
                        User.ChatTextColor = "@green@";
                    if (Message.StartsWith("@purple@") || Message.StartsWith("@violet@"))
                        User.ChatTextColor = "@purple@";
                    if (Message.StartsWith("@black@") || Message.StartsWith("@noir@"))
                        User.ChatTextColor = "";
                }

                if (Message.StartsWith(":", StringComparison.CurrentCulture) && AkiledEnvironment.GetGame().GetChatManager().GetCommands().Parse(Session, User, Room, Message))
                {
                    Room.GetChatMessageManager().AddMessage(Session.GetHabbo().Id, Session.GetHabbo().Username, Room.Id, string.Format("{0} ha utilizado el comando {1}", Session.GetHabbo().Username, Message));
                    return;
                }

                if (Message.Contains("<3")) //emoji corazon
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(670);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if (Message.Contains("(y)")) //emoji manito
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(671);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if (Message.Contains(":)")) //emoji feliz
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(672);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if (Message.Contains(":')")) //emoji feliz lagrima
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(673);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if ((Message.Contains("O:)")) || (Message.Contains("o:)"))) //emoji angelito
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(674);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if (Message.Contains("3:)")) //emoji diablo
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(675);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if (Message.Contains(";)")) //emoji guiño
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(676);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if ((Message.Contains(":p")) || (Message.Contains(":P"))) //emoji lengua
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(677);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if (Message.Contains(":3")) //emoji :3
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(678);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }


                if ((Message.Contains("love")) || (Message.Contains("(love)")) || (Message.Contains("amor")) || (Message.Contains("AMOR")))  //emoji amor
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(679);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if (Message.Contains("8|")) //emoji lentes
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(656);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if ((Message.Contains(">:p")) || (Message.Contains(">:P"))) //emoji lengua 2
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(657);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if (Message.Contains(":(")) //emoji trsite
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(658);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if ((Message.Contains(":@")) || (Message.Contains(":@"))) //emoji molesto
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(659);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if ((Message.Contains(":'(")) || (Message.Contains(";("))) //emoji llorando
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(660);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if ((Message.Contains(">:(")) || (Message.Contains(">:("))) //emoji TRSITE2
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(661);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if ((Message.Contains(":o")) || (Message.Contains(":O"))) //emoji sorprendido
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(662);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if ((Message.Contains(">:o")) || (Message.Contains(">:O"))) //emoji sorprendido2
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(663);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if ((Message.Contains(">:'(")) || (Message.Contains(">;("))) //emoji llorando2
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(664);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if ((Message.Contains(":l")) || (Message.Contains(":L")) || (Message.Contains(".-.")) || (Message.Contains("._."))) //emoji dead
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(665);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if ((Message.Contains("x_X")) || (Message.Contains("x_x")) || (Message.Contains("X_x")) || (Message.Contains("X_X"))) //emoji dead
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(666);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if (Message.Contains(":$")) //emoji sonrrojado
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(667);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if ((Message.Contains("aus")) || (Message.Contains("AUS")) || (Message.Contains("ausente")) || (Message.Contains("zzz"))) //emoji dormido
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(668);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if ((Message.Contains("mierda")) || (Message.Contains("suck")) || (Message.Contains("MIERDA"))) //emoji mierda
                {
                    Task.Run(async delegate
                    {
                        User.ApplyEffect(669);
                        await Task.Delay(5000);
                        User.ApplyEffect(0);
                    });
                }

                if ((Message.StartsWith("disparar")) && (Room.IsRoleplay) || (Message.StartsWith("pan")) && (Room.IsRoleplay)) //emoji mierda
                {
                    if (Room.IsRoleplay)
                    {
                        RolePlayer Rp = User.Roleplayer;

                        if (Rp != null && Rp.Dead)
                            return;
                        if (Rp.Dead || !Rp.PvpEnable || Rp.SendPrison)
                            return;

                        if (Rp.Munition <= 0)
                        {
                            User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.munitionnotfound", Session.Langue));
                            return;
                        }

                        if (Rp.GunLoad <= 0)
                        {
                            User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.reloadweapon", Session.Langue));
                            return;
                        }

                        MovementDirection movement = MovementManagement.GetMovementByDirection(User.RotBody);

                        int WeaponEanble = Rp.WeaponGun.Enable;

                        User.ApplyEffect(WeaponEanble, true);
                        User.TimerResetEffect = Rp.WeaponGun.FreezeTime;

                        Rp.AggroTimer = 30;

                        if (User.FreezeEndCounter <= Rp.WeaponGun.FreezeTime)
                        {
                            User.Freeze = true;
                            User.FreezeEndCounter = Rp.WeaponGun.FreezeTime;
                        }

                        for (int i = 0; i < Rp.WeaponGun.FreezeTime; i++)
                        {
                            if (Rp.Munition <= 0 || Rp.GunLoad <= 0)
                                break;

                            Rp.Munition--;
                            Rp.GunLoad--;

                            int Dmg = AkiledEnvironment.GetRandomNumber(Rp.WeaponGun.DmgMin, Rp.WeaponGun.DmgMax);

                            if (Rp.WeaponGun.Id == 21)
                            {
                                Room.GetProjectileManager().AddGrenade(User.VirtualId, User.SetX, User.SetY, User.SetZ, movement, Dmg, Rp.WeaponGun.Distance);
                            }
                            else
                                Room.GetProjectileManager().AddProjectile(User.VirtualId, User.SetX, User.SetY, User.SetZ, movement, Dmg, Rp.WeaponGun.Distance);

                        }


                        if (Rp.WeaponGun.Id == 5)
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("revolver", 2)); //Type = Trax
                        }
                        if (Rp.WeaponGun.Id == 6)
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("ak47", 2)); //Type = Trax
                        }
                        if (Rp.WeaponGun.Id == 7)
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("sniper", 2)); //Type = Trax
                        }
                        if (Rp.WeaponGun.Id == 8)
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("glock", 2)); //Type = Trax
                        }
                        if (Rp.WeaponGun.Id == 9)
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("shotgun", 2)); //Type = Trax
                        }
                        if (Rp.WeaponGun.Id == 10)
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("mp5", 2)); //Type = Trax
                        }
                        if (Rp.WeaponGun.Id == 11)//pistoladoble
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("pistoladoble", 2)); //Type = Trax
                        }
                        if (Rp.WeaponGun.Id == 12)//pistoladeoro
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("pistoladeoro", 2)); //Type = Trax
                        }
                        if (Rp.WeaponGun.Id == 13)//pistolasilenciador
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("pistolasilenciadora", 2)); //Type = Trax
                        }
                        if (Rp.WeaponGun.Id == 14)//OTRAPISTOLA
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("otrapistola", 2)); //Type = Trax
                        }
                        if (Rp.WeaponGun.Id == 15)//OTRAPISTOLA
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("otrapistola1", 2)); //Type = Trax
                        }
                        if (Rp.WeaponGun.Id == 16)//OTRAPISTOLA
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("otrapistola2", 2)); //Type = Trax
                        }
                        if (Rp.WeaponGun.Id == 17)//OTRAPISTOLA
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("otrapistola3", 2)); //Type = Trax
                        }
                        if (Rp.WeaponGun.Id == 18)//OTRAPISTOLA
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("otrapistola4", 2)); //Type = Trax
                        }
                        if (Rp.WeaponGun.Id == 19)//OTRAPISTOLA
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("otrapistola5", 2)); //Type = Trax
                        }
                        if (Rp.WeaponGun.Id == 20)//OTRAPISTOLA
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("otrapistola6", 2)); //Type = Trax
                        }
                        if (Rp.WeaponGun.Id == 21)//GRANADA
                        {
                            Room.SendPacketWeb(new PlaySoundComposer("granada", 2)); //Type = Trax
                        }

                        Rp.SendUpdate();

                    }
                }



                if (Message.Contains("@"))
                {
                    Regex linkParser = new Regex("(?:[@])\\S+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    string rawString = Message;
                    {
                        IEnumerator enumerator = linkParser.Matches(rawString).GetEnumerator();
                        try
                        {
                            if (enumerator.MoveNext())
                            {
                                Match i = (Match)enumerator.Current;
                                if (i.Value.Length >= 3)
                                {
                                    Habbo Client = AkiledEnvironment.GetHabboByUsername(i.Value.Substring(1).ToString());
                                    if (Client != null)
                                    {
                                        if (Session.GetHabbo().LastMarkedFriend + 100 > AkiledEnvironment.GetIUnixTimestamp() && !Session.GetHabbo().HasFuse("override_limit_command"))
                                        {
                                            int min = (Session.GetHabbo().LastMarkedFriend + 100 - AkiledEnvironment.GetIUnixTimestamp()) / 10;
                                            Session.SendWhisper("Debes esperar 1 mínuto para poder #mencionar nuevamente!", 1);
                                            return;
                                        }
                                        else if (Client.Id == Session.GetHabbo().Id)
                                        {
                                            Session.SendWhisper("Que te pasa chaval, no tienes amig@s!", 1);
                                            return;
                                        }
                                        else if (Message.Contains("<img src="))
                                        {
                                            Session.SendWhisper("hijo de tu puta madre", 1);
                                            return;
                                        }
                                        else if (Client.HasFuse("override_hashtag_command"))
                                        {
                                            Session.SendWhisper("No se puede mencionar a este usuario@.", 1);
                                            return;
                                        }
                                        else if (Client.GetCliente() != null)
                                        {
                                            if (!Session.GetHabbo().GetMessenger().FriendshipExists(Client.Id))
                                            {
                                                Session.SendWhisper("No puedes #mencionar a este que usuari@, porque no es tu amig@!", 1);
                                                return;
                                            }
                                            else
                                            {
                                                Session.GetHabbo().LastMarkedFriend = AkiledEnvironment.GetIUnixTimestamp();
                                                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                                {
                                                    dbClient.SetQuery("UPDATE users SET last_marked_friend = @timestamp WHERE id = '" + Session.GetHabbo().Id + "' LIMIT 1");
                                                    dbClient.AddParameter("timestamp", AkiledEnvironment.GetIUnixTimestamp());
                                                    dbClient.RunQuery();
                                                }
                                                Client.GetCliente().SendMessage(RoomNotificationComposer.SendBubble("hashtag", Session.GetHabbo().Username + " te ha #mencionado en la sala: \"" + Session.GetHabbo().CurrentRoom.RoomData.Name + "\".\r\r con el mensaje: " + Message + ". \r\rHaz click en este mensaje, para ir a la sala.", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
                                                Client.GetCliente().SendWhisper(Session.GetHabbo().Username + " te #menciono: " + Message, 33);
                                                //AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifTopComposer(Message), Session.Langue);
                                                //AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("hashtagstaff", "El usuario: " + Session.GetHabbo().Username + "  esta mencionando al usuari@ : " + Client.GetCliente().GetHabbo().Username + ", en la sala llamada" + Session.GetHabbo().CurrentRoom.RoomData.Name + "."));
                                                Message = "@blue@" + Message;
                                            }
                                        }
                                        else
                                        {
                                            Session.SendWhisper("El usuario se encuentra desconectad@, por lo tanto no puedes #mencionarlo!", 1);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        finally
                        {
                            IDisposable disposable = enumerator as IDisposable;
                            if (disposable != null)
                            {
                                disposable.Dispose();
                            }
                        }
                    }
                }

                if (Session != null && !User.IsBot)
                {
                    if (Session.Antipub(Message, "<TCHAT>", Room.Id))
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
                User.SendWhisperChat(Message, false);
                return;
            }

            Room.OnUserSay(User, Message, false);

            if (User.IsSpectator)
                return;

            if (User.muted)
            {
                User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("user.muted", Session.Langue));
                return;
            }

            if (!string.IsNullOrEmpty(User.ChatTextColor))
                Message = User.ChatTextColor + " " + Message;

            User.OnChat(Message, Colour, false);
        }
    }
}
