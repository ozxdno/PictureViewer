using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer
{
    class Strings
    {
        /// <summary>
        /// 语种（默认中文）
        /// </summary>
        public static LANGUAGE Language = LANGUAGE.Chinese;
        /// <summary>
        /// 语种
        /// </summary>
        public enum LANGUAGE
        {
            /// <summary>
            /// 中文
            /// </summary>
            Chinese,
            /// <summary>
            /// 英文
            /// </summary>
            English,
            /// <summary>
            /// 日文
            /// </summary>
            Japanese
        }
        
        public static string FileName
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "文件名"; }
                if (Language == LANGUAGE.English) { return "FileName"; }
                return "未设置该项文本";
            }
        }
        public static string File
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "文件"; }
                if (Language == LANGUAGE.English) { return "File"; }
                return "未设置该项文本";
            }
        }

        public static string Failed
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "失败"; }
                if (Language == LANGUAGE.English) { return "Failed"; }
                return "未设置该项文本";
            }
        }
        public static string Successed
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "成功"; }
                if (Language == LANGUAGE.English) { return "Successed"; }
                return "未设置该项文本";
            }
        }
        public static string Confirm
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "请确认"; }
                if (Language == LANGUAGE.English) { return "Please Confirm"; }
                return "未设置该项文本";
            }
        }
        public static string Error
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "错误"; }
                if (Language == LANGUAGE.English) { return "Error"; }
                return "未设置该项文本";
            }
        }
        public static string Semicolon
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "："; }
                if (Language == LANGUAGE.English) { return ":"; }
                return "未设置该项文本";
            }
        }
        public static string Existed
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "已存在"; }
                if (Language == LANGUAGE.English) { return "Existed"; }
                return "未设置该项文本";
            }
        }
        public static string NotExist
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "不存在"; }
                if (Language == LANGUAGE.English) { return "Not Exist"; }
                return "未设置该项文本";
            }
        }
        public static string IsUsing
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "正在使用"; }
                if (Language == LANGUAGE.English) { return "Using"; }
                return "未设置该项文本";
            }
        }
        public static string Exclamation
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "！"; }
                if (Language == LANGUAGE.English) { return "!"; }
                return "未设置该项文本";
            }
        }
        public static string Sour
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "源"; }
                if (Language == LANGUAGE.English) { return "Source"; }
                return "未设置该项文本";
            }
        }
        public static string Dest
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "目标"; }
                if (Language == LANGUAGE.English) { return "Destination"; }
                return "未设置该项文本";
            }
        }
        public static string Export
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "导出"; }
                if (Language == LANGUAGE.English) { return "Export"; }
                return "未设置该项文本";
            }
        }

        public static string ConfirmFileMove
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "是否移动文件？"; }
                if (Language == LANGUAGE.English) { return "Move File ?"; }
                return "未设置该项文本";
            }
        }
        public static string ConfirmFileRename
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "是否重命名文件？"; }
                if (Language == LANGUAGE.English) { return "Rename File ?"; }
                return "未设置该项文本";
            }
        }
        public static string ConfirmFileExport
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "是否导出文件？"; }
                if (Language == LANGUAGE.English) { return "Export File ?"; }
                return "未设置该项文本";
            }
        }
        public static string FileExportSuccessed
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "文件导出成功！"; }
                if (Language == LANGUAGE.English) { return "Export successed !"; }
                return "未设置该项文本";
            }
        }
        public static string FileExportFailed
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "文件导出失败！"; }
                if (Language == LANGUAGE.English) { return "Export failed !"; }
                return "未设置该项文本";
            }
        }
        public static string FolderExportSuccessed
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "文件夹导出成功！"; }
                if (Language == LANGUAGE.English) { return "Export successed !"; }
                return "未设置该项文本";
            }
        }
        public static string FolderExportFailed
        {
            get
            {
                if (Language == LANGUAGE.Chinese) { return "文件夹导出失败！"; }
                if (Language == LANGUAGE.English) { return "Export failed !"; }
                return "未设置该项文本";
            }
        }
        
    }
}
