using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindomKeeper
{
    class BanLimitTimer
    {
        internal int Bans = 0;
        internal SocketGuildUser user = null;
        public uint Time = 0;

        internal System.Timers.Timer loopingTimer;

        internal Task StartTimer()
        {
            loopingTimer = new System.Timers.Timer()
            {
                Interval = 3600000,
                AutoReset = false,
                Enabled = false
            };
            loopingTimer.Elapsed += LoopingTimer_Elapsed;
            return Task.CompletedTask;
        }

        private void LoopingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Global.banTimers.Remove(this);
            user.SendMessageAsync($"Your ban timer has ended, you now have {Global.BanRateLimit} Bans avalable for {Global.Client.GetGuild(Global.GuildID).Name}");      
        }
    }
}
