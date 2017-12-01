using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 提示文件的文件信息表模型
    /// </summary>
    public class TipModel : BaseModel
    {
        /// <summary>
        /// 文件已经被加载了
        /// </summary>
        public bool Loaded
        {
            get;
            set;
        }
        /// <summary>
        /// 提示文件的文件类型
        /// </summary>
        public FileType Type
        {
            get;
            set;
        }
        /// <summary>
        /// 提示图（若提示文件为图片、GIF文件则加载该项）
        /// </summary>
        public System.Drawing.Image Image
        {
            get { return System.Drawing.Image.FromFile(Full); }
        }
        /// <summary>
        /// 提示文件的高度，单位：像素
        /// </summary>
        public int Height
        {
            get;
            set;
        }
        /// <summary>
        /// 提示文件的宽度，单位：像素
        /// </summary>
        public int Width
        {
            get;
            set;
        }
        /// <summary>
        /// 总时长
        /// </summary>
        public string PlayTime
        {
            set;
            get;
        }
        /// <summary>
        /// 播放到
        /// </summary>
        public string PlayAt
        {
            set;
            get;
        }

        /// <summary>
        /// 创建一个空的提示文件模型
        /// </summary>
        public TipModel()
        {
            base.FillBaseModel(); initialize();
        }
        /// <summary>
        /// 创建提示文件模型
        /// </summary>
        /// <param name="full">文件全称</param>
        public TipModel(string full)
        {
            base.FillBaseModel(full); initialize();
        }
        /// <summary>
        /// 创建提示文件模型
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="name">文件名称</param>
        public TipModel(string path, string name)
        {
            base.FillBaseModel(path, name); initialize();
        }
        /// <summary>
        /// 创建提示文件模型
        /// </summary>
        /// <param name="file">文件信息</param>
        public TipModel(System.IO.FileInfo file)
        {
            base.FillBaseModel(file); initialize();
        }

        /// <summary>
        /// 加载该文件
        /// </summary>
        /// <returns></returns>
        public void Load()
        {
            Dispose();

            if (!Exist) { Loaded = false; return; }
            System.IO.FileInfo file = new System.IO.FileInfo(Full);
            if (Loaded && file.LastWriteTime.ToFileTime() == Time) { return; }

            Loaded = false;
            Time = file.LastWriteTime.ToFileTime();

            if (Type == FileType.Picture || Type == FileType.Gif)
            {
                try
                {
                    Image = System.Drawing.Image.FromFile(Full);
                    Loaded = true;
                    Height = Image.Height;
                    Width = Image.Width;
                }
                catch
                {
                    Loaded = false;
                }
            }
            if (Type == FileType.Music)
            {
                if (base.Type == FileType.TipMus)
                {
                    Height = Tools.Screen.MinWindowSide;
                    Width = Tools.Screen.MinWindowSide;
                    Loaded = true;
                }
                else
                {
                    Height = Manager.TipMus.Model.Height;
                    Width = Manager.TipMus.Model.Width;
                    Loaded = true;
                }
            }
            if (Type == FileType.Video)
            {
                int h = 0, w = 0;
                bool ok = Tools.VideoOperate.GetSize(Full, ref w, ref h);
                if (ok)
                {
                    Loaded = true;
                    Height = h;
                    Width = w;
                }
                else
                {
                    Loaded = false;
                }
            }
        }
        /// <summary>
        /// 释放该文件
        /// </summary>
        /// <returns></returns>
        public void Dispose()
        {
            try { Image.Dispose(); } catch { }
            Image = null;
        }


        private void initialize()
        {
            Loaded = false;
            Type = FileType.Unsupport;
            Height = 0;
            Width = 0;
        }
    }
}
