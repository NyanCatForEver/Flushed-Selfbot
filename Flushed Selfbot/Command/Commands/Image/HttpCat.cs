using Discord;

namespace FlushedSelfbot.Command.Commands.Image
{
    public class HttpCat : Command
    {
        public HttpCat() : base("HttpCat", "A Cat for every HTTP status", "HttpCat <status>", Category.Image)
        {

        }

        public override void Execute()
        {
            if (Args.Count == 0)
            {
                Message.Edit(new MessageEditProperties {Content = "> Missing arguments. Syntax: " + Syntax});
                return;
            }
            
            Message.Edit(new MessageEditProperties {Content = "https://http.cat/" + Args[0]});
        }
    }
}