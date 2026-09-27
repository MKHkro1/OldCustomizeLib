using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SubspeciesEntry.BepInEx.Plants
{
    #region 血月
    [HarmonyPatch(typeof(Lunar))]
    public static class LunarPatch
    {
        [HarmonyPatch(nameof(Lunar.SummonUpdate))]
        [HarmonyPrefix]
        public static void PreSummonUpdate(Lunar __instance)
        {
            if (CoreTools.TravelUltimate("人造太阳"))
                __instance.summonTimer -= 2 * Time.deltaTime;
        }

        [HarmonyPatch(nameof(Lunar.Init))]
        [HarmonyPostfix]
        public static void PostInit(Lunar __instance)
        {
            if (CoreTools.TravelUltimate("人造太阳"))
                __instance.lifeTimer *= 3;
        }
    }

    [HarmonyPatch(typeof(Lawnf))]
    public static class LawnfPatch
    {
        [HarmonyPatch(nameof(Lawnf.GetPlantCount), new Type[] { typeof(PlantType), typeof(Board) })]
        [HarmonyPostfix]
        public static void PostGetPlantCount(ref PlantType theSeedType, ref int __result)
        {
            var callByUpdate = false;
            for (int i = 0; i < new StackTrace()?.FrameCount; i++)
            {
                if (new StackTrace()?.GetFrame(i)?.GetMethod()?.Name == "DMD<Lunar::Update>" && new StackTrace()?.GetFrame(i)?.GetMethod()?.GetParameters().Length == 1)
                    callByUpdate = true;
            }
            if (CoreTools.TravelUltimate("金光闪闪") && theSeedType == PlantType.UltimateRedLunar && callByUpdate)
            {
                var maxLevel = Lawnf.GetAllPlants().ToArray().ToList().Where(plant => plant.thePlantType == PlantType.UltimateRedLunar).Max(plant => plant.currentLightLevel);
                maxLevel = Mathf.Min(maxLevel, 20);
                __result += maxLevel;
            }
        }
    }
    #endregion
}
