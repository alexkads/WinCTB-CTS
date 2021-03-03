using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace WinCTB_CTS.Module.Win.Services
{
    public class RegisterWindowsManipulation
    {
        private const string userRoot = "HKEY_CURRENT_USER"; 
        
        private static string GetNameExecutingAssembly
        {
            get
            {
                string result;
                try
                {
                    result = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                }
                catch (Exception)
                {
                    throw;
                }
                return result;
            }
        }

        public static void SetRegister(string key, string value)
        {
            if (String.IsNullOrEmpty(key))            
                throw new ArgumentNullException("É necessário informar a chave");
            
            if (String.IsNullOrEmpty(value))
                throw new ArgumentNullException("É necessário informar o valor");

            try
            {
                Registry.SetValue($@"{userRoot}\SOFTWARE\{GetNameExecutingAssembly}", key, value);
            }
            catch (Exception)
            {
                throw;
            }            
        }

        public static string GetRegister(string key)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException("É necessário informar a chave");

            try
            {
                return Registry.GetValue($@"{userRoot}\SOFTWARE\{GetNameExecutingAssembly}", key, "NULL")?.ToString();
            }
            catch (Exception)
            {
                throw;
            }            
        }
    }
}
