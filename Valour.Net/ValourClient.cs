using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Valour.Net.CommandHandling;
using Valour.Net.CommandHandling.InfoModels;
using Valour.Net.ErrorHandling;
using Valour.Net.Models;

namespace Valour.Net
{
    public class ValourClient
    {
        static HttpClient httpClient = new HttpClient();
        public static string Token { get; set; }

        public static ulong BotId { get; set; }

        public static List<string> BotPrefixList = new List<string>();

        public static bool DisallowSelfRespond = true;

        public static bool DisallowBotRespond = true;

        public static Func<PlanetMessage, Task> OnMessage;

        public static HubConnection hubConnection = new HubConnectionBuilder()
            .WithUrl("https://valour.gg/planethub")
            .WithAutomaticReconnect()
            .Build();

        //public static ErrorHandler errorHandler = new();

        /// <summary>
        /// This is how to "Login" as the bot. All other requests require a token so therefore this must be run first.
        /// </summary>
        public static async Task RequestTokenAsync(string Email, string Password)
        {
            Token = await GetData<string>($"https://valour.gg/User/RequestStandardToken?email={System.Web.HttpUtility.UrlEncode(Email)}&password={System.Web.HttpUtility.UrlEncode(Password)}");
        }

        /// <summary>
        /// This will overwrite the current prefix list removing any current prefixes and setting the provided prefix as the only prefix
        /// </summary>
        /// <param name="prefix">The new prefix</param>
        public static void SetPrefix(string prefix)
        {
            /*
            if (Char.IsLetterOrDigit(prefix.Last()))
            {
                errorHandler.ReportError(new GenericError($"Attempted to set invalid prefix {prefix}. A prefix must contain a non-alphanumeric character at the end.", ErrorSeverity.WARN));
                return;
            }
            */
            BotPrefixList.Clear();
            BotPrefixList.Add(prefix);
        }

        /// <summary>
        /// Add new prefix to recognised prefixes list
        /// </summary>
        /// <param name="prefix">New prefix to be added</param>
        public static void AddPrefix(string prefix)
        {
            /*
            if (Char.IsLetterOrDigit(prefix.Last()))
            {
                errorHandler.ReportError(new GenericError($"Attempted to add invalid prefix {prefix}. A prefix must contain a non-alphanumeric character at the end.", ErrorSeverity.WARN));
                return;
            }
            */
            if (!BotPrefixList.Contains(prefix))
            {
                BotPrefixList.Add(prefix);
            }
            else
            {
                ErrorHandler.ReportError(new GenericError($"Attempted to add prefix {prefix} while it is already a recognised prefix.", ErrorSeverity.WARN));
            }
        }

        /// <summary>
        /// Remove prefix from currently recognised prefixes
        /// </summary>
        /// <param name="prefix">Prefix to be removed</param>
        public static void RemovePrefix(string prefix)
        {
            if (!BotPrefixList.Remove(prefix))
            {
                ErrorHandler.ReportError(new GenericError($"Attempted to remove prefix {prefix} but it was not a recognised prefix.", ErrorSeverity.WARN));
            }
        }

