using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valour.Net.ErrorHandling
{
    public class ErrorHandler
    {
        public ErrorHandler()
        {
        }


        public void ReportError(GenericError error)
        {
            Console.WriteLine(error);
        }

    }
}
