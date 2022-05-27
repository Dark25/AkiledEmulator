namespace Akiled.HabboHotel.Support
{
    public class ModerationPresetActions
    {
        public int Id;
        public int ParentId;
        public string Type;
        public string Caption;
        public string MessageText;
        public int MuteTime;
        public int BanTime;
        public int IPBanTime;
        public int TradeLockTime;
        public string DefaultSanction;

        public ModerationPresetActions(int id, int parentId, string type, string caption, string messageText, int muteText, int banTime, int ipBanTime, int tradeLockTime, string defaultSanction)
        {
            this.Id = id;
            this.ParentId = parentId;
            this.Type = type;
            this.Caption = caption;
            this.MessageText = messageText;
            this.MuteTime = muteText;
            this.BanTime = banTime;
            this.IPBanTime = ipBanTime;
            this.TradeLockTime = tradeLockTime;
            this.DefaultSanction = defaultSanction;
        }
    }
}
