using Akiled.Communication.Encryption;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GenerateSecretKeyEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string CipherPublickey = Packet.PopString();

            //BigInteger SharedKey = HabboEncryptionV2.CalculateDiffieHellmanSharedKey(CipherPublickey);
            //if (SharedKey != 0)
            {
                //Session.RC4Client = new ARC4(SharedKey.getBytes());
                Session.SendPacket(new SecretKeyComposer(HabboEncryptionV2.GetRsaDiffieHellmanPublicKey()));
            }
        }
    }
}
