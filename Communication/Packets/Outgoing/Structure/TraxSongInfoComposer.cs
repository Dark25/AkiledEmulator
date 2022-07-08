using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    internal class TraxSongInfoComposer : ServerPacket
    {
        public TraxSongInfoComposer()
          : base(3365)
        {
            this.WriteInteger(0);
        }
    }
}
