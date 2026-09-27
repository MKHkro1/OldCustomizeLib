using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Extensions
{
    internal static class IntegerExtensions
    {
        /// <summary>
        /// 将 <paramref name="value"/> 转换为枚举类型 <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static T ToEnum<T>(this long value) where T : Enum =>
            (T)Enum.ToObject(typeof(T), value);

        /// <summary>
        /// 将 <paramref name="value"/> 转换为枚举类型 <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static T ToEnum<T>(this int value) where T : Enum => ToEnum<T>((long)value);
    }
}
