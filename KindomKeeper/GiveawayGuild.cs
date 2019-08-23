using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindomKeeper
{
    class GiveawayGuild
    {
        internal DiscordSocketClient _client = Global.Client;
        internal RestVoiceChannel chantimer;
        internal CommandHandler.GiveAway currgiveaway;
        internal string inviteURL;

        internal async Task createguild(CommandHandler.GiveAway currGiveaway)
        {
            var newguild = await _client.CreateGuildAsync($"{currGiveaway.GiveAwayItem} Giveaway", _client.VoiceRegions.FirstOrDefault(n => n.Name == "US East"));
            Global.GiveAwayGuildID = newguild.Id;
            GuildPermissions adminguildperms = new GuildPermissions(true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true);
            GuildPermissions Contestantperms = new GuildPermissions(false, false, false, false, false, false, true, false, true, true, false, false, true, true, true, false, true, true, true, false, false, false, true, false, true, false, false, false, false);

            await newguild.CreateRoleAsync("Admins", adminguildperms, Color.Red, true);
            await newguild.CreateRoleAsync("Contestants", Contestantperms, Color.Blue, false);

            var chanContestants = await newguild.CreateTextChannelAsync("Contestants", x => x.Topic = "talk in here till bans are unleashed >:)");
            var chanInfo = await newguild.CreateTextChannelAsync("Info", x => x.Topic = "Rules and info");
            var chanCount = await newguild.CreateVoiceChannelAsync("Time: xxx");
            chantimer = chanCount;

            EmbedBuilder eb = new EmbedBuilder();
            eb.Title = "***INFO***";
            eb.Color = Color.Gold;
            eb.Description = $"Welcome to the giveaway guild! the prize for this giveaway is {currGiveaway.GiveAwayItem}!\n\n **How to play** once the timer reaches 0 everyone with the `Contesters` role will be givin access to the \"ban command, its a FFA to the death! the last player(s) remaining will get the prize! this is a fun interactive competative giveaway where users can decide who wins!";
            eb.Footer = new EmbedFooterBuilder();
            eb.Footer.Text = $"Giveaway by {_client.GetGuild(Global.GuildID).GetUser(currGiveaway.GiveAwayUser).Username}#{_client.GetGuild(Global.GuildID).GetUser(currGiveaway.GiveAwayUser).Discriminator}";
            eb.Footer.IconUrl = _client.GetGuild(Global.GuildID).GetUser(currGiveaway.GiveAwayUser).GetAvatarUrl();
            await chanInfo.SendMessageAsync("", false, eb.Build());

            OverwritePermissions adminperms = new OverwritePermissions(PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow);
            await chanInfo.AddPermissionOverwriteAsync(newguild.Roles.FirstOrDefault(r => r.Name == "Admins"), adminperms);
            OverwritePermissions contesterperms = new OverwritePermissions(PermValue.Deny, PermValue.Deny, PermValue.Allow, PermValue.Allow, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Deny, PermValue.Deny, PermValue.Allow, PermValue.Allow, PermValue.Deny, PermValue.Deny, PermValue.Deny, PermValue.Allow, PermValue.Deny, PermValue.Deny);
            await chanInfo.AddPermissionOverwriteAsync(newguild.Roles.FirstOrDefault(r => r.Name == "Contestants"), contesterperms);
            var url = chanInfo.CreateInviteAsync(null, null, false, false);
            _client.UserJoined += userjoinGiveaway;
            inviteURL = url.Result.Url;
        }

        private async Task userjoinGiveaway(SocketGuildUser arg)
        {
            if(arg.Guild.Id == Global.GiveAwayGuildID)
            {
                if(arg.Id == Global.jakeID || _client.GetGuild(Global.GuildID).GetUser(arg.Id).Roles.Contains(_client.GetGuild(Global.GuildID).Roles.FirstOrDefault(r => r.Id == Global.developerRoleID)) || arg.Id == currgiveaway.GiveAwayUser)
                {
                    var role = _client.GetGuild(Global.GiveAwayGuildID).Roles.FirstOrDefault(r => r.Name == "Admins");
                    await arg.AddRoleAsync(role);
                }
                else
                {
                    var role = _client.GetGuild(Global.GiveAwayGuildID).Roles.FirstOrDefault(r => r.Name == "Contestants");
                    await arg.AddRoleAsync(role);
                }
            }
        }

        internal async Task UpdateTime(int seconds)
        {
            TimeSpan ts = TimeSpan.FromSeconds(seconds);
            string timefromsec = "";
            if (ts.Days != 0)
                timefromsec += $"{ts.Days} Days, ";
            if (ts.Hours != 0)
                timefromsec += $"{ts.Hours} Hours, ";
            if (ts.Minutes != 0)
                timefromsec += $"{ts.Minutes} Minutes";
            if (ts.Seconds != 0)
                timefromsec += $", and {ts.Seconds}";
            await chantimer.ModifyAsync(x => x.Name = $"Time: {timefromsec}");
        }
        internal async Task AllowBans()
        {
            Global.GiveawayBans = true;
            var guild = _client.GetGuild(Global.GiveAwayGuildID);
            ulong id = guild.Channels.FirstOrDefault(x => x.Name == "Contestants").Id;
            await guild.GetTextChannel(id).SendMessageAsync("@everyone BANS ARE NOW ACTIVE!! Use `\"ban @user` to ban people! you cannot ban admins so dont try");
        }
    }
}
