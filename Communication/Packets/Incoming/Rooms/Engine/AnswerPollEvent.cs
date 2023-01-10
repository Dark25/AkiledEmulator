using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class AnswerPollEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;

            RoomUser User = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (User == null)
                return;

            int Id = Packet.PopInt();
            int QuestionId = Packet.PopInt();

            int Count = Packet.PopInt();//Count

            string Value = "0";
            for (int i = 0; i < Count; i++)
            {
                Value = Packet.PopString();
            }

            Value = (Value != "0" && Value != "1") ? "0" : Value;

            if (Value == "0") room.VotedNoCount++;

            else room.VotedYesCount++;

            ServerPacket Message = new ServerPacket(ServerPacketHeader.RoomUserQuestionAnsweredComposer);
            Message.WriteInteger(Session.GetHabbo().Id); //userId
            Message.WriteString(Value); ///value
            Message.WriteInteger(2); //count

            Message.WriteString("0"); //key
            Message.WriteInteger(room.VotedNoCount); //Négatif
            Message.WriteString("1"); //key
            Message.WriteInteger(room.VotedYesCount); //Positif
            room.SendPacket(Message);

            string WiredCode = (Value == "0") ? "QUESTION_NO" : "QUESTION_YES";
            if (room.AllowsShous(User, WiredCode))
                User.SendWhisperChat(WiredCode, false);
        }
    }
}