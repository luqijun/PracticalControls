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

        private string _groupName;

        public string GroupName
        {
            get { return _groupName; }
            set { Set(ref _groupName, value); }
        }


        public DataGridItem(string name, string type, string value, int level, bool isVisible)
        {
            _name = name;
            _type = type;
            _value = value;
            this.Level = level;
            this.IsVisible = isVisible;

            this.IsExpanded = true;
        }
    }
}
