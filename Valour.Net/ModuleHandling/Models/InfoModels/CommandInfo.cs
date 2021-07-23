using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Valour.Net.Models;
using Valour.Net.TypeConverters;

namespace Valour.Net.CommandHandling.InfoModels
{
    public class CommandInfo
    {
        public string MainAlias { get; set; }
        public List<string> Aliases { get; set; }
        //public List<Attribute> Attributes { get; set; }
        public List<string> OnlyRoles { get; set; }
        public List<string> ExpectRoles { get; set; }
        public List<ParameterInfo> Parameters { get; set; }
        public ModuleInfo moduleInfo {get ;set;}
        public MethodInfo Method { get; set; }
        public bool IsFallback { get; set; }

        /*
        public CommandInfo(string MainAlias, List<string> Aliases, List<ParameterInfo> Parameters, MethodInfo Method)
        {
            this.MainAlias = MainAlias;
            this.Aliases = Aliases;
            //this.Attributes = Attributes;
            this.Parameters = Parameters;
            this.Method = Method;
        }
        */

        public async Task<List<object>> ConvertStringArgs(List<string> args, CommandContext ctx) {
            List<object> objects = new List<object>();
            objects.Add(ctx);
            for (int i = 0; i < Parameters.Count; i++)
            {
                TypeConverter typeConverter = TypeDescriptor.GetConverter(Parameters[i].Type);
                if (Parameters[i].IsRemainder == false)
                {
                    objects.Add(typeConverter.ConvertFrom(new CommandArgConverterContext(ctx), System.Globalization.CultureInfo.CurrentCulture, args[i]));
                }
                else
                {
                    string remainder = "";
                    foreach (string arg in args.GetRange(i, args.Count - i))
                    {
                        remainder += $"{arg} ";
                    }
                    //remainder.Substring(0,remainder.Count()-2);
                    remainder = remainder.TrimEnd();
                    objects.Add(remainder);
                    return objects;
                }
               
            }
            return objects;
        }

        // check if a commandname is this command

        public async Task<bool> CheckIfCommand(string name, List<string> args, CommandContext ctx) {
            //Console.WriteLine(Aliases);
            if (MainAlias.ToLower() == name.ToLower() || Aliases.Contains(name.ToLower())) {
                if (args.Count != Parameters.Count) {
                    if (Parameters.Count > 0) {
                        if (Parameters.Last().IsRemainder == false) {
                            return false;
                        }
                    }
                    else {
                        return false;
                    }
                    
                }
                if (args.Count == 0 && Parameters.Count > 0) {
                    return false;
                }

                for (int i = 0; i < Parameters.Count; i++)
                {
                    try {
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(Parameters[i].Type);
                        
                        if (!typeConverter.CanConvertFrom(args[i].GetType()))
                        {
                            return false;
                        }                        
                    }
                    catch (Exception e){
                        ErrorHandling.ErrorHandler.ReportError(new("Severe error converting argument", ErrorHandling.ErrorSeverity.FATAL, e));
                        return false;
                    }
                }

                // check OnlyRoles

                if (OnlyRoles != null) {
                    foreach (string RoleName in OnlyRoles) {
                        if (ctx.Member.Roles.Any(x => x.Name == RoleName) != true) {
                            await EventService.UserLacksTheRolesToUseACommand(this, ctx);
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
