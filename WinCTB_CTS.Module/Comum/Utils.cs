using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using DevExpress.ExpressApp;
using System.IO;

namespace WinCTB_CTS.Module.Comum
{
    public static class Utils
    {
        public static string Left(this string str, int length)
        {
            return str.Substring(0, Math.Min(length, str.Length));
        }

        public static T GetMasterObjectFromListView<T>(this View view)
        {
            return (T)(((ListView)view).CollectionSource as PropertyCollectionSource).MasterObject;
        }

        public static void PurgeAllRecords<T>(Session session)
        {
            XPClassInfo classInfo = session.Dictionary.GetClassInfo(typeof(T));
            string tableName = classInfo.TableName;
            try
            {
                session.ExecuteNonQuery(string.Format("delete from {0} where GCRecord is not null", tableName));
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Falha");
            }
        }

        public static void DeleteAllRecords<T>(Session session)
        {
            XPClassInfo classInfo = session.Dictionary.GetClassInfo(typeof(T));

            string tableName = classInfo.TableName;

            try
            {
                session.ExecuteNonQuery(string.Format("delete from {0}", tableName));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public static void TruncateTable<T>(Session session)
        {
            XPClassInfo classInfo = session.Dictionary.GetClassInfo(typeof(T));

            string tableName = classInfo.TableName;

            try
            {
                session.ExecuteNonQuery(string.Format("Truncate Table {0}", tableName));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public static string GetYearAndWeek(DateTime datetime)
        {
            var GetWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(datetime.Date, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday).ToString().PadLeft(2, '0');
            var GetYear = datetime.Date.ToString("yy").PadLeft(2, '0');
            return GetYear + GetWeek;
        }

        public static IEnumerable<Enum> GetFlags(Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }

        public static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        private static Stream GetManifestResource(string ResourceName)
        {
            Type moduleType = typeof(WinCTB_CTSModule);
            string name = $"{moduleType.Namespace}.Resources.{ResourceName}";
            return moduleType.Assembly.GetManifestResourceStream(name);
        }
    }
}
