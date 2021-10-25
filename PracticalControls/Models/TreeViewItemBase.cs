using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalControls.Models
{
    public abstract class TreeViewItemBase : ViewModelBase
    {
        /// <summary>
        /// 源数据
        /// </summary>
        public object SourceData { get; set; }

        public string Id { get; set; }

        public string ParentId { get; set; }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        private object _imageSource;
        /// <summary>
        /// 图标
        /// </summary>
        public object ImageSource
        {
            get { return _imageSource; }
            set { Set(ref _imageSource, value); }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (Set(ref _isExpanded, value))
                    OnIsExpandChanged(value);
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(ref _isSelected, value); }
        }

        public TreeViewItemBase Parent { get; set; }
        public IEnumerable<TreeViewItemBase> Children { get; set; } = new ObservableCollection<TreeViewItemBase>();

        /// <summary>
        /// IsExpand属性改变事件
        /// </summary>
        protected virtual void OnIsExpandChanged(bool isExpand)
        {

        }
    }
}
