using System.Drawing;
using System.Net;
using Discord;
using Newtonsoft.Json.Linq;

namespace FlushedSelfbot.Command.Commands.Image
{
    public class Fox : Command
    {
        public Fox() : base("Fox", "Sends a random fox picture", null, Category.Image)
        {

        }

        public override void Execute()
        {
            var embedMaker = new EmbedMaker
            {
                Title = "Failed to send fox picture",
                Color = Color.FromArgb(0xFF0000),
                Footer = new EmbedFooter
                {
                    Text = Bot.Name,
                    IconUrl = Bot.FlushedUrl
                }
            };

            using (var client = new WebClient())
            {
                var json = client.DownloadString("https://some-random-api.ml/animal/fox");
                embedMaker.Title = "Fox";
                embedMaker.ImageUrl = (string) ((JValue) JObject.Parse(json).SelectToken("image"))?.Value;
                embedMaker.Color = Color.FromArgb(0x00FFAA);
            }

            Message.Edit(Bot.ConfigManager.DisableEmbeds
                ? new MessageEditProperties {Content = embedMaker.ImageUrl ?? embedMaker.Title}
                : new MessageEditProperties {Content = "", Embed = embedMaker});
        }
    }
}