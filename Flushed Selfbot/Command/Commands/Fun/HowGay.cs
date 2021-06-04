using System;
using System.Drawing;
using Discord;

namespace FlushedSelfbot.Command.Commands.Fun
{
    public class HowGay : Command
    {
        public HowGay() : base("HowGay", "How much gay?", "HowGay <user (optional)>", Category.Fun)
        {

        }

        public override void Execute()
        {
            var howMuchGay = new Random().Next(100);
            var embedMaker = new EmbedMaker
            {
                Title = "How much gay?",
                Description = "You are **" + howMuchGay + "%** gay. " + (howMuchGay >= 50 ? ":rainbow_flag:" : ""),
                ThumbnailUrl = "https://images.emojiterra.com/twitter/v13.0/512px/1f3f3-1f308.png",
                Footer = new EmbedFooter
                {
                    Text = Bot.Name,
                    IconUrl = Bot.FlushedUrl
                },
                Color = Color.FromArgb(howMuchGay < 25 ? 0x00FF00 :
                    howMuchGay < 50 ? 0xFFFB00 :
                    howMuchGay < 75 ? 0xFF4600 : 0xFF0000)
            };


            if (Args.Count > 0)
            {
                string name;

                if (Message.Mentions.Count > 0)
                    name = Message.Mentions[0].Username;
                else
                {
                    try
                    {
                        name = Bot.Client.GetUser(ulong.Parse(Args[0])).Username;
                    }
                    catch (Exception)
                    {
                        name = BuildString();
                    }
                }

                embedMaker.Description = name + " is **" + howMuchGay + "%** gay. " +
                                         (howMuchGay >= 50 ? ":rainbow_flag:" : "");
            }

            Message.Edit(Bot.ConfigManager.DisableEmbeds
                ? new MessageEditProperties {Content = embedMaker.Description}
                : new MessageEditProperties {Embed = embedMaker});
        }
    }
}