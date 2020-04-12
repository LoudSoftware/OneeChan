using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace OneeChan.Modules
{
    public class MiscModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commands;

        public MiscModule(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();

        }

        [Command("ping")]
        [Summary("ey, you there?")]
        [Alias("pong", "hello")]
        public async Task PingAsync()
        {
            await ReplyAsync("pong!");
        }

        [Command("userinfo")]
        [Summary("Displays your username")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user ??= Context.User;
            await ReplyAsync(user.ToString());
        }

        [Command("Help")]
        [Summary("Prints out this help screen.")]
        public async Task Help()
        {
            List<CommandInfo> commands = _commands.Commands.ToList();
            EmbedBuilder embedBuilder = new EmbedBuilder();

            foreach (CommandInfo command in commands)
            {
                // TODO Check if the user has permission to run command before adding it to the help menu
                // Get the command Summary attribute information
                string embedFieldText = command.Summary ?? "No description available\n";

                embedBuilder.AddField(command.Name, embedFieldText);
            }

            await ReplyAsync("Here's a list of commands and their description: ", false, embedBuilder.Build());
        }
    }
}