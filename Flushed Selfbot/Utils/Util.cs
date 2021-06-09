using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.Gateway;

namespace FlushedSelfbot.Utils
{
    public static class Util
    {
        public static IEnumerable<GuildMember> GetMembersByGuildChannel(ulong guild, ulong channel,
            int count = int.MaxValue, int offset = 0)
        {
            if (!Bot.CachedMembers.ContainsKey(guild))
                Bot.CachedMembers.Add(guild,
                    Bot.Client.GetGuildChannelMembers(guild, channel, new MemberListQueryOptions {Count = 4000})
                        .ToList());

            return Bot.CachedMembers[guild].Skip(offset).Take(count).ToList();
        }
    }
}