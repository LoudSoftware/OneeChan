using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace OneeChan.Modules
{
    [Group("Housekeeper settings")]
    public class HouseKeepingSettings : ModuleBase<SocketCommandContext>
    {
        // [Command("ping")]
        //
        // [Alias("pong", "hello")]
        // public async Task PingAsync()
        // {
        //     await ReplyAsync("pong!");
        // }
        //
        // [Command("userinfo")]
        // public async Task UserInfoAsync(IUser user = null)
        // {
        //     user ??= Context.User;
        //     await ReplyAsync(user.ToString());
        // }
    }
}