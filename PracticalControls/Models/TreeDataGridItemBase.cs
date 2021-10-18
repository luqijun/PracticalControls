using GalaSoft.MvvmLight;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalControls.Models
{
    public abstract class TreeDataGridItemBase : ObservableObject
    {
        private int _level;
        /// <summary>
        /// 级别
        /// </summary>
        public int Level
        {
            get { return _level; }
            set { Set(ref _level, value); }
        }

        private bool _isExpanded;
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (Set(ref _isExpanded, value))
                    UpdateChildrenVisibility(this, value);
            }
        }


        private bool _isVisible;
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set { Set(ref _isVisible, value); }
        }

        private TreeDataGridItemBase _parent;

        public virtual TreeDataGridItemBase Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        private IList _children;
        /// <summary>
        /// 子节点
        /// </summary>
        public IList Children
        {
            get { return _children; }
            set { Set(ref _children, value); }
        }

        #region 构造函数
        public TreeDataGridItemBase()
        {

        }
        #endregion

        /// <summary>
        /// 收缩展开时 递归设置显示隐藏
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isVisible"></param>
        protected void UpdateChildrenVisibility(TreeDataGridItemBase data, bool isVisible)
        {
            foreach (TreeDataGridItemBase child in data.Children)
            {
                child.IsVisible = isVisible;
                if (child.IsExpanded)
                    UpdateChildrenVisibility(child, isVisible);
            }
        }
    }
}
