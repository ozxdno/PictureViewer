using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 所支持的文件
    /// </summary>
    public class SupportInfo
    {
        private List<SupportModel> models;

        /// <summary>
        /// 指定后缀是否是所支持的后缀
        /// </summary>
        /// <param name="extension">指定后缀</param>
        /// <returns></returns>
        public bool IsSupport(string extension)
        {
            foreach (SupportModel m in models)
            {
                if (m.ShowExtension == extension) { return true; }
                if (!SupportModel.SupportHide) { continue; }
                if (m.HideExtension == extension) { return true; }
            }

            return false;
        }
        /// <summary>
        /// 判断指定后缀是不是所支持的显式后缀
        /// </summary>
        /// <param name="extension">指定后缀</param>
        /// <returns></returns>
        public bool IsSupportShow(string extension)
        {
            foreach (SupportModel m in models)
            {
                if (m.ShowExtension == extension) { return true; }
            }

            return false;
        }
        /// <summary>
        /// 判断指定后缀是不是所支持的隐式后缀
        /// </summary>
        /// <param name="extension">指定后缀</param>
        /// <returns></returns>
        public bool IsSupportHide(string extension)
        {
            foreach (SupportModel m in models)
            {
                if (m.HideExtension == extension) { return true; }
            }

            return false;
        }

        /// <summary>
        /// 添加支持的后缀模型（若重复则替换）
        /// </summary>
        /// <param name="model">模型</param>
        public void Add(SupportModel model)
        {
            for (int i = 0; i < models.Count; i++)
            {
                if (models[i].ShowExtension == model.ShowExtension) { models[i] = model; return; }
            }

            models.Add(model);
        }
        /// <summary>
        /// 删除对某一后缀的支持
        /// </summary>
        /// <param name="model">模型</param>
        public void Delete(SupportModel model)
        {
            for (int i = 0; i < models.Count; i++)
            {
                if (models[i].ShowExtension == model.ShowExtension) { models.RemoveAt(i); return; }
            }
        }
        /// <summary>
        /// 删除对某一显式后缀的支持（同时也会删除对该后缀的隐式后缀的支持）
        /// </summary>
        /// <param name="showExtension">显式后缀</param>
        public void DeleteShow(string showExtension)
        {
            for (int i = 0; i < models.Count; i++)
            {
                if (models[i].ShowExtension == showExtension) { models.RemoveAt(i); return; }
            }
        }
        /// <summary>
        /// 删除对某一隐式后缀的支持（不会删除对该后缀的显式后缀的支持）
        /// </summary>
        /// <param name="hideExtension">隐式后缀</param>
        public void DeleteHide(string hideExtension)
        {
            for (int i = 0; i < models.Count; i++)
            {
                if (models[i].HideExtension == hideExtension) { models[i].HideExtension = null; return; }
            }
        }
        /// <summary>
        /// 获取指定后缀文件的文件类型
        /// </summary>
        /// <param name="extension">指定后缀</param>
        /// <returns></returns>
        public FileType GetType(string extension)
        {
            for (int i = 0; i < models.Count; i++)
            {
                if (models[i].ShowExtension == extension) { return models[i].Type; }
            }
            for (int i = 0; i < models.Count; i++)
            {
                if (models[i].HideExtension == extension) { return models[i].Type; }
            }

            return FileType.Unsupport;
        }

        /// <summary>
        /// 创建支持文件信息
        /// </summary>
        public SupportInfo()
        {
            models = new List<SupportModel>();
        }
    }
}
