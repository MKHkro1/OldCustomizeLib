using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.LowLevel;

namespace CustomizeLib.BepInEx.Internal.Tools
{
    // 对Tools里的工具的封装
    internal static class InternalTools
    {
        #region TypeTools
        /// <inheritdoc cref="TypeTools.DefaultFlag"/>
        internal static BindingFlags TypeTools_DefaultFlag => TypeTools.DefaultFlag;

        /// <inheritdoc cref="TypeTools.DefaultDeclaredOnly"/>
        internal static BindingFlags TypeTools_DefaultDeclaredOnly => TypeTools.DefaultDeclaredOnly;

        /// <summary>
        /// 获取当前加载的所有程序集
        /// </summary>
        /// <returns>所有程序集</returns>
        internal static Assembly[] TypeTools_GetAllAssemblies() => TypeTools.GetAllAssemblies();

        /// <summary>
        /// 获取当前已加载的程序集中所有类型
        /// </summary>
        /// <returns>所有类型</returns>
        internal static Type[] TypeTools_GetAllTypes() => TypeTools.GetAllTypes(TypeTools.GetAllAssemblies());

        /// <summary>
        /// 获取程序集内的所有类型
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <returns>所有类型</returns>
        internal static Type[] TypeTools_GetAllTypes(Assembly[] assemblies) => TypeTools.GetAllTypes(assemblies);

        /// <summary>
        /// 获取程序集内的所有类型
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <returns>所有类型</returns>
        internal static Type[] TypeTools_GetAllTypes(Assembly assembly) => TypeTools.GetAllTypes(assembly);

        /// <summary>
        /// 判断 <paramref name="type"/> 是否是 <paramref name="baseType"/> 的子类
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="baseType">基类</param>
        /// <param name="containBase">是否包含基类</param>
        /// <returns><paramref name="type"/> 是否是 <paramref name="baseType"/> 的子类</returns>
        internal static bool TypeTools_IsSubTypeOf(Type type, Type baseType, bool containBase = true) => TypeTools.IsSubTypeOf(type, baseType, containBase);
        
        /// <summary>
        /// 获取所有派生类型
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="baseType">基类</param>
        /// <param name="containBase">是否包含基类</param>
        /// <returns>所有派生类</returns>
        internal static Type[] TypeTools_GetAllDerivedTypes(Assembly assembly, Type baseType, bool containBase = true) =>
            TypeTools.GetAllDerivedTypes([assembly], baseType, containBase);

        /// <summary>
        /// 获取所有派生类型
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="baseType">基类</param>
        /// <param name="containBase">是否包含基类</param>
        /// <returns>所有派生类</returns>
        internal static Type[] TypeTools_GetAllDerivedTypes(Assembly[] assemblies, Type baseType, bool containBase = true) =>
            TypeTools.GetAllDerivedTypes(assemblies, baseType, containBase);
        
        /// <summary>
        /// 获取所有派生类型
        /// </summary>
        /// <typeparam name="TBase">基类</typeparam>
        /// <param name="assembly">程序集</param>
        /// <param name="containBase">是否包含基类</param>
        /// <returns>所有派生类</returns>
        internal static Type[] TypeTools_GetAllDerivedTypes<TBase>(Assembly assembly, bool containBase = true) =>
            TypeTools.GetAllDerivedTypes([assembly], typeof(TBase), containBase);

        /// <summary>
        /// 获取所有派生类型
        /// </summary>
        /// <typeparam name="TBase">基类</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <param name="containBase">是否包含基类</param>
        /// <returns>所有派生类</returns>
        internal static Type[] TypeTools_GetAllDerivedTypes<TBase>(Assembly[] assemblies, bool containBase = true) =>
            TypeTools.GetAllDerivedTypes(assemblies, typeof(TBase), containBase);

        /// <summary>
        /// 获取所有派生类型 (在 <paramref name="baseType"/>.Assembly 里搜索)
        /// </summary>
        /// <param name="baseType">基类</param>
        /// <param name="containBase">是否包含基类</param>
        /// <returns>所有派生类</returns>
        internal static Type[] TypeTools_GetAllDerivedTypes(Type baseType, bool containBase = true) =>
            TypeTools.GetAllDerivedTypes([baseType.Assembly], baseType, containBase);

        /// <summary>
        /// 获取所有派生类型 (在 <typeparamref name="TBase"/>.Assembly 里搜索)
        /// </summary>
        /// <typeparam name="TBase">基类</typeparam>
        /// <param name="containBase">是否包含基类</param>
        /// <returns>所有派生类</returns>
        internal static Type[] TypeTools_GetAllDerivedTypes<TBase>(bool containBase = true) =>
            TypeTools.GetAllDerivedTypes([typeof(TBase).Assembly], typeof(TBase), containBase);

        /// <inheritdoc cref="TypeTools.GetAllMethods(Type[], BindingFlags)"/>
        internal static MethodInfo[] TypeTools_GetAllMethods(Type[] types, BindingFlags flags) =>
            TypeTools.GetAllMethods(types, flags);

