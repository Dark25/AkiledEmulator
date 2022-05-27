using Akiled.Core;

namespace Akiled.HabboHotel.Navigators
{
    public class FeaturedRoom
    {
        public int RoomId;
        public string Image;
        public Language Langue;

        public FeaturedRoom(int RoomId, string Image, Language Langue)
        {
            this.RoomId = RoomId;
            this.Image = Image;
            this.Langue = Langue;
        }
    }
}