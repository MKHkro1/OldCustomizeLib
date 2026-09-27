using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace HypnoSquash.BepInEx
{
    [HarmonyPatch(typeof(Squash), "AttackZombie")]
    public static class SquashPatch
    {
        public static bool Prefix(Squash __instance)
        {
            if (__instance.thePlantType is (PlantType)302)
            {
                var pos = __instance.transform.position;
                var array = Physics2D.OverlapBoxAll(pos, new(1, 1), 0);
                foreach (var z in array)
                {
                    if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zombie) && !TypeMgr.IsAirZombie(zombie.theZombieType) && zombie.theZombieRow == __instance.thePlantRow)
                    {
                        zombie.SetMindControl();
                        if (!zombie.isMindControlled)
                        {
                            zombie.TakeDamage(DmgType.Squash, 1800);
                        }
                    }
                }
                GameAPP.PlaySound(74);
                ScreenShake.shakeDuration = 0.05f;
                return false;
            }
            return true;
        }
    }

    [BepInPlugin("inf75.hypnosquash", "HypnoSquash", "1.0")]
    public class Core : BasePlugin//302
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "hypnosquash");
            CustomCore.RegisterCustomPlant<Squash>(302, ab.GetAsset<GameObject>("HypnoSquashPrefab"),
                ab.GetAsset<GameObject>("HypnoSquashPreview"), [(13, 8), (8, 13)], 3, 0, 20, 300, 7.5f, 300);
            CustomCore.AddPlantAlmanacStrings(302, "魅惑窝瓜(302)", "魅惑砸到的僵尸。\n<color=#3D1400>贴图作者：@10000why</color>\n<color=#3D1400>伤害：</color><color=red>1800或魅惑</color>\n<color=#3D1400>融合配方：</color><color=red>窝瓜+魅惑菇</color>\n<color=#3D1400>魅惑窝瓜把自己涂得全身斑斓，只为完成秽土转生之术。尽管法术的祭品是他自己，他依然乐此不疲，“没有什么魔术比把被压扁的僵尸复活更有意思了”。</color>");
        }
    }
}