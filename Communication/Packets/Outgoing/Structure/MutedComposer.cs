using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
	internal class MutedComposer : ServerPacket
	{
		public MutedComposer(double TimeMuted)
			: base(1092)
		{
			WriteInteger(Convert.ToInt32(TimeMuted));
		}
	}
}
