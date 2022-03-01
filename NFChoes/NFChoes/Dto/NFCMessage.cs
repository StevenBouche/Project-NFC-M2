using Newtonsoft.Json;

namespace NFChoes.Dto
{
    public class NFCMessage
    {
        [JsonProperty("userId")]
        public string UserId { get; set; } = string.Empty;
        [JsonProperty("storeId")]
        public string StoreId { get; set; } = string.Empty;
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
    }
}
