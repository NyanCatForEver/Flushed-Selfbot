using System.Text.RegularExpressions;
using Discord;

namespace FlushedSelfbot.Command.Commands.Fun
{
    public class Big : Command
    {
        public Big() : base("Big", "Sends a big message", "Big <text>", Category.Fun)
        {
            
        }

        public override void Execute()
        {
            if (Args.Count == 0)
            {
                Message.Edit(new MessageEditProperties {Content = "> Missing argument. Syntax: " + Syntax});
                return;
            }
            
            var big = "";
            foreach (var ch in BuildString())
            {
                if (Regex.IsMatch(ch.ToString(), "[a-z]", RegexOptions.IgnoreCase))
                {
                    big += $":regional_indicator_{ch.ToString().ToLower()}:";
                    continue;
                }
                if (ch == ' ')
                {
                    big += "   ";
                    continue;
                }
                big += ch;
            }

            Message.Edit(new MessageEditProperties
            {
                Content = big
            });
        }
    }
}