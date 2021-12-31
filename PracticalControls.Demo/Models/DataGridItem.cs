using PracticalControls.Common.Helpers;
using PracticalControls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalControls.Demo.Models
{
    public class DataGridItem : TreeDataGridItemBase<DataGridItem>
    {

        static bool _isUpdatingIsChecked = false;
        public static Action<DataGridItem, bool?> IsCheckedAction { get; set; }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _type;

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _value;

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private bool? _isChecked;

        public bool? IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (Set(ref _isChecked, value))
                {
                    if (!_isUpdatingIsChecked)
                    {
                        _isUpdatingIsChecked = true;
                        UpdateDescendantsIsChecked(this, value);
                        UpdateAncestorsIsChecked(this, value);
                        _isUpdatingIsChecked = false;

                        IsCheckedAction?.Invoke(this, value);
                    }
                }
            }
        }


        private List<string> _lstGroupName;

        public List<string> LstGroupName
        {
            get { return _lstGroupName; }
            set { Set(ref _lstGroupName, value); }
        }

        public DataGridItem()
        {

        }

        public DataGridItem(string name, string type, string value, int level, bool isVisible)
        {
            _name = name;
            _type = type;
            _value = value;
            this.Level = level;
            this.IsVisible = isVisible;
        }

        private void UpdateAncestorsIsChecked(DataGridItem planInfo, bool? isChecked)
        {
            DataGridItem parent = planInfo.Parent;
            if (isChecked == true)
            {
                //判断是否可勾选
                while (parent != null)
                {
                    if (TreeDataGridHelper.GetAllDescendants(parent, false).All(o => o.IsChecked == true))
                        parent.IsChecked = true;
                    parent = parent.Parent;
                }
            }
            else
            {
                //置空
                while (parent != null)
                {
                    parent.IsChecked = null;
                    parent = parent.Parent;
                }
            }

        }

        private void UpdateDescendantsIsChecked(DataGridItem planInfo, bool? isChecked)
        {
            if (isChecked == null)
                isChecked = false;

            foreach (var child in planInfo.Children)
            {
                child.IsChecked = isChecked;
                UpdateDescendantsIsChecked(child, isChecked);
            }
        }
    }
}
