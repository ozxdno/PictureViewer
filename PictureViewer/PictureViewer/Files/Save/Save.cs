using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 保存指定的文本文件
    /// </summary>
    public class Save
    {
        private List<string> fields;
        private List<string> values;
        private string full;
        private System.IO.StreamWriter writer;

        /// <summary>
        /// 创建并初始化类
        /// </summary>
        public Save()
        {
            fields = new List<string>();
            values = new List<string>();
        }
        /// <summary>
        /// 把当前所存储的变量保存到指定文件，失败返回 FALSE。
        /// </summary>
        /// <param name="full">文件全称</param>
        public bool SaveAs(string full)
        {
            this.full = full;
            try
            {
                writer = new System.IO.StreamWriter(full, false);
            }
            catch
            {
                return false;
            }

            for (int i = 0; i < fields.Count; i++)
            {
                writer.WriteLine(fields[i] + "\\" + values[i]);
            }

            writer.Close();
            return true;
        }

        /// <summary>
        /// 把 int 型变量保存到该文件的指定数据域中。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        public void Set(string field, int value)
        {
            fields.Add(field);
            values.Add(value.ToString());
        }
        /// <summary>
        /// 把 long 型变量保存到该文件的指定数据域中。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        public void Set(string field, long value)
        {
            fields.Add(field);
            values.Add(value.ToString());
        }
        /// <summary>
        /// 把 double 型变量保存到该文件的指定数据域中。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        public void Set(string field, double value)
        {
            fields.Add(field);
            values.Add(value.ToString());
        }
        /// <summary>
        /// 把 bool 型变量保存到该文件的指定数据域中。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        public void Set(string field, bool value)
        {
            fields.Add(field);
            values.Add(value ? "1" : "0");
        }
        /// <summary>
        /// 把 string 型变量保存到该文件的指定数据域中。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        public void Set(string field, string value)
        {
            fields.Add(field);
            values.Add(value);
        }


        /// <summary>
        /// 把 int 型链表变量保存到该文件的指定数据域中。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        public void Set(string field, List<int> value)
        {
            if (value == null) { value = new List<int>(); }
            string vstr = "";
            foreach (int i in value)
            {
                vstr += i.ToString() + "|";
            }
            if (vstr.Length != 0) { vstr = vstr.Remove(vstr.Length - 1); }

            fields.Add(field);
            values.Add(vstr);
        }
        /// <summary>
        /// 把 long 型链表变量保存到该文件的指定数据域中。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        public void Set(string field, List<long> value)
        {
            if (value == null) { value = new List<long>(); }
            string vstr = "";
            foreach (long i in value)
            {
                vstr += i.ToString() + "|";
            }
            if (vstr.Length != 0) { vstr = vstr.Remove(vstr.Length - 1); }

            fields.Add(field);
            values.Add(vstr);
        }
        /// <summary>
        /// 把 double 型链表变量保存到该文件的指定数据域中。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        public void Set(string field, List<double> value)
        {
            if (value == null) { value = new List<double>(); }
            string vstr = "";
            foreach (double i in value)
            {
                vstr += i.ToString() + "|";
            }
            if (vstr.Length != 0) { vstr = vstr.Remove(vstr.Length - 1); }

            fields.Add(field);
            values.Add(vstr);
        }
        /// <summary>
        /// 把 bool 型链表变量保存到该文件的指定数据域中。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        public void Set(string field, List<bool> value)
        {
            if (value == null) { value = new List<bool>(); }
            string vstr = "";
            foreach (bool i in value)
            {
                vstr += (i ? "1" : "0") + "|";
            }
            if (vstr.Length != 0) { vstr = vstr.Remove(vstr.Length - 1); }

            fields.Add(field);
            values.Add(vstr);
        }
    }
}
