using System;
using System.Collections.Generic;

namespace WpfApplication
{
    [Serializable]
    public class TableViewData
    {
        public string ImageName { get; set; }
        public List<TableElementData> Elements { get; set; }
    }
}