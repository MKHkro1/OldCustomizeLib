using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Extensions
{
    internal static class DictionaryExtensions
    {
        /// <summary>
        /// <para>向 <paramref name="dictionary"/> 里添加键值对 (<paramref name="key"/>, <paramref name="value"/>)</para>
        /// <para>若 <paramref name="dictionary"/> 里已存在键 <paramref name="key"/> 则 向控制台输出 <paramref name="message"/></para>
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dictionary">目标字典</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="message">重复消息</param>
        /// <exception cref="ArgumentNullException">当 <paramref name="dictionary"/> 为 null 时抛出</exception>
        internal static void AddAndLogIfDup<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value, string message)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
            if (!dictionary.ContainsKey(key)) dictionary.Add(key, value);
            else Logger.LogError(message);
        }
    }
}
