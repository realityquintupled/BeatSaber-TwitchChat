using System;
using System.IO;

namespace TwitchIRC
{
    public class Strings
    {
        public static string configPath = Path.Combine(Environment.CurrentDirectory, "twitchconfig.json");
        public static string chatPanelName = "Twitch Chat Container";
        public static string responsePanelName = "Twitch Response Manager";
        public static string cubeShader = "Custom/SpriteNoGlow";
    }
}
