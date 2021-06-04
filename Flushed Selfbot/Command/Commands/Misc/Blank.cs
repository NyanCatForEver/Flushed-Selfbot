using System;
using Discord;

namespace FlushedSelfbot.Command.Commands.Misc
{
    public class Blank : Command
    {
        public Blank() : base("Blank", "Sends an \"empty\" message.", "Blank <lines number (optional)>", Category.Misc)
        {
            
        }
        
        public override void Execute()
        {
            if (Args.Count == 0)
            {
                Message.Edit(new MessageEditProperties {Content = "_ _"});
                return;
            }
            
            int count;
            try
            {
                count = int.Parse(Args[0]);
            }
            catch (FormatException)
            {
                Message.Edit(new MessageEditProperties {Content = "> Invalid argument. It should be a number."});
                return;
            }

            if (count > 1998)
            {
                Message.Edit(new MessageEditProperties {Content = "> Too big argument. Should be smaller than 1998."});
                return;
            }
            
            var blank = "_";
            for (var i = 0; i < (count > 0 ? count : 1); i++)
                blank += "\n";
            blank += "_";

            Message.Edit(new MessageEditProperties {Content = blank});
        }
    }
}