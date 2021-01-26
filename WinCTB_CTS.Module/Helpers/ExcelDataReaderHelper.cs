using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCTB_CTS.Module.ExcelDataReaderHelper.Excel
{
    public class Reader : IDisposable
    {
        private IExcelDataReader excelDataReader;
        private DataSet dataSetResult;

        public Reader(Stream stream)
        {
            excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        }

        public DataTableCollection CreateDataTableCollection()
        {
            dataSetResult = excelDataReader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });
            return dataSetResult.Tables;
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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            excelDataReader.Close();
            excelDataReader.Dispose();
            dataSetResult.Dispose();
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
    }
}
