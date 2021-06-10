using System;
using System.Threading;
using Discord;

namespace FlushedSelfbot.Command.Commands.Utility
{
    public class Spam : Command
    {
        public Spam() : base("Spam", "Spams the specified message", "Spam <count> <delay (milliseconds)> <text>", Category.Utility)
        {
            
        }
        
        public static bool spamming;

        public override void Execute()
        {
            if (Args.Count < 3)
            {
                Message.Edit(new MessageEditProperties {Content = "> Missing arguments. Syntax: " + Syntax});
                return;
            }

            if (spamming)
            {
                Message.Edit(new MessageEditProperties {Content = "> You're already spamming! Stop the spammer using StopSpamming command"});
                return;
            }

            try
            {
                var count = int.Parse(Args[0]);

                try
                {
                    var delay = int.Parse(Args[1]);
                    var text = BuildString(2);

                    spamming = true;
                    new Thread(() =>
                    {
                        for (var i = 0; i < count; i++)
                        {
                            if (!spamming)
                                break;
                            Message.Channel.SendMessageAsync(text);
                            Thread.Sleep(delay);
                        }

                        spamming = false;
                    }).Start();
                }
                catch (FormatException)
                {
                    Message.Channel.SendMessage("> Invalid delay.");
                }
            }
            catch (FormatException)
            {
                Message.Channel.SendMessage("> Invalid count.");
            }

            Message.Delete();
        }
    }
}