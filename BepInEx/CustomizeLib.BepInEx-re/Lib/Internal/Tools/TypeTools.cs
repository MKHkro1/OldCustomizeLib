using CustomizeLib.BepInEx.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Tools
{
    internal static class TypeTools
    {
        private static readonly Lazy<BindingFlags> _Default = new(() => BindingFlags.Default.AddPublic().AddNonPublic().AddStatic().AddInstance());
        /// <summary>
        /// 默认查找的 <see cref="BindingFlags"/> (Public/NonPublic/Static/Instance)
        /// </summary>
        internal static BindingFlags DefaultFlag => _Default.Value;

        /// <summary>
        /// 默认查找的 <see cref="BindingFlags"/> (<see cref="DefaultFlag"/> / DeclaredOnly)
        /// </summary>
        internal static BindingFlags DefaultDeclaredOnly => _Default.Value | BindingFlags.DeclaredOnly;

        /// <summary>
        /// 获取当前加载的所有程序集
        /// </summary>
        /// <returns>所有程序集</returns>
        internal static Assembly[] GetAllAssemblies() => AppDomain.CurrentDomain.GetAssemblies();

        /// <summary>
        /// 获取程序集内的所有类型
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <returns>所有类型</returns>
        internal static Type[] GetAllTypes(Assembly[] assemblies)
        {
            var res = new List<Type>();
            foreach (var assembly in assemblies)
                res.AddRange(GetAllTypes(assembly));
            return [.. res];
        }

        /// <summary>
        /// 获取程序集内的所有类型
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <returns>所有类型</returns>
        internal static Type[] GetAllTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                if (ex.Types != null)
                    return [.. ex.Types.Where(t => t != null).Select(t => t!)];
                return [];
            }
        }

        /// <summary>
        /// 判断 <paramref name="type"/> 是否是 <paramref name="baseType"/> 的子类
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="baseType">基类</param>
        /// <param name="containBase">是否包含基类</param>
        /// <returns><paramref name="type"/> 是否是 <paramref name="baseType"/> 的子类</returns>
        internal static bool IsSubTypeOf(Type type, Type baseType, bool containBase = true)
        {
            if (type == null || baseType == null) return false;
            if (baseType.IsGenericTypeDefinition)
            {
                bool Matches(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == baseType;

                if (Matches(type))
                {
                    if (type.IsGenericTypeDefinition && type == baseType)
                        return containBase;
                    return true;
                }

                if (baseType.IsInterface)
                {
                    foreach (var iface in type.GetInterfaces())
                    {
                        if (Matches(iface))
                            return true;
                    }
                    return false;
                }
                else
                {
                    var iter = type.BaseType;
                    while (iter != null && iter != typeof(object))
                    {
                        if (Matches(iter))
                            return true;
                        iter = iter.BaseType;
                    }
                    return false;
                }
            }
            else
            {
                if (containBase)
                    return baseType.IsAssignableFrom(type);
                else
                    return baseType.IsAssignableFrom(type) && type != baseType;
            }
        }

        /// <summary>
        /// 获取所有派生类型
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="baseType">基类</param>
        /// <param name="containBase">是否包含基类</param>
        /// <returns>所有派生类</returns>
        internal static Type[] GetAllDerivedTypes(Assembly[] assemblies, Type baseType, bool containBase = true) =>
            [.. GetAllTypes(assemblies).Where(t => IsSubTypeOf(t, baseType, containBase))];

        /// <summary>
        /// 获取 <paramref name="types"/> 里的每个 <see cref="Type"/> 的方法
        /// </summary>
        /// <param name="types">类型</param>
        /// <param name="flags"><see cref="BindingFlags"/> 过滤器</param>
        /// <returns>所有方法</returns>
        internal static MethodInfo[] GetAllMethods(Type[] types, BindingFlags flags)
        {
            var res = new List<MethodInfo>();
            foreach (var t in types) res.AddRange(t.GetMethods(flags));
            return [.. res];
        }

        /// <summary>
        /// 获取 <paramref name="types"/> 里的每个 <see cref="Type"/> 的方法 (按默认搜索条件)
        /// </summary>
        /// <param name="types"></param>
        /// <returns>所有方法</returns>
        internal static MethodInfo[] GetAllMethods(Type[] types) =>
            GetAllMethods(types, DefaultFlag);

        /// <summary>
        /// 获取 <paramref name="types"/> 里所有符合 <paramref name="filter"/> 的方法
        /// </summary>
        /// <param name="types">类型</param>
        /// <param name="filter">过滤器</param>
        /// <returns>符合条件的方法</returns>
        internal static MethodInfo[] GetAllMethods(Type[] types, BindingFlags flags, Func<MethodInfo, bool> filter) =>
            [.. GetAllMethods(types, flags).Where(filter.Invoke)];

        /// <summary>
        /// 获取 <paramref name="types"/> 里所有符合 <paramref name="filter"/> 的方法
        /// </summary>
        /// <param name="types">类型</param>
        /// <param name="filter">过滤器</param>
        /// <returns>符合条件的方法</returns>
        internal static MethodInfo[] GetAllMethods(Type[] types, Func<Type, (MethodInfo? info, bool exist)> filter) =>
            [.. types.Where(t => filter.Invoke(t).exist).Select(t => filter(t).info!)];

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="name">名字</param>
        /// <param name="flags">过滤器</param>
        /// <returns>(方法, 是否存在)</returns>
        internal static (MethodInfo? info, bool exist) TryGetMethodWithFlag(Type type, string name, BindingFlags flags)
        {
            var method = TryGetMethod(type, name, flags);
            return (method, method != null);
        }

        internal static MethodInfo? TryGetMethod(Type type, string name, BindingFlags flags) => type.GetMethod(name, flags);
        internal static bool HasMethod(Type type, string name, BindingFlags flags) => TryGetMethod(type, name, flags) != null;
    }
}
