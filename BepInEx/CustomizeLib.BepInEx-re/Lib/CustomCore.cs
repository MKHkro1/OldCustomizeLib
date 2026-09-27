using CustomizeLib.BepInEx.Internal.Datas;
using CustomizeLib.BepInEx.Internal.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomizeLib.BepInEx
{
    public static class CustomCore
    {
        #region TypeMgrExtra
        /// <inheritdoc cref="RegData.AddTag(PlantType, string)"/>
        public static void AddTag(PlantType plantType, string tag) =>
            RegData.Instance.AddTag(plantType, tag);

        /// <summary>
        /// 添加植物Tag
        /// </summary>
        /// <param name="plantType">植物类型</param>
        /// <param name="tag">目标Tag</param>
        public static void AddTag(PlantType plantType, PlantTags tag) =>
            AddTag(plantType, tag.ToString());


        /// <inheritdoc cref="RegData.RemoveTag(PlantType, string)"/>
        public static void RemoveTag(PlantType plantType, string tag) =>
            RegData.Instance.RemoveTag(plantType, tag);

        /// <summary>
        /// 移除植物Tag
        /// </summary>
        /// <param name="plantType">植物类型</param>
        /// <param name="tag">目标Tag</param>
        public static void RemoveTag(PlantType plantType, PlantTags tag) =>
            RemoveTag(plantType, tag.ToString());


        /// <inheritdoc cref="RegData.AddTag(ZombieType, string)"/>
        public static void AddTag(ZombieType zombieType, string tag) =>
            RegData.Instance.AddTag(zombieType, tag);

        /// <summary>
        /// 添加僵尸Tag
        /// </summary>
        /// <param name="zombieType">僵尸类型</param>
        /// <param name="tag">目标Tag</param>
        public static void AddTag(ZombieType zombieType, ZombieTags tag) =>
            RegData.Instance.AddTag(zombieType, tag.ToString());

        /// <inheritdoc cref="TypeExtra.RemoveTag(ZombieType, string)"/>
        public static void RemoveTag(ZombieType zombieType, string tag) =>
            RegData.Instance.RemoveTag(zombieType, tag);

        /// <summary>
        /// 移除僵尸Tag
        /// </summary>
        /// <param name="zombieType">僵尸类型</param>
        /// <param name="tag">目标Tag</param>
        public static void RemoveTag(ZombieType zombieType, ZombieTags tag) =>
            RemoveTag(zombieType, tag.ToString());
        #endregion

        /// <inheritdoc cref="ModImpl.GetAssetBundle(Assembly, string)"/>
        public static AssetBundle GetAssetBundle(Assembly assembly, string name) => ModImpl.GetAssetBundle(assembly, name);

        /// <summary>
        /// 获取嵌入dll里的ab包 (调用方dll)
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>ab包</returns>
        /// <exception cref="ArgumentException"></exception>
        public static AssetBundle GetAssetBundle(string name) => ModImpl.GetAssetBundle(Assembly.GetCallingAssembly(), name);

        /// <inheritdoc cref="RegData.RegisterCustomPlant{TBase, TBehaviour}(CustomPlant)"/>
        public static void RegisterCustomPlant<TBase, TBehaviour>(CustomPlant customPlant) where TBase : Plant where TBehaviour : MonoBehaviour =>
            RegData.Instance.RegisterCustomPlant<TBase, TBehaviour>(customPlant);

        /// <inheritdoc cref="RegData.RegisterCustomPlant{TBase}(CustomPlant)"/>
        public static void RegisterCustomPlant<TBase>(CustomPlant customPlant) where TBase : Plant =>
            RegData.Instance.RegisterCustomPlant<TBase>(customPlant);
    }
}
