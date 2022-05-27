using Akiled.HabboHotel.Items;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces
{
    public interface IWiredCondition : IWired
    {
        bool AllowsExecution(RoomUser user, Item item);
    }
}
