using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace SubspeciesEntry.BepInEx.Plants
{
    #region 鮟鱇鱼
    [HarmonyPatch(typeof(UltimateBigChomper))]
    public static class UltimateBigChomperPatch
    {
        [HarmonyPatch(nameof(UltimateBigChomper.Awake))]
        [HarmonyPostfix]
        public static void PostAwake(UltimateBigChomper __instance)
        {
            __instance.attributeFloat = 5f;
        }

        [HarmonyPatch(nameof(UltimateBigChomper.Chomp))]
        [HarmonyPrefix]
        public static void PreChomp(UltimateBigChomper __instance, ref Zombie zombie, out (int, int) __state)
        {
            var center = BigChomperBox.Center(__instance);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(center, BigChomperBox.Size(__instance), 0f, LayerMask.GetMask("Zombie"));
            var boxZombies = 0;
            var totalHealth = 0;
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<Zombie>(out var z) && z.IsObjExist())
                {
                    if (z.theZombieRow == __instance.thePlantRow)
                    {
                        if (!TypeMgr.IsBossZombie(z.theZombieType) && __instance.CheckZombie(z))
                            boxZombies++;
                        totalHealth += (int)z.CurrentAllHealth;   // 4.0 起 CurrentAllHealth 是 long（CS0266）
                    }
                }
            }
            __state = (boxZombies, totalHealth);
        }

        [HarmonyPatch(nameof(UltimateBigChomper.Chomp))]
        [HarmonyPostfix]
        public static void PostChomp(UltimateBigChomper __instance, ref Zombie zombie, (int, int) __state)
        {
            if (CoreTools.TravelUltimate("光芒四射"))
            {
                CreatePlant.Instance.AdjustLightLevel(__instance.thePlantColumn, __instance.thePlantRow, -2, 2); // 抵消原来的

                __instance.add = true;
                var boxZombies = __state.Item1;
                CreatePlant.Instance.AdjustLightLevel(__instance.thePlantColumn, __instance.thePlantRow, Mathf.Min(boxZombies + 1, 10), 4);
                __instance.SetData("UltimateBigChomper_level", Mathf.Min(boxZombies + 1, 10));
                __instance.SetData("UltimateBigChomper_Box", (__instance.thePlantColumn, __instance.thePlantRow));
                __instance.GetOrAddComponent<Timer>().AddTimer(10f, () =>
                {
                    var (column, row) = __instance.GetData<(int, int)>("UltimateBigChomper_Box");
                    CreatePlant.Instance.AdjustLightLevel(column, row, -Mathf.Min(boxZombies + 1, 10), 4);
                    __instance.add = false;
                });
            }
            if (CoreTools.TravelUltimate("深渊巨口"))
            {
                Collider2D[] colliders = Physics2D.OverlapBoxAll(BigChomperBox.Center(__instance), BigChomperBox.Size(__instance), 0f, LayerMask.GetMask("Zombie"));
                var zombies = new List<Zombie>();
                foreach (var collider in colliders)
                    if (collider.TryGetComponent<Zombie>(out var z) && z.IsObjExist() && z.theZombieRow == __instance.thePlantRow)
                        zombies.Add(z);
                if (zombies.Count <= 0)
                    return;
                var totalHealth = __state.Item2;
                var timer = 0.25f + 0.65f * totalHealth / (totalHealth + Mathf.Pow(10, 6));
                foreach (var z in zombies)
                {
                    var damage = z.TotalAllHealth * timer;
                    damage += CoreTools.GetReducedArmorDamage(z.theArmor, damage, z.theArmor * 0.9f);
                    z.TakeDamage((int)damage, __instance.Cast<IDamageMaker>(), DamageType.Normal, PlantType.UltimateBigChomper);
                }
                __instance.canToChew = true;

                __instance.anim.ResetTrigger("back");
                __instance.attributeCountdown = 90f;
            }
        }

        [HarmonyPatch(nameof(UltimateBigChomper.OnMove))]
        [HarmonyPostfix]
        public static void PostOnMove(UltimateBigChomper __instance, int originalColumn, int originalRow, int newColumn, int newRow)
        {
            if (__instance.add)
            {
                // 抵消原来的效果
                CreatePlant.Instance.AdjustLightLevel(originalColumn, originalRow, 2, 2);
                CreatePlant.Instance.AdjustLightLevel(newColumn, newRow, -2, 2);

                // 新
                __instance.SetData("UltimateBigChomper_Box", (newColumn, newRow));
                CreatePlant.Instance.AdjustLightLevel(originalColumn, originalRow, -__instance.GetData<int>("UltimateBigChomper_level"), 4);
                CreatePlant.Instance.AdjustLightLevel(newColumn, newRow, __instance.GetData<int>("UltimateBigChomper_level"), 4);
            }
        }

        [HarmonyPatch(nameof(UltimateBigChomper.TakeDamage))]
        [HarmonyPrefix]
        public static void PreTakeDamage(ref int damage)
        {
            if (damage > 2000)
                damage = 2000;
        }
    }

    [HarmonyPatch(typeof(UltimateFootballZombie))]
    public static class UltimateFootBallZombiePatch
    {
        [HarmonyPatch(nameof(UltimateFootballZombie.AttackEffect))]
        [HarmonyPrefix]
        public static void PreAttackEffect(ref Plant plant, out (bool, Plant.PlantTag) __state)
        {
            if (plant.IsObjExist() && plant.thePlantType is PlantType.UltimateBigChomper)
            {
                __state = (true, plant.plantTag);
                var tag = plant.plantTag;
                tag.nutPlant = true;
                plant.plantTag = tag;
            }
            else
                __state = (false, default);
        }

        [HarmonyPatch(nameof(UltimateFootballZombie.AttackEffect))]
        [HarmonyPostfix]
        public static void PostAttackEffect(ref Plant plant, (bool, Plant.PlantTag) __state)
        {
            if (__state.Item1)
            {
                plant.plantTag = __state.Item2;
            }
        }
    }

    [HarmonyPatch(typeof(Chomper))]
    public static class ChomperPatch
    {
        [HarmonyPatch(nameof(Chomper.ChompBack))]
        [HarmonyPrefix]
        public static void PreChompBack(Chomper __instance, out (bool, int) __state)
        {
            if (__instance.thePlantType is PlantType.UltimateBigChomper && CoreTools.TravelUltimate("深渊巨口"))
            {
                var center = BigChomperBox.Center(__instance);
                Collider2D[] colliders = Physics2D.OverlapBoxAll(center, BigChomperBox.Size(__instance), 0f, LayerMask.GetMask("Zombie"));
                var totalHealth = 0;
                foreach (var collider in colliders)
                {
                    if (collider.TryGetComponent<Zombie>(out var z) && z.IsObjExist())
                    {
                        if (z.theZombieRow == __instance.thePlantRow)
                        {
                            totalHealth += (int)z.CurrentAllHealth;   // 4.0 起 CurrentAllHealth 是 long（CS0266）
                        }
                    }
                }
                __state = (true, totalHealth);
                return;
            }
            __state = (false, 0);
        }

        [HarmonyPatch(nameof(Chomper.ChompBack))]
        [HarmonyPostfix]
        public static void PostChompBack(Chomper __instance, (bool, int) __state)
        {
            if (__state.Item1)
            {
                Collider2D[] colliders = Physics2D.OverlapBoxAll(BigChomperBox.Center(__instance), BigChomperBox.Size(__instance), 0f, LayerMask.GetMask("Zombie"));
                var zombies = new List<Zombie>();
                foreach (var collider in colliders)
                    if (collider.TryGetComponent<Zombie>(out var z) && z.IsObjExist() && z.theZombieRow == __instance.thePlantRow)
                        zombies.Add(z);
                if (zombies.Count <= 0)
                    return;
                var totalHealth = __state.Item2;
                var timer = 0.25f + 0.65f * totalHealth / (totalHealth + Mathf.Pow(10, 6));
                foreach (var z in zombies)
                {
                    var damage = z.TotalAllHealth * timer;
                    damage += CoreTools.GetReducedArmorDamage(z.theArmor, damage, z.theArmor * 0.9f);
                    z.TakeDamage((int)damage, __instance.Cast<IDamageMaker>(), DamageType.Normal, PlantType.UltimateBigChomper);
                }
                __instance.canToChew = true;

                __instance.anim.ResetTrigger("back");
                __instance.attributeCountdown = 90f;
            }
        }
    }
    #endregion

    /// <summary>
    /// 大嘴花吞噬判定盒的统一定义（全部按 4.0 原生口径还原，IDA 实锤）。
    /// 原生：Chomper$$ChomperSearchZombie @0x1804983D0
    ///   origin = axis.position + (1, 0)
    ///   Lawnf.GetBoxCollidersFromOrigin(origin, Vector2.right, get_ChompRange(), 2.0, zombieLayer)
    /// 4.0 删掉了 Chomper.centerOffset 与 Chomper.range（CS1061），
    /// 只剩单值 ChompRange（半宽）与写死的垂直半高 2.0。
    /// </summary>
    internal static class BigChomperBox
    {
        /// <summary>盒心：与原生一致，axis.position + (1,0)。</summary>
        internal static Vector3 Center(Chomper chomper) => chomper.axis.transform.position + new Vector3(1f, 0f, 0f);

        /// <summary>盒尺寸：宽 = ChompRange*2 + attributeFloat，高 = 2（ChompRange 是半宽，故乘 2）。</summary>
        internal static Vector2 Size(Chomper chomper) => new(chomper.ChompRange * 2f + chomper.attributeFloat, 2f);
    }
}
