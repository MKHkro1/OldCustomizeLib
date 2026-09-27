using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Reflection;
using Unity.Collections;

namespace SuperPeaFume.BepInEx
{
    [BepInPlugin("salmon.superpeafume", "SuperPeaFume", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(PeaFume))]
    public static class PeaFumePatch
    {
        [HarmonyPatch(nameof(PeaFume.FixedUpdate))]
        [HarmonyPostfix]
        public static void PostFixedUpdate(PeaFume __instance)
        {
            if (__instance.Active && __instance.theStatus == PlantStatus.Raised)
            {
                foreach (var bullet in __instance.board.boardEntity.bulletArray)
                {
                    if (bullet != null)
                    {
                        if (bullet.theBulletRow == __instance.thePlantRow)
                        {
                            float bulletX = bullet.transform.position.x;
                            float plantX = __instance.axis.transform.position.x;

                            // 如果子弹在植物右侧且未标记
                            if (bulletX > plantX && !bullet.shootByZombie)
                            {
                                // 增加计数并增加子弹速度
                                bullet.Damage += 99;
                            }
                        }
                    }
                }
            }
        }
    }
}
