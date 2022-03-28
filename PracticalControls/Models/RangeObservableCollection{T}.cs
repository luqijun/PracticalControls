using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PracticalControls.Models
{
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {

        public RangeObservableCollection()
        {

        }

        public RangeObservableCollection(IEnumerable<T> collection) : base(collection)
        {

        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<T> items)
        {
            this.CheckReentrancy();
            foreach (var item in items)
                this.Items.Add(item);
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="insertIndex"></param>
        /// <param name="items"></param>
        public void InsertRange(int insertIndex, IEnumerable<T> items)
        {
            this.CheckReentrancy();
            foreach (var item in items)
                this.Items.Insert(insertIndex++, item);
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="items"></param>
        /// <param name="isConsecutiveRemove">删除的数据是连续</param>
        public void RemoveRange(IEnumerable<T> items, bool isConsecutiveRemove = false)
        {
            this.CheckReentrancy();

            if (isConsecutiveRemove)
            {
                int index01 = this.Items.IndexOf(items.FirstOrDefault());
                int index02 = this.Items.IndexOf(items.LastOrDefault());
                int minIndex = Math.Min(index01, index02);
                int maxIndex = Math.Max(index01, index02);
                if (minIndex != -1)
                {
                    if (this.Items is List<T> lst)
                        lst.RemoveRange(minIndex, maxIndex - minIndex + 1);
                }
            }
            else
            {
                foreach (var item in items)
                {
                    this.Items.Remove(item);
                }
            }
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
            });
        }
    }
}

