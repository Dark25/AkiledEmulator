using Akiled.Communication.Packets.Outgoing;

namespace Akiled.HabboHotel.Users.Messenger
{
    public class MessengerRequest
    {
        private readonly int _toUser;
        private readonly int _fromUser;
        private readonly string _username;

        public int To
        {
            get
            {
                return this._toUser;
            }
        }

        public int From
        {
            get
            {
                return this._fromUser;
            }
        }

        public MessengerRequest(int ToUser, int FromUser, string pUsername)
        {
            this._toUser = ToUser;
            this._fromUser = FromUser;
            this._username = pUsername;
        }

        public void Serialize(ServerPacket Request)
        {
            Request.WriteInteger(this._fromUser);
            Request.WriteString(this._username);
            Request.WriteString("");
        }
    }
}
