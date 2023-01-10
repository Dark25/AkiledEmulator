using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class SSOTicketEvent : IPacketEvent
    {
        public async void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() != null)
                return;

            await Session.TryAuthenticateAsync(Packet.PopString()).ConfigureAwait(true);
        }
    }
}