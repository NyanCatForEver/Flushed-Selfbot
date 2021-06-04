using System;
using System.Drawing;
using Discord;

namespace FlushedSelfbot.Command.Commands.Utility
{
    public class Avatar : Command
    {
        public Avatar() : base("Avatar", "Sends the avatar of the specified user", "Avatar <user/id (optional)>", Category.Utility)
        {

        }

        public override void Execute()
        {
            DiscordUser user = Args.Count == 0 ? Client.User : null;
            if (Message.Mentions.Count > 0)
                user = Message.Mentions[0];
            else
            {
                try
                {
                    user = Bot.Client.GetUser(ulong.Parse(Args[0]));
                }
                catch (Exception)
                {
                    Message.Edit(new MessageEditProperties {Content = "> Invalid argument. Syntax: " + Syntax});
                }
            }

            if (user == null)
            {
                Message.Edit(new MessageEditProperties {Content = "> Invalid user."});
                return;
            }

            if (Bot.ConfigManager.DisableEmbeds)
                Message.Edit(new MessageEditProperties
                {
                    Content = $"{user}'s avatar\n{user.Avatar.Url}?size=2048"
                });
            else
                Message.Edit(new MessageEditProperties
                {
                    Content = "",
                    Embed = new EmbedMaker
                    {
                        Title = $"{user}'s avatar",
                        Color = Color.FromArgb(0x2CEF00),
                        ImageUrl = user.Avatar.Url + "?size=2048",
                        Footer = new EmbedFooter
                        {
                            Text = Bot.Name,
                            IconUrl = Bot.FlushedUrl
                        }
                    }
                });
        }
    }
}