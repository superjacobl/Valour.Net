﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling.InfoModels;
using Valour.Net.ErrorHandling;

namespace Valour.Net.CommandHandling.Builders
{
    public class ModuleBuilder
    { 
        ModuleInfo Module { get; set; }
        public ModuleBuilder(CommandService commandService)
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

            CommandBuilder builder = new();

            foreach (MethodInfo method in module.GetMethods().Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0))
            {
                builder.BuildCommand(method);
            }
            
        }


        /// <summary>
        /// Registers module to provided command service
        /// </summary>
        public void Register()
        {

        }
    }
}