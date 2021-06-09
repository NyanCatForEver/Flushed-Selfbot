using System.Collections.Generic;
using Discord;
using Discord.Gateway;
using FlushedSelfbot.Utils;

namespace FlushedSelfbot.Command
{
    public abstract class Command
    {
        public readonly string Name;
        public readonly string Description;
        public readonly string Syntax;
        public readonly Category Category;

        protected DiscordSocketClient Client;
        protected DiscordMessage Message;
        protected List<string> Args = new List<string>();

        protected Command(string name, string description, string syntax, Category category)
        {
            Name = name;
            Description = description;
            Syntax = syntax ?? name;
            Category = category;
            Client = Bot.Client;
        }

        internal Command Prepare(DiscordSocketClient client, DiscordMessage message, List<string> args)
        {
            Client = client;
            Message = message;
            Args = args;
            return this;
        }

        public abstract void Execute();


        protected string BuildString(int start = 0)
        {
            var s = "";
            for (var i = start; i < Args.Count; i++)
                s += " " + Args[i];
            return s.Substring(1);
        }
    }
}