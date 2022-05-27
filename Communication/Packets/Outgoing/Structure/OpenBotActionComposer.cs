using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class OpenBotActionComposer : ServerPacket
    {
        public OpenBotActionComposer(RoomUser BotUser, int ActionId, string BotSpeech)
            : base(ServerPacketHeader.OpenBotActionMessageComposer)
        {
            WriteInteger(BotUser.BotData.Id);
            WriteInteger(ActionId);
            if (ActionId == 2)
                WriteString(BotSpeech);
            else if (ActionId == 5)
                WriteString(BotUser.BotData.Name);
        }
    }
}
