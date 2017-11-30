using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Tools
{
    class BoxStrings
    {
        public static string Title_Confirm
        {
            get
            {
                if (SetLanguage.Current == Language.Chinese) { return "请确认！"; }
                if (Language.language == Language.LANGUAGE.English) { return "Please Confirm !"; }
                return "文本缺失！";
            }
        }
        public static string Title_Tip
        {
            get
            {
                if (Language.language == Language.LANGUAGE.Chinese) { return "提示："; }
                if (Language.language == Language.LANGUAGE.English) { return "Tips :"; }
                return "文本缺失！";
            }
        }
        public static string Title_Warning
        {
            get
            {
                if (Language.language == Language.LANGUAGE.Chinese) { return "警告！"; }
                if (Language.language == Language.LANGUAGE.English) { return "Warning !"; }
                return "文本缺失！";
            }
        }
    }
}
