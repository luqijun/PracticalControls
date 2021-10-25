using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalControls.Interfaces
{
    /// <summary>
    /// 父子关系
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IParentChildren<T>
    {
        public T Parent { get; set; }

        public IList<T> Children { get; set; }
    }
}
