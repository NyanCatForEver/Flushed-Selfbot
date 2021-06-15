using System;
using System.Threading;
using Discord;

namespace FlushedSelfbot.Command.Commands.Utility
{
    public class StopSpamming : Command
    {
        public StopSpamming() : base("StopSpamming", "Stops the spammer", null, Category.Utility)
        {
            
        }

        public override void Execute()
        {
            if (!Spam.spamming)
            {
                Message.Edit(new MessageEditProperties {Content = "> Spammer isn't running!"});
                return;
            }

            Spam.spamming = false;
            Message.Edit(new MessageEditProperties {Content = "> Stopped spammer."});
        }
    }
}