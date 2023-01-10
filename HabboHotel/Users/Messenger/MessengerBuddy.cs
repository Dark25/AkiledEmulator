using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Users.Messenger
{
    public class MessengerBuddy
    {
        private int _userId;
        private string _username;
        private string _look;
        private int _relation;
        private bool _isOnline;
        private bool _hideInroom;
        string lookchatpub = (AkiledEnvironment.GetConfig().data["lookchatpub"]);
        string lookchatinter = (AkiledEnvironment.GetConfig().data["lookchatinter"]);
        string lookchatguia = (AkiledEnvironment.GetConfig().data["lookchatguia"]);
        string lookchatgm = (AkiledEnvironment.GetConfig().data["lookchatgm"]);
        string lookchatmod = (AkiledEnvironment.GetConfig().data["lookchatmod"]);
        string lookchatstaff = (AkiledEnvironment.GetConfig().data["lookchatstaff"]);
        public MessengerBuddy(int UserId, string pUsername, string pLook, int relation)
        {
            this._userId = UserId;
            this._username = pUsername;
            this._look = pLook;
            this._relation = relation;
        }

        public void UpdateRelation(int Type)
        {
            this._relation = Type;
        }

        public void UpdateUser()
        {
            GameClient client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(_userId);
            if (client != null && client.GetHabbo() != null && client.GetHabbo().GetMessenger() != null && !client.GetHabbo().GetMessenger().AppearOffline)
            {
                _isOnline = true;
                _look = client.GetHabbo().Look;
                _hideInroom = client.GetHabbo().HideInRoom;
            }
            else if (_userId == 0x7fffffff)
            {
                _isOnline = true;
                _look = lookchatstaff;
                _hideInroom = true;
            }
            else if (_userId == 0x6fffffff)
            {
                _isOnline = true;
                _look = lookchatmod;
                _hideInroom = true;
            }
            else if (_userId == 0x5fffffff)
            {
                _isOnline = true;
                _look = lookchatgm;
                _hideInroom = true;
            }
            else if (_userId == 0x4fffffff)
            {
                _isOnline = true;
                _look = lookchatinter;
                _hideInroom = true;
            }
            else if (_userId == 0x3fffffff)
            {
                _isOnline = true;
                _look = lookchatguia;
                _hideInroom = true;
            }
            else if (_userId == 0x2fffffff)
            {
                _isOnline = true;
                _look = lookchatpub;
                _hideInroom = true;
            }
            else
            {
                _isOnline = false;
                _look = "";
                _hideInroom = true;
            }

        }

        public void Serialize(ServerPacket reply)
        {

            if (_userId == 0x7fffffff)
            {
                _isOnline = true;
                _look = lookchatstaff;
                _hideInroom = true;
            }

            if (_userId == 0x6fffffff)
            {
                _isOnline = true;
                _look = lookchatmod;
                _hideInroom = true;
            }

            if (_userId == 0x5fffffff)
            {
                _isOnline = true;
                _look = lookchatgm;
                _hideInroom = true;
            }

            if (_userId == 0x4fffffff)
            {
                _isOnline = true;
                _look = lookchatinter;
                _hideInroom = true;
            }
            if (_userId == 0x3fffffff)
            {
                _isOnline = true;
                _look = lookchatguia;
                _hideInroom = true;
            }
            if (_userId == 0x2fffffff)
            {
                _isOnline = true;
                _look = lookchatpub;
                _hideInroom = true;
            }
            reply.WriteInteger(this._userId);
            reply.WriteString(this._username);
            reply.WriteInteger(1);
            bool isOnline = this._isOnline;
            reply.WriteBoolean(isOnline);

            if (isOnline)
                reply.WriteBoolean(!this._hideInroom);
            else
                reply.WriteBoolean(false);

            reply.WriteString(isOnline ? this._look : "");
            reply.WriteInteger(0);
            reply.WriteString(""); //Motto ?
            reply.WriteString(string.Empty);
            reply.WriteString(string.Empty);
            reply.WriteBoolean(true); // Allows offline messaging
            reply.WriteBoolean(false);
            reply.WriteBoolean(false);
            reply.WriteShort(this._relation);
        }
    }
}
