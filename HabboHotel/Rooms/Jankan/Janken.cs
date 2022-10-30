namespace Akiled.HabboHotel.Rooms.Janken
{
    public class Janken
    {
        public int UserOne;
        public int UserTwo;

        public JankenEnum ChoixOne;
        public JankenEnum ChoixTwo;

        public bool Started;
        public int Timer;

        public Janken(int userid, int dueluserid)
        {
            this.UserOne = userid;
            this.UserTwo = dueluserid;

            this.ChoixOne = JankenEnum.None;
            this.ChoixTwo = JankenEnum.None;

            this.Started = false;
            this.Timer = 0;
        }
    }
}