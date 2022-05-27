using System.Collections.Generic;

namespace Akiled.Core.FigureData.Types
{
    class Set
    {
        public int Id;
        public string Gender;
        public int ClubLevel;
        public bool Colorable;

        private Dictionary<string, Part> _parts;

        public Set(int id, string gender, int clubLevel, bool colorable)
        {
            this.Id = id;
            this.Gender = gender;
            this.ClubLevel = clubLevel;
            this.Colorable = colorable;

            this._parts = new Dictionary<string, Part>();
        }

        public Dictionary<string, Part> Parts
        {
            get { return this._parts; }
            set { this._parts = value; }
        }
    }
}
