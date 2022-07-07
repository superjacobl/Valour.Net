using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Valour.Net
{
    public class ValourResponse<T>
    {
        [JsonInclude]
        public string Message { get; set; }

        [JsonInclude]
        public bool Success { get; set; }

        [JsonInclude]
        public T Data { get; set; }
    }
}