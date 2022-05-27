namespace Akiled.HabboHotel.Support
{
    public class HelpTopic
    {
        public int TopicId;
        public string Caption;
        public string Body;
        public int CategoryId;

        public HelpTopic(int Id, string Caption, string Body, int CategoryId)
        {
            this.TopicId = Id;
            this.Caption = Caption;
            this.Body = Body;
            this.CategoryId = CategoryId;
        }
    }
}
