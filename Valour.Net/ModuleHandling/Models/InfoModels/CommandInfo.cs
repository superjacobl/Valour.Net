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
                if (Parameters[i].Type == typeof(ValourUser))
                {
                    if (!ulong.TryParse(args[i], out ulong UserID))
                    {
                        ValourUser StringUser = await Cache.GetValourUser(Cache.PlanetMemberCache.Values.FirstOrDefault(x => x.Nickname.ToLower() == args[i].ToLower()).User_Id); //Replace to directly get User
                        if (StringUser == null)
                        {
                            ValourClient.errorHandler.ReportError(new("Incorrect input for ValourUser argument", ErrorHandling.ErrorSeverity.FATAL));
                            StringUser = new();
                        }
                        objects.Add(StringUser);
                    }
                    else
                    {
                        ValourUser User = await Cache.GetValourUser(UserID);
                        if (User == null)
                        {
                            ValourClient.errorHandler.ReportError(new("Incorrect input for ValourUser argument", ErrorHandling.ErrorSeverity.FATAL));
                            User = new();
                        }
                        objects.Add(User);
                    }

                }
                else if (Parameters[i].Type == typeof(PlanetMember))
                {
                    if (!ulong.TryParse(args[i], out ulong MemberID))
                    {
                        PlanetMember StringMember = Cache.PlanetMemberCache.Values.FirstOrDefault(x => x.Nickname.ToLower() == args[i].ToLower()); //Replace to directly get User
                        if (StringMember == null)
                        {
                            ValourClient.errorHandler.ReportError(new("Incorrect input for ValourUser argument", ErrorHandling.ErrorSeverity.FATAL));
                            StringMember = new();
                        }
                        objects.Add(StringMember);
                    }
                    else
                    {
                        PlanetMember Member = await Cache.GetPlanetMember(ulong.Parse(args[i]), ctx.Planet.Id);
                        if (Member == null)
                        {
                            ValourClient.errorHandler.ReportError(new("Incorrect input for ValourUser argument", ErrorHandling.ErrorSeverity.FATAL));
                            Member = new();
                        }
                        objects.Add(Member);
                    }
                }
                else if (Parameters[i].IsRemainder == false)
                {
                    objects.Add(typeConverter.ConvertFromString(args[i]));
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
            if (MainAlias.ToLower() == name || Aliases.Contains(name)) {
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
                        if (Parameters[i].Type == typeof(ValourUser))
                        {
                            if (!ulong.TryParse(args[i], out ulong UserID))
                            {
                                ValourUser StringUser = await Cache.GetValourUser(Cache.PlanetMemberCache.Values.FirstOrDefault(x => x.Nickname.ToLower() == args[i].ToLower()).User_Id); //Replace to directly get User
                                if (StringUser == null)
                                {
                                    ValourClient.errorHandler.ReportError(new("Incorrect input for ValourUser argument", ErrorHandling.ErrorSeverity.FATAL));
                                    StringUser = new();
                                }
                                object obj = StringUser;
                            }
                            else
                            {
                                object obj = Cache.GetValourUser(ulong.Parse(args[i]));
                            }
                        }  
                        else if (Parameters[i].Type == typeof(PlanetMember))
                        {
                            if (!ulong.TryParse(args[i], out ulong MemberID))
                            {
                                PlanetMember StringMember = Cache.PlanetMemberCache.Values.FirstOrDefault(x => x.Nickname.ToLower() == args[i].ToLower()); //Replace to directly get User
                                if (StringMember == null)
                                {
                                    ValourClient.errorHandler.ReportError(new("Incorrect input for ValourUser argument", ErrorHandling.ErrorSeverity.FATAL));
                                    StringMember = new();
                                }
                                object obj = StringMember;
                            }
                            else
                            {
                                object obj = Cache.GetPlanetMember(ulong.Parse(args[i]),ctx.Planet.Id);
                            }
                        }
                        else
                        {
                            TypeConverter typeConverter = TypeDescriptor.GetConverter(Parameters[i].Type);
                            typeConverter.ConvertFromString(args[i]);
                        }
                        
                    }
                    catch (Exception e){
                        Console.WriteLine(e.Message);
                        Console.WriteLine(args[i]);
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
