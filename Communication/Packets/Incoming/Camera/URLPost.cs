
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Akiled.Communication.Packets.Incoming.Camera
{
  internal class URLPost
  {
    internal static void Web_POST_JSON(string URL, string JSON)
    {
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(URL);
      httpWebRequest.ContentType = "text/json";
      httpWebRequest.Method = "POST";
      using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
      {
        streamWriter.Write(JSON);
        streamWriter.Flush();
        streamWriter.Close();
      }
      using (StreamReader streamReader = new StreamReader(httpWebRequest.GetResponse().GetResponseStream()))
        streamReader.ReadToEnd();
    }

    internal static string GetDataFromJSON(string json, string data)
    {
      string[] strArray = json.Split('"');
      bool flag = false;
      foreach (string str in strArray)
      {
        if (flag)
          return str.Substring(0, str.Length - 1).Substring(3);
        if (str.Equals(data))
          flag = true;
      }
      return "";
    }

    internal static string GetMD5(string str)
    {
      MD5 md5 = MD5.Create();
      ASCIIEncoding asciiEncoding = new ASCIIEncoding();
      StringBuilder stringBuilder = new StringBuilder();
      foreach (int num in md5.ComputeHash(asciiEncoding.GetBytes(str)))
        stringBuilder.AppendFormat("{0:x2}", (object) (byte) num);
      return stringBuilder.ToString();
    }
  }
}
