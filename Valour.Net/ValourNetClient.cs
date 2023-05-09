global using Valour.Api.Models;
global using Valour.Api.Items;
global using Valour.Shared.Models;
global using Valour.Net.Client;
global using Valour.Api.Models.Messages;
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
global using Valour.Api.Models.Messages.Embeds;

//using Microsoft.AspNetCore.SignalR.Client;
using Valour.Net.CommandHandling;
using Valour.Net.CommandHandling.InfoModels;
using Valour.Net.ErrorHandling;
using IdGen;
using Microsoft.AspNetCore.SignalR.Client;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Valour.Net.Client.MessageHelper;
using System.Xml.Linq;
using System.Threading;
using System.Collections.Concurrent;
using Valour.Net.ModuleHandling.Models.InfoModels;
using Valour.Net.CustomAttributes;
using System.Numerics;

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

        //internal static ConcurrentDictionary<long, PlanetTransaction> ProcessedTransactions = new();

        /// <summary>
        /// If true, your bot will receive planet transactions and will enable other economic features
        /// </summary>
        public static bool EnablePlanetEconomies = false;

        /// <summary>
        /// If true, your bot will not receive messages, nor will it send messages
        /// </summary>
        public static bool UnitTesting = false;

        /// <summary>
        /// If true, your bot will respond to itself.
        /// </summary
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

        /// <summary>
        /// If true, interactions will be executed in parallel.
        /// Please note, if you use a database be sure to prevent two threads from executing something at the same time on a single database context.
        /// </summary>
        public static bool ExecuteInteractionsInParallel = false;

        /// <summary>
        /// If true, planet transactions will be executed in parallel. Events for planet transactions will also be handled in parallel. 
        /// Please note, if you use a database, be sure to prevent two threads from executing something at the same time on a single database context.
        /// </summary>
        public static bool ExecuteTransactionsInParallel = false;

        internal static readonly ServiceCollection _serviceCollection = new();
        internal static IServiceProvider? _appServices;

        public static IServiceCollection Services => _serviceCollection;

        internal static async ValueTask InvokeMethod(ValourMethodInfo valourMethodInfo, CommandModuleBase instance, object[] args)
        {
            //methodInfo.ReturnType.GetMethod(nameof(Task.GetAwaiter));
            if (valourMethodInfo.eventFilterAttributes is not null && valourMethodInfo.eventFilterAttributes.Count > 0)
            {
                foreach (var filterattr in valourMethodInfo.eventFilterAttributes)
                {
                    var filter = filterattr.eventFilter;
                    EventFilterInvocationContext context = new()
                    {
                        Context = (IContext)args[0],
                        Arguments = args
                    };
                    var result = await filter.InvokeAsync(context);
                    if (result is Task || result is ValueTask)
                    {
                        await (Task)result;
                        return;
                    }
                }
            }
            if (valourMethodInfo.methodInfo.ReturnType == typeof(Task) || valourMethodInfo.methodInfo.ReturnType == typeof(ValueTask))
            {
                await (Task)valourMethodInfo.methodInfo.Invoke(instance, args);
            }
            else
            {
                valourMethodInfo.methodInfo.Invoke(instance, args);
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
            
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}api/users/token", content);

            Console.WriteLine($"Url:{BaseUrl}api/users/token");
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
                await ctx.Set(m);
                await EventService.OnChannelWatching(ctx);
            };
            ValourClient.OnFriendsUpdate += async () => { };

			ValourClient.BaseAddress = BaseUrl;

            // set up signar stuff
        
            Console.WriteLine("Registering Modules");

            ModuleRegistrar.RegisterAllCommands();

            Console.WriteLine("Grabbing Bot's User info & logging in");

            var client = new HttpClient()
            {
                BaseAddress = new Uri(BaseUrl)
            };

			ValourClient.SetHttpClient(client);

            Token = (await ValourClient.GetToken(email, password)).Data;
            if (Token == null) //Token returned null meaning valour is unavailable
                return;

			await ValourClient.InitializeUser(Token);

            BotId = ValourClient.Self.Id;

            Console.WriteLine("Loading all node data");

            foreach (var _planet in ValourCache.HCache[typeof(Planet)].Values.ToList())
            {
                var planet = (Planet)_planet;
                if (NodeManager.NameToNode.ContainsKey("debug-node"))
                    planet.NodeName = "debug-node";

                if (!NodeManager.NameToNode.ContainsKey(planet.NodeName))
                {
                    var node = new Node();
                    await node.InitializeAsync(planet.NodeName, ValourClient.Token);
                }
            }

            Console.WriteLine("Loading data into cache");

            await Task.Run(() => Parallel.ForEach(ValourCache.HCache[typeof(Planet)].Values.ToList(), async _planet =>
            {
				var planet = (Planet)_planet;
				Console.WriteLine($"Loading: {planet.Name}");
				await planet.LoadRolesAsync();
				await planet.LoadMemberDataAsync();
				await planet.LoadChannelsAsync();
                await planet.LoadCategoriesAsync();
                //if (EnablePlanetEconomies)
                    //await planet.LoadCurrencyAccountsAsync();
                await Task.Delay(10);
            }));

			Console.WriteLine("Loaded All Planet Data & Channel Data into Cache");

			Console.WriteLine("Joining Planets & Channels");

            foreach (var node in NodeManager.Nodes)
            {
                await HubConnection_Reconnected(node);
            }

            Console.WriteLine("Done Joining Planets & Channels");

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

            //ValourClient.OnTransactionRecieved += async (PlanetTransaction transaction) =>
            //{
            //    if (ExecuteTransactionsInParallel)
           //     {
            //        OnTransactionRelay(transaction);
           //     }
           //     else
           //     {
          //          await OnTransactionRelay(transaction);
           //     }
          // };

            ItemObserver<PlanetChatChannel>.OnAnyUpdated += OnChannelUpdate;

            ItemObserver<PermissionsNode>.OnAnyUpdated += OnPermissionsNodeUpdate;

            Console.WriteLine("\n\r-----Ready-----\n\r");
        }

        internal static async Task JoinCategory(PlanetCategory category) {
            foreach(PlanetChatChannel channel in ValourCache.GetAll<PlanetChatChannel>().Where(x => x.ParentId == category.Id)) 
            {
                var servernode = await NodeManager.GetNodeForPlanetAsync(category.PlanetId);
                await servernode.HubConnection.SendAsync("JoinChannel", channel.Id);
            }
            foreach(PlanetCategory _category in ValourCache.GetAll<PlanetCategory>().Where(x => x.ParentId == category.Id)) 
            {
                JoinCategory(_category);
            }
        }

        internal static async Task OnPermissionsNodeUpdate(PermissionsNode node, bool newitem, int flags)
        {
            // try to connect to the channel
            if (node.TargetType == PermChannelType.PlanetChatChannel) {
                var servernode = await NodeManager.GetNodeForPlanetAsync(node.PlanetId);
                await servernode.HubConnection.SendAsync("JoinChannel", node.TargetId, Token);
            }
            // or try to connect to all channels in the category
            else if (node.TargetType == PermChannelType.PlanetCategoryChannel) {
                var category = await PlanetCategory.FindAsync(node.TargetId, node.PlanetId);
                JoinCategory(category);
            }
        }

        internal static async Task OnChannelUpdate(PlanetChatChannel channel, bool newitem, int flags) 
        {
            if (newitem) {
                var node = await NodeManager.GetNodeForPlanetAsync(channel.PlanetId);
                // send JoinChannel to hub so we can get messages from the new channel
                await node.HubConnection.SendAsync("JoinChannel", channel.Id);
            }
        }


        internal static async Task HubConnection_Reconnected(Node node)
        {
            // Joins SignalR groups
            var options = new ParallelOptions { MaxDegreeOfParallelism = 10 };
            var planets = ValourCache.HCache[typeof(Planet)].Values.Select(x => (Planet)x).Where(x => x.NodeName == node.Name).ToList();
            await Parallel.ForEachAsync(planets, async (planet, token) => 
            {
                var result = await node.HubConnection.InvokeAsync<TaskResult>("JoinPlanet", planet.Id);

                Console.WriteLine($"Connected to planet: {planet.Name} (node: {planet.NodeName})");

                await node.HubConnection.SendAsync("JoinInteractionGroup", planet.Id);
                
                if (EnablePlanetEconomies)
                    await node.HubConnection.SendAsync("JoinPlanetTransactions", planet.Id);

                node.HubConnection.Remove("InteractionEvent");
                node.HubConnection.On<EmbedInteractionEvent>("InteractionEvent", OnInteractionEvent);
                
                var channels = await planet.GetChannelsAsync();


                foreach (PlanetChatChannel channel in channels)
                {
                    await node.HubConnection.SendAsync("JoinChannel", channel.Id);
                }

            });
        }

        //internal static async Task OnTransactionRelay(PlanetTransaction transaction)
       // {
          //  ProcessedTransactions[transaction.Id] = transaction;
          //  var trans = ProcessedTransactions.Values.Where(x => x.Executed.AddMinutes(1) < DateTime.UtcNow);
          //  foreach(var tran in trans)
           //     ProcessedTransactions.Remove(tran.Id, out _);

           // if (!transaction.Result.Value.Success)
          //     return;

            //PlanetCurrencyAccount fromAccount = await PlanetCurrencyAccount.FindAsync(transaction.FromId);
           // PlanetCurrencyAccount toAccount = await PlanetCurrencyAccount.FindAsync(transaction.ToId);
            //fromAccount.Balance -= transaction.Amount;
            //toAccount.Balance += transaction.Amount;

            //PlanetMessage message = (PlanetMessage)_message;
            //CommandContext ctx = new()
           // {
           //    TimeReceived = DateTime.UtcNow,
            //    MessageTimeTook = DateTime.UtcNow - message.TimeSent
           // };
       // }

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
            if (ExecuteInteractionsInParallel)
                EventService.OnInteraction(interactionEvent);
            else
                await EventService.OnInteraction(interactionEvent);
        }

       // public static async Task<PlanetTransaction> SendTransactionRequestAsync(PlanetTransactionRequest request)
       // {
       //     var result = await request.SendAsync();

       //     while (!ValourNetClient.ProcessedTransactions.ContainsKey(result.Data)) 
       //         await Task.Delay(10);

       //     ValourNetClient.ProcessedTransactions.Remove(result.Data, out var transaction);
       //     return transaction;
       // }

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