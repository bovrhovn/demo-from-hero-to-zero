using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Functions
{
    public class Event
    {
        public Event()
        {
            Id = Guid.NewGuid().ToString();
            EventTime = DateTime.Now;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
 
        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }
 
        [JsonProperty(PropertyName = "eventtype")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EventTypes EventTypes { get; set; }
 
        [JsonProperty(PropertyName = "eventtime")]
        public DateTime EventTime { get; set; }
    }

    public enum EventTypes
    {
        GENERAL_INFORMATION
    }
}