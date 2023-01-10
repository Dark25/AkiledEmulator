using Akiled.Communication.Encryption;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class InitCryptoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet) => Session.SendPacket(new InitCryptoComposer(HabboEncryptionV2.GetRsaDiffieHellmanPrimeKey(), HabboEncryptionV2.GetRsaDiffieHellmanGeneratorKey()));
    }
}