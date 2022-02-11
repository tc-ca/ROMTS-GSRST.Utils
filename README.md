# ROMTS-GSRST.Utils
Utilities for the ROMTS-GSRST project

## Legislation Data Migration Tool
This WPF desktop app is used for creating an Excel spreadsheet that can be used in a Dataflow to be imported into the Legeslation tables in ROMTS-GSRST.

### Technologies used
- .NET Framework 4.7.2

### NuGet Packages
| Package Name                | Description                         |
|--------------------------------|------------------------------------------------------------------------|
| ExcelDataReader                | Used to read data from the inputted Excel file
| FastExcel                      | Used to write data to the Excel file that will be used in the Dataflow |
| Microsoft.Office.Interop.Excel | Used to create the Excel file that will be used in the Dataflow        |

### How to use it
Refer to the document at this [link](https://034gc.sharepoint.com/:w:/r/sites/SolutionsCenterProjectsProjetsduCentredessolutions-ROM-TS/_layouts/15/Doc.aspx?sourcedoc=%7BE46490BC-C759-48A1-81C8-708BCB6AD262%7D&file=How%20to%20migrate%20Legislation%20data%20to%20a%20Dataflow.docx&action=default&mobileredirect=true)

### How to run the app
Open the **LegislationDataMigrationTool.sln** file with Visual Studio and hit **Start** to run the app
