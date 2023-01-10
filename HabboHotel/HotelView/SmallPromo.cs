using Akiled.Communication.Packets.Outgoing;

namespace Akiled.HabboHotel.HotelView
{
    public class SmallPromo
    {
        public int Index;
        public string Header;
        public string Body;
        public string Button;
        public int inGamePromo;
        public string SpecialAction;
        public string Image;

        public SmallPromo(int index, string header, string body, string button, int inGame, string specialAction, string image)
        {
            this.Index = index;
            this.Header = header;
            this.Body = body;
            this.Button = button;
            this.inGamePromo = inGame;
            this.SpecialAction = specialAction;
            this.Image = image;
        }

        public ServerPacket Serialize(ServerPacket Composer)
        {
            Composer.WriteInteger(Index);
            Composer.WriteString(Header);
            Composer.WriteString(Body);
            Composer.WriteString(Button);
            Composer.WriteInteger(inGamePromo);
            Composer.WriteString(SpecialAction);
            Composer.WriteString(Image);

            return Composer;
        }

    }
}
