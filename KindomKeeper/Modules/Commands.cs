using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Discord.Rest;
using Discord;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;

namespace KindomKeeper.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("welcome")]
        public async Task test()
        {
            var arg = Context.Guild.GetUser(Context.Message.Author.Id);
            EmbedBuilder b = new EmbedBuilder();
            b.Color = Color.Green;
            b.ImageUrl = "https://cdn.discordapp.com/attachments/608080803733176325/610360300880789514/17fd2ebcb1f2.gif";
            b.Title = $"***Welcome, {arg.Username}#{arg.Discriminator}!***";
            b.Description = CommandHandler.welcomeMessageBuilder(arg, Global.welcomeMessage, Context.Guild);
            b.Footer = new EmbedFooterBuilder();
            b.Footer.IconUrl = arg.GetAvatarUrl();
            b.Footer.Text = $"{arg.Username}#{arg.Discriminator}";
            await Context.Channel.SendMessageAsync("", false, b.Build());
        }
        [Command("modify")]
        public async Task modify(string configItem, string value)
        {
            string newvalue = value.Replace("\\", " ");
            if(checkDebugServer(Context))//allow full modify
            {
                if (Global.JsonItemsListDevOps.Keys.Contains(configItem))
                {
                    JsonData data = Global.CurrentJsonData;
                    data = modifyJsonData(data, configItem, newvalue);
                    if(data.Token != null)
                    {
                        Global.saveConfig(data);
                        await Context.Channel.SendMessageAsync($"Sucessfuly modified the config, Updated the item {configItem} with the new value of {value}");
                        EmbedBuilder b = new EmbedBuilder();
                        b.Footer = new EmbedFooterBuilder();
                        b.Footer.Text = "**Dev Config**";
                        b.Title = "Dev Config List";
                        string list = "**Here is the current config file** \n";
                        foreach (var item in Global.JsonItemsListDevOps) { list += $"```json\n \"{item.Key}\" : \"{item.Value}\"```\n"; }
                        b.Description = list;
                        b.Color = Color.Green;
                        b.Footer.Text = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " ZULU";
                        await Context.Channel.SendMessageAsync("", false, b.Build());
                    }
                    
                }
                else { await Context.Channel.SendMessageAsync($"Could not find the config item {configItem}! Try `{Global.preflix}modify list` for a list of the Config!"); }
            }
            if(checkMainServer(Context))
            {
                if(checkTestingChannel(Context))//allow some modify
                {
                    if (Global.jsonItemsList.Keys.Contains(configItem))
                    {
                        JsonData data = Global.CurrentJsonData;
                        data = modifyJsonData(data, configItem, newvalue);
                        if (data.Token != null)
                        {
                            Global.saveConfig(data);
                            await Context.Channel.SendMessageAsync($"Sucessfuly modified the config, Updated the item {configItem} with the new value of {value}");
                            EmbedBuilder b = new EmbedBuilder();
                            b.Footer = new EmbedFooterBuilder();
                            b.Footer.Text = "**Admin Config**";
                            b.Title = "Admin Config List";
                            string list = "**Here is the current config file** \n";
                            foreach (var item in Global.jsonItemsList) { list += $"```json\n \"{item.Key}\" : \"{item.Value}\"```\n"; }
                            b.Description = list;
                            b.Color = Color.Green;
                            b.Footer.Text = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " ZULU";
                            await Context.Channel.SendMessageAsync("", false, b.Build());
                        }
                    }
                    else
                    {
                        if(Global.JsonItemsListDevOps.Keys.Contains(configItem))
                        {
                            EmbedBuilder b = new EmbedBuilder();
                            b.Color = Color.Red;
                            b.Title = "You need Better ***PERMISSION***";
                            b.Description = "You do not have permission to modify this item, if you think this is incorrect you can DM quin#3017 for help";

                            await Context.Channel.SendMessageAsync("", false, b.Build());
                        }
                        else { await Context.Channel.SendMessageAsync($"Could not find the config item {configItem}! Try `{Global.preflix}modify list` for a list of the Config!"); }
                    }

                }
            }
        }
        [Command("modify")]
        public async Task modify(string configItem)
        {
            if(configItem == "list")
            {
                if(checkDebugServer(Context))
                {
                    EmbedBuilder b = new EmbedBuilder();
                    b.Footer = new EmbedFooterBuilder();
                    b.Footer.Text = "**Dev Config**";
                    b.Title = "Dev Config List";
                    string list = "**Here is the current config file** \n";
                    foreach(var item in Global.JsonItemsListDevOps) { list += $"```json\n \"{item.Key}\" : \"{item.Value}\"```\n"; }
                    b.Description = list;
                    b.Color = Color.Green;
                    b.Footer.Text = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " ZULU";
                    await Context.Channel.SendMessageAsync("", false, b.Build());
                }
                else
                {
                    if(checkTestingChannel(Context))
                    {
                        EmbedBuilder b = new EmbedBuilder();
                        b.Footer = new EmbedFooterBuilder();
                        b.Footer.Text = "**Admin Config**";
                        b.Title = "Admin Config List";
                        string list = "**Here is the current config file** \n";
                        foreach (var item in Global.jsonItemsList) { list += $"```json\n \"{item.Key}\" : \"{item.Value}\"```\n"; }
                        b.Description = list;
                        b.Color = Color.Green;
                        b.Footer.Text = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " ZULU";
                        await Context.Channel.SendMessageAsync("", false, b.Build());
                    }
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"No value was provided for the variable `{configItem}`");
            }
        }
        [Command("ban")]
        public async Task ban(string userstring, string reason)
        {
            string rg = userstring.Trim('<', '>', '@');
            ulong d = Convert.ToUInt64(rg);
            SocketGuildUser user = Context.Guild.GetUser(d);
            if (user.Roles.Contains(Context.Guild.Roles.FirstOrDefault(r => r.Id == Global.developerRoleID))) { await Context.Channel.SendMessageAsync($"You cannot ban {user.Mention} because they make this bot!"); return; }
            if (goodPerms(Context, Context.Guild.GetUser(Context.Message.Author.Id), user))
            {
                try
                {
                    var checkbanlimit = Global.banTimers.FirstOrDefault(t => t.user == Context.Guild.GetUser(Context.Message.Author.Id));
                    BanLimitTimer blt = new BanLimitTimer();
                    blt.Bans = 1;
                    blt.user = Context.Guild.GetUser(Context.Message.Author.Id);
                    blt.loopingTimer.Enabled = true;
                    await Context.Guild.AddBanAsync(user, 0, reason);
                    string banNum = "";
                    switch (blt.Bans)
                    {
                        case 1:
                            banNum = "1st";
                            break;
                        case 2:
                            banNum = "2nd";
                            break;
                        case 3:
                            banNum = "3rd";
                            break;
                    }
                    Global.banTimers.Add(blt);
                    if (banNum == "") { banNum = $"{blt.Bans}th"; }
                    await Context.Channel.SendMessageAsync($"Banned {user.Mention} with reason: \"{reason}\", This is your {banNum} ban per hour!");
                }
                catch (Exception ex)
                {
                    var checkbanlimit = new BanLimitTimer();
                    if (checkbanlimit.Bans == Global.BanRateLimit)
                    {
                        await Context.Channel.SendMessageAsync($"Cannot ban {user.Mention} Because you have reach the ban limit of {Global.BanRateLimit} per Hour!");
                    }
                    else
                    {
                        checkbanlimit.Bans = checkbanlimit.Bans + 1;
                        await Context.Guild.AddBanAsync(user, 0, reason);
                        string banNum = "";
                        switch (checkbanlimit.Bans)
                        {
                            case 1:
                                banNum = "1st";
                                break;
                            case 2:
                                banNum = "2nd";
                                break;
                            case 3:
                                banNum = "3rd";
                                break;
                        }
                        if (banNum == "") { banNum = $"{checkbanlimit}th"; }
                        await Context.Channel.SendMessageAsync($"Banned {user.Mention} with reason: \"{reason}\", This is your {banNum} ban per hour!");
                    }
                }
            }
            else { await Context.Channel.SendMessageAsync($"you do not have perm to ban {user.Mention}!"); }
        }
        [Command("ban")]
        public async Task ban(string userstring)
        {
            if(Context.Guild.Id == Global.GiveAwayGuildID)
            {
                if(Global.GiveawayBans)
                {
                    var reciv = Context.Client.GetGuild(Global.GiveAwayGuildID).GetUser(Convert.ToUInt64(userstring.Trim('<', '>', '@')));
                    
                    if (reciv.Roles.Contains(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Admins")))
                    {
                        await Context.Channel.SendMessageAsync("Cannot ban Admins!");
                    }
                    else if(reciv.Roles.Contains(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Contestants")))
                    {
                        await Context.Channel.SendMessageAsync($"{reciv.Mention} HAS BEEN ELIMINATED BY {Context.Message.Author.Mention}: {Context.Guild.Users.Count(x => x.Roles.Contains(Context.Guild.Roles.FirstOrDefault(r => r.Name == "Contestants")))} Contestants left");
                        await Context.Guild.AddBanAsync(reciv);
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync($"Cannot ban {reciv.Mention} because there not a Contestant");
                    }
                }
                else { await Context.Channel.SendMessageAsync($"Giveaway Purge not ready.."); }
            }
            else
            {
                string rg = userstring.Trim('<', '>', '@');
                ulong d = Convert.ToUInt64(rg);
                SocketGuildUser user = Context.Guild.GetUser(d);
                await Context.Channel.SendMessageAsync($"you need to provide a one word reason to ban {user.Mention}!");
            }
        }
        [Command("ban")]
        public async Task ban()
        {
            await Context.Channel.SendMessageAsync($"you need to provide a person and a one word reson to ban!");
        }
        [Command("commandlogs")]
        public async Task logs(string name)
        {
            if(Context.Guild.Id == Global.DevGuildID)
            {
                if (name == "list")
                {
                    string names = "";
                    foreach (var file in Directory.GetFiles(Global.CommandLogsDir))
                    {
                        names += file + "\n";
                    }
                    EmbedBuilder eb = new EmbedBuilder();
                    eb.Color = Color.Green;
                    eb.Title = "**Command logs List**";
                    eb.Description = $"Here are the current Command Logs, To fetch one do `\"commandlogs (name) \n ```{name}```";

                }
                else
                {
                    if (File.Exists(Global.CommandLogsDir + $"\\{name}"))
                    {
                        await Context.Channel.SendFileAsync(Global.CommandLogsDir + $"\\{name}", $"Here is the Log **{name}**");
                    }
                    else
                    {
                        EmbedBuilder eb = new EmbedBuilder();
                        eb.Color = Color.Red;
                        eb.Title = "**Command logs List**";
                        eb.Description = $"The file {name} does not exist, try doing `\"commandlogs list` to view all the command logs";
                        await Context.Channel.SendMessageAsync("", false, eb.Build());
                    }
                }
            }
            else
            {
                if(Context.Channel.Id == Global.KeeperLogsChanId)
                {
                    if (name == "list")
                    {
                        string names = "";
                        foreach (var file in Directory.GetFiles(Global.CommandLogsDir))
                        {
                            names += file + "\n";
                        }
                        EmbedBuilder eb = new EmbedBuilder();
                        eb.Color = Color.Green;
                        eb.Title = "**Command logs List**";
                        eb.Description = $"Here are the current Command Logs, To fetch one do `\"commandlogs (name) \n ```{name}```";
                        await Context.Channel.SendMessageAsync("", false, eb.Build());
                    }
                    else
                    {
                        if (File.Exists(Global.CommandLogsDir + $"\\{name}"))
                        {
                            await Context.Channel.SendFileAsync(Global.CommandLogsDir + $"\\{name}", $"Here is the Log **{name}**");
                        }
                        else
                        {
                            EmbedBuilder eb = new EmbedBuilder();
                            eb.Color = Color.Red;
                            eb.Title = "**Command logs List**";
                            eb.Description = $"The file {name} does not exist, try doing `\"commandlogs list` to view all the command logs";
                        }
                    }
                }
            }
        }
        [Command("messagelogs")]
        public async Task mlogs(string name)
        {
            if (Context.Guild.Id == Global.DevGuildID)
            {
                if (name == "list")
                {
                    string names = "";
                    foreach (var file in Directory.GetFiles(Global.MessageLogsDir))
                    {
                        names += file + "\n";
                    }
                    EmbedBuilder eb = new EmbedBuilder();
                    eb.Color = Color.Green;
                    eb.Title = "**Message logs List**";
                    eb.Description = $"Here are the current Message Logs, To fetch one do `\"messagelogs (name) \n ```{name}```";

                }
                else
                {
                    if (File.Exists(Global.MessageLogsDir + $"\\{name}"))
                    {
                        await Context.Channel.SendFileAsync(Global.MessageLogsDir + $"\\{name}", $"Here is the Log **{name}**");
                    }
                    else
                    {
                        EmbedBuilder eb = new EmbedBuilder();
                        eb.Color = Color.Red;
                        eb.Title = "**Message logs List**";
                        eb.Description = $"The file {name} does not exist, try doing `\"messagelogs list` to view all the command logs";
                        await Context.Channel.SendMessageAsync("", false, eb.Build());
                    }
                }
            }
            else
            {
                if (Context.Channel.Id == Global.KeeperLogsChanId)
                {
                    if (name == "list")
                    {
                        string names = "";
                        foreach (var file in Directory.GetFiles(Global.MessageLogsDir))
                        {
                            names += file + "\n";
                        }
                        EmbedBuilder eb = new EmbedBuilder();
                        eb.Color = Color.Green;
                        eb.Title = "**Message logs List**";
                        eb.Description = $"Here are the current Message Logs, To fetch one do `\"commandlogs (name) \n ```{name}```";
                        await Context.Channel.SendMessageAsync("", false, eb.Build());
                    }
                    else
                    {
                        if (File.Exists(Global.MessageLogsDir + $"\\{name}"))
                        {
                            await Context.Channel.SendFileAsync(Global.MessageLogsDir + $"\\{name}", $"Here is the Log **{name}**");
                        }
                        else
                        {
                            EmbedBuilder eb = new EmbedBuilder();
                            eb.Color = Color.Red;
                            eb.Title = "**Message logs List**";
                            eb.Description = $"The file {name} does not exist, try doing `\"messagelogs list` to view all the command logs";
                        }
                    }
                }
            }
        }
        [Command("giveaway")]
        public async Task giveaway()
        {
            if(Context.Channel.Id == Global.AdminGivawayChannelID)
            {
                Console.WriteLine($"Giveaway Started by {Context.Message.Author.Username}{Context.Message.Author.Discriminator}");
                await CommandHandler.giveaway(Context.Message.Author.Id, Global.GiveawayChanID, Context);
            }
        }
        [Command("deletegiveaways")]
        public async Task delgiveaway()
        {
            if (Context.Guild.Id == Global.DevGuildID)
            {
                foreach(var guild in Context.Client.Guilds)
                {
                    if(guild.Name.Contains("Giveaway"))
                    {
                        await Context.Channel.SendMessageAsync($"Leaving {guild.Name}...");
                        await guild.DeleteAsync();
                    }
                }
            }
        }
        [Command("cloneroles")]
        public async Task cloneroles()
        {
            if (checkDebugServer(Context))
            {
                SocketRole[] sra = new SocketRole[Global.Client.GetGuild(Global.GuildID).Roles.Count];
                foreach (var item in Global.Client.GetGuild(Global.GuildID).Roles) sra[item.Position] = item;
                foreach (var role in sra.Reverse())
                {
                    await Global.Client.GetGuild(Global.DevGuildID).CreateRoleAsync(role.Name, role.Permissions, role.Color, role.IsHoisted);
                    Console.WriteLine($"Created {role.Name}!");
                }
            }
        }
        [Command("clonechannels")]
        public async Task cc()
        {
            if(checkDebugServer(Context))
            {
                //SocketCategoryChannel[] scc = new SocketCategoryChannel[];
                //foreach (var catagory in Global.Client.GetGuild(Global.GuildID).CategoryChannels) scc[catagory.Position] = catagory;
                //foreach(var cat in Global.Client.GetGuild(Global.GuildID).CategoryChannels)
                //{
                //    await Global.Client.GetGuild(Global.DevGuildID).CreateCategoryChannelAsync(cat.Name, p => {
                //        p.Name = cat.Name;
                //        p.Position = cat.Position;
                //        p.CategoryId = cat.Id;
                //    });
                //    Console.WriteLine($"Created Catagory: {cat.Name}!");
                //}

                SocketChannel[] sra = new SocketChannel[Global.Client.GetGuild(Global.GuildID).Channels.Count];
                foreach (var item in Global.Client.GetGuild(Global.GuildID).Channels) sra[item.Position] = item;
                foreach (var channel in sra)
                {
                    if (channel.GetType() == typeof(SocketTextChannel))
                    {
                        var nch = (SocketTextChannel)channel;
                        //TextChannelProperties tcp = new TextChannelProperties();
                        
                        var chanl = await Global.Client.GetGuild(Global.DevGuildID).CreateTextChannelAsync(nch.Name);
                        await chanl.ModifyAsync(tcp =>
                        {
                            tcp.CategoryId = Global.Client.GetGuild(Global.GuildID).CategoryChannels.FirstOrDefault(c => c.Name == nch.Category.Name).Id;
                            tcp.IsNsfw = nch.IsNsfw;
                            //tcp.Name = nch.Name;
                            //tcp.Position = nch.Position;
                            //tcp.SlowModeInterval = nch.SlowModeInterval;
                            tcp.Topic = nch.Topic;
                        });

                        Console.WriteLine($"Created Channel: {nch.Name}");
                    }
                    //if (channel.GetType() == typeof(SocketVoiceChannel))
                    //{
                    //    var nch = (SocketVoiceChannel)channel;

                    //    await Global.Client.GetGuild(Global.DevGuildID).CreateTextChannelAsync(nch.Name, tcp => {
                    //        tcp.CategoryId = Global.Client.GetGuild(Global.GuildID).CategoryChannels.FirstOrDefault(c => c.Name == nch.Category.Name).Id;
                    //        tcp.Name = nch.Name;
                    //        tcp.Position = nch.Position;
                        
                            
                    //    });
                    //    Console.WriteLine($"Created Channel: {nch.Name}");
                    //}
                }
            }
        }

        [Command("deleteroles")]
        public async Task dr()
        {
            if (checkDebugServer(Context))
            {
                var c = Global.Client.GetGuild(Global.DevGuildID).Roles;
                foreach (var role in c)
                {

                    await Global.Client.GetGuild(Global.DevGuildID).GetRole(role.Id).DeleteAsync();
                    Console.WriteLine($"Deleted {role.Name}!");
                }
            }
        }
        public bool checkTestingChannel(SocketCommandContext Context)
        {
            if (Context.Channel.Id == 609620857345540106 || Context.Channel.Id == 609620916606861312 || Context.Channel.Id == 609620964799414296) { return true; }
            else return false; 
        }
        public bool checkDebugServer(SocketCommandContext Context)
        {
            if (Context.Guild.Id == Global.DevGuildID) return true; 
            else return false;
        }
        public bool checkMainServer(SocketCommandContext Context)
        {
            if (Context.Guild.Id == Global.GuildID) return true; 
            else return false;
        }
        public JsonData modifyJsonData(JsonData data, string configItem, string value)
        {
            try
            {
                switch (configItem)
                {
                    case "ChannelID":
                        data.ChannelID = Convert.ToUInt64(value);
                        break;
                    case "DevBotLogsChannel":
                        data.DevBotLogsChannel = Convert.ToUInt64(value);
                        break;
                    case "DevChannelID":
                        data.DevChannelID = Convert.ToUInt64(value);
                        break;
                    case "JakeeID":
                        data.JakeeID = Convert.ToUInt64(value);
                        break;
                    case "Preflix":
                        data.Preflix = value.ToCharArray().First().ToString();
                        break;
                    case "StatusMessage":
                        data.StatusMessage = value;
                        Context.Client.SetGameAsync(value, null, ActivityType.Listening);
                        break;
                    case "WelcomeMessage":
                        data.WelcomeMessage = value;
                        break;
                    case "Welcomemessagechannel":
                        data.Welcomemessagechannel = Convert.ToUInt64(value);
                        break;
                    case "Rules":
                        data.Rules = Convert.ToUInt64(value);
                        break;
                    case "AdminRole":
                        data.AdminRole = Convert.ToUInt64(value);
                        break;
                    case "DeveloperRole":
                        data.DeveloperRole = Convert.ToUInt64(value);
                        break;
                }
                return data;
            }
            catch(Exception ex)
            {
                EmbedBuilder b = new EmbedBuilder()
                {
                    Color = Color.Red,
                    Title = "Exeption!",
                    Description = $"**{ex}**"
                };
                Context.Channel.SendMessageAsync("", false, b.Build());
                return new JsonData();
            }
            
        }
        private bool goodPerms(SocketCommandContext context, SocketGuildUser sender, SocketGuildUser reciever)
        {
            var senderroles = sender.Roles;
            var recieverroles = reciever.Roles;
            int adminrolepos = context.Guild.Roles.FirstOrDefault(n => n.Id == 565747713778647051).Position;
            foreach (var role in senderroles)
            {
                if (role.Position > adminrolepos)
                {
                    foreach (var rl in recieverroles)
                    {
                        if (rl.Position >= adminrolepos)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

    }
}