        /// <inheritdoc cref="TypeTools.GetAllMethods(Type[])"/>
        internal static MethodInfo[] TypeTools_GetAllMethods(Type[] types) =>
            TypeTools.GetAllMethods(types);

        /// <inheritdoc cref="TypeTools.GetAllMethods(Type[], BindingFlags, Func{MethodInfo, bool})"/>
        internal static MethodInfo[] TypeTools_GetAllMethods(Type[] types, BindingFlags flags, Func<MethodInfo, bool> filter) =>
            TypeTools.GetAllMethods(types, flags, filter);

        ///<inheritdoc cref="TypeTools.GetAllMethods(Type[], Func{Type, ValueTuple{MethodInfo, bool}})"/>
        internal static MethodInfo[] TypeTools_GetAllMethods(Type[] types, Func<Type, (MethodInfo? info, bool filter)> filter) =>
            TypeTools.GetAllMethods(types, filter);

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="name">名称</param>
        /// <param name="flags">过滤器</param>
        /// <returns>方法, 如果没有获取到则为null</returns>
        internal static MethodInfo? TypeTools_TryGetMethod(Type type, string name, BindingFlags flags) => TypeTools.TryGetMethod(type, name, flags);

        /// <inheritdoc cref="TypeTools.TryGetMethodWithFlag(Type, string, BindingFlags)"/>
        internal static (MethodInfo? info, bool exist) TypeTools_TryGetMethodWithFlag(Type type, string name, BindingFlags flags) =>
            TypeTools.TryGetMethodWithFlag(type, name, flags);

        /// <summary>
        /// <paramref name="type"/> 里是否存在指定方法
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="name">名称</param>
        /// <param name="flags">过滤器</param>
        /// <returns>是否存在</returns>
        internal static bool TypeTools_HasMethod(Type type, string name, BindingFlags flags) => TypeTools.TryGetMethod(type, name, flags) != null;
        #endregion

        #region PlayerLoopTools
        /// <inheritdoc cref="PlayerLoopTools.GetIndex(PlayerLoopSystem, Func{PlayerLoopSystem, bool})"/>
        internal static List<int> PlayerLoopTools_GetIndex(PlayerLoopSystem root, Func<PlayerLoopSystem, bool> target) =>
            PlayerLoopTools.GetIndex(root, target);

        /// <inheritdoc cref="PlayerLoopTools.AccessIndex(PlayerLoopSystem, List{int})"/>
        internal static PlayerLoopSystem PlayerLoopTools_AccessIndex(PlayerLoopSystem root, List<int> idx) =>
            PlayerLoopTools.AccessIndex(root, idx);

        /// <inheritdoc cref="PlayerLoopTools.InsertSystem(PlayerLoopSystem, Func{PlayerLoopSystem, bool}, Func{PlayerLoopSystem})"/>
        internal static PlayerLoopSystem PlayerLoopTools_InsertSystem(PlayerLoopSystem root, Func<PlayerLoopSystem, bool> target, Func<PlayerLoopSystem> cons) =>
            PlayerLoopTools.InsertSystem(root, target, cons);

        /// <inheritdoc cref="PlayerLoopTools.AppendSystem(PlayerLoopSystem, Func{PlayerLoopSystem, bool}, Func{PlayerLoopSystem})"/>
        internal static PlayerLoopSystem PlayerLoopTools_AppendSystem(PlayerLoopSystem root, Func<PlayerLoopSystem, bool> target, Func<PlayerLoopSystem> cons) =>
            PlayerLoopTools.AppendSystem(root, target, cons);

        /// <inheritdoc cref="PlayerLoopTools.CheckByType(PlayerLoopSystem, Type)"/>
        internal static bool PlayerLoopTools_CheckByType(PlayerLoopSystem system, Type targetType) =>
            PlayerLoopTools.CheckByType(system, targetType);

        /// <inheritdoc cref="PlayerLoopTools.ConsLoopSystem(Type, Action)"/>
        internal static PlayerLoopSystem PlayerLoopTools_ConsLoopSystem(Type type, Action updateDelegate) =>
            PlayerLoopTools.ConsLoopSystem(type, updateDelegate);

        /// <inheritdoc cref="PlayerLoopTools.ConsLoopSystem(Type, Action, IEnumerable{PlayerLoopSystem})"/>
        internal static PlayerLoopSystem PlayerLoopTools_ConsLoopSystem(Type type, Action updateDelegate, IEnumerable<PlayerLoopSystem> subs) =>
            PlayerLoopTools.ConsLoopSystem(type, updateDelegate, subs);
        #endregion

        #region ConsoleTools
        /// <inheritdoc cref="ConsoleTools.SetOutputEncoding(Encoding)"/>
        internal static void ConsoleTools_SetOutputEncoding(Encoding encoding) =>
            ConsoleTools.SetOutputEncoding(encoding);

        /// <inheritdoc cref="ConsoleTools.SetOutputEncoding()"/>
        internal static void ConsoleTools_SetOutputEncoding() =>
            ConsoleTools.SetOutputEncoding();
        #endregion
    }
}
