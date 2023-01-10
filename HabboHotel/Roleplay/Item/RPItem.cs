using Akiled.HabboHotel.Roleplay.Item;

namespace Akiled.HabboHotel.Roleplay
{
    public class RPItem
    {
        public int Id;
        public string Name;
        public string Title;
        public string Desc;
        public int Price;
        public string Type;
        public int Value;
        public bool AllowStack;
        public RPItemCategory Category;
        public int UseType;

        public RPItem(int pId, string pName, string pDesc, int pPrice, string pType, int pValue, bool pAllowStack, RPItemCategory pCatagory)
        {
            this.Id = pId;
            this.Name = pName;
            this.Title = pDesc;
            this.Desc = generateDesc(pDesc, pType, pValue);
            this.Price = pPrice;
            this.Type = pType;
            this.Value = pValue;
            this.AllowStack = pAllowStack;
            this.Category = pCatagory;
            this.UseType = getUseType(Type);
        }

        private int getUseType(string Type)
        {
            /*  UseType:
                0: Non utilisable
                1: Utilisable mais limité à 1
                2: Utilisable et possibilité de choisir le nombre à utiliser
            */
            switch (Type)
            {
                case "openguide":
                    return 1;
                case "hit":
                    return 2;
                case "enable":
                    return 1;
                case "showtime":
                    return 1;
                case "money":
                    return 2;
                case "munition":
                    return 2;
                case "energytired":
                    return 2;
                case "healthtired":
                    return 2;
                case "healthenergy":
                    return 2;
                case "energy":
                    return 2;
                case "health":
                    return 2;
                case "weapon_cac":
                    return 1;
                case "weapon_far":
                    return 1;
                default:
                    return 0;
            }
        }

        private string generateDesc(string Desc, string Type, int Value)
        {
            string Text = "<u>" + Desc + "</u></br />";

            switch (Type)
            {
                case "openguide":
                    {
                        Text += "Varias ayudas para comenzar el juego.";
                        break;
                    }
                case "hit":
                    {
                        Text += "Te hace perder " + Value + " vida";
                        break;
                    }
                case "enable":
                    {
                        Text += "Activa un Enable " + Value;
                        break;
                    }
                case "showtime":
                    {
                        Text += "Muestra el tiempo del juego.";
                        break;
                    }
                case "money":
                    {
                        Text += "Agregar " + Value + " Dolares";
                        break;
                    }
                case "munition":
                    {
                        Text += "Agregar " + Value + " municiones";
                        break;
                    }
                case "energytired":
                    {
                        Text += "Aumenta tu energía a " + Value + " pero disminuye tu vida en la misma cantidad.";
                        break;
                    }
                case "healthtired":
                    {
                        Text += "Aumenta tu vida a " + Value + " , pero disminuye tu energía en la misma cantidad.";
                        break;
                    }
                case "healthenergy":
                    {
                        Text += "Aumenta tu vida y energía al " + Value;
                        break;
                    }
                case "energy":
                    {
                        Text += "Aumentar tu energía " + Value;
                        break;
                    }
                case "health":
                    {
                        Text += "Curar " + Value + " puntos de vida";
                        break;
                    }
                case "weapon_cac":
                    {
                        Text += "Arma Cuerpo a Cuerpo (:cac x)";
                        break;
                    }
                case "weapon_far":
                    {
                        Text += "Arma a distancia (:pan)";
                        break;
                    }
                default:
                    {
                        Text += "No utilizable";
                        break;
                    }
            }

            return Text;
        }
    }
}
