﻿using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using AkiledEmulator.HabboHotel.Camera;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class CameraPurchaseEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {

            if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(45810, out ItemData ItemData))
            {
                Session.SendNotification("Invalid id 01");
                return;
            }

            if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(45970, out ItemData ItemDataSmall))
            {
                Session.SendNotification("Invalid id 02");
                return;
            }

            JSONCamera jsonInfo = Session.GetHabbo().lastPhotoPreview;

            string roomId = jsonInfo.room_id;
            double timestamp = jsonInfo.timestamp;
            string md5image = jsonInfo.encrypted_id;
            string username = Session.GetHabbo().Username;

            string ExtraData = "{\"w\":\"" + CameraHelper.BASE_URL + "photos/" + md5image + ".png" + "\", \"n\":\"" + username + "\", \"s\":\"" + Session.GetHabbo().Id + "\", \"u\":\"" + "0" + "\", \"t\":\"" + timestamp + "000" + "\"}";

            Item ItemSmall = ItemFactory.CreateSingleItemNullable(ItemDataSmall, Session.GetHabbo(), ExtraData);
            Session.GetHabbo().GetInventoryComponent().TryAddItem(ItemSmall);

            Item Item = ItemFactory.CreateSingleItemNullable(ItemData, Session.GetHabbo(), ExtraData);
            Session.GetHabbo().GetInventoryComponent().TryAddItem(Item);

            Session.SendPacket(new CameraPurchaseSuccesfullComposer());


            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("INSERT INTO user_photos (user_id,photo,time) VALUES ('" + Session.GetHabbo().Id + "', @photoid, '" + timestamp + "');");
                queryreactor.AddParameter("photoid", md5image);
                queryreactor.RunQuery();
            }

            //Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.valide", Session.Langue));
        }
    }
}
