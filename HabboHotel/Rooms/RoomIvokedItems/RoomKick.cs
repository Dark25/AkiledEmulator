namespace Akiled.HabboHotel.Rooms.RoomIvokedItems
{
    public struct RoomKick
    {
        public string Alert;
        public int SaufId;

        public RoomKick(string alert, int saufid)
        {
            this.Alert = alert;
            this.SaufId = saufid;
        }
    }
}
