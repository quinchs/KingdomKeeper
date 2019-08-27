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
        internal static string CommandLogsDir = Environment.CurrentDirectory + @"\CommandLogs\";
        internal static string MessageLogsDir = Environment.CurrentDirectory + @"\MessageLogs\";
        internal static List<LogFiles> CommandLogsList = new List<LogFiles>();
        internal static List<LogFiles> MessageLogsList = new List<LogFiles>();
        internal static string BotToken { get; set; }
        internal static ulong GuildID { get; set; }
        internal static ulong DevGuildID { get; set; }
        internal static string Status { get; set; }
        internal static ulong jakeID { get; set; }
        internal static ulong devlogchannel { get; set; }
        internal static string welcomeMessage { get; set; }
        internal static ulong WelcomemessagechannelID { get; set; }
        internal static Dictionary<string, string> jsonItemsList { get; private set; }
        internal static Dictionary<string, string> JsonItemsListDevOps { get; private set; }
        internal static char preflix { get; set; }
        internal static ulong rulesID { get; set; }
        internal static JsonData CurrentJsonData;
        internal static ulong adminRoleID { get; set; }
        internal static ulong developerRoleID { get; set; }
        internal static int BanRateLimit { get; set; }
        internal static List<BanLimitTimer> banTimers { get; set; }
        internal static ulong AdminGivawayChannelID { get; set; }
        internal static ulong GiveawayChanID { get; set; }
        internal static ulong GiveAwayGuildID { get; set; }
        internal static bool GiveawayBans { get; set; }
        internal static ulong KeeperLogsChanId { get; set; }
        internal static string WecomemessageURL { get; set; }
        internal static ulong JakeeGuildDebugChanID { get; set; }

        internal static void readConfig()
        {
            if (!Directory.Exists(CommandLogsDir)) { Directory.CreateDirectory(CommandLogsDir); }
            if (!Directory.Exists(MessageLogsDir)) { Directory.CreateDirectory(MessageLogsDir); }
            var data = JsonConvert.DeserializeObject<JsonData>(File.ReadAllText(jsonGlobalData));
            CurrentJsonData = data;
            BotToken = data.Token;
            GuildID = data.ChannelID;
            rulesID = data.Rules;
            DevGuildID = data.DevChannelID;
            Status = data.StatusMessage;
            jakeID = data.JakeeID;
            devlogchannel = data.DevBotLogsChannel;
            welcomeMessage = data.WelcomeMessage;
            WelcomemessagechannelID = data.Welcomemessagechannel;
            preflix = data.Preflix.ToCharArray().First();
            jsonItemsList = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(jsonGlobalData));
            jsonItemsList.Remove("Token"); //Dont want the token in there >:)
            jsonItemsList.Remove("Preflix");
            jsonItemsList.Remove("DevBotLogsChannel");
            jsonItemsList.Remove("DevChannelID");
            jsonItemsList.Remove("JakeeID");
            JsonItemsListDevOps = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(jsonGlobalData));
            JsonItemsListDevOps.Remove("Token");
            developerRoleID = data.DeveloperRole;
            adminRoleID = data.AdminRole;
            BanRateLimit = data.BanLimit;
            AdminGivawayChannelID = data.AdminGiveawayChannID;
            GiveawayChanID = data.GiveawayChanID;
            KeeperLogsChanId = data.KeeperLogsChanID;
            WecomemessageURL = data.WecomemessageURL;
            JakeeGuildDebugChanID = data.JakeeGuildDebugChanID;
        }
        internal static void saveConfig(JsonData jsonData)
        {
            string json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);   
            File.WriteAllText(jsonGlobalData, json);
            readConfig();
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
        public string WelcomeMessage { get; set; }
        public ulong Welcomemessagechannel { get; set; }
        public ulong Rules { get; set; }
        public string Preflix { get; set; }
        public ulong AdminRole { get; set; }
        public ulong DeveloperRole { get; set; }
        public int BanLimit { get; set; }
        public ulong AdminGiveawayChannID { get; set; }
        public ulong GiveawayChanID { get; set; }
        public ulong KeeperLogsChanID { get; set; }
        public string WecomemessageURL { get; set; }
        public ulong JakeeGuildDebugChanID { get; set; }
    }
    public struct BotList
    {
        public static List<SocketGuildUser> botList { get; set; }
    }
    public struct LogFiles
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
