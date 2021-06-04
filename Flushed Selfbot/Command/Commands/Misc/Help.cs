using System;
using System.Drawing;
using System.Linq;
using System.Text;
using Discord;

namespace FlushedSelfbot.Command.Commands.Misc
{
    public class Help : Command
    {
        public Help() : base("Help", "Sends help message.", "Help <category (optional)>", Category.Misc)
        {

        }

        public override void Execute()
        {
            var embedMaker = new EmbedMaker
            {
                Title = "Help",
                Color = Color.FromArgb(0xF30071),
                Footer = new EmbedFooter
                {
                    Text = Bot.Name,
                    IconUrl = Bot.FlushedUrl
                }
            };

            if (Args.Count > 0 && !Args[0].Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                var category =
                    Category.Values.FirstOrDefault(c => c.Name.Equals(Args[0], StringComparison.OrdinalIgnoreCase));
                if (category == null)
                {
                    Message.Edit(new MessageEditProperties {Content = "> Category " + Args[0] + " doesn't exist!"});
                    return;
                }

                embedMaker.Title = category.Name;

                var field = new StringBuilder();
                foreach (var command in Bot.CommandManager.GetCommands(category))
                    field.Append("**").Append(command.Name).Append("** | ").Append(command.Description).Append(" | ")
                        .Append(command.Syntax).Append("\n");

                embedMaker.AddField("> **" + category.Emote + "   " + category.Name + "**", field.ToString());
                Message.Edit(new MessageEditProperties {Embed = embedMaker});
                return;
            }

            foreach (var category in Category.Values)
            {
                var field = new StringBuilder();
                foreach (var command in Bot.CommandManager.GetCommands(category))
                    field.Append("**").Append(command.Name).Append("** | ").Append(command.Description).Append(" | ")
                        .Append(command.Syntax).Append("\n");

                embedMaker.AddField("> **" + category.Emote + "   " + category.Name + "**", field.ToString());
            }

            Message.Edit(Bot.ConfigManager.DisableEmbeds ? new MessageEditProperties {Content = $"> **{embedMaker.Title}**\n\n{((DiscordEmbed) embedMaker).Fields.Aggregate("", (current, field) => current + (field.Name + "\n" + field.Content))}"}
                : new MessageEditProperties {Content = "", Embed = embedMaker});
        }
    }
}