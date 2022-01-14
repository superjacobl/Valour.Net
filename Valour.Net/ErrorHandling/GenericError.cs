using System;
using System.Collections.Generic;
using System.Text;

namespace Valour.Net.ErrorHandling
{
    public enum ErrorSeverity
    {
        FATAL,
        WARN,
        INFO
    }
    public class GenericError : Exception
    {
        public ErrorSeverity errorTier { get; set; }
        public DateTime Time { get; set; }
        public GenericError(string message, ErrorSeverity errorTier, Exception innerException) : base(message, innerException)
        {
            this.errorTier = errorTier;
            this.Time = DateTime.Now;
        }

        public GenericError(string message, ErrorSeverity errorTier) : base(message)
        {
            this.errorTier = errorTier;
            this.Time = DateTime.Now;
        }

        public override string ToString()
        {
            if (InnerException != null)
            {
                return $"[{Time:HH:mm:ss}][{errorTier}] {Message}\n{InnerException.StackTrace}";
            }
            return $"[{Time:HH:mm:ss}][{errorTier}] {Message}";
        }
    }
}
