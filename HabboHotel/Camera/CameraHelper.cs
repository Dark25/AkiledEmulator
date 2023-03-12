using Akiled;
using Nancy.Json;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace AkiledEmulator.HabboHotel.Camera
{
    public class CameraHelper
    {
        public static string BASE_URL = AkiledEnvironment.GetConfig().data["Url_camera"];
        private static string REQUEST_URL = BASE_URL + "camera.php";

        public static string request(string type, int userId, int roomId, string base64)
        {

            using (var client = new HttpClient())
            {

                string json = new JavaScriptSerializer().Serialize(new
                {
                    type,
                    user_id = userId,
                    room_id = roomId,
                    base_64 = base64,
                    timestamp = AkiledEnvironment.GetUnixTimestamp()
                });

                var httpResponse = client.PostAsJsonAsync(REQUEST_URL, json);
                
                return httpResponse.Result.Content.ReadAsStringAsync().Result;
            }
            
        }
    }
}
