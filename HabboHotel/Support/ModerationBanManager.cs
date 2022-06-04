using Akiled.HabboHotel.GameClients;
using Akiled.Database.Interfaces;
using System;
using JNogueira.Discord.Webhook.Client;
using System.Threading.Tasks;

namespace Akiled.HabboHotel.Support
{
    public class ModerationBanManager
    {
        public async Task BanUserAsync(GameClient Client, string Moderator, double LengthSeconds, string Reason, bool IpBan, bool MachineBan)
        {
            if (string.IsNullOrEmpty(Reason))
                Reason = "Ne respect pas les régles";

            string Variable = Client.GetHabbo().Username.ToLower();
            string typeBan = "user";
            double Expire = (double)AkiledEnvironment.GetUnixTimestamp() + LengthSeconds;
            if (IpBan)
            {
                //Variable = Client.GetConnection().getIp();
                Variable = Client.GetHabbo().IP;
                typeBan = "ip";
            }

            if (MachineBan)
            {
                Variable = Client.MachineId;
                typeBan = "machine";
            }

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("INSERT INTO bans (bantype,value,reason,expire,added_by,added_date) VALUES (@rawvar, @var, @reason, '" + Expire + "', @mod, UNIX_TIMESTAMP())");
                queryreactor.AddParameter("rawvar", typeBan);
                queryreactor.AddParameter("var", Variable);
                queryreactor.AddParameter("reason", Reason);
                queryreactor.AddParameter("mod", Moderator);
                queryreactor.RunQuery();
            }
            if (MachineBan)
            {
                await this.BanUserAsync(Client, Moderator, LengthSeconds, Reason, true, false);
            }
            else if (IpBan)
            {
                await this.BanUserAsync(Client, Moderator, LengthSeconds, Reason, false, false);
            }
            else
            {
                Client.Disconnect();
                
                string Webhook = AkiledEnvironment.GetConfig().data["Webhook"];
                string Webhook_bans_ProfilePicture = AkiledEnvironment.GetConfig().data["Webhook_bans_Image"];
                string Webhook_bans_UserNameD = AkiledEnvironment.GetConfig().data["Webhook_bans_Username"];
                string Webhook_bans_WebHookurl = AkiledEnvironment.GetConfig().data["Webhook_bans_URL"];

                if (Webhook == "true")
                {

                    var client = new DiscordWebhookClient(Webhook_bans_WebHookurl);

                    var message = new DiscordMessage(
                     "La Seguridad es importante para nosotros! " + DiscordEmoji.Grinning,
                        username: Webhook_bans_UserNameD,
                        avatarUrl: Webhook_bans_ProfilePicture,
                        tts: false,
                        embeds: new[]
            {
                                new DiscordMessageEmbed(
                                "Notificacion de ban por mootools" + DiscordEmoji.Thumbsup,
                                 color: 0,
                                author: new DiscordMessageEmbedAuthor(Client.GetHabbo().Username),
                                description: "Informacion del ban",
                                fields: new[]
                                {
                                    new DiscordMessageEmbedField("Razón", Reason, true),
                                    new DiscordMessageEmbedField("Duración", LengthSeconds.ToString() + " segundos", true),
                                    new DiscordMessageEmbedField("Moderador", Moderator, true),
                                    new DiscordMessageEmbedField("Fecha", DateTime.Now.ToString(), true),
                                },
                                thumbnail: new DiscordMessageEmbedThumbnail("https://hrecu.site/habbo-imaging/avatar/" + Client.GetHabbo().Look),
                                footer: new DiscordMessageEmbedFooter("Creado por: "+Webhook_bans_UserNameD, Webhook_bans_ProfilePicture)
        )
            }
            );
                    await client.SendToDiscord(message);
                    
                    Console.WriteLine("Ban enviado a Discord ", ConsoleColor.DarkCyan);

                }

            }
        }
    }
}
