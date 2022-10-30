using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Catalog.Vouchers;
using Akiled.HabboHotel.GameClients;
using System.Data;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class RedeemVoucherEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string VoucherCode = Packet.PopString().Replace("\r", "");

            Voucher Voucher = null;
            if (!AkiledEnvironment.GetGame().GetCatalog().GetVoucherManager().TryGetVoucher(VoucherCode, out Voucher))
            {
                Session.SendPacket(new VoucherRedeemErrorComposer(0));
                return;
            }

            if (Voucher.CurrentUses >= Voucher.MaxUses)
                return;

            DataRow GetRow = null;
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `user_vouchers` WHERE `user_id` = @userId AND `voucher` = @Voucher LIMIT 1");
                dbClient.AddParameter("userId", Session.GetHabbo().Id);
                dbClient.AddParameter("Voucher", VoucherCode);
                GetRow = dbClient.GetRow();
            }

            if (GetRow != null)
                return;
            else
            {
                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `user_vouchers` (`user_id`,`voucher`) VALUES (@userId, @Voucher)");
                    dbClient.AddParameter("userId", Session.GetHabbo().Id);
                    dbClient.AddParameter("Voucher", VoucherCode);
                    dbClient.RunQuery();
                }
            }

            Voucher.UpdateUses();

            if (Voucher.Type == VoucherType.CREDIT)
            {
                Session.GetHabbo().Credits += Voucher.Value;
                Session.SendPacket(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }
            if (Voucher.Type == VoucherType.KAKAS)
            {
                Session.GetHabbo().AkiledPoints += Voucher.Value;
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, 0, 105));

                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    queryreactor.RunQuery("UPDATE users SET vip_points = vip_points - " + Voucher.Value + " WHERE id = " + Session.GetHabbo().Id + " LIMIT 1");

            }
            else if (Voucher.Type == VoucherType.DUCKET)
            {
                Session.GetHabbo().Duckets += Voucher.Value;
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Voucher.Value));



                //Session.SendPacket(new VoucherRedeemOkComposer());
            }
        }
    }
}
