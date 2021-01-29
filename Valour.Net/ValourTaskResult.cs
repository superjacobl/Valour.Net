using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Valour.Net
{
    public class ValourResponse<T>
    {
        [JsonProperty]
        public string Message { get; set; }

        [JsonProperty]
        public bool Success { get; set; }

        [JsonProperty]
        public T Data { get; set; }
    }
}