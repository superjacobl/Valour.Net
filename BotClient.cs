namespace Valour.Net
{
    class BotClient
    {
        public string Token { get; set; }

        public BotClient(string token)
        {
            Token = token;
        }
    }
}
