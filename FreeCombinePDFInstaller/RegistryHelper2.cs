using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace FreeCombinePDFInstaller
{
    class RegistryHelper2
    {
        public static string GetKeyValue(string AppRegKey, string keystr)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser;

                key = key.OpenSubKey("Software",true);

                RegistryKey key2 = null;

                try
                {
                    key2 = key.OpenSubKey("softpcapps Software",true);

                    if (key2 == null)
                    {
                        key2 = key.CreateSubKey("softpcapps Software");
                    }

                    RegistryKey key3 = key2.OpenSubKey(AppRegKey,true);

                    try
                    {

                        if (key3 == null)
                        {
                            key3 = key2.CreateSubKey(AppRegKey);
                        }

                        if (key3 != null)
                        {
                            return key3.GetValue(keystr, "").ToString();
                        }

                        return "";

                    }
                    finally
                    {
                        if (key3 != null)
                        {
                            key3.Close();
                        }
                    }

                }
                finally
                {
                    if (key2 != null)
                    {
                        key2.Close();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static bool SetKeyValue(string AppRegKey, string keystr, string keyvalue)
        {
            RegistryKey key = Registry.CurrentUser;

            key = key.OpenSubKey("Software");

            RegistryKey key2 = null;

            try
            {
                key2 = key.OpenSubKey("softpcapps Software",true);

                if (key2 == null)
                {
                    key2 = key.CreateSubKey("softpcapps Software");
                }

                RegistryKey key3 = key2.OpenSubKey(AppRegKey, true);

                try
                {

                    if (key3 == null)
                    {
                        key3 = key2.CreateSubKey(AppRegKey);

                        key3.Close();
                    }

                    key3 = key2.OpenSubKey(AppRegKey, true);

                    if (key3 != null)
                    {
                        key3.SetValue(keystr, keyvalue);
                    }

                }
                finally
                {
                    if (key3 != null)
                    {
                        key3.Close();
                    }
                }
            }
            finally
            {
                if (key2 != null)
                {
                    key2.Close();
                }
            }

            return true;
        }

        public static string GetKeyValueLM(string AppRegKey, string keystr)
        {
            try
            {
                RegistryKey key = Registry.LocalMachine;

                key = key.OpenSubKey("Software",true);

                RegistryKey key2 = null;

                try
                {
                    key2 = key.OpenSubKey("softpcapps Software",true);

                    if (key2 == null)
                    {
                        key2 = key.CreateSubKey("softpcapps Software");
                    }

                    RegistryKey key3 = key2.OpenSubKey(AppRegKey,true);

                    try
                    {

                        if (key3 == null)
                        {
                            key3 = key2.CreateSubKey(AppRegKey);
                        }

                        if (key3 != null)
                        {
                            return key3.GetValue(keystr, "").ToString();
                        }

                        return "";

                    }
                    finally
                    {
                        if (key3 != null)
                        {
                            key3.Close();
                        }
                    }

                }
                finally
                {
                    if (key2 != null)
                    {
                        key2.Close();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetKeyValueLMLowPriv(string AppRegKey, string keystr)
        {
            try
            {
                RegistryKey key = Registry.LocalMachine;

                key = key.OpenSubKey("Software", false);

                RegistryKey key2 = null;

                try
                {
                    key2 = key.OpenSubKey("softpcapps Software", false);

                    if (key2 == null)
                    {
                        key2 = key.CreateSubKey("softpcapps Software");
                    }

                    RegistryKey key3 = key2.OpenSubKey(AppRegKey, false);

                    try
                    {

                        if (key3 == null)
                        {
                            key3 = key2.CreateSubKey(AppRegKey);
                        }

                        if (key3 != null)
                        {
                            return key3.GetValue(keystr, "").ToString();
                        }

                        return "";

                    }
                    finally
                    {
                        if (key3 != null)
                        {
                            key3.Close();
                        }
                    }

                }
                finally
                {
                    if (key2 != null)
                    {
                        key2.Close();
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static bool SetKeyValueLM(string AppRegKey, string keystr, string keyvalue)
        {
            RegistryKey key = Registry.LocalMachine;

            key = key.OpenSubKey("Software");

            RegistryKey key2 = null;

            try
            {
                key2 = key.OpenSubKey("softpcapps Software",true);

                if (key2 == null)
                {
                    key2 = key.CreateSubKey("softpcapps Software");
                }

                RegistryKey key3 = key2.OpenSubKey(AppRegKey, true);

                try
                {

                    if (key3 == null)
                    {
                        key3 = key2.CreateSubKey(AppRegKey);

                        key3.Close();
                    }

                    key3 = key2.OpenSubKey(AppRegKey, true);

                    if (key3 != null)
                    {
                        key3.SetValue(keystr, keyvalue);
                    }

                }
                finally
                {
                    if (key3 != null)
                    {
                        key3.Close();
                    }
                }
            }
            finally
            {
                if (key2 != null)
                {
                    key2.Close();
                }
            }

            return true;
        }
    }  
}
