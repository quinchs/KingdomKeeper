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
        internal static string BotToken = "";

        internal static void readConfig()
        {
            JsonData data = JsonConvert.DeserializeObject<JsonData>(File.ReadAllText(jsonGlobalData));
            BotToken = data.Token;
        }
    }
    internal struct JsonData
    {
        internal string Token { get; set; }
        internal ulong ChannelID { get; set; }
    }
}
