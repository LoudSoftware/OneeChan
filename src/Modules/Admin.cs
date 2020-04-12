using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneeChan.Database;
using OneeChan.Database.Entities;
using OneeChan.Services;

namespace OneeChan.Modules
{
    public class Admin : ModuleBase<SocketCommandContext>
    {
        private static DiscordSocketClient _client;
        private readonly IConfigurationRoot _config;
        private string _prefix;
        private readonly ILogger<Admin> _logger;

        public Admin(IServiceProvider services)
        {
            _client = services.GetRequiredService<DiscordSocketClient>();
            _config = services.GetRequiredService<IConfigurationRoot>();
            _prefix = _config["prefix"];
            _logger = services.GetRequiredService<ILogger<Admin>>();

            _logger.LogInformation("Admin module loaded!");
        }

        [Command("change-prefix", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Change the bot's default prefix")]
        public async Task ChangePrefix(char prefix)
        {
            await using var db = new OneeChanEntities();
            
            // Get current Guild from DB
            var guild = db.Guilds.FirstOrDefault(p => p.GuildId == (long) Context.Guild.Id);

            // If there is no guild row in the DB
            if (guild == null)
            {
                _logger.LogWarning("Guild not found in db, creating new guild object");
                var newGuild = new Guild {GuildId = (long) Context.Guild.Id};
                guild = newGuild;
            }

            var guildSettings = guild.ServerSettings ?? new ServerSettings();
            guildSettings.Prefix = prefix;
            guild.ServerSettings = guildSettings;
            db.Guilds.Add(guild);
            db.SaveChanges();
        }
    }
}