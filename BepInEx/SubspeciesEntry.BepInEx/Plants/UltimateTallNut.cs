using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubspeciesEntry.BepInEx.Plants
{
    [HarmonyPatch(typeof(UltimateTallNut))]
    public static class UltimateTallNutPatch
    {
        [HarmonyPatch(nameof(UltimateTallNut.Awake))]
        [HarmonyPostfix]
        public static void PostAwake(UltimateTallNut __instance)
        {
            __instance.StartCoroutine(AddHealth().WrapToIl2Cpp());
            IEnumerator AddHealth()
            {
                yield return null;
                if (CoreTools.TravelUltimateLevel("世纪之盾") >= 2)
                {
                    if (__instance == null) yield break;
                    __instance.thePlantHealth *= 3;
                    __instance.thePlantMaxHealth *= 3;
                    __instance.UpdateText();
                }
            }
        }
    }
}
