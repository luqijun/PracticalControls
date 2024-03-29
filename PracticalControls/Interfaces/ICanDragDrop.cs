﻿using PracticalControls.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalControls.Interfaces
{
    public interface ICanDragDrop
    {
        /// <summary>
        /// 是否可Drag
        /// </summary>
        public bool CanDrag { get; set; }

        /// <summary>
        /// 是否可以Drop
        /// </summary>
        public bool CanDrop { get; set; }

        /// <summary>
        /// 是否可以Drop到同级节点
        /// </summary>
        public bool CanDropToSameGrade { get; set; }

        /// <summary>
        /// 是否可以Drop到下一级点
        /// </summary>
        public bool CanDropToNextGrade { get; set; }
    }
}
