using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace T.Web.Areas.Admin.Models
{
    public class DataTableResponse<T>
    {
        [JsonProperty("draw")]
        [JsonPropertyName("draw")]
        public int Draw { get; set; }

        [JsonProperty("recordsTotal")]
        [JsonPropertyName("recordsTotal")]
        public int RecordsTotal { get; set; }

        [JsonProperty("recordsFiltered")]
        [JsonPropertyName("recordsFiltered")]
        public int RecordsFiltered { get; set; }

        [JsonProperty("data")]
        [JsonPropertyName("data")]
        public List<T> Data { get; set; }
        public string Error { get; set; }
    }
}
