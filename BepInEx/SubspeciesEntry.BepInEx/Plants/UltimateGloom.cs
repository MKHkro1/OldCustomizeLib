using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace SubspeciesEntry.BepInEx.Plants
{
    #region 曾哥&牢灯
    // 曾哥part
    [HarmonyPatch(typeof(IceDoomGloom))]
    public static class IceDoomGloomPatch
    {
        [HarmonyPatch(nameof(IceDoomGloom.DieEvent))]
        [HarmonyPrefix]
        public static bool PostDieEvent(IceDoomGloom __instance, Plant.DieReason reason)
        {
            if (CoreTools.TravelUltimate("万籁俱寂") && reason != Plant.DieReason.ByMix)
            {
                __instance.board.boardAction.SetDoom(__instance.thePlantColumn, __instance.thePlantRow, false, true, damage: 1000_0000, effect: 3, fromType: __instance.thePlantType);
                foreach (var zombie in Lawnf.GetAllZombies())
                {
                    if (zombie == null || zombie.IsDestroyed() || zombie.gameObject == null || zombie.gameObject.IsDestroyed()) continue;
                    zombie.TakeDamage((int)(zombie.CurrentAllHealth * 0.5f), __instance.Cast<IDamageMaker>(), DamageType.Carred, PlantType.UltimateGloom);
                }
                return false;
            }
            return true;
        }

        [HarmonyPatch(nameof(IceDoomGloom.TakeDamage))]
        [HarmonyPrefix]
        public static bool PreTakeDamage(IceDoomGloom __instance, ref DamageType damageType, ref int damage)
        {
            if (CoreTools.TravelUltimate("以爆制爆"))
            {
                __instance.power += damage;
                var data = __instance.GetOrAddComponent<DataSave>();
                data.SetData("UltimateGloom_TotalDamage", data.GetData<int>("UltimateGloom_TotalDamage") + damage);
                if (__instance.power >= 10000)
                {
                    __instance.board.boardAction.SetDoom(__instance.thePlantColumn, __instance.thePlantRow,
                        false, false, damage: (int)(data.GetData<int>("UltimateGloom_TotalDamage") / 3.8f), fromType: __instance.thePlantType);

                    __instance.power = 0;
                }

                if (damageType != DamageType.Normal)
                    return false;
            }
            return true;
        }
    }

    // 牢灯part
    [HarmonyPatch(typeof(UltimatePlantern))]
    public static class UltimatePlanternPatch
    {
        [HarmonyPatch(nameof(UltimatePlantern.DieEvent))]
        [HarmonyPostfix]
        public static void PostDieEvent(UltimatePlantern __instance, Plant.DieReason reason)
        {
            if (CoreTools.TravelUltimate("万籁俱寂") && reason != Plant.DieReason.ByMix)
            {
                foreach (var zombie in Lawnf.GetAllZombies())
                {
                    if (zombie == null || zombie.IsDestroyed() || zombie.gameObject == null || zombie.gameObject.IsDestroyed()) continue;
                    zombie.freezeMaxLevel = 100;
                }
            }
        }

        [HarmonyPatch(nameof(UltimatePlantern.TakeDamage))]
        [HarmonyPrefix]
        public static bool PreTakeDamage(UltimatePlantern __instance, ref DamageType damageType, ref int damage)
        {
            if (CoreTools.TravelUltimate("以爆制爆"))
            {
                __instance.attributeCount += damage;
                var data = __instance.GetOrAddComponent<DataSave>();
                data.SetData("UltimatePlantern_TotalDamage", data.GetData<int>("UltimatePlantern_TotalDamage") + damage);
                if (__instance.attributeCount >= 10000)
                {
                    __instance.board.boardAction.SetDoom(__instance.thePlantColumn, __instance.thePlantRow,
                        false, false, damage: (int)(data.GetData<int>("UltimatePlantern_TotalDamage") / 3.8f), fromType: __instance.thePlantType);

                    __instance.attributeCount = 0;
                }

                if (damageType != DamageType.Normal)
                    return false;
            }
            return true;
        }
    }
    #endregion
}
