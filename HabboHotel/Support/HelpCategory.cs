namespace Akiled.HabboHotel.Support
{
    public class HelpCategory
    {
        public int CategoryId;
        public string Caption;

        public HelpCategory(int Id, string Caption)
        {
            this.CategoryId = Id;
            this.Caption = Caption;
        }
    }
}
