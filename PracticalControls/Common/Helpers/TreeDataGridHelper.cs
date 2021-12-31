using PracticalControls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalControls.Common.Helpers
{
    public class TreeDataGridHelper
    {
        /// <summary>
        /// 根据级别设置父子关系
        /// </summary>
        /// <param name="lstData"></param>
        public static void ResetRelationShip<T>(IEnumerable<T> lstData) where T : TreeDataGridItemBase<T>
        {
            if (!lstData.Any())
                return;

            Stack<T> stack = new Stack<T>();

            int startLevel = lstData.FirstOrDefault().Level;

            foreach (var data in lstData)
            {
                if (data.Level == startLevel)
                {
                    stack.Push(data);
                    continue;
                }

                var peek = stack.Peek();
                while (peek != null && data.Level <= peek.Level)
                {
                    stack.Pop();
                    if (stack.Count > 0)
                        peek = stack.Peek();
                    else
                        peek = null;
                }

                if (peek != null)
                {
                    peek.Children.Add(data);
                    data.Parent = peek;
                }

                stack.Push(data);//进栈
            }
        }

        /// <summary>
        /// 获取祖先节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T GetAncestor<T>(T item) where T : TreeDataGridItemBase<T>
        {
            T parent = item;
            while (parent.Parent != null)
                parent = parent.Parent;
            return parent;
        }

        /// <summary>
        /// 递归向下遍历节点
        /// </summary>
        /// <param name="lstNodes"></param>
        /// <param name="action"></param>
        /// <param name="containself"></param>
        public static void TraverseDescendants<T>(IEnumerable<T> lstNodes, Action<T> action, bool containself = true) where T : TreeDataGridItemBase<T>
        {
            foreach (var node in lstNodes)
            {
                if (containself)
                {
                    action(node);
                }
                TraverseDescendants(node.Children, action);
            }
        }

        /// <summary>
        /// 递归向上遍历所有祖先节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="action"></param>
        /// <param name="containself"></param>
        public static void TraverseAncestors<T>(T node, Action<T> action, bool containself = true) where T : TreeDataGridItemBase<T>
        {
            if (containself)
                action(node);

            T parent = node.Parent;
            while (parent != null)
            {
                action(parent);
                parent = parent.Parent;
            }
        }

        /// <summary>
        /// 获取所有的后代节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstNodes"></param>
        /// <param name="containself"></param>
        public static IEnumerable<T> GetAllDescendants<T>(IEnumerable<T> lstNodes, bool containself = true) where T : TreeDataGridItemBase<T>
        {
            foreach (var node in lstNodes)
            {
                foreach (var descendant in GetAllDescendants(node, containself))
                    yield return descendant;
            }
        }

        /// <summary>
        /// 获取所有的后代节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="containself"></param>
        public static IEnumerable<T> GetAllDescendants<T>(T node, bool containself = true) where T : TreeDataGridItemBase<T>
        {
            if (containself)
                yield return node;

            foreach (var child in node.Children)
            {
                foreach (var descendant in GetAllDescendants(child))
                    yield return descendant;
            }
        }

        /// <summary>
        /// 获取满足条件的后代节点(单个根节点）
        /// </summary>
        /// <param name="node"></param>
        /// <param name="predicate"></param>
        /// <param name="containself">是否包含自身</param>
        /// <returns></returns>
        public static IEnumerable<T> GetDescendants<T>(T node, Func<T, bool> predicate, bool containself = true) where T : TreeDataGridItemBase<T>
        {
            if (containself && predicate(node))
                yield return node;

            foreach (var child in node.Children)
            {
                foreach (var descendant in GetDescendants(child, predicate))
                    yield return descendant;
            }
        }

        /// <summary>
        /// 获取满足条件的后代节点（多个根节点）
        /// </summary>
        /// <param name="lstNodes"></param>
        /// <param name="predicate"></param>
        /// <param name="containself">是否包含自身</param>
        /// <returns></returns>
        public static IEnumerable<T> GetDescendants<T>(IEnumerable<T> lstNodes, Func<T, bool> predicate, bool containself = true) where T : TreeDataGridItemBase<T>
        {
            foreach (var node in lstNodes)
            {
                foreach (var descendant in GetDescendants(node, predicate, containself))
                    yield return descendant;
            }
        }

        /// <summary>
        /// 重置数据源时 展开原来已经展开的项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstNodes"></param>
        /// <param name="resetFunc">重置操作</param>
        /// <param name="comp">重置操作</param>
        public static void ResetSourceWithExpand<T>(IList<T> lstNodes, Func<IEnumerable<T>> resetFunc, Func<T, T, bool> compareFunc = null) where T : TreeDataGridItemBase<T>
        {
            if (compareFunc == null)
                compareFunc = (n1, n2) => n1 == n2;

            if (lstNodes == null)
            {
                //重置数据源
                resetFunc.Invoke();
            }
            else
            {
                //获取原有展开信息
                List<T> lstExpandedNode = TreeDataGridHelper.GetDescendants(lstNodes, node => node.IsExpanded, true).ToList();

                //重置数据源
                var newSource = resetFunc.Invoke();

                //展开节点
                TreeDataGridHelper.TraverseDescendants(newSource, node =>
                {
                    if (lstExpandedNode.Any(n => compareFunc(n, node)))
                        node.IsExpanded = true;
                });
            }
        }
    }
}
