using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.WebSocket
{
    class NotifTopInitComposer : ServerPacket
    {
        public NotifTopInitComposer(List<string> Messages)
         : base(19)
        {
            WriteInteger(Messages.Count);

            foreach (string Message in Messages)
                WriteString(Message);
        }
    }
}
