using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KindomKeeper.Users
{
    class UserAccounts
    {
        internal static void SaveUserAccounts(IEnumerable<jsonUser> userAccounts)
        {
            string jsonstring = JsonConvert.SerializeObject(userAccounts);
            File.WriteAllText(Global.jsonAccountFilePath, jsonstring);
        }
        internal static List<jsonUser> LoadUserAccounts()
        {
            string currentJson = File.ReadAllText(Global.jsonAccountFilePath);
            return JsonConvert.DeserializeObject<List<jsonUser>>(currentJson);
        }
    }
}
