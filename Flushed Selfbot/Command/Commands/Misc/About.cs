using System.Drawing;
using Discord;

namespace FlushedSelfbot.Command.Commands.Misc
{
    public class About : Command
    {
        public About() : base("About", "Sends information about the selfbot", null, Category.Misc)
        {
            
        }

        public override void Execute()
        {
            var embedMaker = new EmbedMaker
            {
                Title = "About",
                Color = Color.FromArgb(0xFFDD00),
                ThumbnailUrl = Bot.FlushedUrl,
                Description = "**Selfbot name:** " + Bot.Name + ".\n" +
                              "**Selfbot version:** " + Bot.Version + ".\n" +
                              "**Selfbot developer:** " + "NyanCatForEver" + ".\n" +
                              "**Selfbot creation date:** " + "28/05/2021" + ".\n",
                Footer = new EmbedFooter
                {
                    Text = Bot.Name,
                    IconUrl = Bot.FlushedUrl
                }
            };

            Message.Edit(Bot.ConfigManager.DisableEmbeds
                ? new MessageEditProperties {Content = $"> **{embedMaker.Title}**\n{embedMaker.Description}"}
                : new MessageEditProperties {Content = "", Embed = embedMaker});
        }
    }
}