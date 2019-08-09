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
            
            _client.SetGameAsync("0.0.1 Testing build", null, ActivityType.Playing);

            _client.SetStatusAsync(UserStatus.Online);

            _service = new CommandService();

            _service.AddModulesAsync(Assembly.GetEntryAssembly(), null);


            _client.Ready += _client_Connected;
            _client.MessageReceived += HandleCommandAsync;

            Console.WriteLine("[" + DateTime.Now.TimeOfDay + "] - " + "Services loaded");
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

                }
            }
        }
    }
}
