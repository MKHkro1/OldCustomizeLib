using CustomizeLib.BepInEx.Internal.Events;
using CustomizeLib.BepInEx.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeLib.BepInEx.Extensions
{
    /// <summary>
    /// 通用扩展方法类, 封装了所有扩展方法
    /// </summary>
    public static class Extensions
    {
        #region BindingFlagExtensions
        /// <inheritdoc cref="BindingFlagExtensions.AddFlags(BindingFlags, BindingFlags)"/>
        public static BindingFlags AddFlags(this BindingFlags flags, BindingFlags add) => BindingFlagExtensions.AddFlags(flags, add);

        /// <inheritdoc cref="BindingFlagExtensions.AddPublic(BindingFlags)"/>
        public static BindingFlags AddPublic(this BindingFlags flags) => BindingFlagExtensions.AddPublic(flags);

        /// <inheritdoc cref="BindingFlagExtensions.AddNonPublic(BindingFlags)"/>
        internal static BindingFlags AddNonPublic(this BindingFlags flags) => BindingFlagExtensions.AddNonPublic(flags);

        /// <inheritdoc cref="BindingFlagExtensions.AddStatic(BindingFlags)"/>
        internal static BindingFlags AddStatic(this BindingFlags flags) => BindingFlagExtensions.AddStatic(flags);

        /// <inheritdoc cref="BindingFlagExtensions.AddInstance(BindingFlags)"/>
        internal static BindingFlags AddInstance(this BindingFlags flags) => BindingFlagExtensions.AddInstance(flags);

        /// <inheritdoc cref="BindingFlagExtensions.AddDeclaredOnly(BindingFlags)"/>
        internal static BindingFlags AddDeclaredOnly(this BindingFlags flags) => BindingFlagExtensions.AddDeclaredOnly(flags);
        #endregion

        #region AssetBundleExtensions
        /// <inheritdoc cref="AssetBundleExtensions.TryGetAsset{T}(AssetBundle, string, out T?)"/>
        internal static bool TryGetAsset<T>(this AssetBundle ab, string name, out T? obj) where T : UnityEngine.Object =>
            AssetBundleExtensions.TryGetAsset(ab, name, out obj);

        /// <inheritdoc cref="AssetBundleExtensions.GetAsset{T}(AssetBundle, string)"/>
        internal static T GetAsset<T>(this AssetBundle ab, string name) where T : UnityEngine.Object =>
            AssetBundleExtensions.GetAsset<T>(ab, name);

        /// <inheritdoc cref="AssetBundleExtensions.GetAllAssets(AssetBundle)"/>
        internal static UnityEngine.Object[] GetAllAssets(this AssetBundle ab) => AssetBundleExtensions.GetAllAssets(ab);
        #endregion

        #region IntegerExtensions
        /// <inheritdoc cref="IntegerExtensions.ToEnum{T}(long)"/>
        public static T ToEnum<T>(this long value) where T : Enum => IntegerExtensions.ToEnum<T>(value);

        /// <inheritdoc cref="IntegerExtensions.ToEnum{T}(int)"/>
        public static T ToEnum<T>(this int value) where T : Enum => IntegerExtensions.ToEnum<T>(value);
        #endregion

        #region DictionaryExtensions
        /// <inheritdoc cref="Internal.Extensions.DictionaryExtensions.AddAndLogIfDup{TKey, TValue}(IDictionary{TKey, TValue}, TKey, TValue, string)"/>
        public static void AddAndLogIfDup<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value, string message) =>
            Internal.Extensions.DictionaryExtensions.AddAndLogIfDup(dictionary, key, value, message);
        #endregion

        #region ObjectExtensions
        /// <inheritdoc cref="ObjectExtensions.ToIEnumerable{T}(T)"/>
        public static IEnumerable<T> ToIEnumerable<T>(this T item) =>
            ObjectExtensions.ToIEnumerable(item);

        /// <inheritdoc cref="ObjectExtensions.To{T}(object)"/>
        internal static T To<T>(this object obj) =>
            ObjectExtensions.To<T>(obj);
        #endregion

        #region IPlantEventExtensions
        /// <inheritdoc cref="IPlantEventExtensions.Register"/>
        internal static void Register(this IPlantEvent plantEvent) => IPlantEventExtensions.Register(plantEvent);

        /// <inheritdoc cref="IPlantEventExtensions.Unregister"/>
        internal static void Unregister(this IPlantEvent plantEvent) => IPlantEventExtensions.Unregister(plantEvent);

        /// <inheritdoc cref="IPlantEventExtensions.TryGetBindEvents(object, out List{IPlantEvent})"/>
        internal static bool TryGetBindEvents(this object obj, out List<IPlantEvent> plantEvents) =>
            IPlantEventExtensions.TryGetBindEvents(obj, out plantEvents);

        /// <inheritdoc cref="IPlantEventExtensions.Bind(object)"/>
        internal static void Bind(this IPlantEvent plantEvent, object obj) =>
            IPlantEventExtensions.Bind(plantEvent, obj);
        #endregion
    }
}
