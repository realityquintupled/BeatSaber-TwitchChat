namespace TwitchIRC
{
    public class ChatMessage
    {
        public string username;
        public string message;
        public string color;
        public ChatMessage(string username, string message, string color)
        {
            this.username = username;
            this.message = message;
            this.color = color;
        }
    }
}
