using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace SuperConveyorBelt2.BepInEx
{
    [BepInPlugin("salmon.superconveyorbelt2", "SuperConveyorBelt2", "1.0")]
    public class Core : CorePlugin
    {
        public static int levelID = -1;
        public static List<PlantType> NormalPlants = new();
        public static List<PlantType> UltiPlants = new();

        public override void OnGameInit()
        {
            if (!GameAPP.resourcesManager.allPlants.Contains((PlantType)1931))
            {
                Logger.LogWarning("未安装 黄金模仿者 Mod，超级传送带·福祸相依将不会加载！");
                return;
            }
            if (!GameAPP.resourcesManager.allPlants.Contains((PlantType)1960))
            {
                Logger.LogWarning("未安装 僵尸模仿者 Mod，超级传送带·福祸相依将不会加载！");
                return;
            }
            NormalPlants = GameAPP.resourcesManager.allPlants.ToSystemList().Where(t => !Lawnf.IsUltiPlant(t) && !TypeMgr.IsWaterPlant(t) && !TypeMgr.IsPurplePlant(t) && !Lawnf.TowerPlant(t) && t != PlantType.MixBomb).ToList();
            UltiPlants = GameAPP.resourcesManager.allPlants.ToSystemList().Where(t => Lawnf.IsUltiPlant(t) && !TypeMgr.IsWaterPlant(t) && !TypeMgr.IsPurplePlant(t) && !Lawnf.TowerPlant(t) && t != PlantType.MixBomb).ToList();

            CustomLevelData customLevelData = new CustomLevelData();
            var boardTag = new Board.BoardTag();
            boardTag.isConvey = true;
            boardTag.enableAllTravelPlant = true;
            boardTag.enableTravelPlant = true;
            boardTag.isSuperRandom = true;
            customLevelData.BoardTag = boardTag;
            customLevelData.Name = () => "超级传送带：福祸相依";
            customLevelData.SceneType = SceneType.Day_6;
            customLevelData.RowCount = 6;
            customLevelData.WaveCount = () => 100;
            customLevelData.BgmType = MusicType.Day_drum;
            customLevelData.Logo = GameAPP.resourcesManager.plantPreviews[PlantType.Present].GetComponent<SpriteRenderer>().sprite;
            customLevelData.ZombieList = () => new List<ZombieType>()
            {
                ZombieType.RandomZombie,
                ZombieType.RandomPlusZombie,
                ZombieType.DiamondRandomZombie
            };
            customLevelData.NeedSelectCard = false;
            customLevelData.AdvBuffs = () => new List<int>() { 1000 };
            customLevelData.ConveyBeltPlantTypes = () => new List<PlantType>
            {
                (PlantType)1931,
                (PlantType)1960
            };
            levelID = CustomCore.RegisterCustomLevel(customLevelData);
        }
    }

    [HarmonyPatch(typeof(ConveyManager))]
    public static class ConveyManagerPatch
    {
        [HarmonyPatch(nameof(ConveyManager.NewCardUpdate))]
        [HarmonyPostfix]
        public static void PostStart(ConveyManager __instance)
        {
            if (GameAPP.theBoardLevel == Core.levelID && (int)GameAPP.theBoardType == 66)
            {
                __instance.interval = 3.5f;
            }
        }

        [HarmonyPatch(nameof(ConveyManager.GetCardType))]
        [HarmonyPostfix]
        public static void Postfix(ref PlantType __result)
        {
            if (GameAPP.theBoardLevel == Core.levelID && (int)GameAPP.theBoardType == 66)
            {
                var v = UnityEngine.Random.Range(1, 101);
                if (v <= 30)
                {
                    __result = Core.NormalPlants[UnityEngine.Random.Range(0, Core.NormalPlants.Count)];
                }
                else if (v <= 50)
                {
                    __result = Core.UltiPlants[UnityEngine.Random.Range(0, Core.UltiPlants.Count)];
                }
                else if (v <= 75)
                {
                    __result = (PlantType)1960;
                }
                else if(v <= 95)
                {
                    __result = (PlantType)1931;
                }
                else if (v <= 98)
                {
                    __result = PlantType.DiamondImitater;
                }
                else
                {
                    __result = PlantType.Present;
                }
            }
        }
    }

    [HarmonyPatch(typeof(CheckAdv), nameof(CheckAdv.Start))]
    public static class CheckAdvPatch
    {
        [HarmonyPostfix]
        public static void Postfix(CheckAdv __instance)
        {
            if (__instance.theLevel == Core.levelID && __instance.transform.FindChild("Window/Name").GetComponent<TextMeshProUGUI>().text == "超级传送带：福祸相依")
            {
                __instance.transform.FindChild("Window/Name").GetComponent<TextMeshProUGUI>().text = "超级传送带\n福祸相依";
            }
        }
    }

    [HarmonyPatch(typeof(Board), nameof(Board.NewZombieUpdate))]
    public class ShowTextPatch
    {
        [HarmonyPostfix]
        public static void Postfix(Board __instance)
        {
            if (GameAPP.theBoardLevel == Core.levelID && (int)GameAPP.theBoardType == 66 && __instance.theWave == 0)
            {
                InGameUI.Instance.BackToMenu.SetActive(false);
            }
        }
    }

    [HarmonyPatch(typeof(InGameUI), nameof(InGameUI.Start))]
    public class InGameUIPatch
    {
        [HarmonyPostfix]
        public static void Postfix(InGameUI __instance)
        {
            if (GameAPP.theBoardLevel == Core.levelID && (int)GameAPP.theBoardType == 66)
            {
                if (__instance != null && (int)GameAPP.theBoardType == 66)
                {
                    __instance.SeedBank.SetActive(false);
                }
            }
        }
    }
}
