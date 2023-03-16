using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.RoomBots
{
    public abstract class BotAI
    {
        public int BaseId;
        private int RoomUserId;
        private int RoomId;
        private RoomUser roomUser;
        private Room room;

        public void Init(int pBaseId, int pRoomUserId, int pRoomId, RoomUser user, Room room)
        {
            this.BaseId = pBaseId;
            this.RoomUserId = pRoomUserId;
            this.RoomId = pRoomId;
            this.roomUser = user;
            this.room = room;
        }

        public Room GetRoom()
        {
            return this.room;
        }

        public RoomUser GetRoomUser()
        {
            return this.roomUser;
        }

        public RoomBot GetBotData()
        {
            if (this.GetRoomUser() == null)
                return (RoomBot)null;
            else
                return this.GetRoomUser().BotData;
        }

        public abstract void OnSelfEnterRoom();

        public abstract void OnSelfLeaveRoom(bool Kicked);

        public abstract void OnUserEnterRoom(RoomUser User);

        public abstract void OnUserLeaveRoom(GameClient Client);

        public abstract void OnUserSay(RoomUser User, string Message);

        public abstract void OnUserShout(RoomUser User, string Message);

        public abstract void OnTimerTick();
    }
}
