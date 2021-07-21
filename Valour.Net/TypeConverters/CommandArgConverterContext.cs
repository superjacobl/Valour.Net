using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valour.Net.CommandHandling;

namespace Valour.Net.TypeConverters
{
    class CommandArgConverterContext : ITypeDescriptorContext
    {
        public CommandArgConverterContext(object instance, CommandContext ctx)
        {
            Instance = instance;
            PropertyDescriptor = TypeDescriptor.GetProperties(instance)[0];
            this.ctx = ctx;
        }

        public object Instance { get; private set; }
        public PropertyDescriptor PropertyDescriptor { get; private set; }
        public CommandContext ctx { get; private set; }

        public IContainer Container { get; private set; }

        public void OnComponentChanged()
        {
        }

        public bool OnComponentChanging()
        {
            return true;
        }

        public object GetService(Type serviceType)
        {
            return null;
        }
    }
}
