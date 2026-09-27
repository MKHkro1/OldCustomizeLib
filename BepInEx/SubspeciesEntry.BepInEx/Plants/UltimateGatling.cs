using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SubspeciesEntry.BepInEx.Plants
{
    [HarmonyPatch(typeof(BombCherry))]
    public static class BombCherryPatch
    {
        [HarmonyPatch(nameof(BombCherry.Explode), new Type[] { typeof(float) })]
        [HarmonyPrefix]
        public static void PreExplode(BombCherry __instance)
        {
            if (CoreTools.TravelUltimateLevel("力大砖飞") >= 2 && (__instance.bombType == CherryBombType.Bullet || __instance.bombType == CherryBombType.BulletAll))
            {
                __instance.damageToZombie /= 3;
                __instance.damageToZombie *= 10;
                __instance.damageToZombie = Mathf.Max(__instance.damageToZombie, 300 / 3);
            }
        }
    }
}
