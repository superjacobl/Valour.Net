using System;
using System.Collections.Generic;
using System.Text;

namespace Valour.Net.ErrorHandling
{
    public enum ErrorTier
    {
        FATAL,
        WARN,
        INFO
    }
    class GenericError : Exception
    {
        public ErrorTier errorTier { get; set; }
        
        public GenericError(string message, ErrorTier errorTier, Exception innerException) : base(message, innerException)
        {
            this.errorTier = errorTier;
        }

        public GenericError(string message, ErrorTier errorTier) : base(message)
        {
            this.errorTier = errorTier;
        }

        public override string ToString()
        {
            return $"[{errorTier}] {Message}";
        }

    }
}
