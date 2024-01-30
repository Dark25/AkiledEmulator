using Akiled;
using Nancy.Json;
using System.IO;
using System.Net;

namespace AkiledEmulator.HabboHotel.Camera
{
    public class CameraHelper
    {
        public static string BASE_URL = AkiledEnvironment.GetConfig().data["Url_camera"];
        private static string REQUEST_URL = BASE_URL + "camera.php";

        public static string request(string type, int userId, int roomId, string base64)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(REQUEST_URL);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    type,
                    user_id = userId,
                    room_id = roomId,
                    base_64 = base64,
                    timestamp = AkiledEnvironment.GetUnixTimestamp()
                });

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }
    }
}