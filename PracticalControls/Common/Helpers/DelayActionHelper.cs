using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalControls.Common.Helpers
{
    public class DelayActionHelper
    {
        #region 使用条件的异步操作

        static Dictionary<string, object> dicLockedObj = new Dictionary<string, object>();
        static Dictionary<string, bool> dicDoingStatus = new Dictionary<string, bool>();

        /// <summary>
        /// 当满足某些条件时 执行某项操作
        /// </summary>
        /// <param name="action">需要执行的操作</param> 
        /// <param name="condition">退出等待的条件</param>
        public static async void DoActionWhenMeetCondition(string actionKey, Action action, Func<bool> condition)
        {
            if (!dicLockedObj.ContainsKey(actionKey))
                dicLockedObj.Add(actionKey, new object());
            if (!dicDoingStatus.ContainsKey(actionKey))
                dicDoingStatus.Add(actionKey, false);

            var lockedObj = dicLockedObj[actionKey];
            var isDoing = dicDoingStatus[actionKey];

            lock (lockedObj)
            {
                if (isDoing) return;
                else isDoing = true;
            }

            //等待加载完
            await Task.Run(() =>
            {
                while (!condition.Invoke())
                    System.Threading.Thread.Sleep(10);
            });

            //执行具体操作
            action.Invoke();

            isDoing = false;
        }

        #endregion
    }
}
