using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace SubspeciesEntry.BepInEx
{
    public static class Tools
    {
        public static bool IsObjExist(this Component component) => !(component == null || component.IsDestroyed() || component.gameObject == null || component.gameObject.IsDestroyed());
        public static bool IsTypeOf<T>(this Il2CppSystem.Object obj) => obj.GetIl2CppType() == Il2CppType.From(typeof(T));
        public static (BuffType, int) DestructBuffObject(this Il2CppSystem.Object buff)
        {
            var type = buff.GetIl2CppType();
            if (type.IsTypeOf<AdvBuff>()) return (BuffType.AdvancedBuff, (int)buff.Unbox<AdvBuff>());
            else if (type.IsTypeOf<UltiBuff>()) return (BuffType.UltimateBuff, (int)buff.Unbox<UltiBuff>());
            else if (type.IsTypeOf<TravelDebuff>()) return (BuffType.Debuff, (int)buff.Unbox<TravelDebuff>());
            else if (type.IsTypeOf<InvestBuff>()) return (BuffType.InvestmentBuff, (int)buff.Unbox<InvestBuff>());
            else if (type.IsTypeOf<TravelUnlocks>()) return (BuffType.UnlockPlant, (int)buff.Unbox<TravelUnlocks>());
            return ((BuffType)(-1), -1);
        }
    }

    public static class CoreTools
    {
        public static Dictionary<string, AdvBuff> AdvBuffPair = new();
        public static Dictionary<string, UltiBuff> UltiBuffPair = new();

        public static void Init()
        {
            foreach (var (buff, str) in TravelDictionary.advancedBuffsText)
            {
                int index = str.IndexOf('：');
                if (index == -1)
                    index = str.IndexOf(":");
                if (index != -1)
                {
                    if (!AdvBuffPair.ContainsKey(str.Substring(0, index)))
                        AdvBuffPair.Add(str.Substring(0, index), buff);
                }
            }

            foreach (var (buff, str) in TravelDictionary.ultimateBuffsText)
            {
                int index = str.IndexOf('：');
                if (index == -1)
                    index = str.IndexOf(":");
                if (index != -1)
                {
                    if (!UltiBuffPair.ContainsKey(str.Substring(0, index)))
                        UltiBuffPair.Add(str.Substring(0, index), buff);
                }
            }
        }

        public static AdvBuff GetAdvBuffByString(string name)
        {
            if (AdvBuffPair.ContainsKey(name))
                return AdvBuffPair[name];
            return (AdvBuff)(-1);
        }

        public static UltiBuff GetUltiBuffByString(string name)
        {
            if (UltiBuffPair.ContainsKey(name))
                return UltiBuffPair[name];
            return (UltiBuff)(-1);
        }

        public static bool TravelAdvanced(string name) => Lawnf.TravelAdvanced(GetAdvBuffByString(name));
        public static bool TravelUltimate(string name) => Lawnf.TravelUltimate(GetUltiBuffByString(name));
        public static int TravelUltimateLevel(string name) => Lawnf.TravelUltimateLevel(GetUltiBuffByString(name));

        /// <summary>
        /// 获取 若要将僵尸的护甲值视为(armor - reducedArmor)，应增加造成多少伤害
        /// </summary>
        /// <param name="armor">僵尸原护甲量</param>
        /// <param name="originDamage">原伤害</param>
        /// <param name="reducedArmor">要减免的护甲系数</param>
        /// <returns>应增加的伤害</returns>
        public static float GetReducedArmorDamage(float armor, float originDamage, float reducedArmor)
        {
            float b = 100 + armor - reducedArmor;
            if (b == 0) b = 1E-7f;
            // 增加伤害 = (原伤害 * 免疫护甲量) / (100 + 护甲 - 免疫护甲量)
            return (reducedArmor * originDamage) / b;
        }

        public static int GetStarProbability()
        {
            int total = 0;
            var config = GameAPP.config;
            if (config.levelZombieInRandom) total += 2;
            if (config.strongUltiZombieInRandom) total += 2;
            if (config.leaderInRandom) total += 6;
            return total;
        }
    }

    public static class PlantTools
    {
        public static float ColumnX
        {
            get
            {
                if (Mouse.Instance != null && !Mouse.Instance.IsDestroyed())
                    return Mouse.Instance.GetBoxXFromColumn(1) - Mouse.Instance.GetBoxXFromColumn(0);
                return 1f;
            }
        }
        public static float RowY
        {
            get
            {
                if (Mouse.Instance != null && !Mouse.Instance.IsDestroyed())
                    return Mouse.Instance.GetBoxYFromRow(1) - Mouse.Instance.GetBoxYFromRow(0);
                return 1f;
            }
        }
    }
}
