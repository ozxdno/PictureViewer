using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    class Load_vars
    {
        public static string ToDirectory(string str)
        {
            bool exist = System.IO.Directory.Exists(str);
            string value = "";
            if (exist) { value = str; } else { value = ""; }
            return value;
        }
        public static bool ToBool(string str)
        {
            try
            {
                return int.Parse(str) != 0;
            }
            catch
            {
                return false;
            }
        }
        public static int ToInt(string str)
        {
            try
            {
                int value = int.Parse(str);
                return value;
            }
            catch
            {
                return 0;
            }
        }
        public static long ToLong(string str)
        {
            try
            {
                return long.Parse(str);
            }
            catch
            {
                return 0;
            }
        }
        public static double ToDouble(string str)
        {
            try
            {
                return double.Parse(str);
            }
            catch
            {
                return 0;
            }
        }

        public static List<string> ToStringList(string str)
        {
            return str.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        public static List<int> ToIntList(string str)
        {
            List<string> tempStr = ToStringList(str);
            List<int> tempInt = new List<int>();

            for (int i = 0; i < tempStr.Count; i++)
            {
                try
                {
                    tempInt.Add(int.Parse(tempStr[i]));
                }
                catch
                {
                    return new List<int>();
                }
            }

            return tempInt;
        }

        public static void ToSupport(Support.TYPE type, string str)
        {
            Support support = new Support();
            bool ok = support.ToSupport(type, str);
            if (!ok) { return; }

            Config.Supports.Add(support);
        }
        public static void ToBaseFileInfo(string str)
        {
            BaseFileInfo file = new BaseFileInfo();
            bool ok = file.ToBaseFileInfo(str);
            if (!ok) { return; }

            Config.Files.Add(file);
        }
    }
}
