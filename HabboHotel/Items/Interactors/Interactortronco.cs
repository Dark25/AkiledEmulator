
using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Quests;
using Akiled.HabboHotel.Roleplay.Player;
using Akiled.HabboHotel.Rooms;
using System;
using System.Threading.Tasks;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class Interactortronco : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
            if (Item.InteractingUser <= 0)
                return;
            Item.InteractingUser = 0;
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
            if (Item.InteractingUser <= 0)
                return;
            Item.InteractingUser = 0;
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (Session != null)
                AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_SWITCH);
            string room_idfish = AkiledEnvironment.GetConfig().data["room_idfish"];
            string str1 = AkiledEnvironment.GetConfig().data["baseitem_idfish"];
            string str2 = AkiledEnvironment.GetConfig().data["baseitem_idgusano"];
            string str3 = AkiledEnvironment.GetConfig().data["item_idfish"];
            string str4 = AkiledEnvironment.GetConfig().data["item_idgusano"];
            string str5 = AkiledEnvironment.GetConfig().data["item_idpala"];
            if (Session.GetHabbo().CurrentRoomId != Convert.ToInt32(room_idfish) || Item.GetBaseItem().Id != Convert.ToInt32(str2))
                return;
            if (Session.GetHabbo().is_angeln)
            {
                Session.SendWhisper("No puedes cavar mientras estás pescando!", 1);
            }
            else
            {
                if (!Session.GetHabbo().hasschaufel)
                {
                    using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        queryReactor.SetQuery("SELECT * FROM `user_rpitems` WHERE `user_id` = " + Session.GetHabbo().Id.ToString() + " AND `item_id` = '" + Convert.ToInt32(str5).ToString() + "' LIMIT 1;");
                        if (queryReactor.GetRow() != null)
                            Session.GetHabbo().hasschaufel = true;
                    }
                }
                if (!Session.GetHabbo().hasschaufel)
                {
                    Session.SendWhisper("Necesitas una pala para cavar gusanos.", 1);
                }
                else
                {
                    RoomUser ThisUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                    if (Math.Abs(Item.GetX - ThisUser.X) >= 2 || Math.Abs(Item.GetY - ThisUser.Y) >= 2)
                        Session.SendWhisper("Tienes que estar más cerca.", 1);
                    else
                        Task.Run((Func<Task>)(async () =>
                        {
                            try
                            {
                                if (Session.GetHabbo().onfarm)
                                {
                                    Session.GetHabbo().onfarm = false;
                                    Session.SendWhisper("Dejaste de cavar.", 1);
                                    ThisUser.ApplyEffect(0);
                                }
                                else
                                {
                                    Session.SendWhisper("Empezaste a cavar. Por favor detente aquí por un momento.", 1);
                                    ThisUser.ApplyEffect(546);
                                    Session.GetHabbo().SendPacketWeb((IServerPacket)new PlaySoundComposer("cavar", 2));
                                    Session.GetHabbo().onfarm = true;
                                    int x = 0;
                                    int time = AkiledEnvironment.GetIUnixTimestamp();
                                    while (x <= 30)
                                    {
                                        if (!Session.GetHabbo().onfarm)
                                            return;
                                        if (Math.Abs(Item.GetX - ThisUser.X) >= 2 || Math.Abs(Item.GetY - ThisUser.Y) >= 2)
                                        {
                                            Session.SendWhisper("Dejaste de cavar.", 1);
                                            ThisUser.ApplyEffect(0);
                                            Session.GetHabbo().onfarm = false;
                                            return;
                                        }
                                        if (Session.GetHabbo().CurrentRoomId != Convert.ToInt32(room_idfish))
                                        {
                                            Session.GetHabbo().onfarm = false;
                                            return;
                                        }
                                        if (time < AkiledEnvironment.GetIUnixTimestamp())
                                        {
                                            time = AkiledEnvironment.GetIUnixTimestamp();
                                            ++x;
                                        }
                                    }
                                    await Task.Delay(500);
                                    if (Session.GetHabbo().onfarm)
                                    {
                                        Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
                                        Session.GetHabbo().onfarm = false;
                                        Random rand = new Random();
                                        int wurm = rand.Next(1, 5);
                                        Session.SendWhisper("Has tenido éxito " + wurm.ToString() + " Gusanos desenterrados!", 1);
                                        room.SendPacket((IServerPacket)new ChatComposer(ThisUser.VirtualId, "@blue@*" + ThisUser.GetClient().GetHabbo().Username + " Ha desenterrado " + wurm.ToString() + " Gusanos*", 0, ThisUser.LastBubble));
                                        ThisUser.ApplyEffect(0);
                                        RolePlayer Rp = ThisUser.Roleplayer;
                                        Rp.AddInventoryItem(167, wurm);
                                        room = (Room)null;
                                        rand = (Random)null;
                                        Rp = (RolePlayer)null;
                                        room = (Room)null;
                                        rand = (Random)null;
                                        Rp = (RolePlayer)null;
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }));
                }
            }
        }
    }
}
