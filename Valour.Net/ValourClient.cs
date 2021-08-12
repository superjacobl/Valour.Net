﻿using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Numerics;
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
            .ConfigureLogging(logging => {
                // Set the log level of signalr stuffs
                logging.AddConsole();
                logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Error);
            }).Build();


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
            hubConnection.Reconnected += HubConnection_Reconnected; 

            // load cache from Valour
            Console.WriteLine("Loading up Cache");


            await Cache.UpdatePlanetAsync();

            List<Task> tasks = new List<Task>();

            foreach (Planet _planet in Cache.PlanetCache.Values)
            {
                tasks.Add(Cache.UpdateMembersFromPlanetAsync(_planet.Id));
                tasks.Add(Cache.UpdateChannelsFromPlanetAsync(_planet.Id));
                tasks.Add(Cache.UpdatePlanetRoles(_planet.Id));
            }

            await Task.WhenAll(tasks);

            Planet planet = new Planet();

            // this method took 1.4ms to load 1.4k members

            foreach (PlanetMember member in Cache.PlanetMemberCache.Values)
            {
                if (planet.Id != member.Planet_Id)
                {
                    planet = Cache.PlanetCache.Values.First(x => x.Id == member.Planet_Id);
                }

                foreach (ulong roleid in member.RoleIds)
                {
                    member.Roles.Add(planet.Roles.First(x => x.Id == roleid));
                }
            }


            // set up signar stuff

            // join every planet and channel
            Console.WriteLine("Registering Modules");
            RegisterModules();

            Console.WriteLine("Connecting to Valour");

            foreach (Planet _planet in Cache.PlanetCache.Values)
            {
                tasks.Add(hubConnection.SendAsync("JoinPlanet", _planet.Id, Token));
                // join interaction group
                tasks.Add(hubConnection.SendAsync("JoinInteractionGroup", _planet.Id, Token));
            }

        

            foreach (Channel channel in Cache.ChannelCache.Values)
            {        

                tasks.Add(hubConnection.SendAsync("JoinChannel", channel.Id, Token));
            }

            await Task.WhenAll(tasks);


            // set up events
            hubConnection.On<string>("Relay", OnRelay);
            hubConnection.On<string>("InteractionEvent", OnInteractionEvent);

            Console.WriteLine("\n\r-----Ready-----\n\r");
        }

        
        private static async Task HubConnection_Reconnected(string arg)
        {

            List<Task> tasks = new List<Task>();

            foreach (Planet _planet in Cache.PlanetCache.Values)
            {
                tasks.Add(hubConnection.SendAsync("JoinPlanet", _planet.Id, Token));
                // join interaction group
                tasks.Add(hubConnection.SendAsync("JoinInteractionGroup", _planet.Id, Token));
            }

            foreach (Channel channel in Cache.ChannelCache.Values)
            {
                tasks.Add(hubConnection.SendAsync("JoinChannel", channel.Id, Token));
            }

            await Task.WhenAll(tasks);

        }


        public static void RegisterModules()
        {
            ModuleRegistrar.RegisterAllCommands();
        }
        
        public static async Task PostMessage(ulong channelid, ulong planetid, string msg, ClientEmbed Embed)
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


            if (Embed != null)
            {
                if (Embed.Items.Count != 0)
                {
                    Embed.Pages.Insert(0, Embed.Items);
                }
                message.Embed_Data = JsonConvert.SerializeObject(Embed, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            
       
            HttpResponseMessage httpresponse = await httpClient.PostAsJsonAsync<PlanetMessage>($"https://valour.gg/Channel/PostMessage?token={Token}", message);
            ValourResponse<string> valourResponse = await httpresponse.Content.ReadFromJsonAsync<ValourResponse<string>>();
            if (!httpresponse.IsSuccessStatusCode || valourResponse.Success == false)
            {
                ErrorHandler.ReportError(new($"Error when attempting to post message : {valourResponse.Message}", ErrorSeverity.FATAL));
            }
        }

        public static async Task OnInteractionEvent(string Data)
        {
            await EventService.OnInteraction(JsonConvert.DeserializeObject<InteractionEvent>(Data));
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
