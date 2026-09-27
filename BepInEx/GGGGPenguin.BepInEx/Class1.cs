using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using Core;

namespace GGGGPenguin.BepInEx
{
    [BepInPlugin("salmon.ggggpenguin", "GGGGPenguin", "1.0")]
    public class Core : CorePlugin
    {
        public static List<AudioClip> JumpClips = new();
        public static List<AudioClip> DieClips = new();
        public static bool enable = false;

        public override void OnStart()
        {
            var ab = CustomCore.GetAssetBundle(Tools.GetAssembly(), "凑企鹅");
            ab.GetAsset<GameObject>("PenguinZombie 1").transform.FindChild("body/head1").tag = "ZombieHead";
            CustomCore.RegisterCustomZombie<PenguinZombie, GGGGPenguinZombie>(GGGGPenguinZombie.ZombieID, ab.GetAsset<GameObject>("PenguinZombie 1"),
                ab.GetAsset<GameObject>("PenguinZombiePreview").GetComponent<SpriteRenderer>().sprite, 100, 1350, 0, 0);
            ab.GetAsset<GameObject>("SuperPenguinZombie").transform.FindChild("body/head1").tag = "ZombieHead";
            CustomCore.RegisterCustomZombie<SuperPenguinZombie, SuperGGGGPenguinZombie>(SuperGGGGPenguinZombie.ZombieID, ab.GetAsset<GameObject>("SuperPenguinZombie"),
                ab.GetAsset<GameObject>("SuperPenguinZombiePreview 1").GetComponent<SpriteRenderer>().sprite, 500, 1440, 0, 0);
            JumpClips.Add(ab.GetAsset<AudioClip>("jamp1"));
            JumpClips.Add(ab.GetAsset<AudioClip>("jamp2"));
            JumpClips.Add(ab.GetAsset<AudioClip>("jamp3"));
            DieClips.Add(ab.GetAsset<AudioClip>("die1"));
            DieClips.Add(ab.GetAsset<AudioClip>("die2"));
        }
    }

    public class GGGGPenguinZombie : MonoBehaviour
    {
        public static ID ZombieID = 1900;

        public void GuGuGaGa()
        {
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                GameAPP.music.PlayOneShot(Core.JumpClips[UnityEngine.Random.Range(0, Core.JumpClips.Count)], 5f * GameAPP.currentMusicVolume);
            }
        }
    }

    public class SuperGGGGPenguinZombie : MonoBehaviour
    {
        public static ID ZombieID = 1901;

        public void GuGuGaGa()
        {
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                GameAPP.music.PlayOneShot(Core.JumpClips[UnityEngine.Random.Range(0, Core.JumpClips.Count)], 5f * GameAPP.currentDrumVolume);
            }
        }
    }

    [HarmonyPatch(typeof(CreateZombie))]
    public static class CreateZombiePatch
    {
        [HarmonyPatch(nameof(CreateZombie.SetZombie))]
        [HarmonyPrefix]
        public static void PreSetZombie(ref ZombieType theZombieType)
        {
            if (UnityEngine.Random.Range(0, 2) == 0 && Core.enable)
            {
                if (theZombieType == ZombieType.PenguinZombie)
                    theZombieType = GGGGPenguinZombie.ZombieID;
                if (theZombieType == ZombieType.SuperPenguinZombie)
                    theZombieType = SuperGGGGPenguinZombie.ZombieID;
            }
        }
    }

    [HarmonyPatch(typeof(PenguinZombie))]
    public static class PenguinZombiePatch
    {
        [HarmonyPatch(nameof(PenguinZombie.DieEvent))]
        [HarmonyPrefix]
        public static void PreDieEvent(PenguinZombie __instance)
        {
            if (__instance.theZombieType == GGGGPenguinZombie.ZombieID || __instance.theZombieType == SuperGGGGPenguinZombie.ZombieID)
                GameAPP.music.PlayOneShot(Core.DieClips[UnityEngine.Random.Range(0, Core.DieClips.Count)], 5f * GameAPP.currentDrumVolume);
        }
    }

    [HarmonyPatch(typeof(InitZombieList))]
    public static class InitZombieListAllowZombiePatch
    {
        [HarmonyPatch(nameof(InitZombieList.PickZombie))]
        [HarmonyPrefix]
        public static void PrePickZombie()
        {
            if (Core.enable)
            {
                InitZombieList.zombieToSpawns.Add(ZombieType.PenguinZombie);
                InitZombieList.zombieToSpawns.Add(ZombieType.SuperPenguinZombie);
            }
        }
    }

    [HarmonyPatch(typeof(CheatKey))]
    public static class CheatKeyPatch
    {
        [HarmonyPatch(nameof(CheatKey.Awake))]
        [HarmonyPostfix]
        public static void PostAwake(CheatKey __instance)
        {
            Action action = () =>
            {
                Core.enable = !Core.enable;
                InGameText.Instance.ShowText(Core.enable ? "咕咕嘎嘎！后续进入关卡出怪加入企鹅" : "咕咕嘎嘎！取消出怪加入企鹅", 1f);
            };
            __instance.CheatKeys.Add("gggg", action);
        }
    }
}
