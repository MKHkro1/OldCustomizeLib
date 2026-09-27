using CustomizeLib.BepInEx.Internal.Datas;
using CustomizeLib.BepInEx.Internal.Tools;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomizeLib.BepInEx.Internal.Patches
{
    [HarmonyPatch]
    internal static class TypeMgr_Extra
    {
        [HarmonyTargetMethods]
        internal static IEnumerable<MethodBase> GetTargetMethods()
        {
            return InternalTools.TypeTools_GetAllMethods([typeof(TypeMgr)], InternalTools.TypeTools_DefaultDeclaredOnly, (info) =>
            {
                return info.GetParameters().Length >= 1 && info.ReturnType == typeof(bool);
            });
        }

        [HarmonyPostfix]
        internal static void Postfix(MethodBase __originalMethod, object __0, ref bool __result)
        {
            if (__0 is PlantType plantType)
            {
                // result 额外附加 TypeExtraMgr 里指定的 Tag
                if (RegData.Instance.TypeExtraMgr.CustomPlantTypeDatas.TryGetValue(plantType, out var set))
                    __result |= set.Contains(__originalMethod.Name);
            }
            else if (__0 is ZombieType zombieType)
            {
                // result 额外附加 TypeExtraMgr 里指定的 Tag
                if (RegData.Instance.TypeExtraMgr.CustomZombieTypeDatas.TryGetValue(zombieType, out var set))
                    __result |= set.Contains(__originalMethod.Name);
            }
        }
    }
}
