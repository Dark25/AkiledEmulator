using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class SaveWardrobeOutfitEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int slotId = Packet.PopInt();
            string look = Packet.PopString();
            string gender = Packet.PopString();

            if (gender != "M" && gender != "F") return;

            look = AkiledEnvironment.GetFigureManager().ProcessFigure(look, gender, true);

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT null FROM user_wardrobe WHERE user_id = '" + Session.GetHabbo().Id + "' AND slot_id = '" + slotId + "';");
                queryreactor.AddParameter("look", look);
                queryreactor.AddParameter("gender", gender.ToUpper());
                if (queryreactor.GetRow() != null)
                {
                    queryreactor.SetQuery("UPDATE user_wardrobe SET look = @look, gender = @gender WHERE user_id = " + Session.GetHabbo().Id + " AND slot_id = " + slotId + ";");
                    queryreactor.AddParameter("look", look);
                    queryreactor.AddParameter("gender", gender.ToUpper());
                    queryreactor.RunQuery();
                }
                else
                {
                    queryreactor.SetQuery("INSERT INTO user_wardrobe (user_id,slot_id,look,gender) VALUES (" + Session.GetHabbo().Id + "," + slotId + ",@look,@gender)");
                    queryreactor.AddParameter("look", look);
                    queryreactor.AddParameter("gender", gender.ToUpper());
                    queryreactor.RunQuery();
                }
            }

        }
    }
}