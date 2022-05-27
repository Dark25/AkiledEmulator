using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces
{
    public interface IWired
    {
        void Dispose();

        void SaveToDatabase(IQueryAdapter dbClient);

        void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom);

        void DeleteFromDatabase(IQueryAdapter dbClient);

        void OnTrigger(GameClient Session, int SpriteId);
    }
}
