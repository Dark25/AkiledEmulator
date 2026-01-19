using Akiled.Core.FigureData.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

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

            // Prefer JSON file if present (FigureData.json). If JSON is missing/incomplete, fall back to XML (figuredata.xml)
            string jsonPath = AkiledEnvironment.PatchDir + "Settings/FigureData.json";
            bool loadedFromJson = false;
            if (File.Exists(jsonPath))
            {
                try
                {
                    var root = JsonNode.Parse(File.ReadAllText(jsonPath));

                    // JSON uses: palettes -> [{ id, colors: [{ id, index, club, selectable(bool), hexCode }] }]
                    var palettesNode = root?["palettes"] as JsonArray;
                    if (palettesNode != null)
                    {
                        foreach (var paletteNode in palettesNode)
                        {
                            if (paletteNode == null) continue;

                            int paletteId = paletteNode["id"]?.GetValue<int>() ?? 0;
                            if (paletteId == 0) continue;

                            if (!this._palettes.ContainsKey(paletteId))
                                this._palettes.Add(paletteId, new Palette(paletteId));

                            if (paletteNode["colors"] is JsonArray colArr)
                            {
                                foreach (var col in colArr)
                                {
                                    if (col == null) continue;

                                    int id = col["id"]?.GetValue<int>() ?? 0;
                                    int index = col["index"]?.GetValue<int>() ?? 0;
                                    int club = col["club"]?.GetValue<int>() ?? 0;
                                    bool selectable = col["selectable"]?.GetValue<bool>() ?? false;
                                    string value = col["hexCode"]?.GetValue<string>()
                                        ?? col["value"]?.GetValue<string>()
                                        ?? string.Empty;

                                    if (id != 0)
                                    {
                                        this._palettes[paletteId].Colors[id] = new Color(id, index, club, selectable, value);
                                    }
                                }
                            }
                        }
                    }

                    // JSON uses: setTypes -> [{ type, paletteId, sets: [{ id, gender, club, colorable, parts: [...] }]}]
                    var setTypesNode = root?["setTypes"] as JsonArray;
                    if (setTypesNode != null)
                    {
                        foreach (var setTypeNode in setTypesNode)
                        {
                            if (setTypeNode == null) continue;

                            string type = setTypeNode["type"]?.GetValue<string>() ?? string.Empty;
                            int paletteId = setTypeNode["paletteId"]?.GetValue<int>()
                                ?? setTypeNode["paletteid"]?.GetValue<int>()
                                ?? 0;
                            if (string.IsNullOrEmpty(type)) continue;

                            if (!this._setTypes.ContainsKey(type))
                                this._setTypes.Add(type, new FigureSet(SetTypeUtility.GetSetType(type), paletteId));

                            if (setTypeNode["sets"] is not JsonArray setArr)
                                continue;

                            foreach (var s in setArr)
                            {
                                if (s == null) continue;

                                int sid = s["id"]?.GetValue<int>() ?? 0;
                                if (sid == 0) continue;

                                string gender = s["gender"]?.GetValue<string>() ?? "U";
                                int club = s["club"]?.GetValue<int>() ?? 0;
                                bool colorable = s["colorable"]?.GetValue<bool>()
                                    ?? ((s["colorable"]?.GetValue<int>() ?? 0) == 1);

                                if (!this._setTypes[type].Sets.ContainsKey(sid))
                                    this._setTypes[type].Sets.Add(sid, new Set(sid, gender, club, colorable));

                                if (s["parts"] is not JsonArray partsArr)
                                    continue;

                                foreach (var p in partsArr)
                                {
                                    if (p == null) continue;
                                    int pid = p["id"]?.GetValue<int>() ?? 0;
                                    string ptype = p["type"]?.GetValue<string>() ?? string.Empty;
                                    if (pid == 0 || string.IsNullOrEmpty(ptype)) continue;

                                    bool pcolorable = p["colorable"]?.GetValue<bool>()
                                        ?? ((p["colorable"]?.GetValue<int>() ?? 0) == 1);
                                    int pindex = p["index"]?.GetValue<int>() ?? 0;
                                    int pcolorindex = p["colorindex"]?.GetValue<int>() ?? 0;

                                    this._setTypes[type].Sets[sid].Parts[pid + "-" + ptype] =
                                        new Part(pid, SetTypeUtility.GetSetType(type), pcolorable, pindex, pcolorindex);
                                }
                            }
                        }
                    }

                loadedFromJson = this._setTypes.Count > 0 && this._palettes.Count > 0;
                if (!this._setTypes.ContainsKey("hd") || !this._setTypes.ContainsKey("lg") || !this._setTypes.ContainsKey("ch"))
                    loadedFromJson = false;
                if (loadedFromJson == false)
                {
                    this._palettes.Clear();
                    this._setTypes.Clear();
                }
                }
                catch
                {
                    this._palettes.Clear();
                    this._setTypes.Clear();
                    loadedFromJson = false;
                }
            }
            if (!loadedFromJson)
            {
                string xmlPath = AkiledEnvironment.PatchDir + "Settings/figuredata.xml";
                if (!File.Exists(xmlPath))
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("FigureData: missing Settings/FigureData.json or it is invalid, and Settings/figuredata.xml was not found.");
                    return;
                }

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(xmlPath);

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
                        if (Child.Attributes?["type"] == null)
                            continue;

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
                                        if (!this._setTypes.TryGetValue(Child.Attributes["type"].Value, out var figureSet))
                                            continue;

                                        int setId = Convert.ToInt32(Sub.Attributes["id"].Value);
                                        if (!figureSet.Sets.TryGetValue(setId, out var set))
                                            continue;

                                        set.Parts.Add(
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
            }

            //Faceless. Ensure the "hd" set type exists before adding a faceless set
            if (!this._setTypes.TryGetValue("hd", out var hdSet))
            {
                hdSet = new FigureSet(SetTypeUtility.GetSetType("hd"), 0);
                this._setTypes["hd"] = hdSet;
            }

            if (!hdSet.Sets.ContainsKey(99999))
                hdSet.Sets.Add(99999, new Set(99999, "U", 0, true));


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
