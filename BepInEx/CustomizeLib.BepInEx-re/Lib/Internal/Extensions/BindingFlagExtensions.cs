using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Extensions
{
    internal static class BindingFlagExtensions
    {
        /// <summary>
        /// 为 <paramref name="flags"/> 添加 <paramref name="add"/> 的 <see cref="BindingFlags"/>
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="add"></param>
        /// <returns>添加后的 <see cref="BindingFlags"/></returns>
        internal static BindingFlags AddFlags(this BindingFlags flags, BindingFlags add) => flags | add;

        // 为BindingFlags添加过滤器
        internal static BindingFlags AddPublic(this BindingFlags flags) => flags | BindingFlags.Public;
        internal static BindingFlags AddNonPublic(this BindingFlags flags) => flags | BindingFlags.NonPublic;
        internal static BindingFlags AddStatic(this  BindingFlags flags) => flags | BindingFlags.Static;
        internal static BindingFlags AddInstance(this  BindingFlags flags) => flags | BindingFlags.Instance;
        internal static BindingFlags AddDeclaredOnly(this BindingFlags flags) => flags | BindingFlags.DeclaredOnly;
    }
}
