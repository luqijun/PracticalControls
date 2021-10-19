using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PracticalControls.Common.Helpers
{
    public class UIHelper
    {
        #region 逻辑树、视图树操作

        /// <summary>
        /// 取得祖先元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="func">条件</param>
        /// <param name="needCompareSelf">比较自身</param>
        /// <returns></returns>
        public static T FindAncestor<T>(DependencyObject element, Func<T, bool> func = null, bool needCompareSelf = false) where T : DependencyObject
        {
            if (element == null)
                return default(T);

            var parent = needCompareSelf ? element : VisualTreeHelper.GetParent(element);
            if (parent is T p)
            {
                if (func == null || func(p))
                    return p;
            }
            return FindAncestor<T>(parent);
        }

        /// <summary>
        /// 获取后代元素（单个）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dp"></param>
        /// <param name="func">条件</param>
        /// <param name="needCompareSelf">比较自身</param>
        /// <returns></returns>
        public static T FindDescendant<T>(DependencyObject dp, Func<T, bool> func = null, bool needCompareSelf = false) where T : DependencyObject
        {
            if (dp == null)
                return null;

            //比较自身
            if (needCompareSelf)
            {
                if (dp is T result)
                    if (func == null || func(result))
                        return result;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dp); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(dp, i);
                if (child is T c)
                {
                    if (func == null || func(c))
                        return (T)c;
                }

                var childDescendant = FindDescendant<T>(child, func);
                if (childDescendant != null)
                    return childDescendant;
            }
            return null;
        }

        /// <summary>
        /// 获取后代元素（多个）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dp"></param>
        /// <param name="func">条件</param>
        /// <param name="needCompareSelf">比较自身</param>
        /// <returns></returns>
        public static IEnumerable<T> FindDescendants<T>(DependencyObject dp, Func<T, bool> func = null, bool needCompareSelf = false) where T : DependencyObject
        {
            if (dp == null)
                throw new Exception("元素值不能为空！");

            //比较自身
            if (needCompareSelf)
            {
                if (dp is T result)
                    if (func == null || func(result))
                        yield return result;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dp); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(dp, i);
                if (child != null && child is T c)
                {
                    if (func == null || func(c))
                        yield return (T)c;
                }

                foreach (T childOfChild in FindDescendants<T>(child, func))
                {
                    yield return childOfChild;
                }
            }

        }

        #endregion
    }
}
