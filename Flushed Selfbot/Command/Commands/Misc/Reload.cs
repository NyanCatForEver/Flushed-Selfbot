using System.Drawing;
using Colorful;
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
            Bot.AutoFeur.Load();
            Bot.ConfigManager.Load();
            Bot.MessageLogger.Load();
            Bot.NitroSniper.Load();
            Message.Edit(new MessageEditProperties {Content = "> Reloaded configuration."});
            Console.WriteLine("Reloaded configuration.", Color.SpringGreen);
        }
    }
}