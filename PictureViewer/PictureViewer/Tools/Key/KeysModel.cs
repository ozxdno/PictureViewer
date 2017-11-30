using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Tools
{
    /// <summary>
    /// 按键集合（组合键）的模型
    /// </summary>
    public class KeysModel
    {
        /// <summary>
        /// 按键集合
        /// </summary>
        public List<KeyModel> Keys
        {
            set;
            get;
        }

        /// <summary>
        /// 是否有键按下
        /// </summary>
        public bool IsDownAny
        {
            get
            {
                for (int i = 0; i < Keys.Count; i++) { if (Keys[i].Down) { return true; } }
                return false;
            }
        }
        /// <summary>
        /// 是否所有键都按下
        /// </summary>
        public bool IsDownAll
        {
            get
            {
                for (int i = 0; i < Keys.Count; i++) { if (!Keys[i].Down) { return false; } }
                return true;
            }
        }
        /// <summary>
        /// 指定键值的键是否被按下
        /// </summary>
        /// <param name="keyValue">键值</param>
        /// <returns></returns>
        public bool IsDown(int keyValue)
        {
            int index = Search(keyValue); if (index == -1) { return false; }
            return Keys[index].Down;
        }

        /// <summary>
        /// 是否有键抬起
        /// </summary>
        public bool IsUpAny
        {
            get
            {
                for (int i = 0; i < Keys.Count; i++) { if (Keys[i].Up) { return true; } }
                return false;
            }
        }
        /// <summary>
        /// 是否所有键都抬起
        /// </summary>
        public bool IsUpAll
        {
            get
            {
                for (int i = 0; i < Keys.Count; i++) { if (!Keys[i].Up) { return false; } }
                return true;
            }
        }
        /// <summary>
        /// 指定键值的键是否被抬起
        /// </summary>
        /// <param name="keyValue">键值</param>
        /// <returns></returns>
        public bool IsUp(int keyValue)
        {
            int index = Search(keyValue); if (index == -1) { return false; }
            return Keys[index].Up;
        }

        /// <summary>
        /// 创建按键集合（组合键）的模型
        /// </summary>
        public KeysModel()
        {
            Keys = new List<KeyModel>();
        }
        /// <summary>
        /// 创建按键集合（组合键）的模型
        /// </summary>
        public KeysModel(List<int> keyValues)
        {
            Keys = new List<KeyModel>();
            foreach (int i in keyValues) { Keys.Add(new KeyModel(i)); }
        }

        /// <summary>
        /// 添加键（已经存在则不添加）
        /// </summary>
        /// <param name="keyValue">键值</param>
        public void Add(int keyValue)
        {
            int index = Search(keyValue); if (index != -1) { return; }
            Keys.Add(new KeyModel(keyValue));
        }
        /// <summary>
        /// 删除键
        /// </summary>
        /// <param name="keyValue">键值</param>
        public void Delete(int keyValue)
        {
            int index = Search(keyValue); if (index == -1) { return; }
            Keys.RemoveAt(index);
        }
        
        /// <summary>
        /// 搜索对应键值的按键的索引号
        /// </summary>
        /// <param name="keyValue">键值</param>
        /// <returns></returns>
        public int Search(int keyValue)
        {
            for (int i = 0; i < Keys.Count; i++) { if (Keys[i].KeyValue == keyValue) { return i; } }
            return -1;
        }

        /// <summary>
        /// 按键按下
        /// </summary>
        /// <param name="keyValue">键值</param>
        public void KeyDown(int keyValue)
        {
            int index = Search(keyValue); if (keyValue == -1) { return; }
            Keys[index].KeyDown();
        }
        /// <summary>
        /// 按键按下
        /// </summary>
        /// <param name="keyValue">键值</param>
        /// <param name="pForm">更新窗体 / 控件的位置</param>
        public void KeyDown(int keyValue, System.Drawing.Point pForm)
        {
            int index = Search(keyValue); if (keyValue == -1) { return; }
            Keys[index].KeyDown(pForm);
        }
        /// <summary>
        /// 按键按下
        /// </summary>
        /// <param name="keyValue">键值</param>
        /// <param name="xScroll">更新 X 轴滑动量</param>
        /// <param name="yScroll">更新 Y 轴滑动量</param>
        public void KeyDown(int keyValue, int xScroll, int yScroll)
        {
            int index = Search(keyValue); if (keyValue == -1) { return; }
            Keys[index].KeyDown(xScroll, yScroll);
        }
        /// <summary>
        /// 按键按下
        /// </summary>
        /// <param name="keyValue">键值</param>
        /// <param name="pForm">更新窗体 / 控件的位置</param>
        /// <param name="xScroll">更新 X 轴滑动量</param>
        /// <param name="yScroll">更新 Y 轴滑动量</param>
        public void KeyDown(int keyValue, System.Drawing.Point pForm, int xScroll, int yScroll)
        {
            int index = Search(keyValue); if (keyValue == -1) { return; }
            Keys[index].KeyDown(pForm, xScroll, yScroll);
        }


        /// <summary>
        /// 按键抬起
        /// </summary>
        /// <param name="keyValue">键值</param>
        public void KeyUp(int keyValue)
        {
            int index = Search(keyValue); if (keyValue == -1) { return; }
            Keys[index].KeyUp();
        }
        /// <summary>
        /// 按键抬起
        /// </summary>
        /// <param name="keyValue">键值</param>
        /// <param name="pForm">更新窗体 / 控件的位置</param>
        public void KeyUp(int keyValue, System.Drawing.Point pForm)
        {
            int index = Search(keyValue); if (keyValue == -1) { return; }
            Keys[index].KeyUp(pForm);
        }
        /// <summary>
        /// 按键抬起
        /// </summary>
        /// <param name="keyValue">键值</param>
        /// <param name="xScroll">更新 X 轴滑动量</param>
        /// <param name="yScroll">更新 Y 轴滑动量</param>
        public void KeyUp(int keyValue, int xScroll, int yScroll)
        {
            int index = Search(keyValue); if (keyValue == -1) { return; }
            Keys[index].KeyUp(xScroll, yScroll);
        }
        /// <summary>
        /// 按键抬起
        /// </summary>
        /// <param name="keyValue">键值</param>
        /// <param name="pForm">更新窗体 / 控件的位置</param>
        /// <param name="xScroll">更新 X 轴滑动量</param>
        /// <param name="yScroll">更新 Y 轴滑动量</param>
        public void KeyUp(int keyValue, System.Drawing.Point pForm, int xScroll, int yScroll)
        {
            int index = Search(keyValue); if (keyValue == -1) { return; }
            Keys[index].KeyUp(pForm, xScroll, yScroll);
        }
    }
}
