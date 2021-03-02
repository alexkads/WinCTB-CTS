using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace WinCTB_CTS.Module.Win.Utils
{
    public class RegisterWindowsManipulation
    {
        private const string userRoot = "HKEY_CURRENT_USER";
        private static string GetNameExecutingAssembly()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        }
        private static string NameExecutingAssembly => GetNameExecutingAssembly();

        public static void SetRegister(string valueKey, string value)
        {
            Registry.SetValue($@"{userRoot}\SOFTWARE\{NameExecutingAssembly}", valueKey, value);
        }

        public static string GetRegister(string valueKey)
        {
            return Registry.GetValue($@"{userRoot}\SOFTWARE\{NameExecutingAssembly}", valueKey, "NULL").ToString();
        }
    }
}
