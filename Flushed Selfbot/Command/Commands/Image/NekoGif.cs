using System.Drawing;
using System.Net;
using Discord;
using Newtonsoft.Json.Linq;

namespace FlushedSelfbot.Command.Commands.Image
{
    public class NekoGif : Command
    {
        public NekoGif() : base("NekoGif", "Sends a random neko GIF", null, Category.Image)
        {
            
        }

        public override void Execute()
        {
            var embedMaker = new EmbedMaker
            {
                Title = "Failed to send neko GIF",
                Color = Color.FromArgb(0xFF0000),
                Footer = new EmbedFooter
                {
                    Text = Bot.Name,
                    IconUrl = Bot.FlushedUrl
                }
            };

            using (var client = new WebClient())
            {
                var json = client.DownloadString("https://api.nekos.dev/api/v3/images/sfw/gif/neko/");
                embedMaker.Title = "Neko GIF";
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