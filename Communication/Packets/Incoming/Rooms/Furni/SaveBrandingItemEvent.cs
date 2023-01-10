using Akiled.HabboHotel.GameClients;using Akiled.HabboHotel.Items;using Akiled.HabboHotel.Rooms;using System;namespace Akiled.Communication.Packets.Incoming.Structure{    class SaveBrandingItemEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {            int ItemId = Packet.PopInt();            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);            if (room == null || !room.CheckRights(Session))                return;            Item roomItem = room.GetRoomItemHandler().GetItem(ItemId);            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.adsbackground)                return;            int Data = Packet.PopInt();            string text = Packet.PopString();            string text2 = Packet.PopString();            string text3 = Packet.PopString();            string text4 = Packet.PopString();            string text5 = Packet.PopString();            string text6 = Packet.PopString();            string text7 = Packet.PopString();            string text8 = Packet.PopString();            string BrandData = "";            if (Data != 10 && Data != 8)                return;            BrandData = string.Concat(new object[]
                    {
                        text.Replace("=", ""),
                        "=",
                        text2.Replace("=", ""),
                        Convert.ToChar(9),
                        text3.Replace("=", ""),
                        "=",
                        text4.Replace("=", ""),
                        Convert.ToChar(9),
                        text5.Replace("=", ""),
                        "=",
                        text6.Replace("=", ""),
                        Convert.ToChar(9),
                        text7.Replace("=", ""),
                        "=",
                        text8.Replace("=", "")
                    });            roomItem.ExtraData = BrandData;            roomItem.UpdateState();        }    }}