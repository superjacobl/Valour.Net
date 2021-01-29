using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Valour.Net.ErrorHandling;

namespace Valour.Net
{
    class BotClient
    {
        static HttpClient httpClient;
        private string Token { get; set; }
        private string Email { get; set; }
        private string Password { get; set; }

        /// <summary>
        /// Creates a new instance of a BotClient.
        /// </summary>
        /// <param name="email">The email of the bot account. Will be cleared after token is received from the RequestTokenAsync() method</param>
        /// <param name="password">The password of the bot account. Will be cleared after token is received from the RequestTokenAsync() method</param>
        public BotClient(string email, string password)
        {
            Email = email;
            Password = password;
            httpClient = new HttpClient();
        }


        /// <summary>
        /// This is how to "Login" as the bot. All other requests require a token so therefore this must be run first.
        /// </summary>
        public async void RequestTokenAsync()
        {
            await httpClient.GetAsync($"https://valour.gg/User/RequestStandardToken?email={Email}&password={Password}");
        }

        public static async Task<ValourResponse<T>> GetData<T>(string url)
        {
            var httpResponse = await httpClient.GetAsync(url);
            ValourResponse<T> response = JsonConvert.DeserializeObject<ValourResponse<T>>(await httpResponse.Content.ReadAsStringAsync());

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new GenericError(
                    $"Requesting Data from {url} failed with a status code of {httpResponse.StatusCode} with message {response.Message}",
                    ErrorSeverity.WARN);
            }

            return response;
        }
    }
}