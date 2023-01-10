using System;
using System.Collections.Generic;
using System.Text;

namespace Akiled.Communication.Packets.Outgoing
{
    public class ServerPacket : IServerPacket
    {
        private readonly Encoding Encoding = Encoding.GetEncoding("Windows-1252");
        private readonly List<byte> Body = new List<byte>();

        public ServerPacket(int Header)
        {
            this.Body = new List<byte>();
            this.WriteShort(Header);
        }





        public void WriteByte(byte b)
        {
            this.WriteByte(new byte[] { b }, false);
        }

        public void WriteShort(int i)
        {
            this.WriteByte(BitConverter.GetBytes((short)i), true);
        }

        public void WriteInteger(int i)
        {
            this.WriteByte(BitConverter.GetBytes(i), true);
        }

        public void WriteBoolean(bool b)
        {
            this.WriteByte(new byte[1] { b ? (byte)1 : (byte)0 }, false);
        }

        public void WriteString(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            this.WriteShort(bytes.Length);
            this.WriteByte(bytes, false);
        }

        public void WriteByte(byte[] b, bool IsInt)
        {
            if (IsInt)
            {
                for (int i = b.Length - 1; i > -1; --i)
                {
                    this.Body.Add(b[i]);
                }
            }
            else
                this.Body.AddRange(b);
        }

        public byte[] GetBytes()
        {
            List<byte> Final = new List<byte>();
            Final.AddRange(BitConverter.GetBytes(this.Body.Count));
            Final.Reverse();
            Final.AddRange(this.Body);
            return Final.ToArray();
        }
    }
}
