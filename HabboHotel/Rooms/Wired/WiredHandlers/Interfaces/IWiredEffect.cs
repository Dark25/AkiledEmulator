using Akiled.HabboHotel.Items;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces
{
    public interface IWiredEffect
    {
        void Handle(RoomUser user, Item item);
    }
}
