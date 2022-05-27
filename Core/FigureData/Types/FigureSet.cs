using System.Collections.Generic;

namespace Akiled.Core.FigureData.Types
{
    class FigureSet
    {
        public SetType Type;
        public int PalletId;

        private Dictionary<int, Set> _sets;

        public FigureSet(SetType type, int palletId)
        {
            this.Type = type;
            this.PalletId = palletId;

            this._sets = new Dictionary<int, Set>();
        }

        public Dictionary<int, Set> Sets
        {
            get { return this._sets; }
            set { this._sets = value; }
        }
    }
}