        public static async Task Start(string email, string password)
        {
            Console.WriteLine("Loading up...");

            if (BotPrefixList.Count < 1)
            {
                ErrorHandler.ReportError(new GenericError($"Bot was started with no recognied prefixes. Adding \"/\" as the default prefix.", ErrorSeverity.WARN));
                SetPrefix("/");
            }

            await RequestTokenAsync(email, password);
            if (Token == null) //Token returned null meaning valour is unavailable
                return;
            

            // Get botid from token
            BotId = (await GetData<ValourUser>($"https://valour.gg/User/GetUserWithToken?token={Token}")).Id;
            

            await hubConnection.StartAsync();

            //Create task to register modules while cache is being loaded
            Console.WriteLine("Registering Modules");
            Task.Run(() => RegisterModules());

            // load cache from Valour
            Console.WriteLine("Loading up Cache");

            await Cache.UpdatePlanetAsync();

            List<Task> tasks = new List<Task>();

            foreach (Planet planet in Cache.PlanetCache.Values)
            {
                tasks.Add(Task.Run(async () => await Cache.UpdateMembersFromPlanetAsync(planet.Id)));
                tasks.Add(Task.Run(async () => await Cache.UpdateChannelsFromPlanetAsync(planet.Id)));
                tasks.Add(Task.Run(async () => await Cache.UpdatePlanetRoles(planet.Id)));
            }

            await Task.WhenAll(tasks);

            // Sets every member's RoleNames for speed

            // use basic variable caching to improve the speed of this in the future

            foreach (PlanetMember member in Cache.PlanetMemberCache.Values)
            {
                foreach (ulong roleid in member.RoleIds)
                {
                    member.Roles.Add(Cache.PlanetCache.Values.First(x => x.Id == member.Planet_Id).Roles.First(x => x.Id == roleid));
                }
            }

            // set up signalr stuff

            //returnline = Console.GetCursorPosition().Top;
            Console.WriteLine("Connecting to Valour");


            // join every planet and channel

            foreach (Planet planet in Cache.PlanetCache.Values)
            {
                await hubConnection.SendAsync("JoinPlanet", planet.Id, Token).ConfigureAwait(false);
                foreach (Channel channel in Cache.ChannelCache.Values.Where(x => x.Planet_Id == planet.Id))
                {
                    await hubConnection.SendAsync("JoinChannel", channel.Id, Token).ConfigureAwait(false);
                }
            }

            // set up events

            hubConnection.On<string>("Relay", OnRelay);


            Console.WriteLine("\n-----Ready----- ");
        }

        public static void RegisterModules()
        {
            ModuleRegistrar.RegisterAllCommands();
        }

        public static async Task PostMessage(ulong channelid, ulong planetid, string msg)
        {
            PlanetMessage message = new PlanetMessage()
            {
                Channel_Id = channelid,
                Content = msg,
                TimeSent = DateTime.UtcNow,
                Author_Id = BotId,
                Member_Id = (await Cache.GetPlanetMember(BotId, planetid)).Id,
                Planet_Id = planetid
            };

            //string json = Newtonsoft.Json.JsonConvert.SerializeObject(message);

            HttpResponseMessage httpresponse = await httpClient.PostAsJsonAsync<PlanetMessage>($"https://valour.gg/Channel/PostMessage?token={Token}", message);
            ValourResponse<string> valourResponse = await httpresponse.Content.ReadFromJsonAsync<ValourResponse<string>>();
            if (!httpresponse.IsSuccessStatusCode || valourResponse.Success == false)
            {
                ErrorHandler.ReportError(new($"Error when attempting to post message : {valourResponse.Message}", ErrorSeverity.FATAL));
            }
        }

        public static async Task OnRelay(string data)
        {
            PlanetMessage message = JsonConvert.DeserializeObject<PlanetMessage>(data);
            message.Author = await message.GetAuthorAsync();
            if ((message.Author.IsBot && DisallowBotRespond) || (message.Author.User_Id == BotId && DisallowSelfRespond)) return;

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
            message.Content = message.Content.Trim();
            string commandprefix = BotPrefixList.FirstOrDefault(prefix => message.Content.Substring(0, prefix.Length) == prefix);

            if (commandprefix != null)
            {
                // get clean command string

                string commandstring = message.Content[commandprefix.Length..];

                // get args

                List<string> args = commandstring.Split(" ").ToList();

                // get command

                string commandname = args[0].ToLower();
                args.RemoveAt(0);

                CommandInfo command = await CommandService.RunCommandString(commandname, args, ctx);

                if (command != null)
                {
                    if (command.IsFallback)
                        args.Clear();
                    try
                    {
                        command.Method.Invoke(command.moduleInfo.Instance, command.ConvertStringArgs(args, ctx).ToArray());
                    }
                    catch (Exception e)
                    {
                        ErrorHandler.ReportError(new GenericError(e.Message, ErrorSeverity.FATAL, e));
                    }
                }
            }
        }

        public static async Task<ValourResponse<T>> GetResponse<T>(string url)
        {
            var httpResponse = await httpClient.GetAsync(url);
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.BadGateway)
            {
                ErrorHandler.ReportError(new GenericError($"Valour is currently unavailable.", ErrorSeverity.FATAL));
                return default;
            }
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
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.BadGateway)
            {
                ErrorHandler.ReportError(new GenericError($"Valour is currently unavailable.", ErrorSeverity.FATAL));
                return default;
            }
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
