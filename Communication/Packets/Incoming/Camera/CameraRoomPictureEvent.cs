using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Core;
using Akiled.HabboHotel.GameClients;
using AkiledEmulator.HabboHotel.Camera;
using Newtonsoft.Json;
using System;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class CameraRoomPictureEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session?.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            int count = Packet.PopInt();

            byte[] data = Packet.ReadBytes(count);

            try
            {
                string base64 = Convert.ToBase64String(data);

                //Check if is an PNG valid image
                if (!base64.Substring(0, 5).ToUpper().Equals("IVBOR"))
                {
                    Console.WriteLine("Someone tried take a picture with an invalid mime type! (Username: " + Session.GetHabbo().Username + ")");
                    return;
                }

                string result = CameraHelper.request("camera", Session.GetHabbo().Id, Session.GetHabbo().CurrentRoom.Id, base64);

                JSONCamera jsonCamera = JsonConvert.DeserializeObject<JSONCamera>(result);
                if (!jsonCamera.status)
                {
                    Session.SendNotification("It happened some error while trying save this picture! Try again!");
                    return;
                }

                Console.WriteLine("New photo camera: " + jsonCamera.preview);

                Session.GetHabbo().lastPhotoPreview = jsonCamera;

                Session.SendMessage(new CameraPhotoPreviewComposer(jsonCamera.preview));
            }
            catch (Exception ex)
            {
                Logging.LogException(ex.ToString());
            }
        }
    }
}