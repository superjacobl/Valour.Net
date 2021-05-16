using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Valour.Net.ErrorHandling;
using System;
using Microsoft.AspNetCore.SignalR.Client;
using Valour.Net.Models;
using System.Collections.Generic;
using System.Linq;
using Valour.Net.CommandHandling;
using Valour.Net.CommandHandling.InfoModels;

namespace Valour.Net
{
    public class ValourClient
    {
        static HttpClient httpClient = new HttpClient();
        public static string Token { get; set; }

        public static ulong BotId {get; set;}

        public static string BotPrefix {get; set;}
        public static Func<PlanetMessage, Task> OnMessage;

        public static HubConnection hubConnection = new HubConnectionBuilder()
            .WithUrl("https://valour.gg/planethub")
            .WithAutomaticReconnect()
            .Build();

        /// <summary>
        /// This is how to "Login" as the bot. All other requests require a token so therefore this must be run first.
        /// </summary>
        public static async Task RequestTokenAsync(string Email, string Password)
        {
            Token = await GetData<string>($"https://valour.gg/User/RequestStandardToken?email={System.Web.HttpUtility.UrlEncode(Email)}&password={System.Web.HttpUtility.UrlEncode(Password)}");
        }

        public static async Task Start(string email, string password) 
        {
            await RequestTokenAsync(email, password);

            // get botid from token

            BotId = (await GetData<ValourUser>($"https://valour.gg/User/GetUserWithToken?token={Token}")).Id;

            await hubConnection.StartAsync();

            // load cache from Valour

            await Cache.UpdatePlanetAsync();

            List<Task> tasks = new List<Task>();

            foreach (Planet planet in Cache.PlanetCache.Values) {
                tasks.Add(Task.Run(async () => await Cache.UpdateMembersFromPlanetAsync(planet.Id)));
                tasks.Add(Task.Run(async () => await Cache.UpdateChannelsFromPlanetAsync(planet.Id)));
                tasks.Add(Task.Run(async () => await Cache.UpdatePlanetRoles(planet.Id)));
            }

            await Task.WhenAll(tasks);

            // Sets every member's RoleNames for speed

            // use basic variable caching to improve the speed of this in the future

            foreach (PlanetMember member in Cache.PlanetMemberCache.Values) {
                foreach (ulong roleid in member.RoleIds) {
                    member.Roles.Add(Cache.PlanetCache.Values.First(x => x.Id == member.Planet_Id).Roles.First(x => x.Id == roleid));
                }
            }

            Console.WriteLine("Done loading up Cache!");

            // set up signar stuff

            // join every planet and channel

            foreach(Planet planet in Cache.PlanetCache.Values) {
                await hubConnection.SendAsync("JoinPlanet", planet.Id, Token).ConfigureAwait(false);
                foreach(Channel channel in Cache.ChannelCache.Values.Where(x => x.Planet_Id == planet.Id)) {
                    await hubConnection.SendAsync("JoinChannel", channel.Id, Token).ConfigureAwait(false);
                }
            }

            // set up events

            hubConnection.On<string>("Relay", OnRelay);

        }

        public static void RegisterModules() {
            ModuleRegistrar.RegisterAllCommands(new ErrorHandler());
        }

        public static async Task PostMessage(ulong channelid, ulong planetid, string msg)
        {
            PlanetMessage message = new PlanetMessage()
            {
                Channel_Id = channelid,
                Content = msg,
                TimeSent = DateTime.UtcNow,
                Author_Id = BotId,
                Planet_Id = planetid
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(message);

            HttpResponseMessage httpresponse = await httpClient.PostAsJsonAsync<PlanetMessage>($"https://valour.gg/Channel/PostMessage?token={Token}", message);
        }

        public static async Task OnRelay(string data) {
            PlanetMessage message = JsonConvert.DeserializeObject<PlanetMessage>(data);
            message.Author = await message.GetAuthorAsync();
            message.Channel = await message.GetChannelAsync();
            message.Planet = await message.GetPlanetAsync();
            CommandContext ctx = new CommandContext();
            await ctx.Set(message);
            if (OnMessage != null)
            {
                await OnMessage.Invoke(message);
            }
            
            await EventService.OnMessage(ctx);

            // check to see if message has a command in it

            if (message.Content.Substring(0,1) == BotPrefix) {

                // get clean command string

                string commandstring = message.Content.Replace(BotPrefix, "");

                // get args

                List<string> args = commandstring.Split(" ").ToList();
                args.RemoveAt(0);

                // get command

                string commandname = message.Content.Split(" ")[0].Replace(BotPrefix, "").ToLower();

                CommandInfo command = CommandService.RunCommandString(commandname, args, ctx);

                if (command != null) {
                    command.Method.Invoke(command.moduleInfo.Instance, command.ConvertStringArgs(args,ctx).ToArray());
                }
                
            }

        }

        public static async Task<ValourResponse<T>> GetResponse<T>(string url)
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

        public static async Task<T> GetData<T>(string url)
        {
            var httpResponse = await httpClient.GetAsync(url);
            ValourResponse<T> response = JsonConvert.DeserializeObject<ValourResponse<T>>(await httpResponse.Content.ReadAsStringAsync());

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new GenericError(
                    $"Requesting Data from {url} failed with a status code of {httpResponse.StatusCode} with message {response.Message}",
                    ErrorSeverity.WARN);
            }

            return response.Data;
        }

        public static async Task<Planet> GetPlanet(ulong id)
        {
            return await Cache.GetPlanet(id);
        }

    }
}
