using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 纯文本文件的文件信息模型
    /// </summary>
    public class TextModel : BaseModel
    {
        /// <summary>
        /// 本文件是否已经加载
        /// </summary>
        public bool Loaded
        {
            set;
            get;
        }
        /// <summary>
        /// 读入文本内容
        /// </summary>
        public Load LoadContent
        {
            get;
            set;
        }
        /// <summary>
        /// 保存文本内容
        /// </summary>
        public Save SaveContent
        {
            get;
            set;
        }

        private System.IO.StreamReader reader;

        /// <summary>
        /// 创建一个空的纯文本文件信息模型
        /// </summary>
        public TextModel()
        {
            base.FillBaseModel();

            Loaded = false;
            LoadContent = null;
            SaveContent = new Files.Save();
            reader = null;
        }
        /// <summary>
        /// 创建一个纯文本文件信息模型
        /// </summary>
        /// <param name="full">文件全称</param>
        public TextModel(string full)
        {
            base.FillBaseModel(full);

            Loaded = false;
            LoadContent = null;
            SaveContent = new Files.Save();
            try { reader = new System.IO.StreamReader(Full); } catch { }
        }
        /// <summary>
        /// 创建一个纯文本文件信息模型
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="name">文件名称</param>
        public TextModel(string path, string name)
        {
            base.FillBaseModel(path, name);

            Loaded = false;
            LoadContent = null;
            SaveContent = new Files.Save();
            try { reader = new System.IO.StreamReader(Full); } catch { }
        }
        /// <summary>
        /// 创建一个纯文本文件信息模型
        /// </summary>
        /// <param name="file">文件信息</param>
        public TextModel(System.IO.FileInfo file)
        {
            base.FillBaseModel(file);

            Loaded = false;
            LoadContent = null;
            SaveContent = new Files.Save();
            try { reader = new System.IO.StreamReader(Full); } catch { }
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <returns></returns>
        public void Load()
        {
            LoadContent = new Files.Load(Full);
            reader = new System.IO.StreamReader(Full);
        }
        /// <summary>
        /// 释放文件
        /// </summary>
        /// <returns></returns>
        public void Dispose()
        {
            if (reader == null) { return; }
            try
            {
                reader.Close();
            }
            catch
            {

            }
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        public void Save()
        {
            SaveContent.SaveAs(Full);
        }
    }
}
