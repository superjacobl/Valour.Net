using System.Net.Http;
namespace Valour.Net
{
    class BotClient
    {
        HttpClient httpClient;
        private string Token { get; set; }
        private string Email { get; set; }
        private string Password { get; set; }

        /// <summary>
        /// Creates a new instance of a BotClient.
        /// </summary>
        /// <param name="email">The email of the bot account. Will be cleared after token is received from the LoginAsync() function</param>
        /// <param name="password">The password of the bot account. Will be cleared after token is received from the LoginAsync() function</param>
        public BotClient(string email, string password)
        {
            Email = email;
            Password = password;
            httpClient = new HttpClient();
        }


        /// <summary>
        /// This is how to "Login" as the bot. All other requests require a token so therefore this must be run first.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public async void RequestToken(string email, string password)
        {
            await httpClient.GetAsync($"https://valour.gg/User/RequestStandardToken?email={email}&password={password}");
        }
    }
}
