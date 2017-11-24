using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 所支持文件
    /// </summary>
    class Support
    {
        /// <summary>
        /// 是否支持隐藏文件
        /// </summary>
        public static bool IsSupportHide
        {
            set;
            get;
        }

        /// <summary>
        /// 本文件的文件类型
        /// </summary>
        public TYPE Type
        {
            get;
            set;
        }
        /// <summary>
        /// 文件类型
        /// </summary>
        public enum TYPE
        {
            /// <summary>
            /// 暂不支持
            /// </summary>
            UNSUPPORT,
            /// <summary>
            /// 图片
            /// </summary>
            PICTURE,
            /// <summary>
            /// GIF
            /// </summary>
            GIF,
            /// <summary>
            /// 音频
            /// </summary>
            MUSIC,
            /// <summary>
            /// 视频
            /// </summary>
            VIDEO,
            /// <summary>
            /// 压缩文件
            /// </summary>
            ZIP
        }

        /// <summary>
        /// 本文件是否是图片文件
        /// </summary>
        public bool IsPicture
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是否是 GIF 文件
        /// </summary>
        public bool IsGif
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是否是音频文件
        /// </summary>
        public bool IsMusic
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是否是视频文件
        /// </summary>
        public bool IsVideo
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是否是 ZIP 文件
        /// </summary>
        public bool IsZip
        {
            set;
            get;
        }
        /// <summary>
        /// 本文件是不是不支持类型
        /// </summary>
        public bool IsUnsupport
        {
            set;
            get;
        }
        /// <summary>
        /// 客户自定义类型
        /// </summary>
        public bool IsUserDefine
        {
            set;
            get;
        }
        
        /// <summary>
        /// 显示后缀
        /// </summary>
        public string ShowExtension
        {
            get;
            set;
        }
        /// <summary>
        /// 隐式后缀
        /// </summary>
        public string HideExtension
        {
            get;
            set;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public Support()
        {
            Type = TYPE.UNSUPPORT;
            IsPicture = false;
            IsGif = false;
            IsMusic = false;
            IsVideo = false;
            IsZip = false;
            IsUnsupport = true;
            IsUserDefine = false;

            ShowExtension = "";
            HideExtension = "";
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="showExtension"></param>
        /// <param name="hideExtension"></param>
        public Support(TYPE type, string showExtension, string hideExtension = "")
        {
            Type = type;
            IsPicture = false;
            IsGif = false;
            IsMusic = false;
            IsVideo = false;
            IsZip = false;
            IsUnsupport = false;
            IsUserDefine = false;

            ShowExtension = showExtension;
            HideExtension = hideExtension;

            if (type == TYPE.PICTURE) { IsPicture = true; return; }
            if (type == TYPE.GIF) { IsGif = true; return; }
            if (type == TYPE.MUSIC) { IsMusic = true; return; }
            if (type == TYPE.VIDEO) { IsVideo = true; return; }
            if (type == TYPE.ZIP) { IsZip = true; return; }

            IsUnsupport = true;
        }

        /// <summary>
        /// 填充 Support 变量
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="extraExtension">额外的扩展名</param>
        /// <returns></returns>
        public bool ToSupport(TYPE type, string extraExtension)
        {
            Type = type;
            IsPicture = false;
            IsGif = false;
            IsMusic = false;
            IsVideo = false;
            IsZip = false;
            IsUnsupport = false;
            IsUserDefine = true;

            ShowExtension = extraExtension;
            HideExtension = "";

            if (type == TYPE.PICTURE) { IsPicture = true; return true; }
            if (type == TYPE.GIF) { IsGif = true; return true; }
            if (type == TYPE.MUSIC) { IsMusic = true; return true; }
            if (type == TYPE.VIDEO) { IsVideo = true; return true; }

            IsUnsupport = true;
            return false;
        }
        /// <summary>
        /// 把当前类型转换为字符串
        /// </summary>
        /// <returns></returns>
        public string ToString()
        {
            return ShowExtension;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialize()
        {

        }
        /// <summary>
        /// 默认的支持项
        /// </summary>
        public static void SetDefault()
        {
            Support support = null;

            support = new Support(TYPE.PICTURE, ".jpg", ".pv1"); Config.Supports.Add(support);
            support = new Support(TYPE.PICTURE, ".jpeg", ".pv2"); Config.Supports.Add(support);
            support = new Support(TYPE.PICTURE, ".png", ".pv3"); Config.Supports.Add(support);
            support = new Support(TYPE.PICTURE, ".bmp", ".pv4"); Config.Supports.Add(support);
            support = new Support(TYPE.GIF, ".gif", ".pv5"); Config.Supports.Add(support);
            support = new Support(TYPE.VIDEO, ".avi", ".pv6"); Config.Supports.Add(support);
            support = new Support(TYPE.VIDEO, ".mp4", ".pv7"); Config.Supports.Add(support);
            support = new Support(TYPE.VIDEO, ".webm", ".pv8"); Config.Supports.Add(support);
            support = new Support(TYPE.ZIP, ".zip", ".pv9"); Config.Supports.Add(support);
            support = new Support(TYPE.MUSIC, ".mp3", ".pv10"); Config.Supports.Add(support);
            support = new Support(TYPE.MUSIC, ".wav", ".pv11"); Config.Supports.Add(support);
            support = new Support(TYPE.MUSIC, ".m4a", ".pv12"); Config.Supports.Add(support);
            support = new Support(TYPE.MUSIC, ".flac", ".pv13"); Config.Supports.Add(support);
        }

        /// <summary>
        /// 判断某后缀是否被支持
        /// </summary>
        /// <param name="extension">后缀</param>
        public static bool IsSupport(string extension)
        {
            for (int i = 0; i < Config.Supports.Count; i++)
            {
                if (Config.Supports[i].ShowExtension == extension) { return true; }
                if (!IsSupportHide) { continue; }
                if (Config.Supports[i].HideExtension == extension) { return true; }
            }

            return false;
        }
        /// <summary>
        /// 获取某后缀的类型
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static TYPE GetType(string extension)
        {
            TYPE type = TYPE.UNSUPPORT;

            for (int i = 0; i < Config.Supports.Count; i++)
            {
                if (Config.Supports[i].ShowExtension == extension) { return Config.Supports[i].Type; }
                if (Config.Supports[i].HideExtension == extension) { return Config.Supports[i].Type; }
            }

            return type;
        }
        /// <summary>
        /// 判断某后缀的文件是不是图片文件
        /// </summary>
        /// <param name="extension">后缀</param>
        /// <returns></returns>
        public static bool IsPictureExtension(string extension)
        {
            return GetType(extension) == TYPE.PICTURE;
        }
        /// <summary>
        /// 判断某后缀的文件是不是 GIF 文件
        /// </summary>
        /// <param name="extension">后缀</param>
        /// <returns></returns>
        public static bool IsGifExtension(string extension)
        {
            return GetType(extension) == TYPE.GIF;
        }
        /// <summary>
        /// 判断某后缀的文件是不是音频文件
        /// </summary>
        /// <param name="extension">后缀</param>
        /// <returns></returns>
        public static bool IsMusicExtension(string extension)
        {
            return GetType(extension) == TYPE.MUSIC;
        }
        /// <summary>
        /// 判断某后缀的文件是不是视频文件
        /// </summary>
        /// <param name="extension">后缀</param>
        /// <returns></returns>
        public static bool IsVideoExtension(string extension)
        {
            return GetType(extension) == TYPE.VIDEO;
        }
        /// <summary>
        /// 判断某后缀的文件是不是 ZIP 文件
        /// </summary>
        /// <param name="extension">后缀</param>
        /// <returns></returns>
        public static bool IsZipExtension(string extension)
        {
            return GetType(extension) == TYPE.ZIP;
        }
    }
}
