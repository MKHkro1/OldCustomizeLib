using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeLib.BepInEx.Internal.Extensions
{
    internal static class AssetBundleExtensions
    {
        /// <summary>
        /// 尝试从 <paramref name="ab"/> 里读取名称为 <paramref name="name"/> 且类型为 <typeparamref name="T"/> 的资源
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="ab">目标ab包</param>
        /// <param name="name">名称</param>
        /// <param name="obj">若成功, 则将资源复制到此参数, 否则为null</param>
        /// <returns>是否存在</returns>
        internal static bool TryGetAsset<T>(this AssetBundle ab, string name, out T? obj) where T : UnityEngine.Object
        {
            foreach (var ase in ab.LoadAllAssetsAsync().allAssets)
            {
                if (ase.TryCast<T>()?.name == name)
                {
                    obj = ase.Cast<T>();
                    return true;
                }
            }
            obj = null;
            return false;
        }

        /// <summary>
        /// 从 <paramref name="ab"/> 里读取名称为 <paramref name="name"/> 且类型为 <typeparamref name="T"/> 的资源
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="ab">目标ab包</param>
        /// <param name="name">名称</param>
        /// <returns>读取的资源</returns>
        /// <exception cref="ArgumentException">若不存在, 则抛出此异常</exception>
        internal static T GetAsset<T>(this AssetBundle ab, string name) where T : UnityEngine.Object
        {
            foreach (var ase in ab.LoadAllAssetsAsync().allAssets)
            {
                if (ase.TryCast<T>()?.name == name)
                {
                    return ase.Cast<T>();
                }
            }
            throw new ArgumentException($"Could not find {name} from {ab.name}");
        }

        /// <summary>
        /// 获取ab包里的所有资源
        /// </summary>
        /// <param name="ab">ab包</param>
        /// <returns>所有资源</returns>
        internal static UnityEngine.Object[] GetAllAssets(this AssetBundle ab) => ab.LoadAllAssetsAsync().allAssets;
    }
}
