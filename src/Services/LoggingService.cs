using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OneeChan.Services
{
    public class LoggingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly ILogger _logger;


        public LoggingService(IServiceProvider services)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _commands = services.GetRequiredService<CommandService>();
            _logger = services.GetService<ILogger<LoggingService>>();

            _discord.Ready += OnReadyAsync;
            _commands.Log += OnLogAsync;
            _discord.Log += OnLogAsync;
        }

        private Task OnLogAsync(LogMessage msg)
        {
            var logText = $": {msg.Exception?.ToString() ?? msg.Message}";
            switch (msg.Severity.ToString())
            {
                case "Critical":
                {
                    _logger.LogCritical(logText);
                    break;
                }

                case "Warning":
                {
                    _logger.LogWarning(logText);
                    break;
                }

                case "Info":
                {
                    _logger.LogInformation(logText);
                    break;
                }

                case "Verbose":
                {
                    _logger.LogInformation(logText);
                    break;
                }

                case "Debug":
                {
                    _logger.LogDebug(logText);
                    break;
                }

                case "Error":
                {
                    _logger.LogError(logText);
                    break;
                }
            }

            return Task.CompletedTask;
        }

        private Task OnReadyAsync()
        {
            _logger.LogInformation($"Connected as -> [{_discord.CurrentUser}] :)");
            _logger.LogInformation($"We are on [{_discord.Guilds.Count}] servers");
            return Task.CompletedTask;
        }
    }
}