using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KindomKeeper
{
    class CommandHandler
    {
        private static DiscordSocketClient _client;
        private CommandService _service;
        internal static bool giveawayinProg = false;
        internal static int giveawayStep = 0;
        internal static GiveAway currGiveaway;

        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
            
            _client.SetGameAsync(Global.Status, null, ActivityType.Listening);

            _client.SetStatusAsync(UserStatus.Online);

            _service = new CommandService();

            _service.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            _client.UserJoined += checkBot;

            _client.UserJoined += UserJoined;

            _client.UserUpdated += checkAddRole;

            _client.Ready += _client_Connected;

            _client.MessageReceived += checkGiveaway;

            _client.MessageReceived += HandleCommandAsync;

            Console.WriteLine("[" + DateTime.Now.TimeOfDay + "] - " + "Services loaded");
        }

        

        private async Task UserJoined(SocketGuildUser arg)
        {
            EmbedBuilder b = new EmbedBuilder();
            b.Color = Color.Green;
            b.ImageUrl = "https://cdn.discordapp.com/attachments/608080803733176325/610360300880789514/17fd2ebcb1f2.gif";
            b.Title = $"***Welcome, {arg.Username}#{arg.Discriminator}!***";
            b.Description = welcomeMessageBuilder(arg, Global.welcomeMessage, arg.Guild);
            b.Footer = new EmbedFooterBuilder();
            b.Footer.IconUrl = arg.GetAvatarUrl();
            b.Footer.Text = $"{arg.Username}#{arg.Discriminator}";
            await _client.GetGuild(Global.GuildID).GetTextChannel(Global.WelcomemessagechannelID).SendMessageAsync("", false, b.Build());
        }
        internal static string welcomeMessageBuilder(SocketGuildUser user, string welcomeMessage, SocketGuild guild) //inputs are: (user) -> pings user 
        {
            string newmsg = welcomeMessage;
            newmsg = newmsg.Replace("(user)", user.Mention);
            newmsg = newmsg.Replace("(usercount)", $"{guild.Users.Count}st");
            return newmsg;
        }
        private async Task checkAddRole(SocketUser arg1, SocketUser arg2)
        {
            var user = (SocketGuildUser)arg1;
            if(user.IsBot)
            {
                if (BotList.botList.Contains(user))
                {
                    if(user.Roles.Count >= 1)
                    {
                        foreach(var role in user.Roles)
                        {
                            await user.RemoveRoleAsync(role);
                        }
                    }
                }
            }
        }

        private async Task checkBot(SocketGuildUser arg)
        {
            if(arg.IsBot)
            {
                foreach(var role in arg.Roles)
                {
                    await arg.RemoveRoleAsync(role);
                }
                BotList.botList.Add(arg);

                EmbedBuilder b = new EmbedBuilder();
                b.Color = Color.Red;
                b.Description = $"Someone has invited a bot called: {arg.Username} to the server, Please type **\"Allow** to allow this bot";
                b.Footer.Text = DateTime.UtcNow.ToString();
                b.Footer.IconUrl = "http://icons.iconseeker.com/png/fullsize/sleek-xp-software/windows-close-program-1.png";
                b.Title = "Unknown Bot!";
                await _client.GetGuild(Global.GuildID).GetUser(Global.jakeID).SendMessageAsync("", false, b.Build());
            }
        }

        private async Task _client_Connected()
        {
            
        }
        private void test(string[] arg) { }
        public async Task HandleCommandAsync(SocketMessage s)
        {
            
            var msg = s as SocketUserMessage;
            if (msg == null) return;

            var context = new SocketCommandContext(_client, msg);
            

            int argPos = 0;
            if (msg.HasCharPrefix(Global.preflix, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos, null, MultiMatchHandling.Best);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    EmbedBuilder b = new EmbedBuilder();
                    b.Color = Color.Red;
                    b.Description = $"The following info is the Command error info, `{msg.Author.Username}#{msg.Author.Discriminator}` tried to use the `{msg}` Command in {msg.Channel}: \n \n **COMMAND ERROR**: ```{result.Error.Value}``` \n \n **COMMAND ERROR REASON**: ```{result.ErrorReason}```";
                    b.Author = new EmbedAuthorBuilder();
                    b.Author.Name = msg.Author.Username + "#" + msg.Author.Discriminator;
                    b.Author.IconUrl = msg.Author.GetAvatarUrl();
                    b.Footer = new EmbedFooterBuilder();
                    b.Footer.Text = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " ZULU";
                    b.Title = "Bot Command Error!";
                    await _client.GetGuild(Global.DevGuildID).GetTextChannel(Global.devlogchannel).SendMessageAsync("", false, b.Build());
                }
            }
        }
        internal static async Task giveaway(ulong givawayerID, ulong channelID, SocketCommandContext context)
        {
            GiveAway ga = new GiveAway();
            currGiveaway = ga;
            currGiveaway.GiveAwayUser = givawayerID;
            EmbedBuilder eb = new EmbedBuilder();
            eb.Color = Color.Blue;
            eb.Title = "**Giveaway Builder**";
            eb.Description = $"Welcome {context.Message.Author.Username}{context.Message.Author.Discriminator} to the Giveaway Creator, follow these steps to create a giveaway. \n \n ***Step One*** \n `Enter the time in DD:HH:MM:SS format. ex 1:12:30:00 would be 1 day 12 hours and 30 minutes`";
            eb.Footer = new EmbedFooterBuilder();
            eb.Footer.Text = "to redo a step type **\"redo**";
            await context.Channel.SendMessageAsync("", false, eb.Build());
            giveawayStep++;
            giveawayinProg = true;
        }
        internal static async Task checkGiveaway(SocketMessage msg)
        {
            if (!msg.Author.IsBot)
            {
                if(giveawayinProg)
                {
                    if (msg.Channel.Id == Global.AdminGivawayChannelID)
                    {
                        if (giveawayStep == 1)
                        {
                            try
                            {
                                string[] args = msg.ToString().Split(':');
                                int seconds = 0;
                                if (args.Length == 4)
                                {
                                    int days = Convert.ToInt32(args[0]); //days
                                    seconds = seconds + days * 24 * 60 * 60;

                                    int hours = Convert.ToInt32(args[1]);
                                    seconds = seconds + (hours * 60 * 60);

                                    int minutes = Convert.ToInt32(args[2]);
                                    seconds = seconds + (minutes * 60);

                                    int secs = Convert.ToInt32(args[3]);
                                    seconds = seconds + secs;
                                    Console.WriteLine($"{msg.Author.Username} Created a giveaway with the time of {seconds}");
                                    EmbedBuilder eb = new EmbedBuilder();
                                    eb.Color = Color.Blue;
                                    eb.Footer = new EmbedFooterBuilder();
                                    eb.Footer.Text = "to redo a step type **\"redo**";
                                    eb.Title = "**Giveaway Step 1**";
                                    string time = "";
                                    if (days != 0)
                                        time += $"{days} Days, ";
                                    if (hours != 0)
                                        time += $"{hours} Hours, ";
                                    if (minutes != 0)
                                        time += $"{minutes} Minutes";
                                    if (secs != 0)
                                        time += $" and {secs} Seconds.";

                                    eb.Description = $"Time set to **{time}** ({seconds}) seconds \n\n **Next Step** \n What are you giving away?";
                                    currGiveaway.Seconds = seconds;
                                    await msg.Channel.SendMessageAsync("", false, eb.Build());
                                    giveawayStep++;
                                }
                                else
                                {
                                    await msg.Channel.SendMessageAsync("Invalad Time!");
                                }
                            }
                            catch (Exception ex)
                            {
                                await Exhandle(ex, msg);
                            }
                        }
                        if(giveawayStep == 2)
                        {
                            try
                            {
                                currGiveaway.GiveAwayItem = msg.ToString();
                                EmbedBuilder eb = new EmbedBuilder();
                                eb.Title = "Giveaway Item";
                                eb.Color = Color.Blue;
                                eb.Description = $"The **Giveaway Item** is now set to: \n `{currGiveaway.GiveAwayItem}` \n\n **Next Step** \n how many winners should there be?";
                                eb.Footer = new EmbedFooterBuilder();
                                eb.Footer.Text = "to redo a step type **\"redo**";
                                giveawayStep++;
                                await msg.Channel.SendMessageAsync("", false, eb.Build());
                            }
                            catch(Exception ex)
                            {
                                await Exhandle(ex, msg);
                            }
                        }
                        if(giveawayStep == 3)
                        {
                            try
                            {
                                int numPeople = Convert.ToInt32(msg.ToString());
                                currGiveaway.numWinners = numPeople;
                                EmbedBuilder eb = new EmbedBuilder();
                                eb.Title = "**Confirm?**";
                                eb.Color = Color.Blue;
                                string timefromsec = "";
                                TimeSpan ts = TimeSpan.FromSeconds(currGiveaway.Seconds);
                                if (ts.Days != 0)
                                    timefromsec += $"{ts.Days} Days, ";
                                if (ts.Hours != 0)
                                    timefromsec += $"{ts.Hours} Hours, ";
                                if (ts.Minutes != 0)
                                    timefromsec += $"{ts.Minutes} Minutes";
                                if (ts.Seconds != 0)
                                    timefromsec += $", and {ts.Seconds}";
                                
                                eb.Description = $"Are you sure with these settings? \n\n **GiveawayItem** \n`{currGiveaway.GiveAwayItem}` \n \n **Winners** \n`{currGiveaway.numWinners}` \n\n **Giveawayer** \n `{currGiveaway.GiveAwayUser}` \n\n **Time**\n`{timefromsec}` \n\n to confirm these setting type `confirm` to redo a step type `\"redo`";
                                giveawayStep++;
                                await msg.Channel.SendMessageAsync("", false, eb.Build());
                            }
                            catch(Exception ex)
                            {
                                await msg.Channel.SendMessageAsync($"Uh oh, Looks like we have had a boo boo: {ex.Message}");
                                await Exhandle(ex, msg);
                            }
                        }
                        if(giveawayStep == 4)
                        {
                            //do the channel thing lol
                        }
                    }
                }
            }
        }
        private static async Task Exhandle(Exception ex, SocketMessage msg)
        {
            EmbedBuilder b = new EmbedBuilder();
            b.Color = Color.Red;
            b.Description = $"The following info is the Command error info, `{msg.Author.Username}#{msg.Author.Discriminator}` tried to use the `{msg}` Command in {msg.Channel}: \n \n **COMMAND ERROR**: ```{ex.Message}``` \n \n **COMMAND ERROR REASON**: ```{ex}```";
            b.Author = new EmbedAuthorBuilder();
            b.Author.Name = msg.Author.Username + "#" + msg.Author.Discriminator;
            b.Author.IconUrl = msg.Author.GetAvatarUrl();
            b.Footer = new EmbedFooterBuilder();
            b.Footer.Text = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " ZULU";
            b.Title = "Bot Command Error!";
            await _client.GetGuild(Global.DevGuildID).GetTextChannel(Global.devlogchannel).SendMessageAsync("", false, b.Build());
        }
        public struct GiveAway
        {
            public int Seconds { get; set; }
            public string GiveAwayItem { get; set; }
            public ulong GiveAwayUser { get; set; }
            public string discordInvite { get; set; }
            public int numWinners { get; set; }
        }
    }
}
