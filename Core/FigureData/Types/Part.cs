namespace Akiled.Core.FigureData.Types
{
    class Part
    {
        public int Id;
        public SetType SetType;
        public bool Colorable;
        public int Index;
        public int ColorIndex;


        public Part(int id, SetType setType, bool colorable, int index, int colorIndex)
        {
            this.Id = id;
            this.SetType = setType;
            this.Colorable = colorable;
            this.Index = index;
            this.ColorIndex = colorIndex;
        }
    }
}
