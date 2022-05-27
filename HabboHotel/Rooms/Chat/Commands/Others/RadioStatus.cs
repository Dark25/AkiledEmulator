using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;
using System.Xml;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class RadioStatus : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            if ((double)Session.GetHabbo().last_sms > AkiledEnvironment.GetUnixTimestamp() - 10.0)
            {
                Session.SendWhisper("Debes esperar 10 segundos para volver a consultar la información de la radio.", 1);
                return;
            }

            if (Params.Length == 1)
            {
                string shoutcast_stats = (AkiledEnvironment.GetConfig().data["shoutcast_stats"]);
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(shoutcast_stats);

                XmlNodeList ipxml = xDoc.GetElementsByTagName("SHOUTCASTSERVER"); // Selecionamos la primera etiqueta del XML
                XmlNodeList lista = ((XmlElement)ipxml[0]).GetElementsByTagName("STREAMSTATS");
                XmlNodeList definitivo = ((XmlElement)lista[0]).GetElementsByTagName("STREAM");

                foreach (XmlElement nodo in definitivo)
                {
                    int i = 0;
                    XmlNodeList Title = nodo.GetElementsByTagName("SERVERTITLE");
                    XmlNodeList Song = nodo.GetElementsByTagName("SONGTITLE");
                    XmlNodeList Oyentes = nodo.GetElementsByTagName("PEAKLISTENERS");
                    foreach (GameClient cliente in AkiledEnvironment.GetGame().GetClientManager().GetClients)
                    {
                        var bubbleNotification = new ServerPacket(ServerPacketHeader.RoomNotificationMessageComposer);
                        bubbleNotification.WriteString("LOCOSON");
                        bubbleNotification.WriteInteger(string.IsNullOrEmpty("https://www.hionix.com/") ? 2 : 3);
                        bubbleNotification.WriteString("display");
                        bubbleNotification.WriteString("BUBBLE");
                        bubbleNotification.WriteString("message");
                        bubbleNotification.WriteString("[Ø] Canción: " + Song[i].InnerText + "- [ª] DJ: " + Title[i].InnerText + " [±] Oyentes: " + Oyentes[i].InnerText + "");
                        bubbleNotification.WriteString("linkUrl");
                        bubbleNotification.WriteString("https://www.Hionix.com/");
                        cliente.SendMessage(bubbleNotification);

                    }
                    Session.SendWhisper("    [Ø]Canción: " + Song[i].InnerText + "  -  [ª]DJ: " + Title[i].InnerText + "  [±] Oyentes: " + Oyentes[i].InnerText + "", 33);
                    string Message = "[Ø]Canción: " + Song[i].InnerText + "  -  [ª]DJ: " + Title[i].InnerText + "  [±] Oyentes: " + Oyentes[i].InnerText + "";
                    //AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifTopComposer(Message), Session.Langue);
                }

            }
        }
    }
}