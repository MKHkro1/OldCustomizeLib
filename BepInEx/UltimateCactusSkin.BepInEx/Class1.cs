using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using BepInEx;
using UnityEngine;
using BepInEx.Unity.IL2CPP;
using System.Reflection;
using CustomizeLib.BepInEx;

namespace UltimateCactusSkin.BepInEx
{
    [BepInPlugin("salmon.ultimatecactusskin", "UltimateCactusSkin", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "skin_933");
            CustomCore.RegisterCustomPlantSkin<UltimateCactus>((int)PlantType.UltimateCactus, ab.GetAsset<GameObject>("Prefab"),
                ab.GetAsset<GameObject>("Preview"), (p) =>
                {
                    p.shoot = p.transform.FindChild("Shoot");
                    p.shoot2 = p.transform.FindChild("Shoot2");
                });
            CustomCore.RegisterCustomBullet<Bullet_ultimateCactus>((BulletType)1800, ab.GetAsset<GameObject>("Bullet_ultimateCactus"));
        }
    }

    [HarmonyPatch(typeof(UltimateCactus), nameof(UltimateCactus.GetBulletType))]
    public static class UltimateCactusPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(UltimateCactus __instance, ref BulletType __result)
        {
            if (__instance.gameObject.name == "Prefab")
            {
                __result = (BulletType)1800;
                return false;
            }
            return true;
        }
    }
}