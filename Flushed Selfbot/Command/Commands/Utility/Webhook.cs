using System;
using System.Threading;
using Discord;

namespace FlushedSelfbot.Command.Commands.Utility
{
    public class Webhook : Command
    {
        public Webhook() : base("Webhook", "Manages the specified webhook", "Webhook set <webhook url> | send <message> | spam <count> <message> | delete | info", Category.Utility)
        {

        }

        public static DiscordDefaultWebhook DiscordWebhook;

        public override void Execute()
        {
            if (Args.Count == 0)
            {
                Message.Edit(new MessageEditProperties {Content = "> Missing arguments. Syntax: " + Syntax});
                return;
            }

            if (!Args[0].Equals("set", StringComparison.OrdinalIgnoreCase) && DiscordWebhook == null)
            {
                Message.Edit(new MessageEditProperties
                    {Content = "> Webhook isn't set. Set it with \"Webhook set <webhook url>\""});
                return;
            }

            switch (Args[0].ToLower())
            {
                case "set":
                    if (Args.Count < 2)
                    {
                        Message.Edit(new MessageEditProperties {Content = "> Missing arguments. Syntax: " + Syntax});
                        break;
                    }

                    var url = Args[1].Substring(Args[1].IndexOf("api/webhooks/", StringComparison.OrdinalIgnoreCase) +
                                                "api/webhooks/".Length);
                    var id = ulong.Parse(url.Split('/')[0]);
                    var token = url.Split('/')[1];
                    DiscordWebhook = new DiscordDefaultWebhook(id, token);
                    Message.Edit(new MessageEditProperties {Content = $"> Set webhook to {id}"});
                    Bot.ConfigManager.Save();
                    break;

                case "send":
                    if (Args.Count < 2)
                    {
                        Message.Edit(new MessageEditProperties {Content = "> Missing arguments. Syntax: " + Syntax});
                        break;
                    }

                    DiscordWebhook.SendMessage(BuildString(1));
                    Message.Edit(new MessageEditProperties {Content = $"> Sent message \"{BuildString(1)}\""});
                    break;
                
                case "spam":
                    try
                    {
                        var count = int.Parse(Args[1]);
                        var text = BuildString(2);

                        new Thread(() =>
                        {
                            for (var i = 0; i < count; i++)
                            {
                                DiscordWebhook.SendMessageAsync(text);
                                Thread.Sleep(1400);
                            }
                        }).Start();
                    }
                    catch (FormatException)
                    {
                        Message.Edit(new MessageEditProperties {Content = "> Invalid count."});
                        break;
                    }

                    Message.Edit(new MessageEditProperties {Content = "> Spamming webhook " + DiscordWebhook.Id});
                    break;

                case "info":
                    Message.Edit(new MessageEditProperties
                    {
                        Content = "> **Webhook info**\n" +
                                  $"> ID: {DiscordWebhook.Id}\n" +
                                  $"> Name: {DiscordWebhook.Name}\n" +
                                  $"> Guild: {DiscordWebhook.GuildId}\n" +
                                  $"> Channel: {DiscordWebhook.ChannelId}"
                    });
                    break;

                case "delete":
                    DiscordWebhook.Delete();
                    Message.Edit(new MessageEditProperties {Content = $"> Deleted webhook {DiscordWebhook.Id}"});
                    DiscordWebhook = null;
                    Bot.ConfigManager.Save();
                    break;

                default:
                    Message.Edit(new MessageEditProperties {Content = "> Invalid argument. Syntax: " + Syntax});
                    break;
            }
        }
    }
}