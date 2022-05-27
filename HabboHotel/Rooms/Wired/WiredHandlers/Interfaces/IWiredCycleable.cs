using Akiled.HabboHotel.Items;
namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces
{
    public interface IWiredCycleable
    {
        bool OnCycle(RoomUser User, Item item);

        bool Disposed();

        int Delay { get; set; }
    }
}
