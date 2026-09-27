using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace SubspeciesEntry.BepInEx.Plants
{
    #region 大帝伴侣
    [HarmonyPatch(typeof(UltimateStarBlover))]
    public static class UltimateStarBloverPatch
    {
        [HarmonyPatch(nameof(UltimateStarBlover.StarsUpdate))]
        [HarmonyPrefix]
        public static void PreStarsUpdate(UltimateStarBlover __instance)
        {
            if (CoreTools.TravelUltimate("流星雨"))
                __instance.radius = 1.2f;
            if (CoreTools.TravelUltimate("众星之力"))
                __instance.maxBullets = int.MaxValue;
            if (__instance.starBullets == null) return;
            if (CoreTools.TravelUltimate("流星雨"))
            {
                // starBullets 是 Il2Cpp 的 List<T>，4.0 下用 Count 而不是数组的 Length（CS1061）
                for (int i = 0; i < __instance.starBullets.Count; i++)
                {
                    var star = __instance.starBullets[i];
                    if (star == null || star.IsDestroyed()) continue;
                    star.accelerateTime += Time.deltaTime * 2;
                }
            }
        }

        [HarmonyPatch(nameof(UltimateStarBlover.StarsUpdate))]
        [HarmonyPostfix]
        public static void PostStarsUpdate(UltimateStarBlover __instance)
        {
            if (CoreTools.TravelUltimate("斗转星移") && CoreTools.TravelUltimate("众星之力"))
            {
                foreach (var (_, plant) in __instance.board.boardStatistics.plantDetails)
                {
                    if (plant.plantType == PlantType.StarPumpkin)
                    {
                        __instance.maxBullets = 360 + 10 * Lawnf.GetPlantCount(PlantType.UltimateStar, __instance.board);
                        return;
                    }
                }
                __instance.maxBullets = 90 + 10 * Lawnf.GetPlantCount(PlantType.UltimateStar, __instance.board);
            }
        }
    }

    [HarmonyPatch(typeof(StarBlover))]
    public static class StarBloverPatch
    {
        [HarmonyPatch(nameof(StarBlover.RemoveNode))]
        [HarmonyPostfix]
        public static void PostRemoveNode(StarBlover __instance, ref Bullet_star starBullet)
        {
            if (__instance.thePlantType == PlantType.UltimateBlover)
            {
                if (CoreTools.TravelUltimate("众星之力"))
                {
                    starBullet.Damage *= (CoreTools.TravelUltimateLevel("众星之力") == 2) ? 3 : 2;
                }
            }
        }

        [HarmonyPatch(nameof(StarBlover.StarsUpdate))]
        [HarmonyPostfix]
        public static void PostStarsUpdate(StarBlover __instance)
        {
            if (Lawnf.TravelAdvanced((AdvBuff)19))
            {
                foreach (var (_, plant) in __instance.board.boardStatistics.plantDetails)
                {
                    if (plant.plantType == PlantType.StarPumpkin)
                    {
                        __instance.maxBullets = 120 + 10 * Lawnf.GetPlantCount(PlantType.UltimateStar, __instance.board);
                        return;
                    }
                }
                __instance.maxBullets = 30 + 10 * Lawnf.GetPlantCount(PlantType.UltimateStar, __instance.board);
            }
        }
    }
    #endregion
}
