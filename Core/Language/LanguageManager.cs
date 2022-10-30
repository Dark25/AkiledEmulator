using Akiled.Database.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Akiled.Core
{
    public enum Language
    {
        NONE,
        FRANCAIS,
        ANGLAIS,
        PORTUGAIS,
        SPANISH,
    }

    public class LanguageManager
    {
        private Dictionary<string, string> _valuesFr;
        private Dictionary<string, string> _valuesEn;
        private Dictionary<string, string> _valuesBr;
        private Dictionary<string, string> _valuesEs;

        public void Init()
        {
            this._valuesFr = new Dictionary<string, string>();
            this._valuesEn = new Dictionary<string, string>();
            this._valuesBr = new Dictionary<string, string>();
            this._valuesEs = new Dictionary<string, string>();
            this.InitLocalValues();
        }

        public void InitLocalValues()
        {
            this._valuesFr.Clear();
            this._valuesEn.Clear();
            this._valuesBr.Clear();

            DataTable table;
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM system_locale");
                table = dbClient.GetTable();
            }
            if (table == null)
                return;

            foreach (DataRow dataRow in table.Rows)
            {
                string key = (string)dataRow["identifiant"];
                string value_fr = (string)dataRow["value_fr"];
                string value_en = (string)dataRow["value_en"];
                string value_br = (string)dataRow["value_br"];
                string value_es = (string)dataRow["value_es"];
                this._valuesFr.Add(key, value_fr);
                this._valuesEn.Add(key, value_en);
                this._valuesBr.Add(key, value_br);
                this._valuesEs.Add(key, value_es);
            }
        }

        public string TryGetValue(string value, Language Language)
        {
            if (Language == Language.FRANCAIS)
                return this._valuesFr.ContainsKey(value) ? this._valuesFr[value] : "No se ha encontrado ningún idioma local para[" + value + "] (fr)";
            else if (Language == Language.ANGLAIS)
                return this._valuesEn.ContainsKey(value) ? this._valuesEn[value] : "No se ha encontrado ningún idioma local para[" + value + "] (en)";
            else if (Language == Language.PORTUGAIS)
                return this._valuesBr.ContainsKey(value) ? this._valuesBr[value] : "No se ha encontrado ningún idioma local para[" + value + "] (br)";
            else if (Language == Language.SPANISH)
                return this._valuesEs.ContainsKey(value) ? this._valuesEs[value] : "No se ha encontrado ningún idioma local para[" + value + "] (es)";
            else
                return this._valuesFr.ContainsKey(value) ? this._valuesFr[value] : "No se ha encontrado ningún idioma local para[" + value + "] (def)";
        }

        public static Language ParseLanguage(string v)
        {
            switch (v)
            {
                case "fr":
                    return Language.FRANCAIS;
                case "en":
                    return Language.ANGLAIS;
                case "br":
                    return Language.PORTUGAIS;
                case "es":
                    return Language.SPANISH;
                default:
                    return Language.FRANCAIS;
            }
        }
    }
}