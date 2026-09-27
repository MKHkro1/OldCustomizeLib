using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SubspeciesEntry.BepInEx.Plants
{
    #region 金蛋
    // 上游这里原本有一个补丁类 UltimatePortalNutPatch，补的是 UltimatePortalNut.Revive() 的后缀
    // （复活时取消延迟动作、清诅咒、拆掉同格超级梯子）。
    //
    // 【为什么在 4.0 下没有目标可补 —— 不是"游戏删了 Revive"，而是继承链变了】
    //   Revive() 至今仍在 4.0 的 PortalNut 上（PortalNut$$Revive @0x1804D3440，Slot 72），
    //   3.7 与 4.0 的 PortalNut 成员逐字一致：Revive / SnapShot / RestoreHealthFromSnapshot /
    //   healthSnapshots / restored / invincibleTimer 都在。
    //   但 4.0 的 UltimatePortalNut 改成了「直接继承 WallNut」而不是继承 PortalNut，
    //   并把这套快照复活机制换成了 loseSheildCount / DecreateShield / GetDamage / Backtrack / OpenBlackHole。
    //   ⇒ UltimatePortalNut 自己身上没有 Revive()，原补丁目标在 4.0 确实不存在，整块移除。
    //   ⇒ 功能差异（需实机确认）：PortalNut.Revive 的实际效果是
    //      RestoreHealthFromSnapshot → UpdateText → ReplaceSprite → 粒子11 → invincibleTimer=3 / flashCountDown=3，
    //      也就是"回血 + 3 秒无敌"。4.0 的 UltimatePortalNut 没有 invincibleTimer，
    //      若要保留"无尽贪婪"词条的 3 秒无敌，需插件自己加计时器。
    //   若将来要把该补丁改挂到 PortalNut 上，注意它会影响全部 PortalNut 系植物，不只 UltimatePortalNut。

    [HarmonyPatch(typeof(Plant))]
    public static class Plant_UltimatePortalNut_Patch
    {
        [HarmonyPatch(nameof(Plant.Awake))]
        [HarmonyPostfix]
        public static void PostAwake(Plant __instance)
        {
            if (__instance.thePlantType == PlantType.UltimatePortalNut)
            {
                __instance.attributeCountdown = 30f;
            }
        }

        [HarmonyPatch(nameof(Plant.Update))]
        [HarmonyPostfix]
        public static void PostUpdate(Plant __instance)
        {
            if (__instance.thePlantType == PlantType.UltimatePortalNut)
            {
                if (__instance.attributeCountdown - Time.deltaTime <= 0f)
                {
                    if (CoreTools.TravelUltimate("无尽贪婪"))
                    {
                        // 原先这里调 __instance.GetComponent<UltimatePortalNut>().Revive()，
                        // 4.0 的 UltimatePortalNut 不再继承 PortalNut，没有 Revive() 可调
                        //（详见本文件上方关于补丁类被移除的说明），这里只保留作者写在 Revive() 之后的回血部分。
                        __instance.thePlantHealth += 2000;
                        __instance.thePlantHealth = Mathf.Min(__instance.thePlantMaxHealth, __instance.thePlantHealth);
                        __instance.UpdateText();
                    }
                    __instance.attributeCountdown = 30f;
                }
            }
        }
    }
    #endregion
}
