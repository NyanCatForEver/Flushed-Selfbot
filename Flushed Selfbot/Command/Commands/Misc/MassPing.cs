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

        private ulong _previousChannel;
        private int _offset;

        public override void Execute()
        {
            if (_previousChannel != Message.Channel.Id || _offset >= 4000 ||
                (Bot.CachedMembers.ContainsKey(Message.Guild.Id) &&
                 _offset >= Bot.CachedMembers[Message.Guild.Id].Count))
            {
                _offset = 0;
                _previousChannel = Message.Channel.Id;
            }

            var members = Util
                .GetMembersByGuildChannel(Message.Guild.Id, Message.Channel.Id, 2000 / (18 + 3), _offset)
                .Where(member => member.User.Id != Client.User.Id);

            var message =
                Message.Channel.SendMessage(members.Aggregate("",
                    (current, person) => current + person.AsMessagable()));
            Message.Delete();
            message.Edit(new MessageEditProperties {Content = "hi"});
            message.Delete();
            _offset += 2000 / (18 + 3);
        }
    }
}