using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneeChan.Database;
using OneeChan.Database.Entities;
using static OneeChan.Util.HouseKeeperUtils;

namespace OneeChan.Services
{
    public class HouseKeepingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly ILogger<LoggingService> _logger;
        private IServiceProvider _services;

        public HouseKeepingService(IServiceProvider services)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _logger = services.GetRequiredService<ILogger<LoggingService>>();
            _services = services;

            _discord.UserVoiceStateUpdated += UserVoiceStateUpdated;
            _discord.GuildMemberUpdated += GuildMemberUpdated;
        }

        private async Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState preState, SocketVoiceState postState)
        {
            var guildUser = user as SocketGuildUser;
            // ReSharper disable once PossibleNullReferenceException
            var guildId = (long) guildUser.Guild.Id;
            var guildName = guildUser.Guild.Name;


            // Initializing db so we can check if there are housekeeper settings linked to the guildId retrieved above
            await using var db = new OneeChanEntities();

            var guild = db.Guilds.Include(x => x.HouseKeeperSettings)
                .FirstOrDefault(g => g.GuildId == guildId);
            var houseKeeperSettings = guild?.HouseKeeperSettings;

            if (houseKeeperSettings?.AutoVoiceChannelId == null || houseKeeperSettings?.AutoCategoryChannelId == null)
            {
                _logger.LogWarning(
                    $"This guild has incomplete HouseKeeperSettings or no existing Guild entries in DB.\nCurrently, they are:{houseKeeperSettings}");
                return; // No housekeeper settings could be found for that server
            }

            var guildVoiceCategoryId = (long) houseKeeperSettings.AutoCategoryChannelId;
            var guildVoiceChannelId = (long) houseKeeperSettings.AutoVoiceChannelId;


            _logger.LogInformation(1,
                $"User [{user.Username}] in [{guildName}] moved from [{preState}] -> [{postState}]");

            if (postState.VoiceChannel == null) // User completely disconnected
            {
                if (CheckDelete(preState.VoiceChannel, guildVoiceCategoryId, guildVoiceChannelId))
                {
                    await preState.VoiceChannel.DeleteAsync();
                }
            }
            else // User either entered a channel or moved to another one
            {
                if ((long) postState.VoiceChannel.Id == guildVoiceChannelId) // User entered the auto creation channel
                {
                    var channel = await guildUser.Guild.CreateVoiceChannelAsync("🌴 Just chillin'",
                        c => c.CategoryId = (ulong) guildVoiceCategoryId);
                    await guildUser.ModifyAsync(u => u.Channel = channel);
                    //await UpdateVoiceChannelName(channel as SocketVoiceChannel);
                }

                // Check user's previous voice channel for deletion
                if (preState.VoiceChannel != null)
                {
                    if (CheckDelete(preState.VoiceChannel, guildVoiceCategoryId, guildVoiceChannelId))
                    {
                        await preState.VoiceChannel.DeleteAsync();
                    }
                    else
                    {
                        await UpdateVoiceChannelName(postState.VoiceChannel);
                    }
                }
            }
        }


        /// <summary>
        /// Verifies if we should delete the given channel
        /// </summary>
        /// <param name="voiceChannel"></param>
        /// <param name="catId">HouseKeeper automatic channel category Id</param>
        /// <param name="voiceId">HouseKeeper automatic voice channel Id</param>
        /// <returns></returns>
        private bool CheckDelete(SocketVoiceChannel voiceChannel, long catId, long voiceId)
        {
            return voiceChannel.CategoryId != null && (long) voiceChannel.CategoryId == catId &&
                   (long) voiceChannel.Id != voiceId;
        }

        private async Task GuildMemberUpdated(SocketGuildUser preUser, SocketGuildUser postUser)
        {
            // Check if the user is in a relevant voice channel first

            #region Settings retrieval

            //TODO refactor this out since it's duplicated in other Event listeners as well

            await using var db = new OneeChanEntities();
            HouseKeeper housekeeperSettings = db.Guilds.Include(g => g.HouseKeeperSettings)
                .FirstOrDefault(g => g.GuildId == (long) postUser.Guild.Id)?.HouseKeeperSettings;

            if (postUser.VoiceChannel?.CategoryId != (ulong) housekeeperSettings.AutoCategoryChannelId)
                return;

            #endregion

            var preAct = preUser.Activities;
            var postAct = postUser.Activities;


            await UpdateVoiceChannelName(postUser.VoiceChannel);


            _logger.LogInformation(
                $"User [{postUser.Username}] activity change detected");
        }


        private async Task UpdateVoiceChannelName(SocketVoiceChannel voiceChannel)
        {
            var topGame = voiceChannel.Users
                .Select(user => GetCandidateGameFromActivities(user.Activities).Name)
                .GroupBy(game => game)
                .OrderByDescending(group => group.Count())
                .Select(grp => grp.Key)
                .First();

            await voiceChannel.ModifyAsync(c => c.Name = topGame);
        }
    }
}