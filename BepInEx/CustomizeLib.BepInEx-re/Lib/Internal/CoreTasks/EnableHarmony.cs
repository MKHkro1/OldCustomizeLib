using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.CoreTasks
{
    /// <summary>
    /// 启用HarmonyPatch
    /// </summary>
    internal struct EnableHarmony : ILoadTask
    {
        readonly void IModTask.Do() => Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }
}
