using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegislationDataMigrationTool.RecordFormats
{
    public class InputFileRecord
    {
        public string ImportKeyID { get; set; }
        public string LegislationType { get; set; }
        public string ParentLegislation { get; set; }
        public string Qm_rcparentlegislationid { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string EnglishText { get; set; }
        public string FrenchText { get; set; }
        public string ProvisionsHeadingAppliesTo { get; set; }
        public string LegislationSource { get; set; }
        public string Qm_inforcedte { get; set; }
        public int Order { get; set; } = 0;
        public bool IsRoot { get; set; }
        public bool IsHeader { get; set; }
        public bool Delete { get; set; }
        public List<InputFileRecord> Childern { get; set; } = new List<InputFileRecord>();
    }
}
