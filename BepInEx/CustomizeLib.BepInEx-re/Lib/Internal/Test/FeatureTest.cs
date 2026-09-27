using CustomizeLib.BepInEx.Internal.Datas;
using CustomizeLib.BepInEx.Internal.Events;
using CustomizeLib.BepInEx.Internal.Extensions;
using CustomizeLib.BepInEx.Internal.Tools;
using Cysharp.Threading.Tasks;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace CustomizeLib.BepInEx.Internal.Test
{
    internal struct FeatureTest : ILoadTask
    {
        readonly void IModTask.Do()
        {
            CustomCore.AddTag(PlantType.SunFlower, PlantTags.IsWaterPlant);
            CustomCore.RemoveTag(PlantType.SunFlower, PlantTags.IsWaterPlant);
            _ = new Test().Register().To<Test>().Cons();
        }

        private class Test : IPlantEvent
        {
            public async Task Cons()
            {
                while (true)
                {
                    // interop 版 UniTask 的 Awaiter 不实现 INotifyCompletion（CS4027），改用 Task.Yield()
                    await Task.Yield();
                    if (Board.Instance != null && Board.Instance.boardEntity.plantArray.Count > 0)
                    {
                        this.Bind(Lawnf.GetAllPlants()[0]);
                        return;
                    }
                }
            }

            void IPlantEvent.DieEvent(Trigger trigger, Plant.DieReason reason)
            {
                Logger.LogInfo($"plant die {trigger}, {reason}");
                this.Unregister();
            }
        }
    }
}
