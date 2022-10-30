using Akiled.HabboHotel.Roleplay.Enemy;
using System.Collections.Generic;

namespace Akiled.HabboHotel.Rooms.RoomBots
{
    public class RoomBot
    {
        public int Id;
        public int OwnerId;
        public string Name;
        public string Motto;
        public string Gender;
        public string Look;
        public int RoomId;
        public bool WalkingEnabled;
        public int FollowUser;
        public int X;
        public int Y;
        public double Z;
        public int Rot;
        public bool AutomaticChat;
        public string ChatText;
        public List<string> RandomSpeech;
        public int SpeakingInterval;
        public bool MixSentences;

        public int Enable;
        public int Handitem;
        public int Status;

        public bool IsDancing;
        public AIType AiType;
        public RoleBot RoleBot;

        public bool IsPet
        {
            get
            {
                return this.AiType == AIType.Pet || this.AiType == AIType.RolePlayPet;
            }
        }

        public string OwnerName
        {
            get
            {
                return AkiledEnvironment.GetGame().GetClientManager().GetNameById(this.OwnerId);
            }
        }

        public RoomBot(int BotId, int OwnerId, int RoomId, AIType AiType, bool WalkingEnabled, string Name, string Motto, string Gender, string Look, int X, int Y, double Z, int Rot, bool ChatEnabled, string ChatText, int ChatSeconds, bool IsDancing, int pEffectEnable, int pHanditemId, int pStatus)
        {
            this.Id = BotId;
            this.OwnerId = OwnerId;
            this.RoomId = RoomId;

            this.AiType = AiType;
            this.RoleBot = null;

            this.Name = Name;
            this.Motto = Motto;
            this.Gender = Gender;
            this.Look = Look;

            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.Rot = Rot;

            this.WalkingEnabled = WalkingEnabled;
            this.AutomaticChat = ChatEnabled;
            this.ChatText = ChatText;
            this.SpeakingInterval = ChatSeconds;
            this.IsDancing = IsDancing;
            this.MixSentences = false;
            this.Enable = pEffectEnable;
            this.Handitem = pHanditemId;
            this.Status = pStatus;

            this.RandomSpeech = new List<string>();

            this.LoadRandomSpeech(this.ChatText);
        }

        public void LoadRandomSpeech(string Text)
        {
            if (!Text.Contains("\r")) return;

            if (this.RandomSpeech.Count > 0)
                this.RandomSpeech.Clear();

            foreach (string Message in Text.Split(new char[] { '\r' }))
                if (!string.IsNullOrWhiteSpace(Message))
                    this.RandomSpeech.Add((Message.Length > 150) ? Message.Substring(0, 150) : Message);
        }

        public string GetRandomSpeech()
        {
            return this.RandomSpeech[AkiledEnvironment.GetRandomNumber(0, this.RandomSpeech.Count - 1)];
        }

        public BotAI GenerateBotAI(int VirtualId)
        {
            switch (this.AiType)
            {
                case AIType.RolePlayBot:
                case AIType.RolePlayPet:
                    return (BotAI)new RoleplayBot(VirtualId);
                case AIType.SuperBot:
                    return (BotAI)new SuperBot(VirtualId);
                case AIType.Pet:
                    return (BotAI)new PetBot(VirtualId);
                default:
                    return (BotAI)new GenericBot(VirtualId);
            }
        }
    }
}
