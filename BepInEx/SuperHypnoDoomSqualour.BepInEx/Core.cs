using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using BepInEx;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;

namespace SuperDoomSqualour.BepInEx
{
    [HarmonyPatch(typeof(Squalour), "LourDie")]
    public static class SqualourPatch
    {
        public static bool Prefix(Squalour __instance)
        {
            if (__instance.thePlantType is (PlantType)164)
            {
                __instance.Die();
                return false;
            }
            return true; ;
        }
    }

    [BepInPlugin("inf75.superdoomsqualour", "SuperDoomSqualour", "1.0")]
    public class Core : BasePlugin//164
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<SuperDoomSqualour>();
            ClassInjector.RegisterTypeInIl2Cpp<SuperHypnoDoomCattailLour>();
            ClassInjector.RegisterTypeInIl2Cpp<SuperHypnoDoomCattailLour_fly>();
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "superdoomsqualour");
            CustomCore.RegisterCustomPlant<Squalour, SuperDoomSqualour>(164, ab.GetAsset<GameObject>("SuperDoomSqualourPrefab"),
                ab.GetAsset<GameObject>("SuperDoomSqualourPreview"), [(928, 248)], 3, 0, 3600, 300, 60f, 800);
            CustomCore.AddFusion(928, 164, 8);
            SuperDoomSqualour.Buff = CustomCore.RegisterCustomBuff("难度开了：幻灭猫瓜代码杀全场僵尸", BuffType.AdvancedBuff, () => Lawnf.TravelAdvanced(38) && Lawnf.TravelAdvanced(39), 36100, "red", (PlantType)164);
            CustomCore.AddPlantAlmanacStrings(164, "幻灭猫瓜(164)", "幻灭猫瓜从***那里获得了控制核能的力量，但最好不要让它再次红温。\n<color=#3D1400>贴图作者：@仨硝基甲苯_</color>\n<color=#3D1400>特点：</color><color=red>幻灭菇亚种，使用猫瓜、魅惑菇切换。压扁僵尸时对全屏僵尸造成3600灰烬伤害，同时对所有僵尸附加死亡时毁灭爆炸效果</color>\n<color=#3D1400>融合配方：</color><color=red>幻灭菇+猫瓜</color>\n<color=#3D1400>词条：</color><color=red>难度开了：幻灭猫瓜代码杀全场僵尸(解锁条件：解锁可控核聚变与人多势众)</color>\n<color=#3D1400>别看幻灭猫瓜一副不好惹的模样，实际上她正在幻境中苦练技术，目前已经有了针对每一个僵尸的幻境，不过你得先将她从胜利的幻境中叫醒。</color>");

            CustomCore.RegisterCustomPlant<CattailLour, SuperHypnoDoomCattailLour>(178, ab.GetAsset<GameObject>("SuperHypnoDoomCattailLourPrefab"),
                ab.GetAsset<GameObject>("SuperHypnoDoomCattailLourPreview"), [(928, 926), (926, 928), (1067, 164), (164, 1067)], 3, 0, 600, 300, 60f, 1800);
            CustomCore.RegisterCustomPlantSkin<CattailLour, SuperHypnoDoomCattailLour>(178, ab.GetAsset<GameObject>("SuperHypnoDoomCattailLourPrefab2"),
                ab.GetAsset<GameObject>("SuperHypnoDoomCattailLourPreview2"), [(928, 926), (926, 928), (1067, 164), (164, 1067)], 3, 0, 600, 300, 60f, 1800);
            CustomCore.RegisterCustomBullet<Bullet_lourCactus>((BulletType)904, ab.GetAsset<GameObject>("Bullet_SuperHypnoDoomCattailLourCactus"));
            SuperHypnoDoomCattailLour.flyPrefab = ab.GetAsset<GameObject>("SuperHypnoDoomCattailLour_fly");
            SuperHypnoDoomCattailLour.flyPrefab.AddComponent<LourFly>();
            SuperHypnoDoomCattailLour.flyPrefab.AddComponent<SuperHypnoDoomCattailLour_fly>(); ;
            CustomCore.AddPlantAlmanacStrings(178, "幻灭猫尾草(178)", "可爱的背后是超模的机制和数值。\n<color=#3D1400>贴图作者：@林秋AutumnLin</color>\n<color=#3D1400>特点：</color><color=red>攻击全场索敌，优先对空。自身或浮游炮每击杀1个僵尸在左侧召唤1个浮游炮，本行索敌，3秒后离场。子弹为幻灭尖刺，600伤害，对被攻击的僵尸附加死亡时毁灭爆炸的效果，有10%概率魅惑。亡语为幻灭猫瓜效果</color>\n<color=#3D1400>伤害：</color><color=red>600或魅惑×2/1.5秒；600或魅惑×60/3秒（浮游炮）</color>\n<color=#3D1400>融合配方：</color><color=red>幻灭菇+魔法猫尾草或幻灭猫瓜+猫尾草</color>\n<color=#3D1400>词条：</color><color=red>难度开了：幻灭猫尾草的子弹无视一切限制100%魅惑被其攻击的僵尸，亡语为代码杀全场所有僵尸</color>\n<color=#3D1400></color>");
        }
    }
}