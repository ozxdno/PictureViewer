using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 加载指定的文本文件
    /// </summary>
    public class Load
    {
        private List<string> fields;
        private List<string> values;
        private string full;
        private System.IO.StreamReader reader;
        
        /// <summary>
        /// 加载指定的文本文档
        /// </summary>
        /// <param name="full">文件的全路径</param>
        public Load(string full)
        {
            this.fields = new List<string>();
            this.values = new List<string>();
            this.full = full;
            this.reader = null;
            if (!System.IO.File.Exists(full)) { return; }

            reader = new System.IO.StreamReader(full);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                int idxOfEqu = line.IndexOf('='); if (idxOfEqu == -1) { continue; }
                string field = line.Substring(0, idxOfEqu);
                string value = line.Substring(idxOfEqu + 1);

                fields.Add(field);
                values.Add(value);
            }

            try { reader.Close(); } catch { }
        }
        /// <summary>
        /// 加载指定的文本文档
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="name">文件名称</param>
        public Load(string path, string name)
        {
            this.fields = new List<string>();
            this.values = new List<string>();
            this.full = path + "\\" + name;
            this.reader = null;
            if (!System.IO.File.Exists(full)) { return; }

            reader = new System.IO.StreamReader(full);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                int idxOfEqu = line.IndexOf('='); if (idxOfEqu == -1) { continue; }
                string field = line.Substring(0, idxOfEqu);
                string value = line.Substring(idxOfEqu + 1);

                fields.Add(field);
                values.Add(value);
            }

            try { reader.Close(); } catch { }
        }
        /// <summary>
        /// 加载指定的文本文档
        /// </summary>
        /// <param name="model">文件信息列表</param>
        public Load(BaseModel model)
        {
            this.fields = new List<string>();
            this.values = new List<string>();
            this.full = model.Full;
            this.reader = null;
            if (!System.IO.File.Exists(full)) { return; }

            reader = new System.IO.StreamReader(full);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                int idxOfEqu = line.IndexOf('='); if (idxOfEqu == -1) { continue; }
                string field = line.Substring(0, idxOfEqu);
                string value = line.Substring(idxOfEqu + 1);

                fields.Add(field);
                values.Add(value);
            }

            try { reader.Close(); } catch { }
        }
        
        /// <summary>
        /// 从文本中获取指定的 int 型数据。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        /// <returns></returns>
        public bool Get(string field, ref int value)
        {
            if (field == null) { return false; }
            int idx = fields.IndexOf(field); if (idx == -1) { return false; }

            try
            {
                value = int.Parse(values[idx]);
                fields.RemoveAt(idx);
                values.RemoveAt(idx);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 从文本中获取指定的 long 型数据。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        /// <returns></returns>
        public bool Get(string field, ref long value)
        {
            if (field == null) { return false; }
            int idx = fields.IndexOf(field); if (idx == -1) { return false; }

            try
            {
                value = long.Parse(values[idx]);
                fields.RemoveAt(idx);
                values.RemoveAt(idx);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 从文本中获取指定的 double 型数据。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        /// <returns></returns>
        public bool Get(string field, ref double value)
        {
            if (field == null) { return false; }
            int idx = fields.IndexOf(field); if (idx == -1) { return false; }

            try
            {
                value = double.Parse(values[idx]);
                fields.RemoveAt(idx);
                values.RemoveAt(idx);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 从文本中获取指定的 bool 型数据。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        /// <returns></returns>
        public bool Get(string field, ref bool value)
        {
            if (field == null) { return false; }
            int idx = fields.IndexOf(field); if (idx == -1) { return false; }

            try
            {
                value = int.Parse(values[idx]) != 0;
                fields.RemoveAt(idx);
                values.RemoveAt(idx);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 从文本中获取指定的 string 型数据。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        /// <returns></returns>
        public bool Get(string field, ref string value)
        {
            if (field == null) { return false; }
            int idx = fields.IndexOf(field); if (idx == -1) { return false; }
            value = values[idx];
            if (value == null || value.Length == 0) { return false; }
            return true;
        }

        /// <summary>
        /// 从文本中获取指定的 int 链表型数据。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        /// <returns></returns>
        public bool Get(string field, ref List<int> value)
        {
            if (field == null) { return false; }
            int idx = fields.IndexOf(field); if (idx == -1) { return false; }

            string[] tempStr = values[idx].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (value == null) { value = new List<int>(); }
            value.Clear();

            for (int i = 0; i < tempStr.Length; i++)
            {
                try
                {
                    value.Add(int.Parse(tempStr[i]));
                }
                catch
                {
                    return false;
                }
            }

            fields.RemoveAt(idx);
            values.RemoveAt(idx);
            return true;
        }
        /// <summary>
        /// 从文本中获取指定的 long 链表型数据。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        /// <returns></returns>
        public bool Get(string field, ref List<long> value)
        {
            if (field == null) { return false; }
            int idx = fields.IndexOf(field); if (idx == -1) { return false; }

            string[] tempStr = values[idx].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (value == null) { value = new List<long>(); }
            value.Clear();

            for (int i = 0; i < tempStr.Length; i++)
            {
                try
                {
                    value.Add(long.Parse(tempStr[i]));
                }
                catch
                {
                    return false;
                }
            }

            fields.RemoveAt(idx);
            values.RemoveAt(idx);
            return true;
        }
        /// <summary>
        /// 从文本中获取指定的 double 链表型数据。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        /// <returns></returns>
        public bool Get(string field, ref List<double> value)
        {
            if (field == null) { return false; }
            int idx = fields.IndexOf(field); if (idx == -1) { return false; }

            string[] tempStr = values[idx].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (value == null) { value = new List<double>(); }
            value.Clear();

            for (int i = 0; i < tempStr.Length; i++)
            {
                try
                {
                    value.Add(double.Parse(tempStr[i]));
                }
                catch
                {
                    return false;
                }
            }

            fields.RemoveAt(idx);
            values.RemoveAt(idx);
            return true;
        }
        /// <summary>
        /// 从文本中获取指定的 bool 链表型数据。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        /// <returns></returns>
        public bool Get(string field, ref List<bool> value)
        {
            if (field == null) { return false; }
            int idx = fields.IndexOf(field); if (idx == -1) { return false; }

            string[] tempStr = values[idx].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (value == null) { value = new List<bool>(); }
            value.Clear();

            for (int i = 0; i < tempStr.Length; i++)
            {
                try
                {
                    value.Add(int.Parse(tempStr[i]) != 0);
                }
                catch
                {
                    return false;
                }
            }

            fields.RemoveAt(idx);
            values.RemoveAt(idx);
            return true;
        }
        /// <summary>
        /// 从文本中获取指定的 string 链表型数据。
        /// </summary>
        /// <param name="field">数据域</param>
        /// <param name="value">数据值</param>
        /// <returns></returns>
        public bool Get(string field, ref List<string> value)
        {
            if (field == null) { return false; }
            int idx = fields.IndexOf(field); if (idx == -1) { return false; }

            string[] tempStr = values[idx].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            value = tempStr.ToList();

            fields.RemoveAt(idx);
            values.RemoveAt(idx);
            return true;
        }
    }
}
