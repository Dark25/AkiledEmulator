using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Akiled.Communication.Packets.Incoming.Camera
{
    public class RenderRoomMessageComposerBigPhoto : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket paket)
        {
            Console.WriteLine("packet recv");
            string str = Camera.Decompiler(paket.ReadBytes(paket.PopInt()));

            string roomIdJSON = URLPost.GetDataFromJSON(str, "roomid");
            double timestamp = double.Parse(URLPost.GetDataFromJSON(str, "timestamp"));
            string timestampJSON = (timestamp - (timestamp % 100)).ToString();

            Session.GetHabbo().lastPhotoPreview = roomIdJSON + "-" + timestampJSON;
            Console.WriteLine("DEBUG: " + URLPost.GetMD5(Session.GetHabbo().lastPhotoPreview)); //debug now
            Session.SendMessage(new CameraPhotoPreviewComposer("photos/" + URLPost.GetMD5(Session.GetHabbo().lastPhotoPreview) + ".png"));
        }
    }

    class Camera
    {
        internal static string Decompiler(byte[] input)
        {
            // Primero desofuscar el ZLIB
            return DecompressBytes(input);
        }

        private static string DecompressBytes(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes, 2, bytes.Length - 2))
            using (var inflater = new DeflateStream(stream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(inflater))
            {
                return streamReader.ReadToEnd();
            }
        }
    }

    class URLPost
    {
        internal static void Web_POST_JSON(string URL, string JSON)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(JSON);
                streamWriter.Flush();
                streamWriter.Close();
            }

            HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
            }
        }

        internal static string GetDataFromJSON(string json, string data)
        {
            string[] values = json.Split('\"');
            bool enable = false;

            foreach (string value in values)
            {
                if (enable)
                {
                    return value.Substring(0, value.Length - 1).Substring(3);
                }
                else if (value.Equals(data))
                {
                    enable = true;
                    continue;
                }
            }

            return "";
        }

        internal static string GetMD5(string str)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = md5.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

    }
}