using Newtonsoft.Json;
using System;

namespace Inspiro.DataModels
{
    public class Accounts
    {
            [JsonProperty(PropertyName = "Id")]
            public string ID { get; set; }

            [JsonProperty(PropertyName = "cheque")]
            public double Cheque { get; set; }

            [JsonProperty(PropertyName = "savings")]
            public double Savings { get; set; }

            [JsonProperty(PropertyName = "createdOn")]
            public DateTime DateCreated { get; set; }

            [JsonProperty(PropertyName = "lastAccessed")]
            public DateTime DateAccessed { get; set; }
        
    }
}