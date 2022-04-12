global using Valour.Api.Items.Planets;
global using Valour.Api.Items.Planets.Members;
global using Valour.Api.Items.Planets.Channels;
global using Valour.Api.Items.Messages;
global using Valour.Api.Items.Users;
global using Valour.Shared.Items.Messages.Embeds;

using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Valour.Api.Client;
//using Valour.Api.Messages
using Valour.Net.CommandHandling;
using Valour.Net.CommandHandling.InfoModels;
using Valour.Net.ErrorHandling;
using IdGen;
//using Valour.Net.Models;
//using Valour.Net.Models.Messages;

namespace Valour.Net
{
    public class ValourNetClient
    {
        static HttpClient httpClient = new HttpClient();
        public static string Token { get; set; }
        public static IdManager idManager = new IdManager();

        public static ulong BotId { get; set; }

        public static List<string> BotPrefixList = new List<string>();

        public static bool DisallowSelfRespond = true;

        public static bool DisallowBotRespond = true;

        public static Func<PlanetMessage, Task> OnMessage;

        //public static ErrorHandler errorHandler = new();

        /// <summary>
        /// This is how to "Login" as the bot. All other requests require a token so therefore this must be run first.
        /// </summary>
        public static async Task<string> RequestTokenAsync(string email, string password)
        {
            string encodedEmail = System.Web.HttpUtility.UrlEncode(email);
            string encodedPassword = System.Web.HttpUtility.UrlEncode(password);

            TokenRequest content = new TokenRequest(email, password);
            var response = await httpClient.PostAsJsonAsync($"https://valour.gg/api/user/requesttoken", content);

            var message = await response.Content.ReadAsStringAsync();

            return message;
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

            ValourClient.OnLogin += async () => { };
            ValourClient.OnJoinedPlanetsUpdate += async () => { };

            ValourClient.SetHttpClient(new HttpClient
            {
                BaseAddress = new Uri("https://valour.gg")
            });

            Token = await RequestTokenAsync(email, password);
            if (Token == null) //Token returned null meaning valour is unavailable
                return;

            await ValourClient.InitializeSignalR();

            Planet planet = new Planet();

            // set up signar stuff

            // join every planet and channel
            Console.WriteLine("Registering Modules");
            RegisterModules();

            Console.WriteLine("Connecting to Valour");

            httpClient.DefaultRequestHeaders.Add("authorization", Token);

            await ValourClient.InitializeUser(Token);

            Planet _planet = (Planet)ValourCache.HCache[typeof(Valour.Api.Items.Planets.Planet)].Values.First();

            BotId = ValourClient.Self.Id;

            Console.WriteLine("Loading up channels");

            // load up channels, members, role
            //
            // VERY BROKEN

            //await Task.Run(() => Parallel.ForEach(ValourCache.HCache[typeof(Planet)].Values, async _planet =>
           // {
            //    planet = (Planet)_planet;
            //    await planet.LoadChannelsAsync();
           //     await planet.LoadCategoriesAsync();
          //      await planet.LoadMemberDataAsync();
           //     await planet.LoadRolesAsync();
          //  }));

            Console.WriteLine("Joining Planets & Channels");

            await HubConnection_Reconnected(null);

            ValourClient.HubConnection.Reconnected += HubConnection_Reconnected;

            ValourClient.OnMessageRecieved += async (message) =>
            {
                await OnRelay(message);
            };
            ValourClient.HubConnection.On<EmbedInteractionEvent>("InteractionEvent", OnInteractionEvent);

            Console.WriteLine("\n\r-----Ready-----\n\r");
        }


        private static async Task HubConnection_Reconnected(string arg)
        {

            var type = typeof(Planet);

            Parallel.ForEach(ValourCache.HCache[typeof(Planet)].Values, async _planet => {
                Planet planet = (Planet)_planet;
                await ValourClient.HubConnection.SendAsync("JoinPlanet", planet.Id, Token);

                await ValourClient.HubConnection.SendAsync("JoinInteractionGroup", planet.Id, Token);

                var channels = await planet.GetChannelsAsync();

                //await ValourClient.HubConnection.SendAsync("JoinChannel", planet.Main_Channel_Id, Token);

                foreach (PlanetChatChannel channel in channels)
                {
                    await ValourClient.HubConnection.SendAsync("JoinChannel", channel.Id, Token);
                }

            });

            return;

            await Task.Run(() => Parallel.ForEach(ValourCache.HCache[typeof(Planet)].Values, async _planet =>
            {
                Planet planet = (Planet)_planet;
                await ValourClient.HubConnection.SendAsync("JoinPlanet", planet.Id, Token);

                await ValourClient.HubConnection.SendAsync("JoinInteractionGroup", planet.Id, Token);

                var channels = await planet.GetChannelsAsync();

                //await ValourClient.HubConnection.SendAsync("JoinChannel", planet.Main_Channel_Id, Token);

                foreach (PlanetChatChannel channel in channels)
                {
                    await ValourClient.HubConnection.SendAsync("JoinChannel", channel.Id, Token);
                }
                
            }));

        }


        public static void RegisterModules()
        {
            ModuleRegistrar.RegisterAllCommands();
        }

