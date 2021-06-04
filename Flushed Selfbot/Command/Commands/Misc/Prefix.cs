using Discord;

namespace FlushedSelfbot.Command.Commands.Misc
{
    public class Prefix : Command
    {
        public Prefix() : base("Prefix", "Changes the selfbot's commands prefix", "Prefix <new prefix>", Category.Misc)
        {
            
        }

        public override void Execute()
        {
            if (Args.Count == 0)
            {
                Message.Edit(new MessageEditProperties {Content = "> Missing arguments. Syntax: " + Syntax});
                return;
            }

            var newPrefix = BuildString();
            Bot.ConfigManager.Prefix = newPrefix;
            Bot.ConfigManager.Save();
            Message.Edit(new MessageEditProperties {Content = $"> Changed prefix to \"{newPrefix}\""});
        }
    }
}