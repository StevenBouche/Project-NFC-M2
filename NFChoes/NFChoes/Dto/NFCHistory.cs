using Newtonsoft.Json;

namespace NFChoes.Dto
{
    public class NFCHistory
    {
        [JsonProperty("userId")]
        public string UserId { get; set; } = string.Empty;
        [JsonProperty("storeId")]
        public string StoreId { get; set; } = string.Empty;
        [JsonProperty("inTimestamp")]
        public long InTimestamp { get; set; }
        [JsonProperty("outTimestamp")]
        public long? OutTimestamp { get; set; } = null;
    }
}
