namespace Akiled.Core.FigureData.Types
{
    public class Color
    {
        public int Id;
        public int Index;
        public int ClubLevel;
        public bool Selectable;
        public string Value;

        public Color(int id, int index, int clubLevel, bool selectable, string value)
        {
            this.Id = id;
            this.Index = index;
            this.ClubLevel = clubLevel;
            this.Selectable = selectable;
            this.Value = value;
        }
    }
}
