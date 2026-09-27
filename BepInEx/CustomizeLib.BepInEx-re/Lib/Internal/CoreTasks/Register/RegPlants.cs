using CustomizeLib.BepInEx.Internal.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.CoreTasks.Register
{
    /// <summary>
    /// 注册二创植物
    /// </summary>
    internal struct RegPlants : IGameAppEvent
    {
        readonly IGameAppEvent.GameAppEvent IGameAppEvent.Filter => IGameAppEvent.GameAppEvent.PreLoadResources;

        readonly void IModTask.Do()
        {
            foreach (var (pt, data) in RegData.Instance.CustomPlants)
            {
                if (GameAPP.resourcesManager.allPlants.Contains(pt)) continue; // 如果已有则跳过

                GameAPP.resourcesManager.plantPrefabs[pt] = data.Prefab;
                GameAPP.resourcesManager.plantPrefabs[pt].tag = "Plant"; // 打tag
                GameAPP.resourcesManager.plantPreviews[pt] = data.Preview;
                GameAPP.resourcesManager.plantPreviews[pt].tag = "Preview"; // 打tag
                GameAPP.resourcesManager.allPlants.Add(pt);
                PlantDataManager.PlantData_Default.Add(pt, data.ToPlantData()); // 添加plantdata
            }

            foreach (var (a, b, res) in RegData.Instance.CustomFusions)
                MixData.AddOrderedRecipe(a, b, res);
        }
    }
}
