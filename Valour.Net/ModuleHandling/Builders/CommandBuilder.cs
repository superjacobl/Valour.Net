﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling.InfoModels;
using Valour.Net.ModuleHandling.Attributes;
using Valour.Net.ErrorHandling;

namespace Valour.Net.CommandHandling.Builders
{

    
    public class CommandBuilder
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

        public void BuildCommand(MethodInfo Method, ModuleInfo moduleInfo)
        {
            //DEBUG
            Type magicType = Method.DeclaringType;
            CommandAttribute CommandAttr = (CommandAttribute)Method.GetCustomAttribute(typeof(CommandAttribute));
            Command.MainAlias = CommandAttr.Name;
            Command.Aliases = new List<string>();
            Command.Method = Method;
            Command.moduleInfo = moduleInfo;
            AliasAttribute AliasAttr = (AliasAttribute)Method.GetCustomAttribute(typeof(AliasAttribute));
            if (AliasAttr != null) {
                foreach (string alias in AliasAttr.Aliases) {
                    Command.Aliases.Add(alias.ToLower());
                }
            }
            
            OnlyRoleAttribute OnlyRolesAttr = (OnlyRoleAttribute)Method.GetCustomAttribute(typeof(OnlyRoleAttribute));
            if (OnlyRolesAttr != null) {
                Command.OnlyRoles = new List<string>();
                Command.OnlyRoles.AddRange(OnlyRolesAttr.Roles);
            }

            ExpectRoleAttribute expectRoleAttribute = (ExpectRoleAttribute)Method.GetCustomAttribute(typeof(ExpectRoleAttribute));
            if (expectRoleAttribute != null) {
                Command.ExpectRoles = new List<string>();
                Command.ExpectRoles.AddRange(expectRoleAttribute.Roles);
            }

            FallBackAttribute fallBackAttribute = (FallBackAttribute)Method.GetCustomAttribute(typeof(FallBackAttribute));
            if (fallBackAttribute != null)
            {
                if (Method.GetParameters().Any(parameter => parameter.ParameterType != typeof(CommandContext)))
                {
                    Console.WriteLine(new GenericError($"Fallback method {Command.MainAlias} in module {moduleInfo.Name} contains parameters other than CommandContext. Command will not be loaded", ErrorSeverity.WARN));
                    return;
                }
                Command.IsFallback = true;
            }



            if (!Command.IsFallback)
            {
                foreach (System.Reflection.ParameterInfo parameterinfo in Method.GetParameters())
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
