using Akiled.HabboHotel.Items;
using System;

namespace Akiled.HabboHotel.Rooms
{
    public class ItemTriggeredArgs : EventArgs
    {
        public readonly RoomUser TriggeringUser;
        public readonly Item item;

        public ItemTriggeredArgs(RoomUser user, Item item)
        {
            this.TriggeringUser = user;
            this.item = item;
        }
    }
}
