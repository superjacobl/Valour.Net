using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool IsOverload { get; set; }
        public CommandInfo MainOverload { get; set; }

        public void AddParameter(ParameterInfo parameter)
        {
            Parameters.Add(parameter);
        }
    }
}
