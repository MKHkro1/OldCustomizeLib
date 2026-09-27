using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx.Internal.Tools;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.LowLevel;

namespace CustomizeLib.BepInEx
{
    // 对外工具类的封装
    public static class Tools
    {
        #region TypeTools
        internal static BindingFlags DefaultFlag => InternalTools.TypeTools_DefaultFlag;
        internal static BindingFlags DefaultDeclaredOnly => InternalTools.TypeTools_DefaultDeclaredOnly;

        /// <summary>
        /// 获取当前加载的所有程序集
        /// </summary>
        /// <returns>所有程序集</returns>
        internal static Assembly[] GetAllAssemblies() => InternalTools.TypeTools_GetAllAssemblies();

        /// <summary>
        /// 获取当前已加载的程序集中所有类型
        /// </summary>
        /// <returns>所有类型</returns>
        internal static Type[] GetAllTypes() => InternalTools.TypeTools_GetAllTypes(InternalTools.TypeTools_GetAllAssemblies());

        /// <summary>
        /// 获取程序集内的所有类型
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <returns>所有类型</returns>
        internal static Type[] GetAllTypes(Assembly[] assemblies) => InternalTools.TypeTools_GetAllTypes(assemblies);

        /// <summary>
        /// 获取程序集内的所有类型
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <returns>所有类型</returns>
        internal static Type[] GetAllTypes(Assembly assembly) => InternalTools.TypeTools_GetAllTypes(assembly);

        /// <summary>
        /// 判断 <paramref name="type"/> 是否是 <paramref name="baseType"/> 的子类
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="baseType">基类</param>
        /// <param name="containBase">是否包含基类</param>
        /// <returns><paramref name="type"/> 是否是 <paramref name="baseType"/> 的子类</returns>
        internal static bool IsSubTypeOf(Type type, Type baseType, bool containBase = true) => InternalTools.TypeTools_IsSubTypeOf(type, baseType, containBase);

        /// <summary>
        /// 获取所有派生类型
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="baseType">基类</param>
        /// <param name="containBase">是否包含基类</param>
        /// <returns>所有派生类</returns>
        internal static Type[] GetAllDerviedTypes(Assembly assembly, Type baseType, bool containBase = true) =>
            InternalTools.TypeTools_GetAllDerivedTypes([assembly], baseType, containBase);

        /// <summary>
        /// 获取所有派生类型
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <param name="baseType">基类</param>
        /// <param name="containBase">是否包含基类</param>
        /// <returns>所有派生类</returns>
        internal static Type[] GetAllDerviedTypes(Assembly[] assemblies, Type baseType, bool containBase = true) =>
            InternalTools.TypeTools_GetAllDerivedTypes(assemblies, baseType, containBase);

        /// <summary>
        /// 获取所有派生类型
        /// </summary>
        /// <typeparam name="TBase">基类</typeparam>
        /// <param name="assembly">程序集</param>
        /// <param name="containBase">是否包含基类</param>
        /// <returns>所有派生类</returns>
        internal static Type[] GetAllDerviedTypes<TBase>(Assembly assembly, bool containBase = true) =>
            InternalTools.TypeTools_GetAllDerivedTypes([assembly], typeof(TBase), containBase);

        /// <summary>
        /// 获取所有派生类型
        /// </summary>
        /// <typeparam name="TBase">基类</typeparam>
        /// <param name="assemblies">程序集</param>
        /// <param name="containBase">是否包含基类</param>
        /// <returns>所有派生类</returns>
        internal static Type[] GetAllDerviedTypes<TBase>(Assembly[] assemblies, bool containBase = true) =>
            InternalTools.TypeTools_GetAllDerivedTypes(assemblies, typeof(TBase), containBase);

        /// <summary>
        /// 获取所有派生类型 (在 <paramref name="baseType"/>.Assembly 里搜索)
        /// </summary>
        /// <param name="baseType">基类</param>
        /// <param name="containBase">是否包含基类</param>
        /// <returns>所有派生类</returns>
        internal static Type[] GetAllDerviedTypes(Type baseType, bool containBase = true) =>
            InternalTools.TypeTools_GetAllDerivedTypes([baseType.Assembly], baseType, containBase);

        /// <summary>
        /// 获取所有派生类型 (在 <typeparamref name="TBase"/>.Assembly 里搜索)
        /// </summary>
        /// <typeparam name="TBase">基类</typeparam>
        /// <param name="containBase">是否包含基类</param>
        /// <returns>所有派生类</returns>
        internal static Type[] GetAllDerviedTypes<TBase>(bool containBase = true) =>
            InternalTools.TypeTools_GetAllDerivedTypes([typeof(TBase).Assembly], typeof(TBase), containBase);

        /// <inheritdoc cref="InternalTools.TypeTools_GetAllMethods(Type[], BindingFlags)"/>
        internal static MethodInfo[] GetAllMethods(Type[] types, BindingFlags flags) =>
            InternalTools.TypeTools_GetAllMethods(types, flags);

        /// <inheritdoc cref="InternalTools.TypeTools_GetAllMethods(Type[])"/>
        internal static MethodInfo[] GetAllMethods(Type[] types) =>
            InternalTools.TypeTools_GetAllMethods(types);

        /// <inheritdoc cref="InternalTools.TypeTools_GetAllMethods(Type[], BindingFlags, Func{MethodInfo, bool})"/>
        internal static MethodInfo[] GetAllMethods(Type[] types, BindingFlags flags, Func<MethodInfo, bool> filter) =>
            InternalTools.TypeTools_GetAllMethods(types, flags, filter);

