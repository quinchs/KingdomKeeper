using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindomKeeper.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("test")]
        public async Task test()
        {
            await Context.Channel.SendMessageAsync("Swiggity swag motherflok");
        }
    }
}
