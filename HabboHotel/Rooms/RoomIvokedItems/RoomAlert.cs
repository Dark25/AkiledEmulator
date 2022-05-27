namespace Akiled.HabboHotel.Rooms.RoomIvokedItems
{
    public class RoomAlert
    {
        public string Message;
        public int MinRank;

        public RoomAlert(string message, int minrank)
        {
            this.Message = message;
            this.MinRank = minrank;
        }
    }
}