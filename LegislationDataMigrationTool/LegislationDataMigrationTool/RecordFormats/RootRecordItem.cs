using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegislationDataMigrationTool.RecordFormats
{
    /// <summary>
    /// Used for sorting out records and determining what the order number of each record is
    /// </summary>
    public class RootRecordItem
    {
        public string ParentID { get; set; } = "";
        public List<String> ChildrenIDs { get; set; } = new List<String>();
    }
}
