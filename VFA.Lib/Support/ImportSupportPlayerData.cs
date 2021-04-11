using System.Collections.Generic;

namespace VFA.Lib.Support
{
    public class ImportSupportPlayerData
    {
        public int count { get; set; }
        public int count_total { get; set; }
        public int page { get; set; }
        public int page_total { get; set; }
        public int items_per_page { get; set; }

        public List<APIPlayerData> items { get; set; }

        public ImportSupportPlayerData()
        {
            this.items = new List<APIPlayerData>();
        }

    }
}
