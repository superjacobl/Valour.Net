global using Valour.Api.Items.Planets;
global using Valour.Api.Items.Planets.Members;
global using Valour.Api.Items.Channels.Planets;
global using Valour.Api.Items.Messages;
global using Valour.Api.Items.Users;
global using Valour.Api.Items.Messages.Embeds;
global using Valour.Api.Items.Messages.Embeds.Items;
global using Valour.Api.Items;
global using Valour.Shared.Items.Authorization;
global using Valour.Net.Client;
global using Valour.Api.Items.Messages;
global using Valour.Net.CommandHandling.Attributes;
global using Valour.Net.EmbedMenu;
global using Valour.Api.Nodes;
global using Valour.Shared;
global using Valour.Shared.Channels;
global using Valour.Api.Client;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Net.Http;
global using System.Net.Http.Json;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading.Tasks;

//using Microsoft.AspNetCore.SignalR.Client;
using Valour.Net.CommandHandling;
using Valour.Net.CommandHandling.InfoModels;
using Valour.Net.ErrorHandling;
using IdGen;
using Microsoft.AspNetCore.SignalR.Client;
using Valour.Shared.Items.Users;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Valour.Api.Items.Authorization;
using Valour.Net.Client.MessageHelper;

namespace Valour.Net.Client
{
    public class ValourNetClient
    {
        static HttpClient httpClient = new();

        public static string Token;

        internal static IdManager idManager = new();

        public static string BaseUrl = "https://app.valour.gg/";

        /// <summary>
        /// The User Id of your bot.
        /// </summary>
        public static long BotId;

        public static List<string> BotPrefixList = new();

        /// <summary>
        /// If true, your bot will not receive messages, nor will it send messages
        /// </summary>
        
        public static bool UnitTesting = false;

        /// <summary>
        /// If true, your bot will respond to itself.
        /// </summary>

        public static bool DisallowSelfRespond = true;

        /// <summary>
        /// If true, your bot will respond to other bots.
        /// </summary>

        public static bool DisallowBotRespond = true;

        /// <summary>
        /// If true, messages will be executed in parallel. Commands will also be handled in parallel. 
        /// Please note, if you use a database be sure to prevent two threads from executing something at the same time on a single database context.
        /// </summary>
        public static bool ExecuteMessagesInParallel = false;

        internal static async ValueTask InvokeMethod(MethodInfo methodInfo, CommandModuleBase instance, object[] args)
        {
            //methodInfo.ReturnType.GetMethod(nameof(Task.GetAwaiter));
            if (methodInfo.ReturnType == typeof(Task) || methodInfo.ReturnType == typeof(ValueTask))
            {
                await (Task)methodInfo.Invoke(instance, args);
            }
            else
            {
                methodInfo.Invoke(instance, args);
            }
        }

        //public static ErrorHandler errorHandler = new();

        /// <summary>
        /// This is how to "Login" as the bot. All other requests require a token; therefore, this must be run first.
        /// </summary>
        internal static async Task<string> RequestTokenAsync(string email, string password)
        {
            string encodedEmail = System.Web.HttpUtility.UrlEncode(email);
            string encodedPassword = System.Web.HttpUtility.UrlEncode(password);

            TokenRequest content = new()
            {
                Email = email,
                Password = password
            };
            
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}api/user/token", content);

            Console.WriteLine($"Url:{BaseUrl}api/user/token");
            Console.WriteLine($"Output:{await response.Content.ReadAsStringAsync()}");

            AuthToken token = JsonSerializer.Deserialize<AuthToken>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions() {PropertyNameCaseInsensitive = true});

            return token.Id;
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

        public static void RemovePrefix(string prefix)
        {
            if (!BotPrefixList.Remove(prefix))
            {
                ErrorHandler.ReportError(new GenericError($"Attempted to remove prefix {prefix} but it was not a recognised prefix.", ErrorSeverity.WARN));
            }
        }

        /// <summary>
        /// Call this method to start the bot.
        /// </summary>

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
            ValourClient.OnMessageDeleted += async (m) => { };
            ValourClient.OnUserChannelStateUpdate += async(m) => { };
            ValourClient.OnChannelWatchingUpdate += async(m) => {
                ChannelWatchingContext ctx = new();
                ctx.Set(m);
                await EventService.OnChannelWatching(ctx);
            };
            ValourClient.OnFriendsUpdate += async () => { };

            Token = await RequestTokenAsync(email, password);
            if (Token == null) //Token returned null meaning valour is unavailable
                return;

            // set up signar stuff
        
            Console.WriteLine("Registering Modules");

            ModuleRegistrar.RegisterAllCommands();

            Console.WriteLine("Grabbing Bot's User info & logging in");

            ValourClient.BaseAddress = BaseUrl;

            ValourClient.SetHttpClient(new HttpClient()
            {
                BaseAddress = new Uri(BaseUrl)
            });

            await ValourClient.InitializeUser(Token);

            //Planet _planet = (Planet)ValourCache.HCache[typeof(Planet)].Values.First();

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

            Console.WriteLine("Loaded All Planet Data & Channel Data into Cache");

            ValourClient.OnNodeReconnect += HubConnection_Reconnected;
            ValourClient.OnMessageRecieved += async (Message msg) =>
            {
                if (ExecuteMessagesInParallel)
                {
                    OnRelay(msg);
                }
                else
                {
                    await OnRelay(msg);
                }
            };

            ItemObserver<PlanetChatChannel>.OnAnyUpdated += OnChannelUpdate;

            ItemObserver<PermissionsNode>.OnAnyUpdated += OnPermissionsNodeUpdate;

