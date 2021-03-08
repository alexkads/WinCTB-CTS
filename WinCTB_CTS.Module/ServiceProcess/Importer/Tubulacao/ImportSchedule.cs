﻿using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.ServiceProcess.Base;

namespace WinCTB_CTS.Module.ServiceProcess.Importer.Tubulacao
{
    public class ImportSchedule : CalculatorProcessBase
    {
        public ImportSchedule(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        : base(cancellationToken, progress)
        {
        }

        protected override void OnMapImporter(UnitOfWork uow, DataTable dataTable, DataRow rowForMap, int expectedTotal, int currentIndex)
        {
            base.OnMapImporter(uow, dataTable, rowForMap, expectedTotal, currentIndex);
            var schedules = ConvertListFromPivot(dataTable);

            if (currentIndex > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var criteriaOperator = CriteriaOperator.Parse("PipingClass = ? And Material = ? And TabDiametro.Wdi = ? And ScheduleTag = ?",
                     schedules[currentIndex].pipingClass, schedules[currentIndex].material, schedules[currentIndex].wdi, schedules[currentIndex].scheduleTag);

                var tabSchedule = uow.FindObject<TabSchedule>(criteriaOperator);

                if (tabSchedule == null)
                    tabSchedule = new TabSchedule(uow);

                tabSchedule.PipingClass = schedules[currentIndex].pipingClass;
                tabSchedule.Material = schedules[currentIndex].material;
                tabSchedule.TabDiametro = uow.FindObject<TabDiametro>(new BinaryOperator("Wdi", schedules[currentIndex].wdi));
                tabSchedule.ScheduleTag = schedules[currentIndex].scheduleTag;
            }
        }

        static private Func<DataTable, IList<LocalScheduleMapping>> ConvertListFromPivot = (dt) =>
        {
            var result = new List<LocalScheduleMapping>();

            for (int idxrow = 0; idxrow < dt.Rows.Count; idxrow++)
            {
                var row = dt.Rows[idxrow];

                if (idxrow > 0)
                {
                    for (int idxcol = 2; idxcol < row.ItemArray.Length; idxcol++)
                    {
                        result.Add(new LocalScheduleMapping
                        {
                            numeroLinha = idxrow,
                            pipingClass = row[0].ToString(),
                            material = row[1].ToString(),
                            wdi = Utils.ConvertDouble(((dt.Rows[0])[idxcol]).ToString()),
                            scheduleTag = row[idxcol].ToString()
                        });
                    }
                }
            }

            return result;
        };


    }

    public class LocalScheduleMapping
    {
        public int numeroLinha { get; set; }
        public string pipingClass { get; set; }
        public string material { get; set; }
        public double wdi { get; set; }
        public string scheduleTag { get; set; }
    }
}
