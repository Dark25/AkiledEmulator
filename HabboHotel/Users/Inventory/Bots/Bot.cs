using Akiled.HabboHotel.Rooms.RoomBots;

namespace Akiled.HabboHotel.Users.Inventory.Bots
{
    public class Bot
    {
        public int Id;
        public int OwnerId;
        public string Name;
        public string Motto;
        public string Figure;
        public string Gender;
        public bool WalkingEnabled;
        public bool ChatEnabled;
        public string ChatText;
        public int ChatSeconds;
        public bool IsDancing;
        public int Enable;
        public int Handitem;
        public int Status;
        public AIType AIType { get; set; }

        public Bot(int Id, int OwnerId, string Name, string Motto, string Figure, string Gender, bool WalkingEnabled, bool ChatEnabled, string ChatText, int ChatSeconds, bool IsDancing, int pEnable, int pHanditem, int pStatus, AIType aiType)
        {
            this.Id = Id;
            this.OwnerId = OwnerId;
            this.Name = Name;
            this.Motto = Motto;
            this.Figure = Figure;
            this.Gender = Gender;
            this.WalkingEnabled = WalkingEnabled;
            this.ChatEnabled = ChatEnabled;
            this.ChatText = ChatText;
            this.ChatSeconds = ChatSeconds;
            this.IsDancing = IsDancing;
            this.Enable = pEnable;
            this.Handitem = pHanditem;
            this.Status = pStatus;
            this.AIType = aiType;
        }
    }
}
