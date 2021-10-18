using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalControls.Interfaces
{
    interface IParentChildren
    {
        public object Parent { get; set; }

        public List<object> Children { get; set; }
    }
}
