using PracticalControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalControls.Models
{
    public abstract class TreeDataGridItemBase<T> : TreeDataGridItemBase, IParentChildren<T>
        where T : TreeDataGridItemBase<T>
    {
        private T _parent;

        public new T Parent
        {
            get { return _parent; }
            set { _parent = value; base.Parent = value; }
        }

        private IList<T> _children;
        /// <summary>
        /// 子节点
        /// </summary>
        public new IList<T> Children
        {
            get { return _children; }
            set { Set(ref _children, value); base.Children = (System.Collections.IList)value; }
        }

        public TreeDataGridItemBase(int level = 0, bool isVisible = true)
        {
            this.Level = level;
            this.IsVisible = isVisible;

            this.Children = new List<T>();
        }
    }
}
