using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.Gateway;
using FlushedSelfbot.Command.Commands.Fun;
using FlushedSelfbot.Command.Commands.Image;
using FlushedSelfbot.Command.Commands.Misc;
using FlushedSelfbot.Command.Commands.Nsfw;
using FlushedSelfbot.Command.Commands.Utility;

namespace FlushedSelfbot.Command
{
    public class CommandManager
    {
        public List<Command> Commands;
        
        public CommandManager()
        {
            InitCommands();
        }

        private void InitCommands()
        {
            Commands = new List<Command>
            {
                new About(), new Avatar(), new Blank(), new Cat(), new MassPing(), new Neko(), new NsfwNeko(),
                new Help(), new Spam(), new HowGay(), new Invert(), new Shutdown(), new Big(), new UserInfo(),
                new Prefix(), new Reload(), new NekoGif(), new NsfwNekoGif(), new Webhook(), new Dog(), new HttpCat(),
                new Lizard(), new Meme(), new Panda(), new Bird(), new Fox(), new Racoon(), new Koala(), new Kangaroo(),
                new StopSpamming()
            };
        }

        public Command OnMessageReceived(DiscordSocketClient client, DiscordMessage message)
        {
            var args = message.Content.Substring(Bot.ConfigManager.Prefix.Length).Split(' ').ToList();
            var command = GetCommand(args[0]);
            if (command == null)
                return null;
            
            args.RemoveAt(0);
            command.Prepare(client, message, args).Execute();
            return command;
        }

        private Command GetCommand(string name)
        {
            return Commands.FirstOrDefault(command => name.Equals(command.Name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Command> GetCommands(Category category)
        {
            return Commands.Where(command => command.Category == category).ToList();
        }
    }
}