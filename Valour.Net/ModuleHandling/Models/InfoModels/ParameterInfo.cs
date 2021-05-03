using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valour.Net.CommandHandling.InfoModels
{
    public class ParameterInfo
    {
        public CommandInfo Command {get; set;}
        public string Name { get; set; }
        public string Summary {get; set;}
        public Type Type { get; set; }
        public Boolean IsRemainder {get; set;}


        public ParameterInfo(string Name, Type Type)
        {
            this.Name = Name;
            this.Type = Type;
        }

        public ParameterInfo()
        {
            
        }
    }
}
