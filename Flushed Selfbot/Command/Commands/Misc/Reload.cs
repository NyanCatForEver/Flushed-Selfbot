using Discord;

namespace FlushedSelfbot.Command.Commands.Misc
{
    public class Reload : Command
    {
        public Reload() : base("Reload", "Reloads the selfbot configuration", null, Category.Misc)
        {
            
        }

        public override void Execute()
        {
            Bot.ConfigManager.Load();
            Message.Edit(new MessageEditProperties {Content = "> Reloaded configuration."});
        }
    }
}