using Akiled.Communication.Packets.Outgoing;
using System;

namespace Akiled.HabboHotel.ChatMessageStorage
{
    public class ChatMessage
    {
        private readonly int userID;
        private readonly string username;
        public readonly int roomID;
        private readonly string message;
        private readonly DateTime timeSpoken;

        public ChatMessage(int userID, string username, int roomID, string message, DateTime timeSpoken)
        {
            this.userID = userID;
            this.username = username;
            this.roomID = roomID;
            this.message = message;
            this.timeSpoken = timeSpoken;
        }

        public void Serialize(ref ServerPacket packet)
        {
            packet.WriteString(timeSpoken.Hour + ":" + timeSpoken.Minute); //this.timeSpoken.Minute
            packet.WriteInteger(this.userID); //this.timeSpoken.Minute
            packet.WriteString(this.username);
            packet.WriteString(this.message);
            packet.WriteBoolean(false); // Text is bold
        }
    }
}
