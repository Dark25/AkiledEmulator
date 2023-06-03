using Akiled.Core.FigureData.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Akiled.Core.FigureData
{
    public class FigureDataManager
    {

        private readonly List<string> _requirements;
        private readonly Dictionary<int, Palette> _palettes; //pallet id, Pallet
        private readonly Dictionary<string, FigureSet> _setTypes; //type (hr, ch, etc), Set

        public FigureDataManager()
        {
            this._palettes = new Dictionary<int, Palette>();
            this._setTypes = new Dictionary<string, FigureSet>();

            this._requirements = new List<string>
            {
                "hd",
                "ch",
                "lg"
            };
        }

        public void Init()
        {

            if (this._palettes.Count > 0) this._palettes.Clear();

            if (this._setTypes.Count > 0) this._setTypes.Clear();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(AkiledEnvironment.PatchDir + "Settings/figuredata.xml");

            XmlNodeList Colors = xDoc.GetElementsByTagName("colors");
            foreach (XmlNode Node in Colors)
            {
                foreach (XmlNode Child in Node.ChildNodes)
                {
                    this._palettes.Add(Convert.ToInt32(Child.Attributes["id"].Value),
                        new Palette(Convert.ToInt32(Child.Attributes["id"].Value)));

                    foreach (XmlNode Sub in Child.ChildNodes)
                    {
                        this._palettes[Convert.ToInt32(Child.Attributes["id"].Value)].Colors.Add(
                            Convert.ToInt32(Sub.Attributes["id"].Value),
                            new Color(Convert.ToInt32(Sub.Attributes["id"].Value),
                                Convert.ToInt32(Sub.Attributes["index"].Value),
                                Convert.ToInt32(Sub.Attributes["club"].Value),
                                Convert.ToInt32(Sub.Attributes["selectable"].Value) == 1,
                                Convert.ToString(Sub.InnerText)));
                    }
                }
            }

            XmlNodeList Sets = xDoc.GetElementsByTagName("sets");
            foreach (XmlNode Node in Sets)
            {
                foreach (XmlNode Child in Node.ChildNodes)
                {
                    this._setTypes.Add(Child.Attributes["type"].Value,
                        new FigureSet(SetTypeUtility.GetSetType(Child.Attributes["type"].Value),
                            Convert.ToInt32(Child.Attributes["paletteid"].Value)));

                    foreach (XmlNode Sub in Child.ChildNodes)
                    {
                        try
                        {
                            this._setTypes[Child.Attributes["type"].Value].Sets.Add(
                                Convert.ToInt32(Sub.Attributes["id"].Value),
                                new Set(Convert.ToInt32(Sub.Attributes["id"].Value),
                                    Convert.ToString(Sub.Attributes["gender"].Value),
                                    Convert.ToInt32(Sub.Attributes["club"].Value),
                                    Convert.ToInt32(Sub.Attributes["colorable"].Value) == 1));
                        }
                        catch
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            //Console.WriteLine("Error 1 anti mutante ID: " + Sub.Attributes["id"].Value);
                        }

                        foreach (XmlNode Subb in Sub.ChildNodes)
                        {
                            if (Subb.Attributes["type"] != null)
                            {
                                try
                                {
                                    this._setTypes[Child.Attributes["type"].Value]
                                        .Sets[Convert.ToInt32(Sub.Attributes["id"].Value)].Parts.Add(
                                            Convert.ToInt32(Subb.Attributes["id"].Value) + "-" +
                                            Subb.Attributes["type"].Value,
                                            new Part(Convert.ToInt32(Subb.Attributes["id"].Value),
                                                SetTypeUtility.GetSetType(Child.Attributes["type"].Value),
                                                Convert.ToInt32(Subb.Attributes["colorable"].Value) == 1,
                                                Convert.ToInt32(Subb.Attributes["index"].Value),
                                                Convert.ToInt32(Subb.Attributes["colorindex"].Value)));
                                }
                                catch
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                    // Console.WriteLine("Error 2 anti mutante ID: " + Sub.Attributes["id"].Value);
                                }
                            }
                        }
                    }
                }
            }

            //Faceless.
            this._setTypes["hd"].Sets.Add(99999, new Set(99999, "U", 0, true));


        }

        public string ProcessFigure(string figure, string gender, bool hasHabboClub)
        {
            try
            {

                figure = figure.ToLower();
                gender = gender.ToUpper();

                string rebuildFigure = string.Empty;

                #region Check clothing, colors & Habbo Club
                string[] figureParts = figure.Split('.');
                foreach (string part in figureParts.ToList())
                {
                    if (!part.Contains('-'))
                        continue;
                    string type = part.Split('-')[0];

                    FigureSet figureSet = null;
                    if (this._setTypes.TryGetValue(type, out figureSet))
                    {
                        string[] splitpart = part.Split('-');
                        if (splitpart.Length < 2)
                            continue;

                        int partId = Convert.ToInt32(splitpart[1]);
                        int colorId = 0;
                        int secondColorId = 0;

                        Set set = null;
                        if (figureSet.Sets.TryGetValue(partId, out set))
                        {
                            #region Gender Check
                            if (set.Gender != gender && set.Gender != "U")
                            {
                                if (figureSet.Sets.Count(x => x.Value.Gender == gender || x.Value.Gender == "U") > 0)
                                {
                                    partId = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U").Value.Id;

                                    //Fetch the new set.
                                    figureSet.Sets.TryGetValue(partId, out set);

                                    colorId = GetRandomColor(figureSet.PalletId);
                                }
                                else
                                {
                                    //No replacable?
                                }
                            }
                            #endregion

                            #region Colors
                            if (set.Colorable)
                            {
                                //Couldn't think of a better way to split the colors, if I looped the parts I still have to remove Type-PartId, then loop color 1 & color 2. Meh

                                int splitterCounter = part.Count(x => x == '-');
                                if (splitterCounter == 2 || splitterCounter == 3)
                                {
                                    #region First Color
                                    if (!string.IsNullOrEmpty(part.Split('-')[2]))
                                    {
                                        if (int.TryParse(part.Split('-')[2], out colorId))
                                        {
                                            colorId = Convert.ToInt32(part.Split('-')[2]);

                                            Palette palette = GetPalette(colorId);
                                            if (palette != null && colorId != 0)
                                            {
                                                if (figureSet.PalletId != palette.Id)
                                                {
                                                    colorId = GetRandomColor(figureSet.PalletId);
                                                }
                                            }
                                            else if (palette == null && colorId != 0)
                                            {
                                                colorId = GetRandomColor(figureSet.PalletId);
                                            }
                                        }
                                        else
                                            colorId = 0;
                                    }
                                    else
                                        colorId = 0;
                                    #endregion
                                }

                                if (splitterCounter == 3)
                                {
                                    #region Second Color
                                    if (!string.IsNullOrEmpty(part.Split('-')[3]))
                                    {
                                        if (int.TryParse(part.Split('-')[3], out secondColorId))
                                        {
                                            secondColorId = Convert.ToInt32(part.Split('-')[3]);

                                            Palette palette = GetPalette(secondColorId);
                                            if (palette != null && secondColorId != 0)
                                            {
                                                if (figureSet.PalletId != palette.Id)
                                                {
                                                    secondColorId = GetRandomColor(figureSet.PalletId);
                                                }
                                            }
                                            else if (palette == null && secondColorId != 0)
                                            {
                                                secondColorId = GetRandomColor(figureSet.PalletId);
                                            }
                                        }
                                        else
                                            secondColorId = 0;
                                    }
                                    else
                                        secondColorId = 0;
                                    #endregion
                                }
                            }
                            else
                            {
                                string[] ignore = new string[] { "ca", "wa" };

                                if (ignore.Contains(type))
                                {
                                    int splitterCounter = part.Count(x => x == '-');
                                    if (splitterCounter > 1)
                                    {
                                        if (!string.IsNullOrEmpty(part.Split('-')[2]))
                                        {
                                            colorId = Convert.ToInt32(part.Split('-')[2]);
                                        }
                                    }
                                }
                            }
                            #endregion

                            if (set.ClubLevel > 0 && !hasHabboClub)
                            {
                                partId = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U" && x.Value.ClubLevel == 0).Value.Id;

                                figureSet.Sets.TryGetValue(partId, out set);

                                colorId = GetRandomColor(figureSet.PalletId);
                            }

                            if (secondColorId == 0)
                                rebuildFigure = rebuildFigure + type + "-" + partId + "-" + colorId + ".";
                            else
                                rebuildFigure = rebuildFigure + type + "-" + partId + "-" + colorId + "-" + secondColorId + ".";
                        }
                    }
                }
                #endregion

                #region Check Required Clothing
                foreach (string requirement in this._requirements)
                {
                    if (!rebuildFigure.Contains(requirement))
                    {
                        if (requirement == "ch" && gender == "M")
                            continue;

                        FigureSet figureSet = null;
                        if (this._setTypes.TryGetValue(requirement, out figureSet))
                        {
                            Set set = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U").Value;
                            if (set != null)
                            {
                                int partId = figureSet.Sets.FirstOrDefault(x => x.Value.Gender == gender || x.Value.Gender == "U").Value.Id;
                                int colorId = GetRandomColor(figureSet.PalletId);

                                rebuildFigure = rebuildFigure + requirement + "-" + partId + "-" + colorId + ".";
                            }
                        }
                    }
                }
                #endregion

                return rebuildFigure.Remove(rebuildFigure.Length - 1);
            }
            catch { }
            return "hd-180-1.lg-270-1408";
        }

        public Palette GetPalette(int colorId) => this._palettes.FirstOrDefault(x => x.Value.Colors.ContainsKey(colorId)).Value;

        public bool TryGetPalette(int palletId, out Palette palette) => this._palettes.TryGetValue(palletId, out palette);

        public int GetRandomColor(int palletId) => this._palettes[palletId].Colors.FirstOrDefault().Value.Id;
    }
}
