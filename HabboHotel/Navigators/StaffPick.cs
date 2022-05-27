
namespace Akiled.HabboHotel.Navigators
{
    public class StaffPick
    {
        public int RoomId { get; set; }

        public string Image { get; set; }

        public StaffPick(int roomId, string image)
        {
            this.RoomId = roomId;
            this.Image = image;
        }
    }
}
