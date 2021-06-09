using System;
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
                Description = $"**Name:** {Bot.Name}\n" +
                              $"**Version:** {Bot.Version}\n" +
                              "**Developer:** NyanCatForEver\n" +
                              "**Creation date:** 28/05/2021\n" +
                              $"**Uptime:** {Bot.UpTime.Elapsed:hh\\:mm\\:ss}",
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