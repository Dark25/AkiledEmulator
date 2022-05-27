using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Roleplay;
using Akiled.HabboHotel.Roleplay.Player;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.WebClients;
using System;

namespace Akiled.Communication.Packets.Incoming.WebSocket
{
    class RpUseItemsEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            int UseCount = Packet.PopInt();

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
                return;

            Room Room = Client.GetHabbo().CurrentRoom;
            if (Room == null || !Room.IsRoleplay)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Client.GetHabbo().Id);
            if (User == null)
                return;

            if (User.Freeze)
                return;

            RolePlayer Rp = User.Roleplayer;
            if (Rp == null || Rp.Dead || Rp.SendPrison || Rp.TradeId > 0)
                return;

            if (Rp.AggroTimer > 0)
            {
                User.SendWhisperChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.useitem.notallowed", Client.Langue), Math.Round((double)Rp.AggroTimer / 2)));
                return;
            }

            RPItem RpItem = AkiledEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(ItemId);
            if (RpItem == null)
                return;

            RolePlayInventoryItem RpItemInventory = Rp.GetInventoryItem(ItemId);
            if (RpItemInventory == null || RpItemInventory.Count <= 0 || RpItem.Type == "none")
                return;

            if (UseCount <= 0 || RpItem.UseType != 2)
                UseCount = 1;

            if (UseCount > RpItemInventory.Count)
                UseCount = RpItemInventory.Count;


            if (User.FreezeEndCounter <= 1)
            {
                User.Freeze = true;
                User.FreezeEndCounter = 1;
            }

            if (RpItem.Id == 75)
                Rp.AddInventoryItem(45, UseCount);

            switch (RpItem.Type)
            {
                case "openpage":
                    {
                        User.GetClient().SendPacket(new NuxAlertComposer("habbopages/roleplay/" + RpItem.Value));
                        break;
                    }
                case "openguide":
                    {
                        User.GetClient().SendPacket(new NuxAlertComposer("habbopages/westworld/westworld"));
                        break;
                    }
                case "hit":
                    {
                        Rp.Hit(User, RpItem.Value * UseCount, Room, false, true, false);
                        Rp.RemoveInventoryItem(RpItem.Id, UseCount);
                        break;
                    }
                case "enable":
                    {
                        User.ApplyEffect(RpItem.Value);
                        break;
                    }
                case "showtime":
                    {
                        User.SendWhisperChat("Son las " + Room.RpHour + " horas y " + Room.RpMinute + " minutos");
                        break;
                    }
                case "money":
                    {
                        Rp.Money += RpItem.Value * UseCount;
                        Rp.RemoveInventoryItem(RpItem.Id, UseCount);
                        Rp.SendUpdate();
                        break;
                    }
                case "munition":
                    {
                        Rp.AddMunition(RpItem.Value * UseCount);
                        Rp.RemoveInventoryItem(RpItem.Id, UseCount);
                        Rp.SendUpdate();
                        break;
                    }
                case "energytired":
                    {
                        User.ApplyEffect(4, true);
                        User.TimerResetEffect = 2;

                        Rp.AddEnergy(RpItem.Value * UseCount);
                        Rp.Hit(User, RpItem.Value * UseCount, Room, false, true, false);
                        Rp.SendUpdate();
                        Rp.RemoveInventoryItem(RpItem.Id, UseCount);

                        User.OnChat("*Consumir " + Char.ToLowerInvariant(RpItem.Title[0]) + RpItem.Title.Substring(1) + "*");
                        break;
                    }
                case "healthtired":
                    {
                        User.ApplyEffect(4, true);
                        User.TimerResetEffect = 2;

                        Rp.RemoveEnergy(RpItem.Value * UseCount);
                        Rp.AddHealth(RpItem.Value * UseCount);
                        Rp.SendUpdate();
                        Rp.RemoveInventoryItem(RpItem.Id, UseCount);

                        User.OnChat("*Consumir " + Char.ToLowerInvariant(RpItem.Title[0]) + RpItem.Title.Substring(1) + "*");
                        break;
                    }
                case "healthenergy":
                    {
                        User.ApplyEffect(4, true);
                        User.TimerResetEffect = 2;

                        Rp.AddEnergy(RpItem.Value * UseCount);
                        Rp.AddHealth(RpItem.Value * UseCount);
                        Rp.SendUpdate();
                        Rp.RemoveInventoryItem(RpItem.Id, UseCount);

                        User.OnChat("*Consumir " + Char.ToLowerInvariant(RpItem.Title[0]) + RpItem.Title.Substring(1) + "*");
                        break;
                    }
                case "energy":
                    {
                        User.ApplyEffect(4, true);
                        User.TimerResetEffect = 2;

                        Rp.AddEnergy(RpItem.Value * UseCount);
                        Rp.SendUpdate();
                        Rp.RemoveInventoryItem(RpItem.Id, UseCount);

                        User.OnChat("*Consumir " + Char.ToLowerInvariant(RpItem.Title[0]) + RpItem.Title.Substring(1) + "*");
                        break;
                    }
                case "health":
                    {
                        User.ApplyEffect(737, true);
                        User.TimerResetEffect = 4;

                        Rp.AddHealth(RpItem.Value * UseCount);
                        Rp.SendUpdate();
                        Rp.RemoveInventoryItem(RpItem.Id, UseCount);

                        User.OnChat("*Consumir " + Char.ToLowerInvariant(RpItem.Title[0]) + RpItem.Title.Substring(1) + "*");
                        break;
                    }
                case "weapon_cac":
                    {
                        if (Rp.WeaponCac.Id == RpItem.Value)
                            break;
                        Rp.WeaponCac = AkiledEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponCac(RpItem.Value);
                        User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.changearmecac", Client.Langue));
                        break;
                    }
                case "weapon_far":
                    {
                        if (Rp.WeaponGun.Id == RpItem.Value)
                            break;
                        Rp.WeaponGun = AkiledEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponGun(RpItem.Value);
                        User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.changearmefar", Client.Langue));
                        break;
                    }
            }
        }
    }
}
