using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace BetterSuperMachine
{
    public class Info
    {
        public const String GUID = "salmon.pvzrh.bettersupermachine";
        public const String NAME = "BetterSuperMachine";
        public const String VER = "0.0.1";
        public const String AUTHOR = "Salmon";
    }

    [BepInPlugin(Info.GUID, Info.NAME, Info.VER)]
    public class BetterSuperMachine : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
    public class SuperMachineNut_DieEvent_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Plant __instance)
        {
            if (__instance.thePlantType == PlantType.SuperMachineNut)
            {
                var list = new List<Plant>();
                foreach (var plant in Lawnf.GetAllPlants())
                    if (plant != null && plant.thePlantType == PlantType.SuperMachineNut && plant != __instance)
                        list.Add(plant);

                float avgHealth = 0f;
                var health = Mathf.Max(8000, __instance.thePlantHealth);
                if (list.Count != 0)
                    avgHealth = health / list.Count;
                foreach (var plant in list)
                    plant.Recover(avgHealth);
            }
        }
    }
}
