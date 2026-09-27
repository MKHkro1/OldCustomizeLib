using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using Core;

namespace MakeDoomSniperGreatAgain.BepInEx
{
    [BepInPlugin("salmon.mdsga", "MakeDoomSniperGreatAgain", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(DoomSniper))]
    public static class DoomSniperPatch
    {
        [HarmonyPatch(nameof(DoomSniper.CheckZombie))]
        [HarmonyPrefix]
        public static bool PreCheckZombie(DoomSniper __instance, ref Zombie zombie, ref bool __result)
        {
            if (zombie == null || zombie.isMindControlled)
            {
                __result = false;
                return false;
            }

            var col = zombie.col;
            if (col == null)
            {
                __result = false;
                return false;
            }
            if (!col.enabled)
            {
                __result = false;
                return false;
            }

            if (zombie.axis.position.x <= __instance.shoot.position.x)
            {
                __result = false;
                return false;
            }
            __result = true;
            return false;
        }

        [HarmonyPatch(nameof(DoomSniper.SearchZombie))]
        [HarmonyPrefix]
        public static bool PreSearchZombie(DoomSniper __instance, ref GameObject __result)
        {
            foreach (var zombie in Lawnf.GetAllZombies())
            {
                if (zombie != null)
                {
                    Vector3 zombiePos = zombie.transform.position;

                    // 检查僵尸是否在视野范围内（X坐标大于视野阈值）
                    if (__instance.vision > zombiePos.x)
                    {
                        Vector3 shootPos = __instance.shoot.position;

                        if (zombiePos.x > shootPos.x)
                        {
                            if ((__instance.SearchUniqueZombie(zombie) && Lawnf.InLandStatus(zombie.theStatus)) || zombie.theStatus == ZombieStatus.Flying)
                            {
                                __result = zombie.gameObject;
                                return false;
                            }
                        }
                    }
                }
            }
            __result = null;
            return false;
        }

        [HarmonyPatch(nameof(DoomSniper.Shoot1))]
        [HarmonyPrefix]
        public static bool PreShoot1(DoomSniper __instance)
        {
            if (__instance != null && __instance.thePlantType == PlantType.DoomSniper)
            {
                GameAPP.PlaySound(140, 0.5f, UnityEngine.Random.Range(0.9f, 1.1f));

                Func<Zombie, bool> func = __instance.CheckZombie;
                Zombie nearestZombie = Lawnf.GetNearestZombie(
                    __instance.board,
                    __instance.shoot.position,
                    func);

                var shootFire = ParticleManager.Instance.SetParticle(ParticleType.ShootFire, __instance.shoot.position, 11);

                if (nearestZombie != null)
                {
                    if (nearestZombie.col == null)
                        return false;
                    Vector2 direction = (nearestZombie.col.bounds.center - __instance.shoot.position).normalized;

                    RaycastHit2D[] hits = Physics2D.RaycastAll(
                        __instance.shoot.position,
                        direction,
                        float.MaxValue,
                        __instance.zombieLayer);

                    shootFire.transform.rotation = global::Core.Lawnf.GetRotateFromSpeed(direction);

                    int damage = __instance.attackDamage;
                    if (UnityEngine.Random.Range(0, 10) == 5)
                        damage *= 6;

                    var hitCount = 0;
                    foreach (var hit in hits)
                    {
                        if (hit.collider.TryGetComponent(out Zombie zombie))
                        {
                            var hasEmber = zombie.TryGetEffect<EmberEffect>(EffectType.Ember, out var _);
                            int finalDamage = hasEmber ? damage * 6 : damage;
                            zombie.TakeDamage(finalDamage, null, DamageType.NormalAll, __instance.thePlantType);
                            hitCount++;

                            if (__instance.crazeTimer == 0f)
                                __instance.craze++;
                        }
                    }

                    __instance.attributeCount += hitCount;

                    if (__instance.attributeCount >= 300)
                    {
                        __instance.attributeCount = 0;
                        __instance.board.boardAction.SetDoom(nearestZombie.Column, nearestZombie.theZombieRow, false, false, default, __instance.attackDamage * 72,
                            0, null, true, __instance.thePlantType);
                    }

                    if (__instance.craze >= 100)
                    {
                        __instance.crazeTimer = 8f;
                        __instance.craze = 0;
                    }

                }

                return false;
            }
            return true;
        }
    }
}
