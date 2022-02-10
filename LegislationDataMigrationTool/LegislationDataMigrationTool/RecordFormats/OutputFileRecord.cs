using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegislationDataMigrationTool.RecordFormats
{
    public class OutputFileRecord
    {
        public int ts_importkey { get; set; }
        public string LegislationType { get; set; }
        public string LegislationTypeFrench { get; set; }
        public string ParentLegislation { get; set; }
        public string Qm_rcparentlegislationid { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string EnglishText { get; set; }
        public string FrenchText { get; set; }
        public string ProvisionsHeadingAppliesTo { get; set; }
        public string LegislationSource { get; set; }
        public string LegislationSourceFrench { get; set; }
        public string LegislationSourceEnglish { get; set; }
        public string Qm_inforcedte { get; set; }
        public int Order { get; set; } = 0;

        public OutputFileRecord(InputFileRecord asm_SsmRecord)
        {
            ts_importkey = Convert.ToInt32(asm_SsmRecord.ImportKeyID);
            LegislationType = asm_SsmRecord.LegislationType;
            LegislationTypeFrench = asm_SsmRecord.LegislationType + " (FR)";
            ParentLegislation = asm_SsmRecord.ParentLegislation;
            Qm_rcparentlegislationid = asm_SsmRecord.Qm_rcparentlegislationid;
            Name = asm_SsmRecord.Name;
            Label = asm_SsmRecord.Label;
            EnglishText = asm_SsmRecord.EnglishText;
            FrenchText = asm_SsmRecord?.FrenchText;
            ProvisionsHeadingAppliesTo = asm_SsmRecord?.ProvisionsHeadingAppliesTo;
            LegislationSource = asm_SsmRecord?.LegislationSource + "::" + asm_SsmRecord.LegislationSource + " - FR"; 
            LegislationSourceFrench = asm_SsmRecord.LegislationSource + " - FR";
            LegislationSourceEnglish = asm_SsmRecord.LegislationSource;
            Qm_inforcedte = asm_SsmRecord?.Qm_inforcedte;
            Order = asm_SsmRecord.Order;
        }
    }
}
