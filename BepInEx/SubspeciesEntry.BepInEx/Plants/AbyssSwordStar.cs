using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SubspeciesEntry.BepInEx.Plants
{
    [HarmonyPatch(typeof(AbyssSwordStar))]
    public static class AbyssSwordStarPatch
    {
        [HarmonyPatch(nameof(AbyssSwordStar.Awake))]
        [HarmonyPostfix]
        public static void PostAwake(AbyssSwordStar __instance)
        {
            if (GameAPP.theGameStatus == GameStatus.InGame && UnityEngine.Random.Range(1, 101) <= CoreTools.GetStarProbability())
            {
                __instance.StarUp();
                __instance.starUp = true;
                __instance.UpdateStarIcon();
            }
            __instance.StartCoroutine(SetDamage().WrapToIl2Cpp());
            IEnumerator SetDamage()
            {
                yield return null;
                if (CoreTools.TravelAdvanced("血淬剑芒"))
                    __instance.attackDamage *= 3;
                yield break;
            }
        }

        [HarmonyPatch(nameof(AbyssSwordStar.SetSwords))]
        [HarmonyPrefix]
        public static bool PreSetSwords(AbyssSwordStar __instance)
        {
            if (UnityEngine.Random.Range(1, 101) <= 1 && CoreTools.TravelAdvanced("无限剑制"))
            {
                var list = Lawnf.GetAllZombies().ToArray().Where(z => z != null && !z.beforeDying).ToList(); // 去除null的僵尸
                foreach (var zombie in list)
                {
                    var pos = new Vector3(UnityEngine.Random.Range(-3f, 8f), __instance.board.boardMaxY + 4f, 0f);
                    var sword = UnityEngine.Object.Instantiate(__instance.bigSwordPrefab, pos, Quaternion.identity, __instance.board.transform).
                        GetComponent<BigSword>();
                    __instance.bigSwords.Add(sword);
                    sword.plant = __instance;

                    Action<Vector2> action = (pos) =>
                    {
                        for (int i = 0; i < 5; i++)
                            __instance.AttackZombie(pos);
                    };
                    sword.action = action;

                    sword.targetPosition = zombie.transform.position;
                    sword.targetPosition += new Vector2(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-1f, 1f));
                    int row = Mouse.Instance.GetRowFromY(sword.targetPosition.x, sword.targetPosition.y);
                    sword.sortingGroup.sortingLayerName = $"bullet{row}";
                    sword.SetData("BigSword_Inited", true);
                }
            }
            else
            {
                var pos = new Vector3(UnityEngine.Random.Range(-3f, 8f), __instance.board.boardMaxY + 4f, 0f);
                var sword = UnityEngine.Object.Instantiate(__instance.bigSwordPrefab, pos, Quaternion.identity, __instance.board.transform).
                    GetComponent<BigSword>();
                __instance.bigSwords.Add(sword);
                sword.plant = __instance;

                Action<Vector2> action = (pos) =>
                {
                    for (int i = 0; i < 5; i++)
                        __instance.AttackZombie(pos);
                };
                sword.action = action;
            }
            return false;
        }

        [HarmonyPatch(nameof(AbyssSwordStar.AttackZombie))]
        [HarmonyPrefix]
        public static bool PreAttackZombie(AbyssSwordStar __instance, Vector2 position)
        {
            foreach (var collider in Physics2D.OverlapCircleAll(position, 1.5f, __instance.zombieLayer))
            {
                if (collider.IsObjExist() && collider.TryGetComponent<Zombie>(out var zombie) && zombie.IsObjExist())
                {
                    // 4.0 的 Lawnf 已经没有 TryGetStrikeDamage（CS0117）。已确认是彻底删除、不是改名：
                    // 整个 4.0 GameAssembly.dll.asm 里 "TryGetStrikeDamage" / "StrikeDamage" 命中 0 次，
                    // 4.0 反编译源码里也没有任何 Strike 相关方法。
                    // 这里退回直接用攻击力原值（后续的 ×5 分支不变）——功能差异需实机确认。
                    int damage = __instance.attackDamage;
                    if (zombie.theFirstArmorHealth <= 0 && zombie.theSecondArmorHealth <= 0 && CoreTools.TravelAdvanced("血淬剑芒"))
                        damage *= 5;
                    zombie.TakeDamage(damage, __instance.Cast<IDamageMaker>(), DamageType.NormalAll, __instance.thePlantType);
                }
            }

            GameAPP.PlaySound(134, 0.5f, UnityEngine.Random.Range(1.2f, 1.4f));

            ParticleManager.Instance.SetParticle(ParticleType.DirtSplat2, position, Mouse.Instance.GetRowFromY(position.x, position.y));
            return false;
        }
    }

    [HarmonyPatch(typeof(AbyssSword))]
    public static class AbyssSwordPatch
    {
        [HarmonyPatch(nameof(AbyssSword.OnTriggerStay2D))]
        [HarmonyPrefix]
        public static bool PreOnTriggerStay2D(AbyssSword __instance, ref Collider2D collision)
        {
            if (__instance.plant != null && __instance.plant.theStatus == PlantStatus.AbyssSwordStar_attacking && collision.IsObjExist() &&
                collision.TryGetComponent<Zombie>(out var zombie) && zombie.IsObjExist())
            {
                if (Mathf.Abs(zombie.theZombieRow - __instance.plant.thePlantRow) <= 1 && __instance.plant.CheckStatus(zombie))
                {
                    var damage = __instance.plant.attackDamage;
                    if (zombie.theFirstArmorHealth <= 0 && zombie.theSecondArmorHealth <= 0)
                        damage *= 5;
                    zombie.TakeDamage(damage, __instance.Cast<IDamageMaker>(), DamageType.NormalAll, __instance.plant.thePlantType);
                }
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(BigSword))]
    public static class BigSwordPatch
    {
        [HarmonyPatch(nameof(BigSword.AttackZombies))]
        [HarmonyPrefix]
        public static bool PreAttackZombies(BigSword __instance)
        {
            if (__instance.GetData<bool>("BigSword_Inited"))
                return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(SwordStarfruit))]
    public static class SwordStarfruitPatch
    {
        [HarmonyPatch(nameof(SwordStarfruit.SearchZombie))]
        [HarmonyPrefix]
        public static bool PreSearchZombie(SwordStarfruit __instance, ref GameObject __result)
        {
            if (__instance.thePlantType == PlantType.AbyssSwordStar)
            {
                var list = Lawnf.GetAllZombies().ToArray().Where(z => z != null && !z.beforeDying).ToList(); // 去除null的僵尸
                if (list.Count > 0)
                    __result = list[0].gameObject;
                else
                    __result = null;
                return false;
            }
            return true;
        }
    }
}
