using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Extensions
{
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// 对 <paramref name="enumerable"/> 里的所有元素执行 <paramref name="action"/> 操作
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="enumerable">集合</param>
        /// <param name="action">操作</param>
        /// <returns>原集合</returns>
        internal static IEnumerable<T> DoAll<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable) action.Invoke(item);
            return enumerable;
        }
    }
}
