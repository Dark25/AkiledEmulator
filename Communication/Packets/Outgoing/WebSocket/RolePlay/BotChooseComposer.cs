using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.WebSocket
{
    class BotChooseComposer : ServerPacket
    {
        public BotChooseComposer(List<string[]> ChooseList)
          : base(23)
        {
            WriteInteger(ChooseList.Count);

            foreach (string[] Choose in ChooseList)
            {
                WriteString(Choose[0]); //Username
                WriteString(Choose[1]); //Code
                WriteString(Choose[2]); //Message
                WriteString(Choose[3]); //Look
            }
        }
    }
}
