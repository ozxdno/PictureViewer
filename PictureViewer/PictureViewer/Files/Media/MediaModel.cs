using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 媒体类型文件的文件信息表
    /// </summary>
    public class MediaModel : BaseModel
    {
        /// <summary>
        /// 是否在子目录中
        /// </summary>
        public bool InFolder
        {
            get;
            set;
        }
        /// <summary>
        /// 已经被加载了
        /// </summary>
        public bool Loaded
        {
            set;
            get;
        }
        /// <summary>
        /// 文件的高度，单位：像素
        /// </summary>
        public int Height
        {
            set;
            get;
        }
        /// <summary>
        /// 文件的高度，单位：像素
        /// </summary>
        public int Width
        {
            set;
            get;
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
        /// 中间行像素点的灰度值
        /// </summary>
        public int[] GraysR
        {
            get;
            set;
        }
        /// <summary>
        /// 中间列像素点的灰度值
        /// </summary>
        public int[] GraysC
        {
            get;
            set;
        }

        /// <summary>
        /// 创建一个空的媒体文件模型
        /// </summary>
        public MediaModel()
        {
            base.FillBaseModel();
            
            InFolder = false;
            Loaded = false;
            Height = 0;
            Width = 0;
            GraysR = null;
            GraysC = null;
        }
        /// <summary>
        /// 创建媒体文件的模型
        /// </summary>
        /// <param name="full">文件全称</param>
        public MediaModel(string full)
        {
            base.FillBaseModel(full);

            InFolder = false;
            Loaded = false;
            Height = 0;
            Width = 0;
            GraysR = null;
            GraysC = null;
        }
        /// <summary>
        /// 创建媒体文件的模型
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="name">文件名称</param>
        public MediaModel(string path, string name)
        {
            base.FillBaseModel(path, name);

            InFolder = false;
            Loaded = false;
            Height = 0;
            Width = 0;
            GraysR = null;
            GraysC = null;
        }
        /// <summary>
        /// 创建媒体文件的模型
        /// </summary>
        /// <param name="file">文件信息</param>
        public MediaModel(System.IO.FileInfo file)
        {
            base.FillBaseModel(file);
            
            InFolder = false;
            Loaded = false;
            Height = 0;
            Width = 0;
            GraysR = null;
            GraysC = null;
        }

        /// <summary>
        /// 导出文件
        /// </summary>
        /// <param name="showConfirm"></param>
        /// <param name="showError"></param>
        /// <returns></returns>
        public FileMoveResult Export(bool showConfirm = false, bool showError = false)
        {
            return base.Export(showConfirm, showError);
        }

        /// <summary>
        /// 旋转 90 度（只能对图片 / GIF 文件进行这项操作）
        /// </summary>
        public void Rotate090()
        {

        }
        /// <summary>
        /// 左右翻转（只能对图片 / GIF 文件进行这项操作）
        /// </summary>
        public void FlipX()
        {

        }
        /// <summary>
        /// 上下翻转（只能对图片 / GIF 文件进行这项操作）
        /// </summary>
        public void FlipY()
        {

        }

        /// <summary>
        /// 加载该文件
        /// </summary>
        public void Load()
        {
            if (!Exist) { Loaded = false; return; }

            System.IO.FileInfo file = new System.IO.FileInfo(Full);
            if (Loaded && file.LastWriteTime.ToFileTime() == Time) { return; }
            Loaded = false;

            if (Type == FileType.Picture || Type == FileType.Gif)
            {
                Loaded = true;
                return;
            }
            if (Type == FileType.Music)
            {
                Loaded = true;
                return;
            }
            if (Type == FileType.Video)
            {
                Loaded = true;
                return;
            }
        }
        /// <summary>
        /// 释放具有指定索引号文件的一个引用
        /// </summary>
        /// <param name="baseIndex">索引号</param>
        public void Dispose(BaseIndex baseIndex)
        {
            if (baseIndex.Exist) { Manager.Media.Models[baseIndex.Base].Count--; }
        }
    }
}
