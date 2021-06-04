using System;
using Discord;

namespace FlushedSelfbot.Command.Commands.Misc
{
    public class Shutdown : Command
    {
        public Shutdown() : base("Shutdown", "Shutdowns the selfbot", null, Category.Misc)
        {

        }

        public override void Execute()
        {
            Message.Edit(new MessageEditProperties {Content = "> Shutting down..."});
            Bot.Client.Logout();
            Environment.Exit(0);
        }
    }
}