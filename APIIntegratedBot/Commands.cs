using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIIntegratedBot
{
    internal class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("getusers")]

        public async Task GetUsers()
        {
            var guild = (SocketGuild)Context.Guild;
            var users = guild.Users;

            foreach (var user in users)
            {
                await ReplyAsync(user.Username);
            }
        }
    }
}
