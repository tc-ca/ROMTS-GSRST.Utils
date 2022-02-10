using ExcelDataReader;
using FastExcel;
using LegislationDataMigrationTool.RecordFormats;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace LegislationDataMigrationTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		public MainWindow()
		{
			InitializeComponent();
		}

		private void SelectFileButton_Click(object sender, RoutedEventArgs e)
		{

			StackPanelGeneratedFile.Visibility = Visibility.Collapsed;
			StackPanelSelectedFile.Visibility = Visibility.Collapsed;

			string startingLetterImportKey = "";
			int startingNumberImportKey = 2;

			// check if the letter portion is valid
			{
				if (letterImportKeyInput.Text == "")
				{
					MessageBox.Show("Please enter the letter portion", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				else
				{
					startingLetterImportKey = letterImportKeyInput.Text.Trim().ToUpper();
				}
			}

			// check if the number portion is valid
			{
				if (!int.TryParse(numberImportKeyInput.Text, out startingNumberImportKey))
				{
					numberImportKeyInput.Text = "";
					MessageBox.Show("Please enter a valid number for the number portion", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
			}

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
			openFileDialog.Title = "Select an Excel file";
			openFileDialog.InitialDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)).FullName;

			if (openFileDialog.ShowDialog() == false)
			{
				SelectedFilePath.Text = "";
			}
			else
			{
				SelectedFilePath.Text = openFileDialog.FileName;

				FileInfo fileInfo = new FileInfo(SelectedFilePath.Text);

				// create the Excel file
				this.CreateExcelImportFile(fileInfo.FullName, startingLetterImportKey, startingNumberImportKey);
			}

		}

		private void CreateExcelImportFile(string filePath, string startingLetterImportKey, int startingNumberImportKey)
		{
			try
			{
				// a list with all the record entries in the file
				var originalRecordList = new List<InputFileRecord>();

				// a list used for sorting record entries in the file
				var sortingRecordList = new List<InputFileRecord>();

				// a list used for creating the file with order numbers
				var updatedRecordList = new List<InputFileRecord>();

				// an object used for sorting records
				var rootRecordItem = new RootRecordItem();

				// Create the list that has the records in the Excel format
				var excelRecords = new List<OutputFileRecord>();

				var encodingType = Encoding.GetEncoding("iso-8859-1");

				// get all the records from the file and mark the root records
				{
					FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
					IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);

					DataSet result = excelReader.AsDataSet();

					DataTable excelDataTable = result.Tables[0];

					for (int i = 1; i < excelDataTable.Rows.Count; i++)
					{
						var legislationRecord = new InputFileRecord
						{
							ImportKeyID = excelDataTable.Rows[i][0].ToString(),
							LegislationType = excelDataTable.Rows[i][1].ToString(),
							ParentLegislation = excelDataTable.Rows[i][2].ToString(),
							Qm_rcparentlegislationid = excelDataTable.Rows[i][3].ToString(),
							Name = excelDataTable.Rows[i][4].ToString(),
							Label = excelDataTable.Rows[i][5].ToString(),
							EnglishText = excelDataTable.Rows[i][6].ToString(),
							FrenchText = excelDataTable.Rows[i][7].ToString(),
							ProvisionsHeadingAppliesTo = excelDataTable.Rows[i][8].ToString(),
							LegislationSource = excelDataTable.Rows[i][9].ToString(),
							Qm_inforcedte = excelDataTable.Rows[i][10].ToString(),
						};

						if (legislationRecord.LegislationType != "Body")
						{
							// record the heading
							if (legislationRecord.Qm_rcparentlegislationid == rootRecordItem.ParentID)
							{
								rootRecordItem.ChildrenIDs.Add(legislationRecord.ImportKeyID);

								legislationRecord.IsHeader = true;

								updatedRecordList.Add(legislationRecord);
							}
							else
							{
								sortingRecordList.Add(legislationRecord);
							}

							originalRecordList.Add(legislationRecord);
						}
						else
						{
							rootRecordItem.ParentID = legislationRecord.ImportKeyID.ToString();
							legislationRecord.IsRoot = true;
							updatedRecordList.Add(legislationRecord);
							excelRecords.Add(new OutputFileRecord(legislationRecord));
						}
					}

					excelReader.Close();
				}

				// go through all the records that are not headings
				{
					while (sortingRecordList.Count() > 0)
					{
						foreach (var record in originalRecordList.Where(x => x.IsHeader == false))
						{
							// get the parent record
							var parentRecord = originalRecordList.FirstOrDefault(x => x.ImportKeyID == record.Qm_rcparentlegislationid);

							// add the record to the children list of the parent - Note: the updatedLegislationRecords will be updated as well
							parentRecord.Childern.Add(record);

							// mark the record for deletion
							record.Delete = true;

						}

						// Remove the record since we don't need to look at it if another loop iteration is required
						sortingRecordList.RemoveAll(x => x.Delete == true);
					}
				}

				// add records to the excelRecords list with order numbers
				{
					int orderNumber = 0;

					foreach (var record in updatedRecordList.Where(x => x.IsRoot == false))
					{
						orderNumber++;
						record.Order = orderNumber;
						excelRecords.Add(new OutputFileRecord(record));

						foreach (var level1 in record.Childern)
						{
							orderNumber++;
							level1.Order = orderNumber;
							excelRecords.Add(new OutputFileRecord(level1));

							foreach (var level2 in level1.Childern)
							{
								orderNumber++;
								level2.Order = orderNumber;
								excelRecords.Add(new OutputFileRecord(level2));

								foreach (var level3 in level2.Childern)
								{
									orderNumber++;
									level3.Order = orderNumber;
									excelRecords.Add(new OutputFileRecord(level3));

									foreach (var level4 in level3.Childern)
									{
										orderNumber++;
										level4.Order = orderNumber;
										excelRecords.Add(new OutputFileRecord(level4));

										foreach (var level5 in level4.Childern)
										{
											orderNumber++;
											level5.Order = orderNumber;
											excelRecords.Add(new OutputFileRecord(level5));

											foreach (var level6 in level5.Childern)
											{
												orderNumber++;
												level6.Order = orderNumber;
												excelRecords.Add(new OutputFileRecord(level6));

												foreach (var level7 in level6.Childern)
												{
													orderNumber++;
													level7.Order = orderNumber;
													excelRecords.Add(new OutputFileRecord(level7));

													foreach (var level8 in level7.Childern)
													{
														orderNumber++;
														level8.Order = orderNumber;
														excelRecords.Add(new OutputFileRecord(level8));

														foreach (var level9 in level8.Childern)
														{
															orderNumber++;
															level9.Order = orderNumber;
															excelRecords.Add(new OutputFileRecord(level9));
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}

				}

				// Update the records to use valid unique import keys
				{

					var importIDPairs = new List<dynamic>();
					int newImportID = startingNumberImportKey;
					
					// go through each record and change the import key in the first column to a unique ID
					foreach (var item in excelRecords.OrderBy(x => x.ts_importkey))
					{
						// create an dynamic object that pairs the import key in Excel with the new unique ID
						dynamic importIDPair = new ExpandoObject();
						importIDPair.originalID = item.ts_importkey;
						importIDPair.newID = newImportID;
						importIDPairs.Add(importIDPair);

						item.ts_importkey = newImportID;

						newImportID++;
					}

					// now go through each record again and update the qm_rcParentLegislationID
					foreach (var item in excelRecords)
					{
						if (item.Qm_rcparentlegislationid != "")
						{
							var newParentID = importIDPairs.FirstOrDefault(x => x.originalID == Convert.ToInt32(item.Qm_rcparentlegislationid)).newID;
							item.Qm_rcparentlegislationid = Convert.ToString(newParentID);
						}
					}
				}

				// Create the Excel File
				{
					FileInfo fileInfo = new FileInfo(filePath);

					// create the file
					DirectoryInfo directoryInfo = new DirectoryInfo(fileInfo.DirectoryName);
					string outputFilePath = $"{directoryInfo.FullName}\\{DateTime.Now.Year}{DateTime.Now.Month.ToString("00")}{DateTime.Now.Day.ToString("00")}{DateTime.Now.Hour.ToString("00")}{DateTime.Now.Minute.ToString("00")}{DateTime.Now.Second.ToString("00")}-{fileInfo.Name.Substring(0, fileInfo.Name.IndexOf("."))}-Import{fileInfo.Extension}";

					var app = new Microsoft.Office.Interop.Excel.Application();
					var wb = app.Workbooks.Add();
					wb.SaveAs(outputFilePath);
					wb.Close();

					// create the worksheet
					var worksheet = new Worksheet();
					var rows = new List<Row>();

					// create the header
					List<Cell> cellsHeader = new List<Cell>();

					cellsHeader.Add(new Cell(1, "ts_importkey"));
					cellsHeader.Add(new Cell(2, "qm_tylegislationtypeid"));
					cellsHeader.Add(new Cell(3, "Parent Legislation"));
					cellsHeader.Add(new Cell(4, "qm_rcparentlegislationid"));
					cellsHeader.Add(new Cell(5, "qm_name"));
					cellsHeader.Add(new Cell(6, "qm_legislationlbl"));
					cellsHeader.Add(new Cell(7, "qm_legislationetxt"));
					cellsHeader.Add(new Cell(8, "qm_legislationftxt"));
					cellsHeader.Add(new Cell(9, "Provisions Heading Applies To"));
					cellsHeader.Add(new Cell(10, "qm_tylegislationsourceid"));
					cellsHeader.Add(new Cell(11, "qm_inforcedte"));
					cellsHeader.Add(new Cell(12, "qm_ordernbr"));
					cellsHeader.Add(new Cell(13, "Legislation Type French"));
					cellsHeader.Add(new Cell(14, "Legislation Source French"));
					cellsHeader.Add(new Cell(15, "Legislation Source English"));

					rows.Add(new Row(1, cellsHeader));

					// create the body

					int counter = 2;

					foreach (var record in excelRecords.OrderBy(x => x.ts_importkey))
					{
						List<Cell> cells = new List<Cell>();

						cells.Add(new Cell(1, startingLetterImportKey + record.ts_importkey));
						cells.Add(new Cell(2, record.LegislationType));
						cells.Add(new Cell(3, record.ParentLegislation));

						if (record.Qm_rcparentlegislationid != "")
						{
							cells.Add(new Cell(4, startingLetterImportKey + record.Qm_rcparentlegislationid));
						}

						cells.Add(new Cell(5, record.Name));
						cells.Add(new Cell(6, record.Label));
						cells.Add(new Cell(7, record.EnglishText));
						cells.Add(new Cell(8, record.FrenchText));
						cells.Add(new Cell(9, record.ProvisionsHeadingAppliesTo));
						cells.Add(new Cell(10, record.LegislationSource ));
						cells.Add(new Cell(11, record.Qm_inforcedte));

						if (record.Order == 0)
						{
							cells.Add(new Cell(12, null));
						}
						else
						{
							cells.Add(new Cell(12, record.Order));
						}

						cells.Add(new Cell(13, record.LegislationTypeFrench));
						cells.Add(new Cell(14, record.LegislationSourceFrench));
						cells.Add(new Cell(15, record.LegislationSourceEnglish));

						Row myRow = new Row(counter, cells);

						rows.Add(new Row(counter, cells));
						counter++;
					}

					worksheet.Rows = rows;

					FileInfo newExcelfileInfo = new FileInfo(outputFilePath);

					using (FastExcel.FastExcel fastExcel = new FastExcel.FastExcel(newExcelfileInfo))
					{
						// Write the data
						fastExcel.Write(worksheet, "sheet1");
					}

					GeneratedFilePath.Text = outputFilePath;

					StackPanelGeneratedFile.Visibility = Visibility.Visible;
					StackPanelSelectedFile.Visibility = Visibility.Visible;

					MessageBox.Show("Excel file has been created");
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
