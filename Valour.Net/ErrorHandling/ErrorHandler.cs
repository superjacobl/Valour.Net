using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valour.Net.ErrorHandling
{
    internal class ErrorHandler
    {
        internal ErrorHandler()
        {
        }


        internal static void ReportError(GenericError error)
        {
            Console.WriteLine(error.ToString());
        }

    }
}