            Console.WriteLine("\n\r-----Ready-----\n\r");
        }

        internal static async Task JoinCategory(PlanetCategoryChannel category) {
            foreach(PlanetChatChannel channel in ValourCache.GetAll<PlanetChatChannel>().Where(x => x.ParentId == category.Id)) 
            {
                var servernode = await NodeManager.GetNodeForPlanetAsync(category.PlanetId);
                await servernode.HubConnection.SendAsync("JoinChannel", channel.Id);
            }
            foreach(PlanetCategoryChannel _category in ValourCache.GetAll<PlanetCategoryChannel>().Where(x => x.ParentId == category.Id)) 
            {
                JoinCategory(_category);
            }
        }

        // this does not work???
        internal static async Task OnPermissionsNodeUpdate(PermissionsNode node, bool newitem, int flags)
        {
            // try to connect to the channel
            if (node.TargetType == PermissionsTargetType.PlanetChatChannel) {
                var servernode = await NodeManager.GetNodeForPlanetAsync(node.PlanetId);
                await servernode.HubConnection.SendAsync("JoinChannel", node.TargetId, Token);
            }
            // or try to connect to all channels in the category
            else if (node.TargetType == PermissionsTargetType.PlanetCategoryChannel) {
                var category = await PlanetCategoryChannel.FindAsync(node.TargetId, node.PlanetId);
                JoinCategory(category);
            }
        }

        internal static async Task OnChannelUpdate(PlanetChatChannel channel, bool newitem, int flags) 
        {
            if (newitem) {
                var node = await NodeManager.GetNodeForPlanetAsync(channel.PlanetId);
                // send JoinChannel to hub so we can get messages from the new channel
                await node.HubConnection.SendAsync("JoinChannel", channel.Id, Token);
            }
        }


        internal static async Task HubConnection_Reconnected(Node _node)
        {
            List<string> DidNodeNames = new();
            // Joins SignalR groups
            var options = new ParallelOptions { MaxDegreeOfParallelism = 10 };
            await Parallel.ForEachAsync(ValourCache.HCache[typeof(Planet)].Values, async (_planet, token) => 
            {
                Planet planet = (Planet)_planet;

                var node = await NodeManager.GetNodeForPlanetAsync(planet.Id);
                await node.AuthenticateSignalR();
                await node.ConnectToUserSignalRChannel();
                var result = await node.HubConnection.InvokeAsync<TaskResult>("JoinPlanet", planet.Id);

                Console.WriteLine($"Connected to planet: {planet.Name}");

                await node.HubConnection.SendAsync("JoinInteractionGroup", planet.Id);

                if (!DidNodeNames.Contains(node.Name)) {
                    node.HubConnection.Remove("InteractionEvent");
                    node.HubConnection.On<EmbedInteractionEvent>("InteractionEvent", OnInteractionEvent);
                    DidNodeNames.Add(node.Name);
                }

                var channels = await planet.GetChannelsAsync();

                foreach (PlanetChatChannel channel in channels)
                {
                    await node.HubConnection.SendAsync("JoinChannel", channel.Id);
                }

            });
        }

        public static async Task PostMessage(long channelid, long planetid, string msg, Embed embed = null)
        {
            PlanetMessage message = new(msg, (await ValourClient.GetSelfMember(planetid)).Id, channelid, planetid)
            {
                Id = idManager.Generate()
            };

            if (embed is not null)
            {
                JsonSerializerOptions options = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };
                message.EmbedData = JsonSerializer.Serialize(embed, options);
            }

            MessageHelpers.GenerateForPost(message);

            await ValourClient.SendMessage(message);

            return;
        }

        internal static async Task OnInteractionEvent(EmbedInteractionEvent interactionEvent)
        {
            Console.WriteLine("TEST");
            await EventService.OnInteraction(interactionEvent);
        }

        internal static async Task OnRelay(Message _message)
        {

            // for now there is no DM handling

            PlanetMessage message = (PlanetMessage)_message;
            CommandContext ctx = new()
            {
                TimeReceived = DateTime.UtcNow,
                MessageTimeTook = DateTime.UtcNow - message.TimeSent
            };

            await ctx.Set(message);

            await EventService.OnMessage(ctx);
            PlanetMember member = await PlanetMember.FindAsync(message.AuthorMemberId, message.PlanetId);
            if (((await member.GetUserAsync()).Bot && DisallowBotRespond) || (member.UserId == BotId && DisallowSelfRespond)) return;

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
                        ctx.CommandStarted = DateTime.UtcNow;
                        ctx.Command = command.MainAlias;
                        await InvokeMethod(command.Method, command.moduleInfo.Instance, command.ConvertStringArgs(args, ctx).ToArray());
                        await EventService.AfterCommand(ctx);
                    }
                    catch (Exception e)
                    {
                        ErrorHandler.ReportError(new GenericError($"Error attempting to execute command: {e.Message}", ErrorSeverity.FATAL, e));
                    }
                }
            }
        }

    }
    internal class IdManager
    {
        internal IdGenerator Generator { get; set; }

        internal IdManager()
        {
            // Fun fact: This is the exact moment that SpookVooper was terminated
            // which led to the development of Valour becoming more than just a side
            // project. Viva la Vooperia.
            var epoch = new DateTime(2021, 1, 11, 4, 37, 0);

            var structure = new IdStructure(45, 10, 8);

            var options = new IdGeneratorOptions(structure, new DefaultTimeSource(epoch));

            Generator = new IdGenerator(0, options);
        }

        internal long Generate()
        {
            return (long)Generator.CreateId();
        }
    }
}
