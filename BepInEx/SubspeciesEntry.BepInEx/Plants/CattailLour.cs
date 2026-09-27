//using HarmonyLib;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.Rendering;

//namespace SubspeciesEntry.BepInEx.Plants
//{
//    #region 珞
//    [HarmonyPatch(typeof(LourFly))]
//    public static class LourFlyPatch
//    {
//        [HarmonyPatch(nameof(LourFly.ShootUpdate))]
//        [HarmonyPrefix]
//        public static bool PreShootUpdate(LourFly __instance)
//        {
//            __instance.timer -= Time.fixedDeltaTime;

//            if (__instance.timer <= 0f)
//            {
//                __instance.timer = UnityEngine.Random.Range(0.95f, 1.05f) * 0.1f * 5f; // 添加5f的系数降低攻击频率，增加留场时间

//                if (__instance.onion)
//                {
//                    __instance.shootCount += 6;
//                    __instance.timer *= 2f;
//                }

//                var bulletType = __instance.GetBullet();
//                var shoot = __instance.shootPos.position;

//                int damage = __instance.GetData<int>("LourFly_Damage");
//                var bullet = CreateBullet.Instance.SetBullet(shoot.x, shoot.y, __instance.theRow, bulletType, BulletMoveWay.Stable)?.
//                    GetComponent<Bullet_lourCactus>();
//                if (bullet != null)
//                {
//                    bullet.fromType = PlantType.CattailLour;
//                    bullet.lour = __instance.lour;
//                    bullet.Damage = damage;
//                    bullet.penetrationTimes = 10;
//                    bullet.normalSpeed *= 1.5f;
//                }

//                var upBullet = CreateBullet.Instance.SetBullet(shoot.x, shoot.y, __instance.theRow, bulletType, BulletMoveWay.Stable)?.
//                    GetComponent<Bullet_lourCactus>();
//                if (upBullet != null)
//                {
//                    upBullet.rb.velocity = new Vector2(-1f, 1f);
//                    upBullet.fromType = PlantType.CattailLour;
//                    upBullet.lour = __instance.lour;
//                    upBullet.Damage = damage;
//                    upBullet.penetrationTimes = 10;
//                    upBullet.transform.Rotate(new Vector3(0f, 0f, 2f));
//                    upBullet.normalSpeed *= 1.5f;
//                }

//                var downBullet = CreateBullet.Instance.SetBullet(shoot.x, shoot.y, __instance.theRow, bulletType, BulletMoveWay.Stable)?.
//                    GetComponent<Bullet_lourCactus>();
//                if (downBullet != null)
//                {
//                    downBullet.rb.velocity = new Vector2(-1f, -1f);
//                    downBullet.fromType = PlantType.CattailLour;
//                    downBullet.lour = __instance.lour;
//                    downBullet.Damage = damage;
//                    downBullet.penetrationTimes = 10;
//                    downBullet.transform.Rotate(new Vector3(0f, 0f, -2f));
//                    downBullet.normalSpeed *= 1.5f;
//                }

//                __instance.shootCount += 3;
//            }

//            if (__instance.shootCount >= 30)
//                UnityEngine.Object.Destroy(__instance.gameObject);
//            return false;
//        }
//    }

//    [HarmonyPatch(typeof(Bullet_lourCactus))]
//    public static class Bullet_lourCactusPatch
//    {
//        [HarmonyPatch(nameof(Bullet_lourCactus.HitZombie))]
//        [HarmonyPrefix]
//        public static void PreHitZombie(Bullet_lourCactus __instance, ref Zombie zombie, out (bool, int, bool) __state)
//        {
//            var isAir = TypeMgr.IsAirZombie(zombie.theZombieType) || !Lawnf.InLandStatus(zombie.theStatus);
//            if (isAir) __instance.Damage *= 10;
//            __state = (zombie.beforeDying, zombie.theHealth, isAir);
//        }

//        [HarmonyPatch(nameof(Bullet_lourCactus.HitZombie))]
//        [HarmonyPostfix]
//        public static void PostHitZombie(Bullet_lourCactus __instance, ref Zombie zombie, (bool, int, bool) __state) // wasDying, theHealth, isAir
//        {
//            if ((!__state.Item1 && zombie.beforeDying) || (zombie.theHealth <= 0 && __state.Item2 > 0 && !zombie.beforeDying))
//            {
//                zombie.SetData("CattailLour_KillByLour", true);
//                if (CoreTools.TravelAdvanced("我是梦珞") && __instance.lour != null)
//                    __instance.lour.SetData("CattailLour_ExtDamage", zombie.TotalAllHealth * 0.1f);
//            }
//            if (__state.Item3) __instance.Damage /= 10;
//            __instance.Damage = (int)(__instance.Damage * 0.9f);
//            __instance.Damage = Mathf.Max(__instance.Damage, 1);
//        }

