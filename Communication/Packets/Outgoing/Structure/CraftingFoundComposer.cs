namespace Akiled.Communication.Packets.Outgoing.Structure
{
    internal class CraftingFoundComposer : ServerPacket
    {
        public CraftingFoundComposer(int count, bool found)
          : base(2124)
        {
            this.WriteInteger(count);
            this.WriteBoolean(found);
        }
    }
}