        ///<inheritdoc cref="InternalTools.TypeTools_GetAllMethods(Type[], Func{Type, ValueTuple{MethodInfo, bool}})"/>
        internal static MethodInfo[] GetAllMethods(Type[] types, Func<Type, (MethodInfo? info, bool filter)> filter) =>
            InternalTools.TypeTools_GetAllMethods(types, filter);

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="name">名称</param>
        /// <param name="flags">过滤器</param>
        /// <returns>方法, 如果没有获取到则为null</returns>
        internal static MethodInfo? TryGetMethod(Type type, string name, BindingFlags flags) => InternalTools.TypeTools_TryGetMethod(type, name, flags);

        /// <inheritdoc cref="InternalTools.TypeTools_TryGetMethodWithFlag(Type, string, BindingFlags)"/>
        internal static (MethodInfo? info, bool exist) TryGetMethodWithFlag(Type type, string name, BindingFlags flags) =>
            InternalTools.TypeTools_TryGetMethodWithFlag(type, name, flags);

        /// <summary>
        /// <paramref name="type"/> 里是否存在指定方法
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="name">名称</param>
        /// <param name="flags">过滤器</param>
        /// <returns>是否存在</returns>
        internal static bool HasMethod(Type type, string name, BindingFlags flags) => InternalTools.TypeTools_TryGetMethod(type, name, flags) != null;
        #endregion

        #region InternalTools
        /// <inheritdoc cref="InternalTools.PlayerLoopTools_GetIndex(PlayerLoopSystem, Func{PlayerLoopSystem, bool})"/>
        internal static List<int> GetIndex(PlayerLoopSystem root, Func<PlayerLoopSystem, bool> target) =>
            InternalTools.PlayerLoopTools_GetIndex(root, target);

        /// <inheritdoc cref="InternalTools.PlayerLoopTools_AccessIndex(PlayerLoopSystem, List{int})"/>
        internal static PlayerLoopSystem AccessIndex(PlayerLoopSystem root, List<int> idx) =>
            InternalTools.PlayerLoopTools_AccessIndex(root, idx);

        /// <inheritdoc cref="InternalTools.PlayerLoopTools_InsertSystem(PlayerLoopSystem, Func{PlayerLoopSystem, bool}, Func{PlayerLoopSystem})"/>
        internal static PlayerLoopSystem InsertSystem(PlayerLoopSystem root, Func<PlayerLoopSystem, bool> target, Func<PlayerLoopSystem> cons) =>
            InternalTools.PlayerLoopTools_InsertSystem(root, target, cons);

        /// <inheritdoc cref="InternalTools.PlayerLoopTools_AppendSystem(PlayerLoopSystem, Func{PlayerLoopSystem, bool}, Func{PlayerLoopSystem})"/>
        internal static PlayerLoopSystem AppendSystem(PlayerLoopSystem root, Func<PlayerLoopSystem, bool> target, Func<PlayerLoopSystem> cons) =>
            InternalTools.PlayerLoopTools_AppendSystem(root, target, cons);

        /// <inheritdoc cref="InternalTools.PlayerLoopTools_CheckByType(PlayerLoopSystem, Type)"/>
        internal static bool CheckByType(PlayerLoopSystem system, Type targetType) =>
            InternalTools.PlayerLoopTools_CheckByType(system, targetType);

        /// <inheritdoc cref="InternalTools.PlayerLoopTools_ConsLoopSystem(Type, Action)"/>
        internal static PlayerLoopSystem ConsLoopSystem(Type type, Action updateDelegate) =>
            InternalTools.PlayerLoopTools_ConsLoopSystem(type, updateDelegate);

        /// <inheritdoc cref="InternalTools.PlayerLoopTools_ConsLoopSystem(Type, Action, IEnumerable{PlayerLoopSystem})"/>
        internal static PlayerLoopSystem ConsLoopSystem(Type type, Action updateDelegate, IEnumerable<PlayerLoopSystem> subs) =>
            InternalTools.PlayerLoopTools_ConsLoopSystem(type, updateDelegate, subs);
        #endregion

        #region InternalTools
        /// <inheritdoc cref="InternalTools.ConsoleTools_SetOutputEncoding(Encoding)"/>
        internal static void SetOutputEncoding(Encoding encoding) =>
            InternalTools.ConsoleTools_SetOutputEncoding(encoding);

        /// <inheritdoc cref="InternalTools.ConsoleTools_SetOutputEncoding()"/>
        internal static void SetOutputEncoding() =>
            InternalTools.ConsoleTools_SetOutputEncoding();
        #endregion
    }

    /// <summary>
    /// 填好了基础通用代码的插件类
    /// </summary>
    public class CorePlugin : BasePlugin
    {
        internal static List<Action> OnLoadCallbacks = [];
        public static bool SkipRegister => false;

        public override void Load()
        {
            Console.OutputEncoding = Encoding.UTF8;
            if (!SkipRegister)
            {
                foreach (var type in InternalTools.TypeTools_GetAllTypes(GetType().Assembly).Where(t => InternalTools.TypeTools_IsSubTypeOf(t, typeof(Il2CppSystem.Object), false)))
                    ClassInjector.RegisterTypeInIl2Cpp(type);
            }
            OnLoadCallbacks.Add(OnLoad);
            Harmony.CreateAndPatchAll(GetType().Assembly);
        }

        public virtual void OnLoad() { }
    }
}
