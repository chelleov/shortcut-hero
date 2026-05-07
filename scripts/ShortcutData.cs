using System.Collections.Generic;

public class ShortcutData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Keys { get; set; }
        public bool IsOrderImportant { get; set; }
    }