using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KindomKeeper
{
    class CommandHandler
    {
        private static DiscordSocketClient _client;
        private CommandService _service;

        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
            
            _client.SetGameAsync(Global.Status, null, ActivityType.Playing);

            _client.SetStatusAsync(UserStatus.Online);

            _service = new CommandService();

            _service.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            _client.UserJoined += checkBot;

            _client.UserUpdated += checkAddRole;

            _client.Ready += _client_Connected;
            _client.MessageReceived += HandleCommandAsync;

            Console.WriteLine("[" + DateTime.Now.TimeOfDay + "] - " + "Services loaded");
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
        public async Task HandleCommandAsync(SocketMessage s)
        {

            var msg = s as SocketUserMessage;
            if (msg == null) return;

            var context = new SocketCommandContext(_client, msg);


            int argPos = 0;
            if (msg.HasCharPrefix('"', ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos, null, MultiMatchHandling.Best);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    EmbedBuilder b = new EmbedBuilder();
                    b.Color = Color.Red;
                    b.Description = $"The following info is the Command error info, `{msg.Author.Username}#{msg.Author.Discriminator}` tried to use the `{msg}` Command: \n \n **COMMAND ERROR**: ```{result.Error.Value}``` \n \n **COMMAND ERROR REASON**: ```{result.ErrorReason}```";
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
    }
}