//        [HarmonyPatch(nameof(Bullet_lourCactus.Update))]
//        [HarmonyPostfix]
//        public static void PostUpdate(Bullet_lourCactus __instance)
//        {
//            if (__instance.MoveWay == BulletMoveWay.MoveRight && __instance.lourTimer >= 0.2f)
//            {
//                __instance.MoveWay = BulletMoveWay.Freefly;
//            }
//        }
//    }

//    [HarmonyPatch(typeof(Zombie))]
//    public static class Zombie_CattailLour_Patch
//    {
//        [HarmonyPatch(nameof(Zombie.Die))]
//        [HarmonyPostfix]
//        public static void PostDie(Zombie __instance)
//        {
//            if (__instance.board != null && !__instance.GetData<bool>("CattailLour_KillByLour") && !__instance.GetData<bool>("CattailLour_Added"))
//            {
//                var current = __instance.board.GetData<int>("CattailLour_KilledZombieCount");
//                var need = (CoreTools.TravelAdvanced("我也是梦珞") ? 40 : 60);
//                if (current + 1 >= need)
//                {
//                    __instance.board.SetData("CattailLour_KilledZombieCount", current + 1 - need);
//                    foreach (var pt in Lawnf.GetAllPlants())
//                    {
//                        if (!pt.IsObjExist()) continue;
//                        if (pt.thePlantType != PlantType.CattailLour) continue;
//                        if (CoreTools.TravelAdvanced("我是梦珞"))
//                            pt.SetData("CattailLour_ExtDamage", __instance.TotalAllHealth * 0.1f);
//                        pt.GetComponent<CattailLour>().Supply();
//                    }
//                }
//                else
//                    __instance.board.SetData("CattailLour_KilledZombieCount", current + 1);
//                __instance.SetData("CattailLour_Added", true);
//            }
//        }
//    }

//    [HarmonyPatch(typeof(Plant))]
//    public static class Plant_CattailLour_Patch
//    {
//        [HarmonyPatch(nameof(Plant.Awake))]
//        [HarmonyPostfix]
//        public static void PostAwake(Plant __instance)
//        {
//            if (__instance.thePlantType == PlantType.CattailLour)
//            {
//                __instance.attributeCountdown = 15f;
//            }
//        }

//        [HarmonyPatch(nameof(Plant.Update))]
//        [HarmonyPostfix]
//        public static void PostUpdate(Plant __instance)
//        {
//            if (__instance.thePlantType == PlantType.CattailLour)
//            {
//                if (__instance.attributeCountdown - Time.deltaTime <= 0f)
//                {
//                    __instance.GetComponent<CattailLour>().Supply();
//                    __instance.attributeCountdown = 15f;
//                }
//            }
//        }
//    }

//    [HarmonyPatch(typeof(CattailLour))]
//    public static class CattailLourPatch
//    {
//        [HarmonyPatch(nameof(CattailLour.Supply))]
//        [HarmonyPrefix]
//        public static bool PreSupply(CattailLour __instance, bool onion)
//        {
//            var flyDamage = __instance.attackDamage * 2 + (int)__instance.GetData<float>("CattailLour_ExtDamage");
//            if (__instance.GetData<LourFly>("CattailLour_fly") != null)
//            {
//                var exist = __instance.GetData<LourFly>("CattailLour_fly");
//                exist.shootCount -= onion ? 12 : 30;
//                exist.SetData("LourFly_Damage", 
//                    exist.GetData<int>("LourFly_Damage") + (int)((exist.GetData<int>("LourFly_Damage") + flyDamage) / (60f - exist.shootCount)));
//                return false;
//            }
//            var fly = UnityEngine.Object.Instantiate(__instance.flyPrefab, __instance.flyPos.position + new Vector3(0f, 2.5f), Quaternion.identity, __instance.board.transform).
//                GetComponent<LourFly>();
//            fly.theRow = __instance.thePlantRow;
//            fly.targetPosition = __instance.flyPos.position;
//            fly.lour = __instance;
//            fly.onion = onion;
//            fly.AddComponent<SortingGroup>().sortingLayerName = $"bullet{__instance.thePlantRow}";
//            fly.SetData("LourFly_Damage", flyDamage);
//            __instance.SetData("CattailLour_ExtDamage", 0f);
//            __instance.SetData("CattailLour_fly", fly);
//            return false;
//        }
//    }
//    #endregion
//}
