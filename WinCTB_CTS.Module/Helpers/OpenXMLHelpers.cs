﻿using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WinCTB_CTS.Module.OpenXMLHelper.Excel
{
    public class Reader : IDisposable
    {

        private SpreadsheetDocument document;
        private WorkbookPart workbookPart;

        public Reader(Stream stream)
        {
            document = SpreadsheetDocument.Open(stream, true);
            workbookPart = document.WorkbookPart;
        }

        public DataTable ReadToDataTable(string sheetName = null)
        {
            DataTable dt = new DataTable();

            Sheet sheet = null;
            IEnumerable<Sheet> sheets = workbookPart.Workbook.Descendants<Sheet>();
            if (!string.IsNullOrEmpty(sheetName))
            {
                sheet = sheets.FirstOrDefault(x => x.Name == sheetName);
            }
            else
            {
                sheet = sheets.FirstOrDefault();
            }
            if (sheet == null)
            {
                //throw new ArgumentException("sheetName");
                throw new InvalidOperationException("Erro na folha de dados");
            }
            dt.TableName = sheet.Name;

            //load sheet
            WorksheetPart sheetPart = workbookPart.GetPartById(sheet.Id) as WorksheetPart;

            //load shard string
            var shareStringPart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            var shareStrings = shareStringPart.SharedStringTable.Elements<SharedStringItem>().Select(x => x.InnerText).ToList();

            SheetData sheetData = sheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();
            foreach (Row row in sheetData)
            {
                if (row.RowIndex != 1)
                {
                    BindRowData(row, shareStrings, ref dt);
                }
                else
                {
                    BindColumnData(row, shareStrings, ref dt);
                }
            }

            return dt;
        }

        public static DataTable CreateDataTableFromStream(Stream stream, string sheetName = null)
        {
            DataTable dt = new DataTable();
            using (var reader = new Reader(stream))
            {
                dt = reader.ReadToDataTable(sheetName);
            }
            return dt;
        }

        private static void BindColumnData(Row row, IList<string> shareStrings, ref DataTable dt)
        {
            DataColumn col = new DataColumn();
            Dictionary<string, int> columnCount = new Dictionary<string, int>();
            foreach (Cell cell in row)
            {
                string cellVal = GetCellValue(cell, shareStrings);
                col = new DataColumn(cellVal);
                if (IsContainsColumn(dt, col.ColumnName))
                {
                    if (!columnCount.ContainsKey(col.ColumnName))
                        columnCount.Add(col.ColumnName, 0);
                    col.ColumnName = col.ColumnName + (columnCount[col.ColumnName]++);
                }
                dt.Columns.Add(col);
            }
        }

        private static void BindRowData(Row row, IList<string> shareStrings, ref DataTable dt)
        {
            DataRow dr = dt.NewRow();
            int columnIndex = 0;
            foreach (Cell cell in row)
            {
                int cellColumnIndex = GetColumnIndexFromName(GetColumnName(cell.CellReference));
                if (columnIndex < cellColumnIndex)
                {
                    do
                    {
                        columnIndex++;
                    }
                    while (columnIndex < cellColumnIndex);
                }
                //columnIndex++;

                dr[columnIndex] = GetCellValue(cell, shareStrings);
            }
            dt.Rows.Add(dr);
        }

        private static string GetCellValue(Cell cell, IList<string> shareStrings)
        {
            string text = string.Empty;
            var cellValue = cell.CellValue;

            if (cellValue != null)
            {
                var cellType = cell.DataType;
                if (cellType == null)
                {
                    text = cellValue.InnerText;
                }
                else
                {
                    switch (cellType.InnerText)
                    {
                        case "s":
                            if (cellType.Value == CellValues.SharedString)
                            {
                                text = shareStrings[int.Parse(cellValue.InnerText)];
                            }
                            else
                            {
                                text = cell.CellValue.ToString();
                            }
                            break;
                        default:
                            text = cellValue.InnerText;
                            break;
                    }
                }
            }

            return text;
        }

        private static bool IsContainsColumn(DataTable dt, string columnName)
        {
            if (dt == null || columnName == null)
            {
                return false;
            }
            return dt.Columns.Contains(columnName);
        }

        /// <summary>
        /// Given a cell name, parses the specified cell to get the column name.
        /// </summary>
        /// <param name="cellReference">Address of the cell (ie. B2)</param>
        /// <returns>Column Name (ie. B)</returns>
        private static string GetColumnName(string cellReference) => new Regex("[A-Za-z]+").Match(cellReference).Value;

        /// <summary>
        /// Given just the column name (no row index), it will return the zero based column index.
        /// Note: This method will only handle columns with a length of up to two (ie. A to Z and AA to ZZ). 
        /// A length of three can be implemented when needed.
        /// </summary>
        /// <param name="columnName">Column Name (ie. A or AB)</param>
        /// <returns>Zero based index if the conversion was successful; otherwise null</returns>
        private static int GetColumnIndexFromName(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
                return 0;
            int n = 0;
            for (int i = 0; i < columnName.Length; i++)
            {
                char c = Char.ToUpper(columnName[i]);
                if (c < 'A' || c > 'Z')
                    return 0;
                n += ((int)c - 'A' + 1) * (int)Math.Pow(26, i);
            }
            return n - 1;
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Reader() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            document.Close();
            document.Dispose();
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

    }
}
