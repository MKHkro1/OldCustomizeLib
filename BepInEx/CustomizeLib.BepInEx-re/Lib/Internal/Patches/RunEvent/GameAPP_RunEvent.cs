using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Patches.RunEvent
{
    [HarmonyPatch(typeof(GameAPP))]
    internal static class GameAPP_RunEvent
    {
        [HarmonyPatch(nameof(GameAPP.Awake))]
        [HarmonyPrefix]
        internal static void PreAwake()
        {
            IGameAppEvent.ManualRunAll(IGameAppEvent.GameAppEvent.PreAwake);
        }

        [HarmonyPatch(nameof(GameAPP.Awake))]
        [HarmonyPostfix]
        internal static void PostAwake()
        {
            IGameAppEvent.ManualRunAll(IGameAppEvent.GameAppEvent.PostAwake);
        }

        [HarmonyPatch(nameof(GameAPP.Start))]
        [HarmonyPrefix]
        internal static void PreStart()
        {
            IGameAppEvent.ManualRunAll(IGameAppEvent.GameAppEvent.PreStart);
        }

        [HarmonyPatch(nameof(GameAPP.Start))]
        [HarmonyPostfix]
        internal static void PostStart()
        {
            IGameAppEvent.ManualRunAll(IGameAppEvent.GameAppEvent.PostStart);
        }

        [HarmonyPatch(nameof(GameAPP.LoadResources))]
        [HarmonyPrefix]
        internal static void PreLoadResources()
        {
            IGameAppEvent.ManualRunAll(IGameAppEvent.GameAppEvent.PreLoadResources);
        }

        [HarmonyPatch(nameof(GameAPP.LoadResources))]
        [HarmonyPostfix]
        internal static void PostLoadResources()
        {
            IGameAppEvent.ManualRunAll(IGameAppEvent.GameAppEvent.PostLoadResources);
        }
    }
}
