namespace Akiled.Communication.Packets.Outgoing.Structure
{
    public class SecretKeyComposer : ServerPacket
    {
        public SecretKeyComposer(string PublicKey)
            : base(ServerPacketHeader.SecretKeyMessageComposer)
        {
            WriteString(PublicKey);
            WriteBoolean(false);
        }
    }
}