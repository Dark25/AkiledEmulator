using Akiled.Database.Interfaces;
using System.Data;

namespace Akiled.HabboHotel.Users.Messenger
{
    class StaffChat
    {
        internal static MessengerBuddy MessengerStaff;

        internal static void Initialize(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("SELECT username, look, motto FROM users WHERE id = '999'");
            DataRow dRow = dbClient.GetRow();

            if (dRow != null)
            {
                MessengerStaff = new MessengerBuddy(0x7fffffff, "Staff Chat", "hr-831-45.fa-1206-91.sh-290-1331.ha-3129-100.hd-180-2.cc-3039-73.ch-3215-92.lg-270-73", 0);
            }
        }
    }
}

