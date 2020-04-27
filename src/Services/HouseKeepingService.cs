using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneeChan.Database;
using OneeChan.Database.Entities;

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
                }

                // Check user's previous voice channel for deletion
                if (preState.VoiceChannel != null &&
                    CheckDelete(preState.VoiceChannel, guildVoiceCategoryId, guildVoiceChannelId))
                {
                    await preState.VoiceChannel.DeleteAsync();
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
    }
}