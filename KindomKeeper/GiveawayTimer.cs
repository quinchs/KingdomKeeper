using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KindomKeeper.CommandHandler;

namespace KindomKeeper
{
    class GiveawayTimer
    {
        internal GiveAway currGiveaway { get; set; }
        internal GiveawayGuild gguild { get; set; }
        internal int Time = 0;
        internal System.Timers.Timer loopingTimer;

        internal Task StartTimer()
        {
            loopingTimer = new System.Timers.Timer()
            {
                Interval = 10000,
                AutoReset = true,
                Enabled = false
            };
            loopingTimer.Elapsed += LoopingTimer_Elapsed;
            return Task.CompletedTask;
        }

        private void LoopingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Time = Time - 15;
            if(Time <= 0)// giveawaytime
            {
                gguild.AllowBans();
            }
            else
            {
                gguild.UpdateTime(Time);
            }
        }
    }
}
