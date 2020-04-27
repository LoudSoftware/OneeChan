using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using OneeChan.Database;
using OneeChan.Database.Entities;

namespace OneeChan.Modules
{
    public class HouseKeepingSettings : ModuleBase<SocketCommandContext>
    {
        [Command("set-auto-category")]
        [RequireBotPermission(ChannelPermission.ManageChannels)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireContext(ContextType.Guild)]
        public async Task setAutoCategory(long id)
        {
            // Check if channel id exists and is valid
            if (Context.Guild.CategoryChannels.FirstOrDefault(c => c.Id == (ulong) id) == null)
            {
                await ReplyAsync($"Channel category with id:[{id}] not found on this server");
                return;
            }

            var channelName = Context.Guild.CategoryChannels.First(c => c.Id == (ulong) id).Name;

            var reply = await ReplyAsync($"Setting this guild's Automatic Category Channel to [{channelName}]");


            await using var db = new OneeChanEntities();
            var guild = db.Guilds.Include(guild => guild.HouseKeeperSettings)
                .FirstOrDefault(g => g.GuildId == (long) Context.Guild.Id);

            var houseKeeperSettings = guild.HouseKeeperSettings ?? new HouseKeeper();

            houseKeeperSettings.AutoCategoryChannelId = id;

            guild.HouseKeeperSettings = houseKeeperSettings;
            
            await db.SaveChangesAsync();

            reply.ModifyAsync(m =>
                m.Content = $"Finished setting this guild's Automatic Category Channel to [{channelName}] ☺");
        }

        
        [Command("set-auto-voice")]
        [RequireBotPermission(ChannelPermission.ManageChannels)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireContext(ContextType.Guild)]
        public async Task setAutoVoice(long id)
        {
            // Check if channel id exists and is valid
            if (Context.Guild.VoiceChannels.FirstOrDefault(c => c.Id == (ulong)id) == null)
            {
                await ReplyAsync($"Automatic Voice Channel with id:[{id}] not found on this server");
                return;
            }

            var channelName = Context.Guild.VoiceChannels.First(c => c.Id == (ulong)id).Name;

            var reply = await ReplyAsync($"Setting this guild's Automatic Voice Channel to [{channelName}]");


            await using var db = new OneeChanEntities();

            var guild = db.Guilds.Include(guild => guild.HouseKeeperSettings)
                .FirstOrDefault(g => g.GuildId == (long) Context.Guild.Id);

            var houseKeeperSettings = guild.HouseKeeperSettings ?? new HouseKeeper();
            houseKeeperSettings.AutoVoiceChannelId = id;
            guild.HouseKeeperSettings = houseKeeperSettings;
            await db.SaveChangesAsync();

            reply.ModifyAsync(m =>
                m.Content = $"Finished setting this guild's Automatic Voice Channel to [{channelName}] ☺");
        }


    }
}