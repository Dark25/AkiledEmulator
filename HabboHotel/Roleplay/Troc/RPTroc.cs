namespace Akiled.HabboHotel.Roleplay.Troc
{
    public class RPTroc
    {
        public RPTrocUser UserOne;
        public RPTrocUser UserTwo;

        public int Id;
        public int RPId;

        public RPTroc(int pId, int pRPId, int pUserOne, int pUserTwo)
        {
            this.Id = pId;
            this.RPId = pRPId;
            this.UserOne = new RPTrocUser(pUserOne);
            this.UserTwo = new RPTrocUser(pUserTwo);
        }

        public RPTrocUser GetUser(int UserId)
        {
            if (this.UserOne.UserId == UserId)
                return UserOne;
            else if (this.UserTwo.UserId == UserId)
                return UserTwo;

            return null;
        }

        public void ResetConfirmed()
        {
            this.UserOne.Confirmed = false;
            this.UserTwo.Confirmed = false;
        }

        public bool AllAccepted
        {
            get
            {
                return UserOne.Accepted == true && UserTwo.Accepted == true;
            }
        }

        public bool AllConfirmed
        {
            get
            {
                return UserOne.Confirmed == true && UserTwo.Confirmed == true;
            }
        }
    }
}
