using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using TMPro;
using UnityEngine;
using Core;

namespace SuperShovelLevel.BepInEx
{
    [BepInPlugin("salmon.supershovellevel", "SuperShovelLevel", "1.0")]
    public class Core : BasePlugin
    {
        public static int levelID = -1;

        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            CustomLevelData customLevelData = new CustomLevelData();
            Board.BoardTag tag = default;
            tag.enableAllTravelPlant = true;
            tag.enableTravelPlant = true;
            customLevelData.BoardTag = tag;
            customLevelData.Name = () => "超级随机：铲子";
            customLevelData.SceneType = SceneType.Day_6;
            customLevelData.RowCount = 6;
            customLevelData.WaveCount = () => 100;
            customLevelData.BgmType = MusicType.Day_drum;
            customLevelData.Sun = () => 1000;
            customLevelData.Logo = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "supershovellevel").GetAsset<Sprite>("icon");
            customLevelData.ZombieList = () => new List<ZombieType>()
            {
                ZombieType.RandomZombie,
                ZombieType.RandomPlusZombie,
                ZombieType.DiamondRandomZombie
            };
            customLevelData.NeedSelectCard = true;
            customLevelData.AdvBuffs = () => new List<int>() { 1000 };
            levelID = CustomCore.RegisterCustomLevel(customLevelData);
        }
    }

    [HarmonyPatch(typeof(CardUI), nameof(CardUI.Start))]
    public static class CardUIPatch
    {
        [HarmonyPostfix]
        public static void PostStart(CardUI __instance)
        {
            if (GameAPP.theBoardLevel == Core.levelID && (int)GameAPP.theBoardType == 66)
            {
                var plantType = (PlantType)__instance.theSeedType;
                if (plantType == PlantType.Present || plantType == PlantType.PresentZombie || plantType == PlantType.SnowPresent)
                    __instance.gameObject.SetActive(false);
            }
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
    public static class PlantPatch
    {
        [HarmonyPostfix]
        public static void PostDieEvent(Plant __instance, ref Plant.DieReason reason)
        {
            if (GameAPP.theBoardLevel == Core.levelID && (int)GameAPP.theBoardType == 66 && reason == Plant.DieReason.ByShovel)
            {
                var plantList = GameAPP.resourcesManager.allPlants.ToArray().Where(t => !TypeMgr.IsWaterPlant(t)).ToList();
                plantList.Remove(PlantType.Nothing);
                plantList.Remove(PlantType.Pit);
                plantList.Remove(PlantType.Refrash);
                plantList.Remove(PlantType.Extract_single);
                plantList.Remove(PlantType.Extract_ten);
                plantList.Remove(PlantType.MagnetBox);
                plantList.Remove(PlantType.MagnetInterface);
                var card = Lawnf.SetDroppedCard(__instance.axis.transform.position, plantList[UnityEngine.Random.Range(0, plantList.Count)], UnityEngine.Random.Range(0, 101));
                float randomValue = UnityEngine.Random.Range(0f, 1f);
                int result = Mathf.FloorToInt(8 * randomValue * randomValue) + 1;
                result = Mathf.Clamp(result, 1, 8);
                card.maxUsedTimes = result; // 随机1~8的值，越大概率越小

                if (UnityEngine.Random.Range(0, 100) <= 2)
                {
                    var mgr = GameAPP.Instance.GetComponent<TravelMgr>();
                    if (mgr == null)
                        return;
                    var data = mgr.data;
                    switch (UnityEngine.Random.Range(0, 3))
                    {
                        case 0:
                            {
                                var list = new List<AdvBuff>();
                                foreach (var (id, _) in TravelDictionary.advancedBuffsText)
                                    if (!data.advBuffs.Contains(id))
                                        list.Add(id);
                                var advBuff = list[UnityEngine.Random.Range(0, list.Count)];
                                data.advBuffs.Add(advBuff);
                                InGameText.Instance.ShowText($"{TravelDictionary.advancedBuffsText[advBuff]}", 5f);
                            }
                            break;
                        case 1:
                            {
                                var list = new List<TravelDebuff>();
                                foreach (var (id, _) in TravelDictionary.debuffData)
                                    if (!data.travelDebuffs.Contains(id))
                                        list.Add(id);
                                var debuff = list[UnityEngine.Random.Range(0, list.Count)];
                                data.travelDebuffs.Add(debuff);
                                InGameText.Instance.ShowText($"{TravelDictionary.debuffData[debuff].Item1}", 5f);
                            }
                            break;
                        case 2:
                            {
                                var list = new List<UltiBuff>();
                                foreach (var (id, _) in TravelDictionary.ultimateBuffsText)
                                    if (!data.ultiBuffs.Contains(id) && !data.ultiBuffs.Contains(id))
                                        list.Add(id);
                                var ultiBuff = list[UnityEngine.Random.Range(0, list.Count)];
                                data.ultiBuffs.Add(ultiBuff);
                                InGameText.Instance.ShowText($"{TravelDictionary.ultimateBuffsText[ultiBuff]}", 5f);
                            }
                            break;
                    }
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
            if (__instance.theLevel == Core.levelID && __instance.transform.FindChild("Window/Name").GetComponent<TextMeshProUGUI>().text == "超级随机：铲子")
            {
                __instance.transform.FindChild("Window/Name").GetComponent<TextMeshProUGUI>().text = "超级随机\n铲子";
            }
        }
    }
}
