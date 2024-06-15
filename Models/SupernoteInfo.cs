using System.Text.Json.Serialization;

namespace SupernoteDesktopClient.Models
{
    public class SupernoteInfo
    {
        public string Model { get; set; } = "N/A";

        [JsonIgnore]
        public string SerialNumber { get; set; } = "N/A";

        public string SerialNumberHash { get; set; } = "N/A";

        [JsonIgnore]
        public int PowerLevel { get; set; } = 0;

        [JsonIgnore]
        public long AvailableFreeSpace { get; set; } = 0;

        [JsonIgnore]
        public long TotalSpace { get; set; } = 0;

        [JsonIgnore]
        public string RootFolder { get; set; } = "N/A";

        public bool Active { get; set; } = false;
    }
}
