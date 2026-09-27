using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Hook;
using CustomizeLib.BepInEx.Internal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Mod
{
    [BepInPlugin("salmon.inf75.pvzcustomization.remake", "PVZCustomization-Remake", "3.9")]
    [BepInPriority(BepInPriority.High + 1)]
    internal class ModCore : BasePlugin
    {
        internal static ModCore Instance { get; private set; } = null!;

        public override void Load()
        {
            Instance = this;

            var methods = InternalTools.TypeTools_GetAllMethods(
                types: InternalTools.TypeTools_GetAllDerivedTypes<IModTask>(InternalTools.TypeTools_GetAllAssemblies()),
                filter: (t) => InternalTools.TypeTools_TryGetMethodWithFlag(t, "RunAll", InternalTools.TypeTools_DefaultDeclaredOnly));
            foreach (var method in methods)
            {
                try
                {
                    method.Invoke(null, []);
                }
                catch (Exception ex)
                {
                    Logger.LogError(
                        $"Error in execute IModTask: " +
                        $"Message: {ex.Message}" +
                        $"{ex.StackTrace}");
                }
            }
        }
    }
}
