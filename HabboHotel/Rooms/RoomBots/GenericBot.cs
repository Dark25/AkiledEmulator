using Akiled.HabboHotel.GameClients;
using System.Drawing;

namespace Akiled.HabboHotel.Rooms.RoomBots
{
    public class GenericBot : BotAI
    {
        private int SpeechTimer;
        private int ActionTimer;

        public GenericBot(int VirtualId)
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
            if (this.GetBotData() == null) return;

            if (this.SpeechTimer <= 0)
            {
                if (this.GetBotData().RandomSpeech.Count > 0 && this.GetBotData().AutomaticChat)
                    this.GetRoomUser().OnChat(this.GetBotData().GetRandomSpeech(), 2, false);

                this.SpeechTimer = this.GetBotData().SpeakingInterval * 2;
            }
            else
                this.SpeechTimer--;

            if (this.ActionTimer <= 0)
            {
                if (this.GetBotData().WalkingEnabled && this.GetBotData().FollowUser == 0)
                {
                    Point randomWalkableSquare = this.GetRoom().GetGameMap().getRandomWalkableSquare(this.GetBotData().X, this.GetBotData().Y);
                    this.GetRoomUser().MoveTo(randomWalkableSquare.X, randomWalkableSquare.Y);
                }
                this.ActionTimer = AkiledEnvironment.GetRandomNumber(10, 60);
            }
            else
                this.ActionTimer--;

            if (this.GetBotData().FollowUser != 0)
            {
                RoomUser user = this.GetRoom().GetRoomUserManager().GetRoomUserByVirtualId(this.GetBotData().FollowUser);
                if (user == null)
                {
                    this.GetBotData().FollowUser = 0;
                }
                else
                {
                    if (!Gamemap.TilesTouching(this.GetRoomUser().X, this.GetRoomUser().Y, user.X, user.Y))
                        this.GetRoomUser().MoveTo(user.X, user.Y, true);
                }
            }
        }
    }
}
