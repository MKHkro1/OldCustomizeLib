using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using CustomizeLib.BepInEx.ExtensionData.Basic;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.Runtime;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace UltimateDoomSniperPea.BepInEx
{
    [BepInPlugin("salmon.ultimatedoomsniperpea", "UltimateDoomSniperPea", "1.0")]
    public class Core : CorePlugin
    {
        public override void OnStart()
        {
            var ab = CustomCore.GetAssetBundle(Tools.GetAssembly(), "ultimatedoomsniperpea");
            CustomCore.RegisterCustomPlant<DoomSniper, UltimateDoomSniperPea>(UltimateDoomSniperPea.PlantID, ab.GetAsset<GameObject>("UltimateDoomSniperPeaPrefab"),
                ab.GetAsset<GameObject>("UltimateDoomSniperPeaPreview"), new List<(int, int)>
                {
                    ((int)PlantType.DeathChomper, (int)PlantType.SniperPea)
                }, 0.3f, 0f, 300, 300, 7.5f, 600);
            CustomCore.AddPlantAlmanacStrings(UltimateDoomSniperPea.PlantID, $"邪神狙击射手",
                "发射激光的邪祟射手，献以獠牙召唤黑洞，撕裂僵尸造成范围杀伤\n" +
                "<color=#0000FF>死神大嘴花同人亚种</color>\n\n" +
                "<color=#3D1400>贴图作者：@林秋-AutumnLin</color>\n" +
                "<color=#3D1400>转化配方：</color><color=red>大嘴花←→狙击射手</color>\n" +
                "<color=#3D1400>伤害：</color><color=red>300/0.3秒</color>\n" +
                "<color=#3D1400>特点：</color><color=red>①攻击无限穿透\n" +
                "②每命中1个目标，获得1点大招充能，对造成伤害的目标施加1层獠牙\n" +
                "③消耗150层大招充能，启动大招。攻击速度提升100%，攻击力x2，每1秒从身后召唤8枚死神碎片追击攻击目标\n" +
                "④有10%概率命中弱点，伤害x6，若目标处于余烬状态或目标是巫毒虚影时，概率提升至100%</color>\n" +
                "<color=#3D1400>獠牙：</color><color=red>①獠牙上限为20层\n" +
                "②邪神狙击射手命中20层獠牙的目标时，消耗20层獠牙，对目标僵尸处：\n" +
                "·召唤一个黑洞，每0.15秒对半径1.5格范围造成1倍攻击力的伤害，持续3秒\n" +
                "·造成一次6倍攻击力的斩击\n" +
                "·召唤8枚死神碎片，并优先攻击该目标\n" +
                "③如果场上的黑洞达到20个，生成黑洞时，改为使最近的黑洞增加20%伤害比例</color>\n" +
                "<color=#3D1400>死神碎片：</color><color=red>伤害为1倍攻击力，对直击的目标降低1层护甲系数</color>\n" +
                "<color=#3D1400>词条1:</color><color=red>罪恶之力：大招和獠牙所需的层数减半</color>\n" +
                "<color=#3D1400>词条2:</color><color=red>深度定制：大招期间攻击力x3</color>\n\n" +
                "<color=#3D1400>“在曾经那道紫色光芒的惨剧下，我的家乡被彻底摧毁，”邪神狙击射手说到，“我曾试着想过祈祷，但无济于事。”那会最后，只有他被这股特别的力量所接受，两枚獠牙嵌入脸庞，在余烬下重获新生。“以牙还牙，以眼还眼”，这是他认定的真理，誓要找出曾经的凶手。</color>");
            CustomCore.RegisterCustomBullet<Bullet_cactus>(UltimateDoomSniperPea.BulletID, ab.GetAsset<GameObject>("Bullet_DoomPiece"));
            ab.GetAsset<GameObject>("DoomHunter").AddComponent<DoomHunter>();
            SetLayer(ab.GetAsset<GameObject>("DoomHunter").transform);
            UltimateDoomSniperPea.Hunter = ab.GetAsset<GameObject>("DoomHunter");
            ab.GetAsset<GameObject>("Piece").AddComponent<Piece>();
            UltimateDoomSniperPea.Pieces.Add(ab.GetAsset<GameObject>("Piece"));
            ab.GetAsset<GameObject>("Piece2").AddComponent<Piece>();
            UltimateDoomSniperPea.Pieces.Add(ab.GetAsset<GameObject>("Piece2"));
            CustomCore.AddUltimatePlant(UltimateDoomSniperPea.PlantID);
            CustomCore.RegisterCustomParticle(UltimateDoomSniperPea.LineID, ab.GetAsset<GameObject>("ShootFire"));
            CustomCore.AddFusion((int)PlantType.DeathChomper, UltimateDoomSniperPea.PlantID, (int)PlantType.Chomper);
        }

        public static void SetLayer(Transform transform)
        {
            var list = new List<Transform>
            {
                transform.FindChild("5"),
                transform.FindChild("5/已插入图像"),
                transform.FindChild("5/已插入图像_1"),
                transform.FindChild("6"),
                transform.FindChild("6/已插入图像"),
                transform.FindChild("6/已插入图像_1"),
                transform.FindChild("7"),
                transform.FindChild("7/已插入图像"),
                transform.FindChild("7/已插入图像_1")
            };
            foreach (var trans in list)
            {
                var renderer = trans.GetComponent<SpriteRenderer>();
                if (renderer != null)
                    renderer.sortingLayerName = "plant11";
                var group = trans.GetComponent<SortingGroup>();
                if (group != null)
                    group.sortingLayerName = "plant11";
            }
        }
    }

    public class UltimateDoomSniperPea : MonoBehaviour
    {
        public static ID PlantID = 1967;
        public static ID BulletID = 1968;
        public static ID LineID = 1967;
        public static List<GameObject?> Pieces = new();
        public static GameObject? Hunter;

        public static string Fang = "UltimateDoomSniperPea_Fang";

        public float fangTimer = 1f;
        public Piece? piece;

        public void Awake()
        {
            plant.shoot = transform.FindChild("PeaShooter_Head/gun_lower/Shoot");
        }

        public void Update()
        {
            if (plant != null)
            {
                plant.attributeCount = 0;
                if (plant.crazeTimer > 0f)
                {
                    fangTimer -= Time.deltaTime;

                    if (fangTimer <= 0f)
                    {
                        SetPiece();
                        fangTimer = 1f;
                    }
                }
                else
                    fangTimer = 0f;
            }
        }

        public void SetPiece(Zombie? zombie = null)
        {
            if (piece == null)
            {
                piece = Instantiate(Pieces[UnityEngine.Random.Range(0, Pieces.Count)], plant.axis.position, Quaternion.identity, plant.board.transform)?.
                    GetComponent<Piece>();
                if (piece != null)
                {
                    piece.target = zombie;
                    piece.damage = plant.attackDamage;
                }
            }
            else
            {
                piece.ExtraShoot(8);
            }
        }

        public void Shoot1()
        {
            GameAPP.PlaySound(140, 0.5f, UnityEngine.Random.Range(0.9f, 1.1f));

            var func = new Func<Zombie, bool>(plant.CheckZombie);

            var nearestZombie = Lawnf.GetNearestZombie(plant.board, plant.shoot.position, func);

            var line = CreateParticle.SetParticle(LineID, plant.shoot.position, 11);

            if (nearestZombie != null)
            {
                var zombieCollider = nearestZombie.col;
                Bounds bounds = zombieCollider.bounds;

                Vector3 direction = (bounds.center - plant.shoot.position).normalized;
                RaycastHit2D[] hits = Physics2D.RaycastAll(
                    plant.shoot.position,
                    direction,
                    float.MaxValue,
                    LayerMask.GetMask("Zombie")
                );

                line.transform.rotation = global::Core.Lawnf.GetRotateFromSpeed(direction);

                foreach (var hit in hits)
                {
                    if (hit.collider.IsObjExist() && hit.collider.TryGetComponent<Zombie>(out var zombie) && zombie.IsObjExist())
                    {
                        if (plant.crazeTimer <= 0f)
                            plant.craze++;
                        var num = CoreTools.TravelAdvanced("罪恶之力") ? 10 : 20;
                        if (zombie.GetData<int>(Fang) >= num)
                        {
                            var pos = zombie.axis.transform.position + new Vector3(0f, 0.5f, 0f);
                            var hunter = Instantiate(Hunter, pos, Quaternion.identity, plant.board.transform)?.GetComponent<DoomHunter>();
                            if (hunter != null)
                            {
                                hunter.damage = plant.attackDamage;
                            }
                            
                            SetPiece(zombie);
                            zombie.SetData(Fang, zombie.GetData<int>(Fang) - num);
                        }
                        if (zombie.GetData<int>(Fang) < 20)
                            zombie.SetData(Fang, zombie.GetData<int>(Fang) + 1);
                        var damage = plant.attackDamage;
                        if (plant.crazeTimer > 0f)
                            damage *= 2 * (CoreTools.TravelAdvanced("深度定制") ? 3 : 1);
                        if (UnityEngine.Random.Range(0, 10) == 5 || zombie.GetAttrTimers().isEmbered || zombie.theZombieType is ZombieType.VoodooDollZombie)
                            damage *= 6;
                        zombie.TakeDamage(DmgType.NormalAll, damage, plant.thePlantType);
                    }
                }

                if (plant.craze >= (CoreTools.TravelAdvanced("罪恶之力") ? 75 : 150))
                {
                    plant.crazeTimer = 8f;
                    plant.craze = 0;
                }
            }
        }

        public DoomSniper plant => gameObject.GetComponent<DoomSniper>();
    }

    public class Piece : MonoBehaviour
    {
        public List<Vector2> shoots = new();
        public int damage = 300;
        public Zombie? target;
        public int count = 0;
        public int extraShoot = 0;

        public void Awake()
        {
            for (int i = 2; i <= 9; i++)
                shoots.Add(transform.FindChild($"Shoot{i}").position);
        }

        public void Shoot()
        {
            if (CreateBullet.Instance != null)
            {
                var extra = extraShoot / (8 - count);
                for (int i = 0; i < extra + 1; i++)
                {
                    var bullet = CreateBullet.Instance.SetBullet(shoots[count].x, shoots[count].y, 0, UltimateDoomSniperPea.BulletID, BulletMoveWay.Track);
                    bullet.fromType = UltimateDoomSniperPea.PlantID;
                    bullet.Damage = damage;
                    bullet.targetZombie = target;
                    bullet.trackSpeed *= 5f;
                }
                extraShoot -= extra;
                count++;
                if (count >= 8)
                    Destroy(gameObject);
            }
        }

        public void ExtraShoot(int count) => extraShoot += count;
    }

    public class DoomHunter : MonoBehaviour
    {
        public static List<DoomHunter> instances = new();

        public float attackCountDown = 0.15f;
        public float liveTime = 3f;
        public bool die = false;
        public bool touch = false;
        public int damage = 300;
        public float coefficient = 1f;

        public void Awake()
        {
            if (instances.Count < 20)
            {
                instances.Add(this);
            }
            else
            {
                var hunter = FindNearst();
                if (hunter != null)
                    hunter.coefficient += 0.2f;
                touch = true;
                Destroy(gameObject);
                return;
            }
        }

        public DoomHunter FindNearst()
        {
            DoomHunter nearest = null;
            var minDistance = float.MaxValue;
            foreach (var hunter in instances)
            {
                if (hunter == null || hunter == this) continue;

                float distance = Vector3.SqrMagnitude(transform.position - hunter.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = hunter;
                }
            }
            return nearest;
        }

        public void Update()
        {
            if (this == null || die) return;
            liveTime -= Time.deltaTime;
            attackCountDown -= Time.deltaTime;
            if (attackCountDown <= 0f)
            {
                foreach (var collider in Physics2D.OverlapCircleAll(transform.position, CoreTools.ColumnX * 1.5f, LayerMask.GetMask("Zombie")))
                {
                    if (collider.IsObjExist() && collider.TryGetComponent<Zombie>(out var z) && z.IsObjExist())
                    {
                        z.TakeDamage(DmgType.NormalAll, (int)(damage * coefficient), UltimateDoomSniperPea.PlantID);
                    }
                }
                attackCountDown = 0.15f;
            }
            if (liveTime <= 0f)
            {
                die = true;
                Destroy(gameObject);
            }
        }

        public void Kill()
        {
            var pos = transform.position;
            foreach (var collider in Physics2D.OverlapCircleAll(pos, CoreTools.ColumnX * 1.5f, LayerMask.GetMask("Zombie")))
            {
                if (collider.IsObjExist() && collider.TryGetComponent<Zombie>(out var z) && z.IsObjExist())
                {
                    z.TakeDamage(DmgType.NormalAll, (int)(damage * coefficient * 6), UltimateDoomSniperPea.PlantID);
                }
            }
        }

        public void OnDestroy()
        {
            if (!touch && instances.Contains(this))
                instances.Remove(this);
        }
    }

    [HarmonyPatch(typeof(DoomSniper))]
    public static class DoomSniperPatch
    {
        [HarmonyPatch(nameof(DoomSniper.Shoot1))]
        [HarmonyPrefix]
        public static bool PreShoot1(DoomSniper __instance)
        {
            if (__instance.thePlantType == UltimateDoomSniperPea.PlantID)
            {
                if (__instance.TryGetComponent<UltimateDoomSniperPea>(out var sniper) && sniper.IsObjExist())
                    sniper.Shoot1();
                return false;
            }
            return true;
        }

        [HarmonyPatch(nameof(DoomSniper.CheckZombie))]
        [HarmonyPrefix]
        public static bool PreCheckZombie(DoomSniper __instance, ref Zombie zombie, ref bool __result)
        {
            if (__instance.thePlantType == UltimateDoomSniperPea.PlantID)
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
            return true;
        }

        [HarmonyPatch(nameof(DoomSniper.SearchZombie))]
        public static bool Prefix(DoomSniper __instance, ref GameObject __result)
        {
            if (__instance.thePlantType == UltimateDoomSniperPea.PlantID)
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
            return true;
        }
    }

    [HarmonyPatch(typeof(Bullet_cactus))]
    public static class Bullet_cactusPatch
    {
        [HarmonyPatch(nameof(Bullet_cactus.HitZombie))]
        [HarmonyPostfix]
        public static void PostHitZombie(Bullet_cactus __instance, ref Zombie zombie)
        {
            if (__instance.theBulletType == UltimateDoomSniperPea.BulletID)
            {
                if (zombie.theArmor > 0)
                    zombie.theArmor = zombie.theArmor - 1f;
            }
        }
    }
}
