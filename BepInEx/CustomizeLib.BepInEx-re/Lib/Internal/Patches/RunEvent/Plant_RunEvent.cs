using CustomizeLib.BepInEx.Internal.Events;
using CustomizeLib.BepInEx.Internal.Extensions;
using CustomizeLib.BepInEx.Internal.Tools;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Patches.RunEvent
{
    [HarmonyPatch]
    [HarmonyPriority(Priority.First)]
    internal static class Plant_RunEvent
    {
        [HarmonyTargetMethods]
        internal static IEnumerable<MethodBase> GetTargetMethods()
        {
            return InternalTools.TypeTools_GetAllMethods(InternalTools.TypeTools_GetAllDerivedTypes<Plant>(), InternalTools.TypeTools_DefaultDeclaredOnly,
                (info) => info.Name == nameof(Plant.Die));
        }

        [HarmonyPrefix]
        internal static void PreDie(Plant __instance, Plant.DieReason __0)
        {
            if (!__instance.TryGetBindEvents(out var list)) return;

            // 处理逻辑
            if (__0 != Plant.DieReason.ByFreeze && __0 != Plant.DieReason.Hid && __0 != Plant.DieReason.Wheel)
                list.DoAll(e => IPlantEvent.EventSingleMaps[IPlantEvent.EventType.DieEvent].Invoke(e, Trigger.Pre, [__0]));
            list.DoAll(e => IPlantEvent.EventSingleMaps[IPlantEvent.EventType.DieEventMustExecute].Invoke(e, Trigger.Pre, [__0]));
        }

        [HarmonyPostfix]
        internal static void PostDie(Plant __instance, Plant.DieReason __0)
        {
            if (!__instance.TryGetBindEvents(out var list)) return;

            // 处理逻辑
            if (__0 != Plant.DieReason.ByFreeze && __0 != Plant.DieReason.Hid && __0 != Plant.DieReason.Wheel)
                list.DoAll(e => IPlantEvent.EventSingleMaps[IPlantEvent.EventType.DieEvent].Invoke(e, Trigger.Post, [__0]));
            list.DoAll(e => IPlantEvent.EventSingleMaps[IPlantEvent.EventType.DieEventMustExecute].Invoke(e, Trigger.Post, [__0]));
        }
    }
}
