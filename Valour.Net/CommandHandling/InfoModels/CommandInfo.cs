using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Valour.Net.CommandHandling.InfoModels
{
    public class CommandInfo
    {
        public string MainAlias { get; set; }
        public List<string> Aliases { get; set; }
        public List<Attribute> Attributes { get; set; }
        public List<ParameterInfo> Parameters { get; set; }
        public MethodInfo Method { get; set; }


        public CommandInfo(string MainAlias, List<string> Aliases, List<Attribute> Attributes, List<ParameterInfo> Parameters, MethodInfo Method)
        {
            this.MainAlias = MainAlias;
            this.Aliases = Aliases;
            this.Attributes = Attributes;
            this.Parameters = Parameters;
            this.Method = Method;
        }

        //DEBUG
        public CommandInfo()
        {

        }
    }
}
