using Akiled.HabboHotel.GameClients;
using Akiled.Utilities.DependencyInjection;
namespace Akiled.HabboHotel.Rooms.Chat.Commands
{
    [Transient]
    public interface IChatCommand
    {
        void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params);
    }
}
