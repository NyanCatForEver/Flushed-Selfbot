using System.Linq;
using Discord;
using FlushedSelfbot.Utils;

namespace FlushedSelfbot.Command.Commands.Misc
{
    public class MassPing : Command
    {
        public MassPing() : base("MassPing", "Mentions all people on current server", null, Category.Misc)
        {

        }

        public override void Execute()
        {
            var message = Message.Channel.SendMessage(Util
                .GetMembersByGuildChannel(Message.Guild.Id, Message.Channel.Id, 2000 / (18 + 3))
                .Where(id => id != Client.User.Id)
                .Aggregate("", (current, person) => current + $"<@{person}>"));
            Message.Delete();
            message.Edit(new MessageEditProperties
            {
                Content = "hi"
            });
            message.Delete();
        }
    }
}