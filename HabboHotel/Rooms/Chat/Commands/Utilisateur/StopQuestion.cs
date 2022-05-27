using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class StopQuestion : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.SimplePollAnswersComposer);
            Message.WriteInteger(1);//PollId
            Message.WriteInteger(2);//Count
            Message.WriteString("0");//Négatif
            Message.WriteInteger(Room.VotedNoCount);//Nombre
            Message.WriteString("1");//Positif
            Message.WriteInteger(Room.VotedYesCount);//Nombre
            Room.SendPacket(Message);

            UserRoom.SendWhisperChat("La encuenta ha terminado!");
        }
    }
}
