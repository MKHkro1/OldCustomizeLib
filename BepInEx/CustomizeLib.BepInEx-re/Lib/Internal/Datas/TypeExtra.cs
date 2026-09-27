using CustomizeLib.BepInEx.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Datas
{
    #region TypeExtra
    internal struct TypeExtra
    {
        #region 数据
        /// <summary>
        /// 植物类型额外数据
        /// </summary>
        internal Dictionary<PlantType, HashSet<string>> CustomPlantTypeDatas = [];

        /// <summary>
        /// 僵尸类型额外数据
        /// </summary>
        internal Dictionary<ZombieType, HashSet<string>> CustomZombieTypeDatas = [];
        #endregion

        /// <summary>
        /// 添加植物Tag
        /// </summary>
        /// <param name="plantType">植物类型</param>
        /// <param name="tag">目标Tag</param>
        internal readonly void AddTag(PlantType plantType, string tag) =>
            CustomPlantTypeDatas[plantType] = [.. CustomPlantTypeDatas.GetValueOrDefault(plantType, []), .. tag.ToIEnumerable()];

        /// <summary>
        /// 移除植物Tag
        /// </summary>
        /// <param name="plantType">植物类型</param>
        /// <param name="tag">目标Tag</param>
        internal readonly void RemoveTag(PlantType plantType, string tag) =>
            CustomPlantTypeDatas.GetValueOrDefault(plantType, []).Remove(tag);

        /// <summary>
        /// 添加僵尸Tag
        /// </summary>
        /// <param name="zombieType">僵尸类型</param>
        /// <param name="tag">目标Tag</param>
        internal readonly void AddTag(ZombieType zombieType, string tag) =>
            CustomZombieTypeDatas[zombieType] = [.. CustomZombieTypeDatas.GetValueOrDefault(zombieType, []), .. tag.ToIEnumerable()];

        /// <summary>
        /// 移除僵尸Tag
        /// </summary>
        /// <param name="zombieType">僵尸类型</param>
        /// <param name="tag">目标Tag</param>
        internal readonly void RemoveTag(ZombieType zombieType, string tag) =>
            CustomZombieTypeDatas.GetValueOrDefault(zombieType, []).Remove(tag);

        public TypeExtra() { }
    }
    #endregion
}
