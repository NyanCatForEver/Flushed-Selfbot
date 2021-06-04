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

        public override void Execute()
        {
            if (Args.Count < 3)
            {
                Message.Edit(new MessageEditProperties {Content = "> Missing arguments. Syntax: " + Syntax});
                return;
            }

            try
            {
                var count = int.Parse(Args[0]);

                try
                {
                    var delay = int.Parse(Args[1]);
                    var text = BuildString(2);

                    new Thread(() =>
                    {
                        for (var i = 0; i < count; i++)
                        {
                            Message.Channel.SendMessageAsync(text);
                            Thread.Sleep(delay);
                        }
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