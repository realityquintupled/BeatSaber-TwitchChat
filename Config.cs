using System.IO;
using UnityEngine;

namespace TwitchIRC
{
    public class Config
    {
        public string username = "username here";
        public string oAuthToken = "OAuth password here";
        public string channelName = "channel to connect to";
        public int maxChatMessages = 20;
        public float chatSize = 1f;
        public int opacity = 255;
        public string ignoreMessagesWithPrefix = "!";
        public string chatMessagesColor = "#FFFFFF";
        public string taggedMessagesColor = "#6200ff";
        public bool overrideNameColor = false;
        public string customNameColor = "#FFBBFF";
        public bool showResponses = false;
        public string responseButtons = "Yes;No;That miss was bull, I totally hit that;Good map;Bad Map";
        public bool disableChatTags = true;
        public bool chatTwitch = false;
        public float chatTwitchOffset = .01f;
        public static Config LoadConfig(string configPath)
        {
            if(!File.Exists(configPath))
            {
                File.Create(configPath).Dispose();
                Config config = new Config();
                config.SaveConfig(configPath);
            }
            return JsonUtility.FromJson<Config>(File.ReadAllText(configPath));
        }
        public void SaveConfig(string configPath)
        {
            File.WriteAllText(configPath, JsonUtility.ToJson(this, true));
        }
    }
}
