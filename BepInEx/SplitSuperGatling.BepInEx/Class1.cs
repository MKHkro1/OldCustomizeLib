using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using UnityEngine;

namespace SplitSuperGatling.BepInEx
{
    [BepInPlugin("salmon.splitsupergatling", "SplitSuperGatling", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<SplitSuperGatling>(); 
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "splitsupergatling");
            CustomCore.RegisterCustomPlant<SuperGatling, SplitSuperGatling>(
                SplitSuperGatling.PlantID,
                ab.GetAsset<GameObject>("SplitSuperGatlingPrefab"),
                ab.GetAsset<GameObject>("SplitSuperGatlingPreview"),
                new List<(int, int)>
                {
                    ((int)PlantType.SuperGatling, (int)PlantType.SuperGatling)
                },
                1.5f, 0f, 20, 300, 7.5f, 1200
            );
            CustomCore.AddUltimatePlant((PlantType)SplitSuperGatling.PlantID);
            CustomCore.AddPlantAlmanacStrings(SplitSuperGatling.PlantID,
                $"超级裂荚机枪射手",
                "发射豌豆的裂荚超级机枪，有概率旋转发射大量豌豆\n\n" +
                "<color=#3D1400>贴图作者：@林秋-AutumnLin</color>\n" +
                "<color=#3D1400>融合配方：</color><color=red>超级机枪射手+超级机枪射手</color>\n" +
                "<color=#3D1400>伤害：</color><color=red>20x18/1.5秒</color>\n" +
                "<color=#3D1400>特点：</color><color=red>每次发射有2%概率开大，5秒内，每0.02秒向周围发射12颗伤害60的豌豆\n\n" +
                "<color=#3D1400>“嘿，我要热狗！双份～”超级裂荚机枪射手最喜欢的就是双份早餐，他家里每一样东西都是双份，双份电视，双份桌子，还有双份的床。“嘿，我们喜欢成对的，就像我们自己！”</color>"
            );
        }
    }

    public class SplitSuperGatling : MonoBehaviour
    {
        public static int PlantID = 1955;

        public SuperGatling plant => gameObject.GetComponent<SuperGatling>();
        public void Awake()
        {
            plant.shoot = transform.FindChild("SplitGatling_head/shooting/Shoot"); //右
            plant.shoot2 = transform.FindChild("SplitGatling_head/shooting/Shoot (1)"); //左
        }

        public void AnimShoot_All()
        {
            if (UnityEngine.Random.Range(0, 100) < 2 || plant.keepShooting)
            {
                plant.timer = 5f;
                plant.flashCountDown = 5f;
                AttributeEvent();
                if (plant.anim != null) plant.anim.SetBool("shooting", true);
                if (!plant.keepShooting) plant.Recover(plant.thePlantMaxHealth);
                return;
            }
            Shoot(true);
        }

        public void AnimShoot_Left()
        {
            if (UnityEngine.Random.Range(0, 100) < 2 || plant.keepShooting)
            {
                plant.timer = 5f;
                plant.flashCountDown = 5f;
                AttributeEvent();
                if (plant.anim != null) plant.anim.SetBool("shooting", true);
                if (!plant.keepShooting) plant.Recover(plant.thePlantMaxHealth);
                return;
            }
            Shoot();
        }

        public void Shoot(bool all = false)
        {
            if (plant.shoot != null)
            {
                {
                    var bullet = CreateBullet.Instance.SetBullet(plant.shoot2.transform.position.x, plant.shoot2.transform.position.y, plant.thePlantRow,
                        plant.GetBulletType(), BulletMoveWay.Split_left);
                    bullet.Damage = plant.attackDamage;
                    bullet.fromType = plant.thePlantType;
                    GameAPP.PlaySound(UnityEngine.Random.Range(3, 5), 0.5f, 1f);
                }

                if (all)
                {
                    var bullet = CreateBullet.Instance.SetBullet(plant.shoot.transform.position.x + 0.1f, plant.shoot.transform.position.y, plant.thePlantRow,
                    plant.GetBulletType(), BulletMoveWay.MoveRight);
                    bullet.Damage = plant.attackDamage;
                    bullet.fromType = plant.thePlantType;
                }
            }
        }

        public void AttributeEvent()
        {
            float x = plant.axis.transform.position.x;
            float y = plant.axis.transform.position.y + 0.5f;
            for (int i = 0; i < 12; i++)
            {
                SuperShoot(UnityEngine.Random.Range(12f, 14f), x + UnityEngine.Random.Range(-0.1f, 0.1f), y + UnityEngine.Random.Range(-0.1f, 0.1f));
            }

            if (plant.timer <= 0f && !plant.keepShooting)
            {
                plant.attributeCountdown = 0f;
                if (plant.anim != null)
                {
                    plant.anim.SetBool("shooting", false);
                }
            }
            else
            {
                plant.attributeCountdown = 0.02f;
            }
        }

        public void SuperShoot(float speed, float x, float y)
        {
            var bullet = CreateBullet.Instance.SetBullet(x, y, plant.thePlantRow, BulletType.Bullet_pea, BulletMoveWay.Free);
            bullet.transform.Rotate(0, 0, UnityEngine.Random.Range(0f, 360f));
            bullet.normalSpeed = speed;
            bullet.Damage = plant.attackDamage * 3;
            bullet.fromType = plant.thePlantType;
        }
    }

    [HarmonyPatch(typeof(SuperGatling))]
    public static class SuperGatlingPatch
    {
        [HarmonyPatch(nameof(SuperGatling.GetBulletType))]
        [HarmonyPrefix]
        public static bool PreGetBulletType(SuperGatling __instance, ref BulletType __result)
        {
            if ((int)__instance.thePlantType == SplitSuperGatling.PlantID)
            {
                __result = BulletType.Bullet_pea;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SuperSnowGatling), nameof(SuperSnowGatling.AttributeEvent))]
    public static class SuperSnowGatling_AttributeEvent_Patch
    {
        [HarmonyPrefix]
        public static bool PreAttributeEvent(SuperGatling __instance)
        {
            if ((int)__instance.thePlantType == SplitSuperGatling.PlantID)
            {
                if (__instance != null && __instance.TryGetComponent<SplitSuperGatling>(out var gatling) && gatling != null)
                {
                    gatling.AttributeEvent();
                }
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.SearchZombie))]
    public static class Plant_SearchZombie_Patch
    {
        [HarmonyPrefix]
        public static bool PreSearchZombie(Plant __instance, ref GameObject __result)
        {
            if ((int)__instance.thePlantType == SplitSuperGatling.PlantID)
            {
                if (__instance.board == null || __instance.board.zombieArray == null) return false;
                foreach (var zombie in __instance.board.zombieArray)
                {
                    if (zombie != null && zombie.theZombieRow == __instance.thePlantRow)
                    {
                        if (__instance.SearchUniqueZombie(zombie))
                        {
                            __result = zombie.gameObject;
                            return false;
                        }
                    }
                }
                return false;
            }
            return true;
        }
    }
}
