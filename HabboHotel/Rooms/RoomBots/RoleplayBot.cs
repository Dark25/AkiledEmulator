using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.RoomBots
{
    public class RoleplayBot : BotAI
    {
        private readonly int SpeechTimer;
        private readonly int ActionTimer;

        public RoleplayBot(int VirtualId)
        {
            this.SpeechTimer = AkiledEnvironment.GetRandomNumber(10, 40);
            this.ActionTimer = AkiledEnvironment.GetRandomNumber(10, 30);
        }

        public override void OnSelfEnterRoom()
        {
        }

        public override void OnSelfLeaveRoom(bool Kicked)
        {
        }

        public override void OnUserEnterRoom(RoomUser User)
        {
        }

        public override void OnUserLeaveRoom(GameClient Client)
        {
        }

        public override void OnUserSay(RoomUser User, string Message)
        {
        }

        public override void OnUserShout(RoomUser User, string Message)
        {
        }

        public override void OnTimerTick()
        {
        }
    }
}
