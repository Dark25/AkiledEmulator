using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class UniqueIDEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string CookieId = Packet.PopString();
            string McId = Packet.PopString();
            string Junk = Packet.PopString();

            string Head = (string.IsNullOrWhiteSpace(CookieId) || CookieId.Length != 13) ? IDGenerator.Instance.Next : CookieId;

            Session.MachineId = Head + McId + Junk;

            Session.SendPacket(new SetUniqueIdComposer(Head));

        }
    }
}