        public static async Task PostMessage(ulong channelid, ulong planetid, string msg, Embed embed = null)
        {
            PlanetMessage message = new(msg, (await ValourClient.GetSelfMember(planetid)).Id, channelid, planetid);

            message.Id = idManager.Generate();

            if (embed is not null)
            {
                JsonSerializerOptions options = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };
                message.Embed_Data = JsonSerializer.Serialize(embed, options);
            }

            await ValourClient.SendMessage(message);

            //string json = JsonSerializer.Serialize(message, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault });

            //StringContent content = new StringContent(json);

            //Console.WriteLine(json);

            //HttpResponseMessage httpresponse = await httpClient.PostAsync($"https://valour.gg/api/channel/{channelid}/messages", content);
            //Console.WriteLine(await httpresponse.Content.ReadAsStringAsync());
            //ValourResponse<string> valourResponse = await httpresponse.Content.ReadFromJsonAsync<ValourResponse<string>>();
            //if (!httpresponse.IsSuccessStatusCode || valourResponse.Success == false)
            //{
            //    ErrorHandler.ReportError(new($"Error when attempting to post message : {valourResponse.Message}", ErrorSeverity.FATAL));
            //}

            return;
        }

        public static async Task OnInteractionEvent(EmbedInteractionEvent interactionEvent)
        {
            Console.WriteLine("TEST");
            await EventService.OnInteraction(interactionEvent);
        }



        public static async Task OnRelay(PlanetMessage message)
        {

            PlanetMember member = await PlanetMember.FindAsync(message.Member_Id);

            if (message.Channel_Id == 4458944803897344 || message.Channel_Id == 4459421423108096 || message.Channel_Id == 4459421423108096) {
                return;
            }

            //if (((await member.GetUserAsync()).Bot && DisallowBotRespond) || (member.User_Id == BotId && DisallowSelfRespond)) return;

            CommandContext ctx = new CommandContext();
            ctx.MessageTimeTook = DateTime.UtcNow-message.TimeSent;
            await ctx.Set(message);
            if (OnMessage != null)
            {
                await OnMessage.Invoke(message);
            }

            await EventService.OnMessage(ctx);

            if (((await member.GetUserAsync()).Bot && DisallowBotRespond) || (member.User_Id == BotId && DisallowSelfRespond)) return;

            // check to see if message has a command in it
            message.Content = message.Content.Trim();
            string commandprefix = "";
            try
            {
                commandprefix = BotPrefixList.FirstOrDefault(prefix => message.Content.Substring(0, prefix.Length) == prefix);
            }
            catch (Exception)
            {
                return; //Message had no content causing the substring to fail. Message will be ignored
            }

            if (commandprefix != null)
            {
                // get clean command string

                string commandstring = message.Content[commandprefix.Length..];

                commandstring = commandstring.Replace("\u00A0", " ");

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
                        ErrorHandler.ReportError(new GenericError($"Error attempting to execute command: {e.Message}", ErrorSeverity.FATAL, e));
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
            ValourResponse<T> response = JsonSerializer.Deserialize<ValourResponse<T>>(await httpResponse.Content.ReadAsStringAsync());

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new GenericError(
                    $"Requesting Data from {url} failed with a status code of {httpResponse.StatusCode} with message {response.Message}",
                    ErrorSeverity.WARN);
            }

            return response;
        }

        public static async Task<T> PostData<T>(string url, Dictionary<T, T> data)
        {
            var httpResponse = await httpClient.PostAsJsonAsync(url, data);
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.BadGateway)
            {
                ErrorHandler.ReportError(new GenericError($"Valour is currently unavailable.", ErrorSeverity.FATAL));
                return default;
            }
            ValourResponse<T> response = JsonSerializer.Deserialize<ValourResponse<T>>(await httpResponse.Content.ReadAsStringAsync());

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new GenericError(
                    $"Requesting Data from {url} failed with a status code of {httpResponse.StatusCode} with message {response.Message}",
                    ErrorSeverity.WARN);
            }

            return response.Data;
        }

        public static async Task<T> GetData<T>(string url)
        {
            var httpResponse = await httpClient.GetAsync(url);
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.BadGateway)
            {
                ErrorHandler.ReportError(new GenericError($"Valour is currently unavailable.", ErrorSeverity.FATAL));
                return default;
            }
            ValourResponse<T> response = JsonSerializer.Deserialize<ValourResponse<T>>(await httpResponse.Content.ReadAsStringAsync());

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new GenericError(
                    $"Requesting Data from {url} failed with a status code of {httpResponse.StatusCode} with message {response.Message}",
                    ErrorSeverity.WARN);
            }

            return response.Data;
        }

    }
    public class IdManager
    {
        public IdGenerator Generator { get; set; }

        public IdManager()
        {
            // Fun fact: This is the exact moment that SpookVooper was terminated
            // which led to the development of Valour becoming more than just a side
            // project. Viva la Vooperia.
            var epoch = new DateTime(2021, 1, 11, 4, 37, 0);

            var structure = new IdStructure(45, 10, 8);

            var options = new IdGeneratorOptions(structure, new DefaultTimeSource(epoch));

            Generator = new IdGenerator(0, options);
        }

        public ulong Generate()
        {
            return (ulong)Generator.CreateId();
        }
    }
}
