using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorManiqui : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (Session == null || Session.GetHabbo() == null || Item == null)
                return;
            if (!Item.ExtraData.Contains(";"))
                return;
            //if (Session.GetHabbo().Gender.ToLower() != Item.ExtraData.Split(new char[1]{';'})[0])
            //return;

            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            string[] strArray = Session.GetHabbo().Look.Split(new char[1] { '.' });
            string str1 = "";
            foreach (string str2 in strArray)
            {
                if (!str2.StartsWith("ch") && !str2.StartsWith("lg") && (!str2.StartsWith("cc") && !str2.StartsWith("ca")) && (!str2.StartsWith("sh") && !str2.StartsWith("wa")))
                    str1 = str1 + str2 + ".";
            }
            string str3 = str1 + Item.ExtraData.Split(new char[1] { ';' })[1];
            Session.GetHabbo().Look = str3;

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("UPDATE users SET look = @look WHERE id = " + Session.GetHabbo().Id);
                queryreactor.AddParameter("look", str3);
                queryreactor.RunQuery();
            }
            if (room == null)
                return;
            RoomUser Roomuser = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (Roomuser == null)
                return;

            if (Roomuser.transformation || Roomuser.IsSpectator)
                return;
            if (!Session.GetHabbo().InRoom)
                return;
            room.SendPacket(new UserChangeComposer(Roomuser, false));
        }
        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
