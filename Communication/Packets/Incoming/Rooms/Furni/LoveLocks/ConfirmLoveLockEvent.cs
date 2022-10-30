using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using System;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class ConfirmLoveLockEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int pId = Packet.PopInt();
            bool isConfirmed = Packet.PopBoolean();

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null || !Room.CheckRights(Session))
                return;

            Item Item = Room.GetRoomItemHandler().GetItem(pId);

            if (Item == null || Item.GetBaseItem() == null || Item.GetBaseItem().InteractionType != InteractionType.LOVELOCK)
                return;

            int UserOneId = Item.InteractingUser;
            int UserTwoId = Item.InteractingUser2;

            RoomUser UserOne = Room.GetRoomUserManager().GetRoomUserByHabboId(UserOneId);
            RoomUser UserTwo = Room.GetRoomUserManager().GetRoomUserByHabboId(UserTwoId);

            if (UserOne == null && UserTwo == null)
            {
                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.1", Session.Langue));
                return;
            }
            else if (UserOne.GetClient() == null || UserTwo.GetClient() == null)
            {
                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.1", Session.Langue));
                return;
            }
            else if (UserOne == null)
            {
                UserTwo.GetClient().SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.1", UserTwo.GetClient().Langue));
                UserTwo.LLPartner = 0;
                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;
                return;
            }
            else if (UserTwo == null)
            {
                UserOne.GetClient().SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.1", UserOne.GetClient().Langue));
                UserOne.LLPartner = 0;
                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;
                return;
            }
            else if (Item.ExtraData.Contains(Convert.ToChar(5).ToString()))
            {
                UserTwo.GetClient().SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.2", UserTwo.GetClient().Langue));
                UserTwo.LLPartner = 0;

                UserOne.GetClient().SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.lovelock.error.2", UserOne.GetClient().Langue));
                UserOne.LLPartner = 0;

                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;
                return;
            }
            else if (!isConfirmed)
            {
                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;

                UserOne.LLPartner = 0;
                UserTwo.LLPartner = 0;
                return;
            }
            else
            {
                if (UserOneId == Session.GetHabbo().Id)
                {
                    Session.SendPacket(new LoveLockDialogueSetLockedMessageComposer(pId));
                    UserOne.LLPartner = UserTwoId;
                }
                else if (UserTwoId == Session.GetHabbo().Id)
                {
                    Session.SendPacket(new LoveLockDialogueSetLockedMessageComposer(pId));
                    UserTwo.LLPartner = UserOneId;
                }

                if (UserOne.LLPartner == 0 || UserTwo.LLPartner == 0)
                    return;
                else
                {
                    Item.ExtraData = "1" + (char)5 + UserOne.GetUsername() + (char)5 + UserTwo.GetUsername() + (char)5 + UserOne.GetClient().GetHabbo().Look + (char)5 + UserTwo.GetClient().GetHabbo().Look + (char)5 + DateTime.Now.ToString("dd/MM/yyyy");

                    Item.InteractingUser = 0;
                    Item.InteractingUser2 = 0;

                    UserOne.LLPartner = 0;
                    UserTwo.LLPartner = 0;

                    Item.UpdateState(true, true);

                    UserOne.GetClient().SendPacket(new LoveLockDialogueCloseMessageComposer(pId));
                    UserTwo.GetClient().SendPacket(new LoveLockDialogueCloseMessageComposer(pId));

                    using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `items` SET `extra_data` = @extraData WHERE `id` = @ID LIMIT 1");
                        dbClient.AddParameter("extraData", Item.ExtraData);
                        dbClient.AddParameter("ID", Item.Id);
                        dbClient.RunQuery();
                    }

                    UserOne = null;
                    UserTwo = null;
                }
            }
        }
    }
}
