using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Games;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class ReloadInventory : IChatCommand
    {
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Room == null)
                return;

            Session.GetHabbo().GetInventoryComponent().LoadUserInventory(Session.GetHabbo().Id);
            Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
            Session.SendWhisper("Tu inventario ha sido actualizado, vuelve abrirlo", 3);
            return;
        }
    }
}