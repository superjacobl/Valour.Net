using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Valour.Net.Models;

namespace Valour.Net.CommandHandling.InfoModels
{
    public class CommandInfo
    {
        public string MainAlias { get; set; }
        public List<string> Aliases { get; set; }
        public List<Attribute> Attributes { get; set; }
        public List<string> OnlyRoles { get; set; }
        public List<string> ExpectRoles { get; set; }
        public List<ParameterInfo> Parameters { get; set; }
        public ModuleInfo moduleInfo {get ;set;}
        public MethodInfo Method { get; set; }


        public CommandInfo(string MainAlias, List<string> Aliases, List<Attribute> Attributes, List<ParameterInfo> Parameters, MethodInfo Method)
        {
            this.MainAlias = MainAlias;
            this.Aliases = Aliases;
            this.Attributes = Attributes;
            this.Parameters = Parameters;
            this.Method = Method;
        }

        public List<object> ConvertStringArgs(List<string> args, CommandContext ctx) {
            List<object> objects = new List<object>();
            objects.Add(ctx);
            for (int i = 0; i < Parameters.Count(); i++)
            {
                TypeConverter typeConverter = TypeDescriptor.GetConverter(Parameters[i].Type);
                if (Parameters[i].IsRemainder == false) {
                    objects.Add(typeConverter.ConvertFromString(args[i]));
                }
                else {
                    string remainder = "";
                    foreach (string arg in args.GetRange(i, args.Count()-1)){
                        remainder += $"{arg} ";
                    }
                    remainder.Substring(0,remainder.Count()-2);
                    objects.Add(remainder);
                    return objects;
                }
               
            }
            return objects;
        }

        // check if a commandname is this command

        public bool CheckIfCommand(string name, List<string> args, CommandContext ctx) {
            if (MainAlias.ToLower() == name || Aliases.Contains(name)) {
                if (args.Count() != Parameters.Count()) {
                    if (Parameters.Count() > 0) {
                        if (Parameters.Last().IsRemainder == false) {
                            return false;
                        }
                    }
                    else {
                        return false;
                    }
                    
                }
                if (args.Count() == 0 && Parameters.Count() > 0) {
                    return false;
                }

                for (int i = 0; i < Parameters.Count(); i++)
                {
                    try {
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(Parameters[i].Type);
                        typeConverter.ConvertFromString(args[i]);
                    }
                    catch {
                        return false;
                    }
                }

                // check OnlyRoles

                if (OnlyRoles != null) {
                    foreach (string RoleName in OnlyRoles) {
                        if (ctx.Member.Roles.Any(x => x.Name == RoleName) != true) {
                            EventService.UserLacksTheRolesToUseACommand(this, ctx);
                            return false;
                        }
                    }
                }

                // check Eepect Roles

                if (ExpectRoles != null) {
                    foreach (string RoleName in ExpectRoles) {
                        if (ctx.Member.Roles.Any(x => x.Name == RoleName) == true) {
                            return false;
                        }
                    }
                }

                return true;
            }
            else {
                return false;
            }
        }

        //DEBUG
        public CommandInfo()
        {
            Parameters = new List<ParameterInfo>();
        }
    }
}
