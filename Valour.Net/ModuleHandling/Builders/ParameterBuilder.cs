using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling.InfoModels;
using Valour.Net.CommandHandling.Attributes;

namespace Valour.Net.CommandHandling.Builders
{
    public class ParameterBuilder
    {
        public ParameterInfo Parameter { get; set; }
        public ParameterBuilder()
        {
            Parameter = new();
        }

        public ParameterBuilder(string name, Type type)
        {
            Parameter = new();
            Parameter.Name = name;
            Parameter.Type = type;
        }

        public ParameterBuilder(System.Reflection.ParameterInfo parameterinfo)
        {
            Parameter = new();
            Parameter.Name = parameterinfo.Name;
            Parameter.Type = parameterinfo.ParameterType;
            foreach (System.Reflection.CustomAttributeData attribute in parameterinfo.CustomAttributes) {
                if (attribute.AttributeType == typeof(RemainderAttribute)) {
                    Parameter.IsRemainder = true;
                }
            }
        }


        public void BuildParameter(string name, Type type)
        {
            Parameter = new ParameterInfo();
            Parameter.Name = name;
            Parameter.Type = type;
        }
    }
}
