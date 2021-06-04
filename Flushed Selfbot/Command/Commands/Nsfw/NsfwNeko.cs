using System.Drawing;
using System.Net;
using Discord;
using Newtonsoft.Json.Linq;

namespace FlushedSelfbot.Command.Commands.Nsfw
{
    public class NsfwNeko : Command
    {
        public NsfwNeko() : base("NsfwNeko", "Sends a random NSFW neko", null, Category.Nsfw)
        {
            
        }

        public override void Execute()
        {
            var embedMaker = new EmbedMaker
            {
                Title = "Failed to send NSFW neko",
                Color = Color.FromArgb(0xFF0000),
                Footer = new EmbedFooter
                {
                    Text = Bot.Name,
                    IconUrl = Bot.FlushedUrl
                }
            };

            using (var client = new WebClient())
            {
                var json = client.DownloadString("https://api.nekos.dev/api/v3/images/nsfw/img/neko_lewd/");
                embedMaker.Title = "NSFW Neko";
                embedMaker.ImageUrl = (string) ((JValue) JObject.Parse(json).SelectToken("data")
                    ?.SelectToken("response")?
                    .SelectToken("url"));
                embedMaker.Color = Color.FromArgb(0x00FFAA);
            }

            Message.Edit(Bot.ConfigManager.DisableEmbeds
                ? new MessageEditProperties {Content = embedMaker.ImageUrl ?? embedMaker.Title}
                : new MessageEditProperties {Content = "", Embed = embedMaker});
        }
    }
}