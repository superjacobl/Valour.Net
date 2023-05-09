using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using Valour.Net.TypeConverters;
using Valour.Net.ModuleHandling.Models.InfoModels;
using Valour.Net.CustomAttributes;

namespace Valour.Net.CommandHandling.InfoModels;

internal class CommandInfo
{
    public string MainAlias { get; set; }
    public List<string> Aliases { get; set; }
    //public List<Attribute> Attributes { get; set; }
    public List<string> OnlyRoles { get; set; }
    public List<string> ExpectRoles { get; set; }
    public List<ParameterInfo> Parameters { get; set; }
    public ModuleInfo moduleInfo {get ;set;}
    public ValourMethodInfo Method { get; set; }
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

    public List<object> ConvertStringArgs(List<string> args, CommandContext ctx) {
        List<object> objects = new();
        objects.Add(ctx);
        for (int i = 0; i < Parameters.Count; i++)
        {
            if (Parameters[i].Info.HasDefaultValue) {
                var attribute = (SwitchInputAttribute)Parameters[i].Info.GetCustomAttribute(typeof(SwitchInputAttribute));
                if (attribute is not null) {
                    string name = args.Where(x => x.StartsWith("--")).Select(x => x.Substring(2)).FirstOrDefault(x => x == attribute.SwitchName);
                    if (name is null) {
                        objects.Add((bool)Parameters[i].Info.DefaultValue);
                    }
                    else {
                        objects.Add(!(bool)Parameters[i].Info.DefaultValue);
                    }
                    continue;
                }
                else {
                    if (i > Parameters.Count-1) {
                        objects.Add(Parameters[i].Info.DefaultValue);
                        continue;
                    }
                }
            }
            TypeConverter typeConverter = TypeDescriptor.GetConverter(Parameters[i].Type);
            if (Parameters[i].IsRemainder == false) {
                if (Parameters[i].Info.HasDefaultValue && i > args.Count-1) {
                    objects.Add(null);
                    continue;
                }
                if (PlanetMemberConverter.CanConvertFrom(args[i])) {
                    objects.Add(PlanetMemberConverter.ConvertFrom(new CommandArgConverterContext(ctx), System.Globalization.CultureInfo.CurrentCulture, args[i]));
                }
                else
                {
                    objects.Add(typeConverter.ConvertFrom(new CommandArgConverterContext(ctx), System.Globalization.CultureInfo.CurrentCulture, args[i]));
                }
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
                    int i = args.Count;
                    foreach(var parameter in Parameters) {
                        if (parameter.Info.HasDefaultValue) {
                            i += 1;
                        }
                    }
                    if (i != Parameters.Count) {
                        if (Parameters.Last().IsRemainder == false) {
                            return false;
                        }
                    }
                }
                else {
                    return false;
                }
                
            }

            if (args.Count == 0 && Parameters.Count > 0 && !Parameters.Any(x => x.Info.HasDefaultValue)) {
                return false;
            }

            for (int i = 0; i < Parameters.Count; i++)
            {
                try {
                    if (Parameters[i].Info.HasDefaultValue) {
                        continue;
                    }
                    TypeConverter typeConverter = TypeDescriptor.GetConverter(Parameters[i].Type);

                    // check planetmembers
                    if (PlanetMemberConverter.CanConvertFrom(args[i])) {
                        Console.WriteLine("HI!");
                    }
                    else 
                    {
                        if (!typeConverter.CanConvertFrom(args[i].GetType()))
                        {
                            return false;
                        }   
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
                    if ((await ctx.Member.GetRolesAsync()).Any(x => x.Name == RoleName) != true) {
                        await EventService.UserLacksTheRolesToUseACommand(this, ctx);
                        return false;
                    }
                }
            }

            // check Eepect Roles

            if (ExpectRoles != null) {
                foreach (string RoleName in ExpectRoles) {
                    if ((await ctx.Member.GetRolesAsync()).Any(x => x.Name == RoleName) == true) {
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
