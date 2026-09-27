using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using UnityEngine;

namespace Mower.BepInEx
{
    [BepInPlugin("salmon.mower", "Mower", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "mower");
            ClassInjector.RegisterTypeInIl2Cpp<Mower>();
            CustomCore.RegisterCustomPlant<Plant, Mower>(Mower.PlantID, ab.GetAsset<GameObject>("MowerPrefab"),
                ab.GetAsset<GameObject>("MowerPreview"), new List<(int, int)> { }, 0f, 0f, int.MaxValue, int.MaxValue, 300f, 950);
            CustomCore.AddPlantAlmanacStrings(Mower.PlantID, $"小推车",
                "小推车可以清除一行的僵尸。\n\n" +
                "<color=#3D1400>特点：</color><color=red>接触僵尸后会向前移动，秒杀路径上的所有僵尸。</color>\n\n" +
                "<color=#3D1400>有人表示这辆割草机能对僵尸造成毁灭性打击。不过当事人并没有说什么，它没有意见。</color>\n\n" +
                "<color=#955300>花费：<color=red>950</color>\n" +
                "<color=#955300>冷却时间：<color=red>5分钟</color>");
            CustomCore.RegisterCustomCardToColorfulCards((PlantType)Mower.PlantID);
        }
    }

    public class Mower : MonoBehaviour
    {
        public static ID PlantID = 1965;

        public void Start()
        {
            if (GameAPP.theGameStatus == GameStatus.InGame)
            {
                if (CreateMower.Instance != null && Mouse.Instance != null && plant != null && plant.board != null)
                {
                    var x = Mouse.Instance.GetBoxXFromColumn(plant.thePlantColumn);
                    var mowerType = MowerType.LawnMower;
                    switch (plant.board.GetBoxType(plant.thePlantColumn, plant.thePlantRow))
                    {
                        case BoxType.Water:
                            mowerType = MowerType.PoolMower;
                            break;
                    }
                    CreateMower.Instance.SetMower(mowerType, x, plant.thePlantRow);
                    plant.Die();
                }
            }
        }

        public Plant plant => gameObject.GetComponent<Plant>();
    }

    [HarmonyPatch(typeof(CreatePlant))]
    public static class CreatePlantPatch
    {
        [HarmonyPatch(nameof(CreatePlant.CheckBox))]
        [HarmonyPrefix]
        public static void PreCheckBox(PlantType theSeedType, ref bool __result)
        {
            if (theSeedType == Mower.PlantID)
                __result = true;
        }

        [HarmonyPatch(nameof(CreatePlant.SetPlant))]
        [HarmonyPrefix]
        public static void PreSetPlant(PlantType theSeedType, ref bool isFreeSet)
        {
            if (theSeedType == Mower.PlantID)
                isFreeSet = true;
        }
    }
}
