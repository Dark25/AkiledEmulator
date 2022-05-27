namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class VoucherRedeemErrorComposer : ServerPacket
    {
        public VoucherRedeemErrorComposer(int Type)
            : base(ServerPacketHeader.VoucherRedeemErrorMessageComposer)
        {
            WriteString(Type.ToString());
        }
    }
}
