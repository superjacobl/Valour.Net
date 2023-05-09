using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling.InfoModels;
using Valour.Net.CommandHandling.Attributes;
using Valour.Net.ErrorHandling;
using Valour.Net.ModuleHandling.Models.InfoModels;

namespace Valour.Net.CommandHandling.Builders
{
    internal class CommandBuilder
    {
        public CommandInfo Command { get; set; }

        public CommandBuilder()
        {
            Command = new CommandInfo();
        }

        public void AddParameter(System.Reflection.ParameterInfo parameterinfo)
        {
            ParameterBuilder builder = new ParameterBuilder(parameterinfo);
            Command.Parameters.Add(builder.Parameter);
        }

        public void BuildCommand(ValourMethodInfo Method, ModuleInfo moduleInfo)
        {
            //DEBUG
            Type magicType = Method.methodInfo.DeclaringType;
            CommandAttribute CommandAttr = (CommandAttribute)Method.methodInfo.GetCustomAttribute(typeof(CommandAttribute));
            Command.MainAlias = CommandAttr.Name;
            Command.Aliases = new List<string>();
            Command.Method = Method;
            Command.moduleInfo = moduleInfo;
            AliasAttribute AliasAttr = (AliasAttribute)Method.methodInfo.GetCustomAttribute(typeof(AliasAttribute));
            if (AliasAttr != null) {
                foreach (string alias in AliasAttr.Aliases) {
                    Command.Aliases.Add(alias.ToLower());
                }
            }
            
            OnlyRoleAttribute OnlyRolesAttr = (OnlyRoleAttribute)Method.methodInfo.GetCustomAttribute(typeof(OnlyRoleAttribute));
            if (OnlyRolesAttr != null) {
                Command.OnlyRoles = new List<string>();
                Command.OnlyRoles.AddRange(OnlyRolesAttr.Roles);
            }

            ExpectRoleAttribute expectRoleAttribute = (ExpectRoleAttribute)Method.methodInfo.GetCustomAttribute(typeof(ExpectRoleAttribute));
            if (expectRoleAttribute != null) {
                Command.ExpectRoles = new List<string>();
                Command.ExpectRoles.AddRange(expectRoleAttribute.Roles);
            }

            FallBackAttribute fallBackAttribute = (FallBackAttribute)Method.methodInfo.GetCustomAttribute(typeof(FallBackAttribute));
            if (fallBackAttribute != null)
            {
                if (Method.methodInfo.GetParameters().Any(parameter => parameter.ParameterType != typeof(CommandContext)))
                {
                    Console.WriteLine(new GenericError($"Fallback method {Command.MainAlias} in module {moduleInfo.Name} contains parameters other than CommandContext. Command will not be loaded", ErrorSeverity.WARN));
                    return;
                }
                Command.IsFallback = true;
            }

            if (!Command.IsFallback)
            {
                foreach (System.Reflection.ParameterInfo parameterinfo in Method.methodInfo.GetParameters())
                {
                    if (parameterinfo.ParameterType != typeof(CommandContext))
                    {
                        AddParameter(parameterinfo);
                    }
                }
            }

            moduleInfo.AddCommand(Command);
            CommandService.RegisterCommand(Command);

            //CommandAttr.
            //Method.Invoke(magicClassObject, null);
        }
    }
}
