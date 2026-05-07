using System.Collections.Generic;
using System.Text.Json.Serialization;

public class ShortcutData
    {
        [JsonPropertyName("shortcutName")]
        public string Name { get; set; }

        [JsonPropertyName("shortcutDescription")]
        public string Description { get; set; }

        [JsonPropertyName("shortcutKeys")]
        public List<string> Keys { get; set; }
    }