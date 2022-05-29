
using System.IO;
using System.IO.Compression;

namespace Akiled.Communication.Packets.Incoming.Camera
{
  internal class Camera
  {
     internal static string Decompiler(byte[] input) => Akiled.Communication.Packets.Incoming.Camera.Camera.DecompressBytes(input);

    private static string DecompressBytes(byte[] bytes)
    {
      using (MemoryStream memoryStream = new MemoryStream(bytes, 2, bytes.Length - 2))
      {
        using (DeflateStream deflateStream = new DeflateStream((Stream) memoryStream, CompressionMode.Decompress))
        {
          using (StreamReader streamReader = new StreamReader((Stream) deflateStream))
            return streamReader.ReadToEnd();
        }
      }
    }
  }
}
