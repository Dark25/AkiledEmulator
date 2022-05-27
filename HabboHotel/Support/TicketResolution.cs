namespace Akiled.HabboHotel.Support
{
    public class TicketResolution
    {
        public string Titre;
        public string Soustitre;
        public int Ban_hours;
        public int Enablemute;
        public int Mute_hours;
        public int Reminder;
        public string Message;

        public TicketResolution(string titre, string soustitre, int ban_hours, int enablemute, int mute_hours, int reminder, string message)
        {
            this.Titre = titre;
            this.Soustitre = soustitre;
            this.Ban_hours = ban_hours;
            this.Enablemute = enablemute;
            this.Mute_hours = mute_hours;
            this.Reminder = reminder;
            this.Message = message;
        }
    }
}