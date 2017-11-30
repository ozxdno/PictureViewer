using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Strings
{
    class FileStrings
    {
        public static string Dest_File_Illegal
        {
            get
            {
                if (Language.language == Language.LANGUAGE.Chinese) { return "目标地址含非法字符！（\\/*?<>|\":）"; }
                if (Language.language == Language.LANGUAGE.English) { return "Illegal chars ! (\\/*?<>|\":)"; }
                return "文本缺失！";
            }
        }
        public static string Sour_File_Not
        {
            get
            {
                if (Language.language == Language.LANGUAGE.Chinese) { return "源文件不存在！"; }
                if (Language.language == Language.LANGUAGE.English) { return "Source file is not exist !"; }
                return "文本缺失！";
            }
        }
        public static string Dest_File_Existed
        {
            get
            {
                if (Language.language == Language.LANGUAGE.Chinese) { return "目标文件已存在！"; }
                if (Language.language == Language.LANGUAGE.English) { return "Destination file existed !"; }
                return "文本缺失！";
            }
        }
        public static string Sour_File_Using
        {
            get
            {
                if (Language.language == Language.LANGUAGE.Chinese) { return "源文件正在使用中或者其他错误！"; }
                if (Language.language == Language.LANGUAGE.English) { return "Source is using or other errors !"; }
                return "文本缺失！";
            }
        }
        public static string Dest_Name_Illegal
        {
            get
            {
                if (Language.language == Language.LANGUAGE.Chinese) { return "新文件名含非法字符！（\\/*?<>|\":）"; }
                if (Language.language == Language.LANGUAGE.English) { return "Illegal chars ! (\\/*?<>|\":)"; }
                return "文本缺失！";
            }
        }

        public static string Ask_Confirm_Move
        {
            get
            {
                if (Language.language == Language.LANGUAGE.Chinese) { return "是否移动当前文件？"; }
                if (Language.language == Language.LANGUAGE.English) { return "Move File ?"; }
                return "文本缺失！";
            }
        }
        public static string Sour_File
        {
            get
            {
                if (Language.language == Language.LANGUAGE.Chinese) { return "源文件："; }
                if (Language.language == Language.LANGUAGE.English) { return "Source : "; }
                return "文本缺失！";
            }
        }
        public static string Dest_File
        {
            get
            {
                if (Language.language == Language.LANGUAGE.Chinese) { return "目标："; }
                if (Language.language == Language.LANGUAGE.English) { return "Destination : "; }
                return "文本缺失！";
            }
        }
        public static string Sour_Name
        {
            get
            {
                if (Language.language == Language.LANGUAGE.Chinese) { return "源文件文件名："; }
                if (Language.language == Language.LANGUAGE.English) { return "Source Name : "; }
                return "文本缺失！";
            }
        }
        public static string Dest_Name
        {
            get
            {
                if (Language.language == Language.LANGUAGE.Chinese) { return "新文件名："; }
                if (Language.language == Language.LANGUAGE.English) { return "New Name : "; }
                return "文本缺失！";
            }
        }

        public static string getResultString(Files.BaseFileInfo.FILE_MOVE_RESULT result)
        {
            if (result == Files.BaseFileInfo.FILE_MOVE_RESULT.DEST_FILE_ILLEGAL) { return Dest_File_Illegal; }
            if (result == Files.BaseFileInfo.FILE_MOVE_RESULT.SOUR_FILE_NOT) { return Sour_File_Not; }
            if (result == Files.BaseFileInfo.FILE_MOVE_RESULT.DEST_FILE_EXISTED) { return Dest_File_Existed; }
            if (result == Files.BaseFileInfo.FILE_MOVE_RESULT.SOUR_FILE_USING) { return Sour_File_Using; }
            if (result == Files.BaseFileInfo.FILE_MOVE_RESULT.DEST_NAME_ILLEGAL) { return Dest_Name_Illegal; }

            return "";
        }
    }
}
