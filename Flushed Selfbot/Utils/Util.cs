using System.Collections.Generic;
using System.Linq;
using Discord.Gateway;

namespace FlushedSelfbot.Utils
{
    public static class Util
    {
        public static IEnumerable<ulong> GetMembersByGuildChannel(ulong guild, ulong channel, int count = int.MaxValue)
        {
            if (!Bot.CachedMembers.ContainsKey(guild))
                Bot.CachedMembers.Add(guild,
                    Bot.Client.GetGuildChannelMembers(guild, channel, new MemberListQueryOptions
                        {
                            Count = 4000
                        })
                        .Select(member => member.User.Id)
                        .ToList());

            return Bot.CachedMembers[guild].Take(count).ToList();
        }
    }
}