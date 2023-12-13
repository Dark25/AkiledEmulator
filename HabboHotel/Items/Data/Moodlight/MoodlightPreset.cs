namespace Akiled.HabboHotel.Items
{
    public class MoodlightPreset
    {
        public string ColorCode { get; set; }
        public int ColorIntensity { get; set; }
        public bool BackgroundOnly { get; set; }

        public MoodlightPreset(string ColorCode, int ColorIntensity, bool BackgroundOnly)
        {
            this.ColorCode = ColorCode;
            this.ColorIntensity = ColorIntensity;
            this.BackgroundOnly = BackgroundOnly;
        }
    }
}
