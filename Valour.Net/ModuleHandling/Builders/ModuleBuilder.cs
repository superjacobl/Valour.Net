using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling.InfoModels;
using Valour.Net.ErrorHandling;
using Valour.Net.ModuleHandling.Models.InfoModels;
using Valour.Net.CommandHandling.Attributes;
using System.Linq.Expressions;
using Valour.Net.CustomAttributes;

namespace Valour.Net.CommandHandling.Builders
{
    internal class ModuleBuilder
    { 
        public ModuleInfo Module { get; set; }
        public ModuleBuilder()
        {
            Module = new();
        }

        /// <summary>
        /// Converts class to Valour.Net module
        /// </summary>
        /// <param name="module">Class type to convert</param>
        public void BuildModule(Type module)
        {
            ConstructorInfo constructor = module.GetConstructor(Type.EmptyTypes);
            CommandModuleBase moduleInstance  = (CommandModuleBase)constructor.Invoke(Array.Empty<object>());
            Module.Instance = moduleInstance;
            Module.Name = module.Name;
            Module.Groups = new();

			foreach (MethodInfo method in module.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).Where(m => m.GetCustomAttributes(typeof(EmbedMenuFuncAttribute), false).Length > 0))
			{
				var name = $"MENU$-{method.Module.Name}.{method.Name}";

                Console.WriteLine(moduleInstance);

				ParameterExpression param = Expression.Parameter(typeof(InteractionContext), method.GetParameters()[0].Name);

                MethodCallExpression c = null;

				if (!method.IsStatic) 
                    c = Expression.Call(Expression.Constant(moduleInstance), method, param);
                else
					c = Expression.Call(method, param);

				Func<InteractionContext, ValueTask> func = Expression.Lambda<Func<InteractionContext, ValueTask>>(c, param).Compile();
                EmbedMenuManager.ElementIdsToFuncs[name] = func;
			}

			foreach (MethodInfo _method in module.GetMethods().Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0))
            {
                CommandBuilder builder = new();
                var method = new ValourMethodInfo(_method);
                builder.BuildCommand(method, Module);
                var attrs = method.methodInfo.GetCustomAttributes(false).Where(x => x is IEventFilterAttribute).Select(x => (IEventFilterAttribute)x).ToList();
                if (attrs.Count > 0)
                    method.eventFilterAttributes.AddRange(attrs);
            }

            foreach (MethodInfo method in module.GetMethods().Where(m => m.GetCustomAttributes(typeof(EventAttribute), false).Length > 0))
            {
                foreach (var EventAttr in (IEnumerable<EventAttribute>)method.GetCustomAttributes(typeof(EventAttribute)))
                {
                    InfoModels.EventInfo eventInfo = new();
                    eventInfo.eventType = EventAttr.eventType;
                    eventInfo.Method = new ValourMethodInfo(method);
                    eventInfo.moduleInfo = Module;
                    EventService._Events.Add(eventInfo);
                }
            }

            foreach (MethodInfo _method in module.GetMethods().Where(m => m.GetCustomAttributes(typeof(InteractionAttribute), false).Length > 0))
            {
                var method = new ValourMethodInfo(_method);
                foreach (var EventAttr in (IEnumerable<InteractionAttribute>)method.methodInfo.GetCustomAttributes(typeof(InteractionAttribute)))
                {
                    InteractionEventInfo eventInfo = new();
                    eventInfo.InteractionElementId = EventAttr.InteractionElementId;
                    eventInfo.EventType = EventAttr.EventType;
                    eventInfo.InteractionFormId = EventAttr.InteractionFormId;
                    eventInfo.Method = method;
                    eventInfo.moduleInfo = Module;
                    EventService._InteractionEvents.Add(eventInfo);
                }
                var attrs = method.methodInfo.GetCustomAttributes(false).Where(x => x is IEventFilterAttribute).Select(x => (IEventFilterAttribute)x).ToList();
                if (attrs.Count > 0)
                    method.eventFilterAttributes.AddRange(attrs);
            }
        }
    }
}
