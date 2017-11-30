using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureViewer.Files
{
    /// <summary>
    /// 文件的基本索引
    /// </summary>
    public class BaseIndex
    {
        /// <summary>
        /// 基本索引号
        /// </summary>
        public int Base
        {
            set;
            get;
        }
        /// <summary>
        /// 该索引是否有效
        /// </summary>
        public bool Exist
        {
            get { return 0 <= Base && Base < Manager.Media.Models.Count; }
        }
        /// <summary>
        /// 树形索引
        /// </summary>
        public List<TreeIndex> Trees
        {
            get { return Manager.Index.Search(Base); }
        }
        /// <summary>
        /// 文件信息
        /// </summary>
        public MediaModel Model
        {
            get { return Exist ? Manager.Media.Models[Base] : new MediaModel(); }
        }
        
        /// <summary>
        /// 构造基本索引号
        /// </summary>
        /// <param name="baseIndex">索引号</param>
        public BaseIndex(int baseIndex = -1)
        {
            Base = baseIndex;
        }

        /// <summary>
        /// 调整索引号
        /// </summary>
        public void Fit()
        {
            if (Base < 0) { Base = 0; }
            if (Base > Manager.Media.Models.Count - 1) { Base = Manager.Media.Models.Count - 1; }
        }
        
        /// <summary>
        /// 复制
        /// </summary>
        /// <returns></returns>
        public BaseIndex Copy()
        {
            return new BaseIndex(Base);
        }

        public static bool operator ==(BaseIndex left, object right)
        {
            int l = left.Base;
            int r = 0;

            if (right is int) { r = (int)right; }
            else if (right is BaseIndex) { r = ((BaseIndex)right).Base; }
            else { return false; }

            return l == r;
        }
        public static bool operator !=(BaseIndex left, object right)
        {
            int l = left.Base;
            int r = -2;

            if (right is int) { r = (int)right; }
            else if (right is BaseIndex) { r = ((BaseIndex)right).Base; }
            else { return true; }

            return l != r;
        }
    }
}
