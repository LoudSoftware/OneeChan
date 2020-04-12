using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneeChan.Database;
using OneeChan.Database.Entities;

namespace OneeChan.Services
{
    internal class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private IConfigurationRoot _config;
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;

        public CommandHandlingService(IServiceProvider services)
        {
            _config = services.GetRequiredService<IConfigurationRoot>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _commands = services.GetRequiredService<CommandService>();
            _logger = services.GetRequiredService<ILogger<LoggingService>>();
            _services = services;

            _discord.MessageReceived += MessageReceivedAsync;
            _commands.CommandExecuted += CommandExcecutedAsync;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages and messages from bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            // Grabbing prefix from local config and db setting
            var prefix = Char.Parse(_config["Prefix"]);

            var currentGuild = message.Channel is SocketGuildChannel channel ? channel.Guild : null;

            Debug.Assert(currentGuild != null, nameof(currentGuild) + " != null");
            var serverSettings = GetPrefix((long) currentGuild.Id);

            // Overriding default prefix with the one set in db (if set)
            if (serverSettings?.Prefix != null) prefix = (char) serverSettings.Prefix;

            // Detecting if command or just a message
            var argPos = 0;
            if (!(message.HasMentionPrefix(_discord.CurrentUser, ref argPos) ||
                  message.HasCharPrefix(prefix, ref argPos))
            ) return;

            var context = new SocketCommandContext(_discord, message);
            var result = await _commands.ExecuteAsync(context, argPos, _services);

            if (result.Error.HasValue &&
                result.Error.Value != CommandError.UnknownCommand)
                await context.Channel.SendMessageAsync(result.ToString());
        }

        public async Task CommandExcecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // if a command isn't found, log that info to console and exit this method
            if (!command.IsSpecified)
            {
                _logger.LogError(
                    $"Command failed to execute for [{context.User.Username}] <-> [{result.ErrorReason}]!");
                return;
            }


            // log success to the console and exit this method
            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    $"Command [{command.Value.Name}] executed for [{context.User.Username}] on [{context.Guild.Name}]");
                return;
            }

            // failure scenario, let's let the user know
            await context.Channel.SendMessageAsync(
                $"Sorry, {context.User.Username}... something went wrong -> [{result}]!");
        }

        private ServerSettings GetPrefix(long GuildId)
        {
            ServerSettings settings = null;

            using var db = new OneeChanEntities();
            var guild = db.Guilds.FirstOrDefault(p => p.GuildId == GuildId);
            settings = guild?.ServerSettings;
            return settings;
        }
    }
}