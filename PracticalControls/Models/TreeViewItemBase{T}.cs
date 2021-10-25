using GalaSoft.MvvmLight;
using PracticalControls.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalControls.Models
{
    public class TreeViewItemBase<T> : TreeViewItemBase where T : TreeViewItemBase
    {
        private T _parent;

        public new T Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                base.Parent = value;
            }
        }

        private ObservableCollection<T> _children = new ObservableCollection<T>();

        public new ObservableCollection<T> Children
        {
            get { return _children; }
            set
            {
                _children = value;
                base.Children = value;
            }
        }

        public TreeViewItemBase()
        {
            base.Children = this.Children;
        }
    }
}
