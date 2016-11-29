using Newtonsoft.Json;
using System;

namespace Inspiro.DataModels
{
    public class Auth
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "createdOn")]
        public DateTime DateCreated { get; set; }

        [JsonProperty(PropertyName = "lastAccessed")]
        public DateTime DateAccessed { get; set; }

    }
}