using System.Drawing;
using System.Net;
using Discord;
using Newtonsoft.Json.Linq;

namespace FlushedSelfbot.Command.Commands.Image
{
    public class Neko : Command
    {
        public Neko() : base("Neko", "Sends a random neko", null, Category.Image)
        {
            
        }

        public override void Execute()
        {
            var embedMaker = new EmbedMaker
            {
                Title = "Failed to send neko",
                Color = Color.FromArgb(0xFF0000),
                Footer = new EmbedFooter
                {
                    Text = Bot.Name,
                    IconUrl = Bot.FlushedUrl
                }
            };

            using (var client = new WebClient())
            {
                var json = client.DownloadString("https://api.nekos.dev/api/v3/images/sfw/img/neko/");
                embedMaker.Title = "Neko";
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