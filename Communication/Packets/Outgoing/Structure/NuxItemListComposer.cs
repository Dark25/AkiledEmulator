using Akiled.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class NuxItemListComposer : ServerPacket
    {
        public NuxItemListComposer() : base(ServerPacketHeader.NuxItemListComposer)
        {
            base.WriteInteger(1);

            base.WriteInteger(1);
            base.WriteInteger(3);
            base.WriteInteger(3);



            using (IQueryAdapter dbQuery = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbQuery.SetQuery("SELECT `image`, `title` FROM `nux_gifts` LIMIT 3");
                DataTable gUsersTable = dbQuery.GetTable();

                foreach (DataRow Row in gUsersTable.Rows)
                {
                    base.WriteString(Convert.ToString(Row["image"])); // image.library.url + string
                    base.WriteInteger(1); // items:
                    base.WriteString(Convert.ToString(Row["title"])); // item_name (product_x_name)
                    base.WriteString(""); // can be null
                }
            }
        }
    }
}
