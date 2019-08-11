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

namespace KindomKeeper.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("test")]
        public async Task test()
        {
            await Context.Channel.SendMessageAsync("Swiggity swag motherflok");
        }
        [Command("modify")]
        public async Task modify(string configItem, string newValue)
        {
            //add config modifier
        }
        [Command("ban")]
        public async Task ban(string userstring)
        {
            string rg = userstring.Trim('<', '>', '@');
            ulong d = Convert.ToUInt64(rg);
            SocketGuildUser user = Context.Guild.GetUser(d);
            SocketRole role = Context.Guild.Roles.FirstOrDefault(r => r.Position < Context.Guild.Roles.FirstOrDefault(n => n.Name == "Admin").Position);
            //if (user.Roles.Contains)
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
    }
}
