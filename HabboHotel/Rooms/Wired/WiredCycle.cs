using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;

namespace Akiled.HabboHotel.Rooms.Wired
{
    public class WiredCycle
    {
        public RoomUser User;
        public Item Item;
        public IWiredCycleable IWiredCycleable;
        public int Cycle;

        public WiredCycle(IWiredCycleable pIWiredCycleable, RoomUser pUser, Item pItem, int pDelay)
        {
            this.IWiredCycleable = pIWiredCycleable;
            this.User = pUser;
            this.Item = pItem;
            this.Cycle = 0;
        }

        public bool OnCycle()
        {
            this.Cycle++;

            if (this.Cycle <= this.IWiredCycleable.Delay)
                return true;

            this.Cycle = 0;

            if (this.User == null || (this.User != null && this.User.IsDispose))
                this.User = null;

            return this.IWiredCycleable.OnCycle(this.User, this.Item);
        }
    }
}