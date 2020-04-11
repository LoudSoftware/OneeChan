using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

        private Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState pre_state, SocketVoiceState post_state)
        {
            _logger.LogInformation(1, $"User [{user.Username}] moved from [{pre_state}] -> [{post_state}]");
            return Task.CompletedTask;
        }
    }
}