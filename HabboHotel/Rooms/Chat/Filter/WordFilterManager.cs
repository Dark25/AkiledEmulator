using Akiled.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Akiled.HabboHotel.Rooms.Chat.Filter
{
    public sealed class WordFilterManager
    {
        private List<string> _filteredWords;
        private List<string> _pubWords;

        public WordFilterManager()
        {
            this._filteredWords = new List<string>();
            this._pubWords = new List<string>();
        }

        public void Init()
        {
            if (this._filteredWords.Count > 0) this._filteredWords.Clear();
            if (this._pubWords.Count > 0) this._pubWords.Clear();

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_swearword_filter`");
                DataTable Data = dbClient.GetTable();

                if (Data != null)
                {
                    foreach (DataRow Row in Data.Rows)
                    {
                        this._filteredWords.Add(Convert.ToString(Row["word"]));
                    }
                }

                dbClient.SetQuery("SELECT * FROM `worldfilterpub`");
                DataTable Data2 = dbClient.GetTable();

                if (Data2 != null)
                {
                    foreach (DataRow Row in Data2.Rows)
                    {
                        this._pubWords.Add(Convert.ToString(Row["word"]));
                    }
                }
            }
        }

        public void AddFilterPub(string Word)
        {
            if (!this._pubWords.Contains(Word))
            {
                this._pubWords.Add(Word);

                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("INSERT INTO worldfilterpub (word) VALUES (@word)");
                    queryreactor.AddParameter("word", Word);
                    queryreactor.RunQuery();
                }
            }
        }

        public string CheckMessage(string Message)
        {
            foreach (string Filter in this._filteredWords.ToList())
            {
                if (Message.ToLower().Contains(Filter))
                {
                    Message = Regex.Replace(Message, Filter, "****", RegexOptions.IgnoreCase);
                }
            }

            return Message;
        }

        private string StringTranslate(ref string input, string frm, char to)
        {
            for (int i = 0; i < frm.Length; i++)
            {
                input = input.Replace(frm[i], to);
            }
            return input;
        }

        private void ClearMessage(ref string message, bool OnlyLetter = true)
        {
            message = message.Replace("()", "o").Replace("Æ", "ae");

            StringTranslate(ref message, "ĀāĂăĄąΑΔΆÀÁÂÃÄÅàáâãäå4@å", 'a');
            StringTranslate(ref message, "ßΒþ", 'b');
            StringTranslate(ref message, "¢©ĆćĈĉĊċČč€Çç", 'c');
            StringTranslate(ref message, "ĎďĐđ", 'd');
            StringTranslate(ref message, "ĒēĔĕĖėĘęĚěΈÈÉÊËèéêë3", 'e');
            StringTranslate(ref message, "ĜĝĞğĠġĢģ", 'g');
            StringTranslate(ref message, "ĤĥĦħΉ", 'h');
            StringTranslate(ref message, "¡ĨĩĪīĬĭĮįİıΊΐìíîïÌÍÎÏ1!", 'i');
            StringTranslate(ref message, "Ĵĵ", 'j');
            StringTranslate(ref message, "Ķķĸ", 'k');
            StringTranslate(ref message, "£¦ĹĺĻļĽľĿŀŁłℓ", 'l');
            StringTranslate(ref message, "M", 'm');
            StringTranslate(ref message, "ŃńŅņŇňŉŊŋπñÑ", 'n');
            StringTranslate(ref message, "¤ŌōŎŏŐőΌoOòóôõöÒÓÔÕÖøΩð0", 'o');
            StringTranslate(ref message, "Pp₱", 'p');
            StringTranslate(ref message, "ŔŕŖŗŘřя®", 'r');
            StringTranslate(ref message, "§ŚśŜŝŞşSŠš", 's');
            StringTranslate(ref message, "ŢţŤťŦŧ", 't');
            StringTranslate(ref message, "ųŨũŪūŬŭŮůŰűŲųùúûüÙÚÛÜ", 'u');
            StringTranslate(ref message, "√", 'v');
            StringTranslate(ref message, "Ŵŵω", 'w');
            StringTranslate(ref message, "×", 'x');
            StringTranslate(ref message, "ŶŷΎýÿÝÝ", 'y');
            StringTranslate(ref message, "ŹźŻż", 'z');

            if (OnlyLetter)
                message = new Regex(@"[^a-z]", RegexOptions.IgnoreCase).Replace(message, string.Empty);

            message = message.ToLower();
        }

        public bool Ispub(string message)
        {
            if (message.Length <= 3)
                return false;

            this.ClearMessage(ref message);

            foreach (string pattern in this._pubWords)
                if (message.Contains(pattern))
                    return true;

            return false;
        }

        public bool CheckMessageWord(string message)
        {
            int OldLength = message.Replace(" ", "").Length;

            this.ClearMessage(ref message, false);

            int LetterDelCount = OldLength - message.Length;

            List<string> WordPub = new List<string>() { "go",
                                                        ".fr",
                                                        ".com",
                                                        ".me",
                                                        ".org",
                                                        ".be",
                                                        ".eu",
                                                        ".net",
                                                        "mobi",
                                                        "nouveau",
                                                        "nouvo",
                                                        "connect",
                                                        "invite",
                                                        "recru",
                                                        "staff",
                                                        "ouvr",
                                                        "rejoign",
                                                        "retro",
                                                        "meilleur",
                                                        "direction",
                                                        "rejoin",
                                                        "gratuit",
                                                        "open",
                                                        "http",
                                                        "recrutement",
                                                        "animation",
                                                        "fps",
                                                        "new",
                                                        "habb",
                                                        "bbo",
                                                        "sansle",
                                                        "city",
                                                        "alpha",
                                                        "hevvo",
                                                        "bibbop",
                                                        "habba",
                                                        "habbol",
                                                        "habboin",
                                                        "haddoz",
                                                        "habboid",
                                                        "hlat",
                                                        "libbo",
                                                        "hekos",
                                                        "hhd2",
                                                        "h22",
                                                        "jadoz",
                                                        "jadozz",
                                                        "javvo",
                                                        "habpvp",
                                                        };


            int CountDetect = 0;
            foreach (string pattern in WordPub)
                if (message.Contains(pattern))
                {
                    CountDetect++;
                    continue;
                }

            if (CountDetect >= 4 || (LetterDelCount > 5 && CountDetect >= 4))
                return true;

            return false;
        }
    }
}