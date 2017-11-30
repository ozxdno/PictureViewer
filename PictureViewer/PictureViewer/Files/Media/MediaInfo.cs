using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 媒体文件信息
    /// </summary>
    public class MediaInfo
    {
        /// <summary>
        /// 多媒体文件的文件信息表
        /// </summary>
        public List<MediaModel> Models
        {
            get;
            set;
        }
        
        /// <summary>
        /// 搜索多媒体文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="name">文件名称</param>
        /// <returns></returns>
        public int Search(string path, string name)
        {
            for (int i = 0; i < Models.Count; i++)
            {
                if (Models[i].Path == path && Models[i].Name == name) { return i; }
            }

            return -1;
        }
        /// <summary>
        /// 搜索多媒体文件
        /// </summary>
        /// <param name="full">文件全称</param>
        /// <returns></returns>
        public int Search(string full)
        {
            string path = Tools.FileOperate.getPath(full);
            string name = Tools.FileOperate.getName(full);

            return Search(path, name);
        }
        /// <summary>
        /// 搜索多媒体文件
        /// </summary>
        /// <param name="model">多媒体文件模型</param>
        /// <returns></returns>
        public int Search(MediaModel model)
        {
            for (int i = 0; i < Models.Count; i++)
            {
                if (Models[i] == model) { return i; }
            }

            return -1;
        }

        /// <summary>
        /// 创建媒体文件信息
        /// </summary>
        public MediaInfo()
        {
            Models = new List<MediaModel>();
        }
    }
}
