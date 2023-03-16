using Akiled.Communication.Packets.Incoming;
using Akiled.Communication.WebSocket;
using Akiled.HabboHotel.GameClients;
using Akiled.Utilities;
using SharedPacketLib;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Akiled.Net
{
    public class GamePacketParser : IDataParser, IDisposable, ICloneable
    {
        private GameClient currentClient;

        public event HandlePacket OnNewPacket;

        private bool _halfDataRecieved = false;
        private byte[] _halfData = null;
        private bool _isWebSocket = false;

        public GamePacketParser(GameClient me)
        {
            this.currentClient = me;
            this.OnNewPacket = (HandlePacket)null;
        }

        public void handlePacketData(byte[] Data, bool deciphered = false)
        {
            try
            {
                if (this.OnNewPacket == null) return;

                if (Data[0] == 71 && Data[1] == 69)
                {
                    PolicyRequest(Data);

                    this._isWebSocket = true;
                    this.currentClient.GetConnection().IsWebSocket = true;
                    return;
                }

                if (Data.Length == 23 || Data.Length == 22)
                {
                    if (Data[0] == 60 && Data[1] == 112)
                    {
                        this.currentClient.GetConnection().SendData(Encoding.Default.GetBytes(GetXmlPolicy()));

                        return;
                    }
                }

                try
                {
                    if (this._isWebSocket) Data = EncodeDecode.DecodeMessage(Data);
                }
                catch
                {
                    return;
                }

                if (currentClient is { RC4Client: { } } && !deciphered)
                {
                    currentClient.RC4Client.Decrypt(ref Data);
                }

                if (this._halfDataRecieved)
                {
                    byte[] FullDataRcv = new byte[this._halfData.Length + Data.Length];
                    Buffer.BlockCopy(this._halfData, 0, FullDataRcv, 0, this._halfData.Length);
                    Buffer.BlockCopy(Data, 0, FullDataRcv, this._halfData.Length, Data.Length);

                    this._halfDataRecieved = false; // mark done this round
                    handlePacketData(FullDataRcv, true); // repeat now we have the combined array
                    return;
                }

                using (BinaryReader Reader = new BinaryReader(new MemoryStream(Data)))
                {
                    if (Data.Length < 4)
                        return;

                    int MsgLen = HabboEncoding.DecodeInt32(Reader.ReadBytes(4));
                    if ((Reader.BaseStream.Length - 4) < MsgLen)
                    {
                        this._halfData = Data;
                        this._halfDataRecieved = true;
                        return;
                    }
                    else if ((MsgLen < 0 || MsgLen > 500000))
                    {
                        return;
                    }

                    byte[] Packet = Reader.ReadBytes(MsgLen);

                    using (BinaryReader R = new BinaryReader(new MemoryStream(Packet)))
                    {
                        int Header = HabboEncoding.DecodeInt16(R.ReadBytes(2));

                        byte[] Content = new byte[Packet.Length - 2];
                        Buffer.BlockCopy(Packet, 2, Content, 0, Packet.Length - 2);

                        ClientPacket Message = new ClientPacket(Header, Content);
                        OnNewPacket.Invoke(Message);
                    }

                    if (Reader.BaseStream.Length - 4 > MsgLen)
                    {
                        byte[] Extra = new byte[Reader.BaseStream.Length - Reader.BaseStream.Position];
                        Buffer.BlockCopy(Data, (int)Reader.BaseStream.Position, Extra, 0, ((int)Reader.BaseStream.Length - (int)Reader.BaseStream.Position));

                        handlePacketData(Extra, true);
                    }
                }
            }
            catch (Exception e)
            {
                string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
                Console.WriteLine("Packet Error! " + e);
            }
        }

        private static string GetXmlPolicy()
        {
            return "<?xml version=\"1.0\"?>\r\n" +
                          "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                          "<cross-domain-policy>\r\n" +
                          "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
                          "</cross-domain-policy>\x0";
        }

        private void PolicyRequest(byte[] packet)
        {
            string data = Encoding.UTF8.GetString(packet);
            /* Handshaking and managing ClientSocket */

            if (!data.Contains("ey:"))
                return;

            string key = data.Replace("ey:", "`")
                      .Split('`')[1]                     // dGhlIHNhbXBsZSBub25jZQ== \r\n .......
                      .Replace("\r", "").Split('\n')[0]  // dGhlIHNhbXBsZSBub25jZQ==
                      .Trim();

            // key should now equal dGhlIHNhbXBsZSBub25jZQ==
            string longKey = key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            SHA1 sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(Encoding.ASCII.GetBytes(longKey));
            string test1 = Convert.ToBase64String(hashBytes);

            string newLine = "\r\n";

            string response = "HTTP/1.1 101 Switching Protocols" + newLine
                 + "Upgrade: websocket" + newLine
                 + "Connection: Upgrade" + newLine
                 + "Sec-WebSocket-Accept: " + test1 + newLine + newLine
                 ;

            // which one should I use? none of them fires the onopen method
            currentClient.GetConnection().SendData(Encoding.UTF8.GetBytes(response));
        }

        public void Dispose()
        {
            this.OnNewPacket = null;
            GC.SuppressFinalize(this);
        }

        public object Clone()
        {
            return new GamePacketParser(this.currentClient);
        }

        public delegate void HandlePacket(ClientPacket message);

    }
}