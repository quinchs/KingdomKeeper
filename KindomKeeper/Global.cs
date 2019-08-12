using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindomKeeper
{
    class Global
    {
        internal static DiscordSocketClient Client { get; set; }
        internal static string jsonAccountFilePath = Environment.CurrentDirectory + @"\Data\UserAccounts.json";
        internal static string jsonGlobalData = Environment.CurrentDirectory + @"\Data\GlobalData.json";
        internal static string BotToken { get; set; }
        internal static ulong GuildID { get; set; }
        internal static ulong DevGuildID { get; set; }
        internal static string Status { get; set; }
        internal static ulong jakeID { get; set; }
        internal static ulong devlogchannel { get; set; }


        internal static void readConfig()
        {
            var data = JsonConvert.DeserializeObject<JsonData>(File.ReadAllText(jsonGlobalData));
            BotToken = data.Token;
            GuildID = data.ChannelID;
            DevGuildID = data.DevChannelID;
            Status = data.StatusMessage;
            jakeID = data.JakeeID;
            devlogchannel = data.DevBotLogsChannel;
        }
    }
    public struct JsonData
    {
        public string Token { get; set; }
        public ulong ChannelID { get; set; }
        public ulong DevChannelID { get; set; }
        public string StatusMessage { get; set; }
        public ulong JakeeID { get; set; }
        public ulong DevBotLogsChannel { get; set; }

    }
    public struct BotList
    {
        public static List<SocketGuildUser> botList { get; set; }
    }
}
