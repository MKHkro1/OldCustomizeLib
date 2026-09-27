using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Preloader.Core.Patching;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

[PatcherPluginInfo("salmon.bepinpriority", "BepInPriority", "1.0")]
public class Core : BasePatcher
{
    public override void Initialize()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        base.Initialize();
    }
}

[HarmonyPatch]
public static class IL2CPPChainloaderPatch
{
    [HarmonyTargetMethod]
    public static MethodBase GetTargetMethod()
    {
        // 指定Patch IL2CPPChainloader的ModifyLoadOrder方法
        return typeof(IL2CPPChainloader).BaseType!.
            GetMethod("ModifyLoadOrder", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)!;
    }

    [HarmonyPostfix]
    public static void PostLoadPlugins(ref IList<PluginInfo> __result)
    {
        var priorities = new Dictionary<PluginInfo, int>();
        foreach (var plugin in __result)
        {
            var asm = Assembly.LoadFrom(plugin.Location);
            var attr = asm.GetType(plugin.TypeName)!.GetCustomAttribute<BepInPriority>();
            if (attr != null) priorities[plugin] = attr.priority;
            else priorities[plugin] = BepInPriority.Default;
        }
        __result = [.. __result.OrderBy(plugin => priorities[plugin])];
    }
}