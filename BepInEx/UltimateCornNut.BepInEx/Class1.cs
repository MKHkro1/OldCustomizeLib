using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using BepInEx;
using UnityEngine;
using BepInEx.Unity.IL2CPP;
using System.Reflection;
using CustomizeLib.BepInEx;

namespace UltimateCornNut.BepInEx
{
    [BepInPlugin("salmon.ultimatecornnut", "UltimateCornNut", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<UltimateCornNut>();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "ultimatecornnut");
            CustomCore.RegisterCustomPlant<UltimatePortalNut, UltimateCornNut>(UltimateCornNut.PlantID, ab.GetAsset<GameObject>("UltimateCornNutPrefab"),
                ab.GetAsset<GameObject>("UltimateCornNutPreview"), new List<(int, int)> { }, 0f, 0f, 0, 16000, 90f, 750);
            CustomCore.AddPlantAlmanacStrings(UltimateCornNut.PlantID, $"究级超时空黄油坚果",
                "回溯和免疫伤害能力极强的超时空黄油坚果\n" +
                "<color=#0000FF>究极超时空坚果的限定形态</color>\n\n" +
                "<color=#3D1400>贴图作者：@林秋-AutumnLin</color>\n" +
                "<color=#3D1400>使用条件：</color><color=red>①融合或转化究级超时空坚果时有2%概率变异\n" +
                "②神秘模式\n" +
                "*可使用坚果切回究级超时空坚果</color>\n" +
                "<color=#3D1400>韧性：</color><color=red>16000，限伤1000</color>\n" +
                "<color=#3D1400>特点：</color><color=red>①铁植物\n" +
                "②免疫碾压并击退，有1/3概率免疫伤害，受到2000伤害后回溯3秒前血量并免疫伤害2秒，30秒未受伤害时，回复1倍韧性血量\n" +
                "③被啃咬时，对啃咬者施加2秒的黄油定身和3秒传送状态\n" +
                "<color=#3D1400>“未来无法改变，过去可以改变吗？我们生活的现在，是过去的延伸，也是未来的基石，也可以是更遥远的过去，我们已经经历的时间是无法改变的，他们有固定的节点和顺序，但是我们可以通过一种微妙的方式去改变它，首先要做的，就是改变我们自己，现在会影响到未来，未来也会影响到过去，我们在尝试改变，我们在努力改变，”究极超时空黄油坚果说完便消失在了原地，随后又突然出现，“死亡是无法跨越的时间节点，如果可以改变它，我愿意尝试。”</color>");
            CustomCore.TypeMgrExtra.IsNut.Add((PlantType)UltimateCornNut.PlantID);
            CustomCore.TypeMgrExtra.LevelPlants.Add((PlantType)UltimateCornNut.PlantID, CardLevel.Red);
            CustomCore.TypeMgrExtra.IsMagnetPlants.Add((PlantType)UltimateCornNut.PlantID);
            CustomCore.AddUltimatePlant((PlantType)UltimateCornNut.PlantID);
            CustomCore.AddFusion((int)PlantType.UltimatePortalNut, UltimateCornNut.PlantID, (int)PlantType.WallNut);
            CustomCore.AddFusion((int)PlantType.UltimatePortalNut, (int)PlantType.WallNut, UltimateCornNut.PlantID);

            foreach (var item in Enum.GetValues<BucketType>())
                CustomCore.RegisterCustomUseItemOnPlantEvent((PlantType)UltimateCornNut.PlantID, item, (p) => p.Recover(500f));
        }
    }

    public class UltimateCornNut : MonoBehaviour
    {
        public static int PlantID = 1943;

        public UltimatePortalNut plant => gameObject.GetComponent<UltimatePortalNut>();
    }

    [HarmonyPatch(typeof(Zombie), nameof(Zombie.PlayEatSound))]
    public static class Zombie_PlayEatSound_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Zombie __instance)
        {
            if (__instance != null && __instance.theAttackTarget != null)
            {
                if (__instance.theAttackTarget.IsPlant(out var plant) && plant != null && (int)plant.thePlantType == UltimateCornNut.PlantID)
                {
                    __instance.BeSmall();
                    __instance.Buttered(2f);
                    __instance.SetPortaled(3f);
                    if (plant.attributeCountdown == 0f && UnityEngine.Random.Range(0, 4) == 0)
                    {
                        plant.attributeCountdown = 10f;

                        // 创建传送粒子效果
                        Vector3 zombiePos = __instance.axis.position;
                        zombiePos.y += 0.5f;
                        ParticleManager.Instance.SetParticle(ParticleType.RandomCloud, zombiePos, __instance.theZombieRow, lim: true);

                        // 传送僵尸到右侧
                        GameObject zombieObj = __instance.gameObject;
                        float targetX = __instance.board.boardMaxX + 0.5f;
                        float targetY = Mouse.Instance.GetLandY(targetX, __instance.theZombieRow);
                        Vector3 targetPos = new Vector3(targetX, targetY, 0f);
                        __instance.AdjustPosition(targetPos);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(UltimatePortalNut), nameof(UltimatePortalNut.DecreaseHealth))]
    public static class UltimatePortalNut_DecreaseHealth_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(UltimatePortalNut __instance, ref int value)
        {
            if (__instance != null && (int)__instance.thePlantType == UltimateCornNut.PlantID)
            {
                if (__instance.attributeCount + value > 2000)
                {
                    __instance.attributeCount = 4001;
                    // 原先这里还有一句 __instance.restoreCount = -1;（CS0117）。
                    // restoreCount 从来不是 PortalNut 的成员（3.7 / 4.0 的 PortalNut 都没有它，
                    // 只有 Revive/SnapShot/RestoreHealthFromSnapshot/healthSnapshots/restored/invincibleTimer），
                    // 它是 3.x 时代 UltimatePortalNut 自己的字段；4.0 该类改为直接继承 WallNut
                    // 并换成 loseSheildCount / DecreateShield / GetDamage / Backtrack 机制后就没有对应物了。
                    // ⇒ 4.0 下无等价成员，该行移除。若确实需要"禁止恢复"，得自行实现（功能差异需实机确认）。
                }
            }
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.GetDamage))]
    public static class Plant_GetDamage_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(Plant __instance, ref int __result)
        {
            if (__instance != null && (int)__instance.thePlantType == UltimateCornNut.PlantID && UnityEngine.Random.Range(0, 3) == 0)
            {
                __result = 0;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(CreatePlant), nameof(CreatePlant.CheckMix))]
    public static class CreatePlant_CheckMix_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(CreatePlant __instance, ref Plant __result)
        {
            if (__result != null && __result.GetComponent<Plant>().thePlantType == PlantType.UltimatePortalNut && UnityEngine.Random.Range(0, 100) <= 1)
            {
                var row = __result.thePlantRow;
                var column = __result.thePlantColumn;
                __result.Die(Plant.DieReason.ByShovel);
                __instance.SetPlant(column, row, (PlantType)UltimateCornNut.PlantID, null, default, true);
            }
        }
    }

    [HarmonyPatch(typeof(TypeMgr), nameof(TypeMgr.UncrashablePlant))]
    public static class TypeMgr_UncrashablePlant_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(TypeMgr __instance, ref Plant plant, ref bool __result)
        {
            if (plant != null && (int)plant.thePlantType == UltimateCornNut.PlantID)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}