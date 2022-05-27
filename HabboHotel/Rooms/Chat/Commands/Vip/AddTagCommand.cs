using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class AddTagCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                string str1 = Session.GetHabbo().Prefix.Split(';')[1];
                if (string.IsNullOrEmpty(str1))
                    str1 = "000000";
                if (Params.Length != 2)
                {
                    Session.SendWhisper("Debes usar el comando + el tag que deseas. (ahora estas sin prefijo)", 34);
                    queryReactor.RunQuery("UPDATE `users` SET `prefix` = ';' WHERE `id` = '" + Session.GetHabbo().Id.ToString() + "'");
                    Session.GetHabbo().Prefix = ";";
                }
                else
                {
                    string str2 = Params[1].ToString() + ";" + str1;
                    if (Session.GetHabbo().Rank <= 2)
                    {
                        if (str2 == "Admin" || str2 == "Admin" || (str2 == "Dueño" || str2 == "Dueño") || (str2 == "Dueñ0" || str2 == "Dueñ0" || (str2 == "Founder" || str2 == "Founder")) || (str2 == "Fundador" || str2 == "Fundador" || (str2 == "Fundad0r" || str2 == "Fundad0r") || (str2 == "staffs" || str2 == "staffs" || (str2 == "staff's" || str2 == "staff's"))) || (str2 == "moderador" || str2 == "moderador" || (str2 == "moderad0r" || str2 == "moderad0r") || (str2 == "embajador" || str2 == "embajador" || (str2 == "embajad0r" || str2 == "embajad0r")) || (str2 == "GM" || str2 == "GM" || (str2 == "gamemaster" || str2 == "gamemaster") || (str2 == "ayudante" || str2 == "ayudante" || (str2 == "publi" || str2 == "publi")))) || (str2 == "publicista" || str2 == "publicista" || (str2 == "M0d" || str2 == "Mod") || (str2 == "C30" || str2 == "C3o" || (str2 == "Ce0" || str2 == "Col")) || (str2 == "Ayu" || str2 == "Emb" || (str2 == "Pub" || str2 == "Staff") || (str2 == "Founder" || str2 == "Adm" || (str2 == "Ceo" || str2 == "[Founder]"))) || (str2 == "[F0under]" || str2 == "[Found3r]" || (str2 == "[F0und3r]" || str2 == "[OWNER]") || (str2 == "[C0L]" || str2 == "[0WNER]" || (str2 == "[OWN3R]" || str2 == "[Own3r]")) || (str2 == "[Adm]" || str2 == "[Adn]" || (str2 == "[Admin]" || str2 == "[St4ff]") || (str2 == "[Staff]" || str2 == "[MOD]" || (str2 == "[Ceo]" || str2 == "[M0d]"))))) || (str2 == "[Col]" || str2 == "[Ayu]" || (str2 == "[Emb]" || str2 == "[Pub]") || (str2 == "(Founder)" || str2 == "(F0under)" || (str2 == "(Found3r)" || str2 == "(F0und3r)")) || (str2 == "(Owner)" || str2 == "(C0l)" || (str2 == "(0wner)" || str2 == "(Own3r)") || (str2 == "(Own3r)" || str2 == "(Adm)" || (str2 == "(Adn)" || str2 == "(Admin)"))) || (str2 == "(St4ff)" || str2 == "(Staff)" || (str2 == "(Mod)" || str2 == "(Ceo)") || (str2 == "(M0d)" || str2 == "(Col)" || (str2 == "(Ayu)" || str2 == "(Emb)")) || (str2 == "(Pub)" || str2 == "{Founder}" || (str2 == "{F0under}" || str2 == "{Found3r}") || (str2 == "{F0und3r}" || str2 == "{Owner}" || (str2 == "{C0l}" || str2 == "{0wner}")))) || (str2 == "{Own3r}" || str2 == "{Own3r}" || (str2 == "{Adm}" || str2 == "{Adn}") || (str2 == "{Admin}" || str2 == "{St4ff}" || (str2 == "{Staff}" || str2 == "{Mod}")) || (str2 == "{Ceo}" || str2 == "{M0d}" || (str2 == "{Col}" || str2 == "{Ayu}") || (str2 == "{Emb}" || str2 == "{Pub}" || (str2 == "<Founder>" || str2 == "<F0under>"))) || (str2 == "<Found3r>" || str2 == "<F0und3r>" || (str2 == "<Owner>" || str2 == "<C0l>") || (str2 == "<0wner>" || str2 == "<Own3r>" || (str2 == "<Own3r>" || str2 == "<Adm>")) || (str2 == "<Adn>" || str2 == "<Admin>" || (str2 == "<St4ff>" || str2 == "<Staff>") || (str2 == "<Mod>" || str2 == "<Ceo>" || (str2 == "<M0d>" || str2 == "<Col>")))))) || (str2 == "<Ayu>" || str2 == "<Emb>" || (str2 == "<Pub>" || str2 == "M0D") || (str2 == "MOD" || str2 == "C30" || (str2 == "C3O" || str2 == "CE0")) || (str2 == "COL" || str2 == "AYU" || (str2 == "EMB" || str2 == "PUB") || (str2 == "STAFF" || str2 == "FOUNDER" || (str2 == "ADM" || str2 == "CEO"))) || (str2 == "[FOUNDER]" || str2 == "[F0UNDER]" || (str2 == "[FOUND3R]" || str2 == "[F0UND3R]") || (str2 == "[OWNER]" || str2 == "[C0L]" || (str2 == "[0WNER]" || str2 == "[OWN3R]")) || (str2 == "[OWN3R]" || str2 == "[ADM]" || (str2 == "[ADN]" || str2 == "[ADMIN]") || (str2 == "[ST4FF]" || str2 == "[STAFF]" || (str2 == "[MOD]" || str2 == "[CEO]")))) || (str2 == "[M0D]" || str2 == "[COL]" || (str2 == "[AYU]" || str2 == "[EMB]") || (str2 == "[PUB]" || str2 == "(FOUNDER)" || (str2 == "(F0UNDER)" || str2 == "(FOUND3R)")) || (str2 == "(F0UND3R)" || str2 == "(OWNER)" || (str2 == "(C0L)" || str2 == "(0WNER)") || (str2 == "(OWN3R)" || str2 == "(OWN3R)" || (str2 == "(ADM)" || str2 == "(ADN)"))) || (str2 == "(ADMIN)" || str2 == "(ST4FF)" || (str2 == "(STAFF)" || str2 == "(MOD)") || (str2 == "(CEO)" || str2 == "(M0D)" || (str2 == "(COL)" || str2 == "(AYU)")) || (str2 == "(EMB)" || str2 == "(PUB)" || (str2 == "{FOUNDER}" || str2 == "{F0UNDER}") || (str2 == "{FOUND3R}" || str2 == "{F0UND3R}" || (str2 == "{OWNER}" || str2 == "{C0L}"))))) || (str2 == "{0WNER}" || str2 == "{OWN3R}" || (str2 == "{OWN3R}" || str2 == "{ADM}") || (str2 == "{ADN}" || str2 == "{ADMIN}" || (str2 == "{ST4FF}" || str2 == "{STAFF}")) || (str2 == "{MOD}" || str2 == "{CEO}" || (str2 == "{M0D}" || str2 == "{COL}") || (str2 == "{AYU}" || str2 == "{EMB}" || (str2 == "{PUB}" || str2 == "<FOUNDER>"))) || (str2 == "<F0UNDER>" || str2 == "<FOUND3R>" || (str2 == "<F0UND3R>" || str2 == "<OWNER>") || (str2 == "<C0L>" || str2 == "<0WNER>" || (str2 == "<OWN3R>" || str2 == "<OWN3R>")) || (str2 == "<ADM>" || str2 == "<ADN>" || (str2 == "<ADMIN>" || str2 == "<ST4FF>") || (str2 == "<STAFF>" || str2 == "<MOD>" || (str2 == "<CEO>" || str2 == "<M0D>")))) || (str2 == "<COL>" || str2 == "<AYU>" || (str2 == "<EMB>" || str2 == "<PUB>") || (str2 == "m0d" || str2 == "mod" || (str2 == "c30" || str2 == "c3o")) || (str2 == "ce0" || str2 == "col" || (str2 == "ayu" || str2 == "emb") || (str2 == "pub" || str2 == "staff" || (str2 == "founder" || str2 == "adm"))) || (str2 == "ceo" || str2 == "[founder]" || (str2 == "[f0under]" || str2 == "[found3r]") || (str2 == "[f0und3r]" || str2 == "[owner]" || (str2 == "[c0l]" || str2 == "[0wner]")) || (str2 == "[own3r]" || str2 == "[own3r]" || (str2 == "[adm]" || str2 == "[adn]") || (str2 == "[admin]" || str2 == "[st4ff]" || (str2 == "[staff]" || str2 == "[mod]"))))))) || (str2 == "[ceo]" || str2 == "[m0d]" || (str2 == "[col]" || str2 == "[ayu]") || (str2 == "[emb]" || str2 == "[pub]" || (str2 == "(founder)" || str2 == "(f0under)")) || (str2 == "(found3r)" || str2 == "(f0und3r)" || (str2 == "(owner)" || str2 == "(c0l)") || (str2 == "(0wner)" || str2 == "(own3r)" || (str2 == "(own3r)" || str2 == "(adm)"))) || (str2 == "(adn)" || str2 == "(admin)" || (str2 == "(st4ff)" || str2 == "(staff)") || (str2 == "(mod)" || str2 == "(ceo)" || (str2 == "(m0d)" || str2 == "(col)")) || (str2 == "(ayu)" || str2 == "(emb)" || (str2 == "(pub)" || str2 == "{founder}") || (str2 == "{f0under}" || str2 == "{found3r}" || (str2 == "{f0und3r}" || str2 == "{owner}")))) || (str2 == "{c0l}" || str2 == "{0wner}" || (str2 == "{own3r}" || str2 == "{own3r}") || (str2 == "{adm}" || str2 == "{adn}" || (str2 == "{admin}" || str2 == "{st4ff}")) || (str2 == "{staff}" || str2 == "{mod}" || (str2 == "{ceo}" || str2 == "{m0d}") || (str2 == "{col}" || str2 == "{ayu}" || (str2 == "{emb}" || str2 == "{pub}"))) || (str2 == "<founder>" || str2 == "<f0under>" || (str2 == "<found3r>" || str2 == "<f0und3r>") || (str2 == "<owner>" || str2 == "<c0l>" || (str2 == "<0wner>" || str2 == "<own3r>")) || (str2 == "<own3r>" || str2 == "<adm>" || (str2 == "<adn>" || str2 == "<admin>") || (str2 == "<st4ff>" || str2 == "<staff>" || (str2 == "<mod>" || str2 == "<ceo>"))))) || (str2 == "<m0d>" || str2 == "<col>" || (str2 == "<ayu>" || str2 == "<emb>"))) || str2 == "<pub>")
                            Session.SendWhisper("Error en la creación de su tag, alparecer contiene palabras prohibidas o no cumple el rango mínimo para usarla.", 3);
                        if (str2.Length > 12)
                        {
                            Session.SendWhisper("Solo esta permitido 12 caracteres, numeros y letras.", 3);
                        }
                        else
                        {
                            queryReactor.RunQuery("UPDATE `users` SET `prefix` = '" + str2 + "' WHERE `id` = '" + Session.GetHabbo().Id.ToString() + "'");
                            Session.SendWhisper("Felicidades, usted se ha asignado un nuevo tag (prefijo)", 33);
                            Session.GetHabbo().Prefix = str2;
                        }
                    }
                    else
                    {
                        queryReactor.RunQuery("UPDATE `users` SET `prefix` = '" + str2 + "' WHERE `id` = '" + Session.GetHabbo().Id.ToString() + "'");
                        Session.SendWhisper("Felicidades, usted se ha asignado un nuevo tag (prefijo)", 33);
                        Session.GetHabbo().Prefix = str2;
                    }
                }
            }
        }
    }
}
