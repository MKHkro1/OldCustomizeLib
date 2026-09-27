using AlmanacData;
using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SubspeciesEntry.BepInEx
{
    public class GameInfo : MonoBehaviour
    {
        public static PlantInfo GetPlantInfo(PlantType plantType) => AlmanacDataLoader.GetPlantData(plantType);
        public static string GetBuffText(BuffType buffType, int id)
        {
            var type = typeof(AdvBuff);
            switch (buffType)
            {
                case BuffType.UnlockPlant:
                    type = typeof(TravelUnlocks);
                    break;
                case BuffType.AdvancedBuff:
                    type = typeof(AdvBuff);
                    break;
                case BuffType.UltimateBuff:
                    type = typeof(UltiBuff);
                    break;
                case BuffType.Debuff:
                    type = typeof(TravelDebuff);
                    break;
                case BuffType.InvestmentBuff:
                    type = typeof(InvestBuff);
                    break;
            }
            var obj = Il2CppSystem.Enum.ToObject(Il2CppType.From(type), id);
            return TravelMgr.Instance.GetText(obj);
        }

        public static bool TravelAdvanced(string name) => Lawnf.TravelAdvanced(CoreTools.GetAdvBuffByString(name));
        public static bool TravelUltimate(string name) => Lawnf.TravelUltimate(CoreTools.GetUltiBuffByString(name));
        public static int TravelUltimateLevel(string name) => Lawnf.TravelUltimateLevel(CoreTools.GetUltiBuffByString(name));
        public static Il2CppSystem.Collections.Generic.Dictionary<PlantType, GameLevel.RogueShooting.BaseConfig> Config() => GameLevel.RogueShooting.Config.configs;
    }
}
