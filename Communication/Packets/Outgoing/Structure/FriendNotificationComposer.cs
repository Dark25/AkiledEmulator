using Akiled.HabboHotel.Users.Messenger;

namespace Akiled.Communication.Packets.Outgoing.Messenger
{
    internal class FriendNotificationComposer : ServerPacket
    {
        public int UserId { get; }
        public MessengerEventTypes Type { get; }
        public string Data { get; }

        public FriendNotificationComposer(int userId, MessengerEventTypes type, string data)
            : base(ServerPacketHeader.FriendNotificationMessageComposer)
        {
            UserId = userId;
            Type = type;
            Data = data;

            WriteString(UserId.ToString());
            WriteInteger(MessengerEventTypesUtility.GetEventTypePacketNum(Type));
            WriteString(Data);
        }

    }
}