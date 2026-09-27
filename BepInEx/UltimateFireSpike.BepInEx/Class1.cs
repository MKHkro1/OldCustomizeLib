using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Unity.VisualScripting;
using UnityEngine;

namespace UltimateFireSpike.BepInEx
{
    [BepInPlugin("salmon.ultimatefirespike", "UltimateFireSpike", "1.0")]
    public class Core : CorePlugin
    {
        public override void OnStart()
        {
            var ab = CustomCore.GetAssetBundle(Tools.GetAssembly(), "ultimatefirespike");
            CustomCore.RegisterCustomPlant<CaltropTorch, UltimateFireSpike>(UltimateFireSpike.PlantID,
                ab.GetAsset<GameObject>("UltimateFireSpikePrefab"),
                ab.GetAsset<GameObject>("UltimateFireSpikePreview"), new List<(int, int)>
                {
                    ((int)PlantType.UltimateTorch, (int)PlantType.Caltrop)
                }, 1f, 0f, 300, 300, 7.5f, 600);
            CustomCore.RegisterCustomPlantSkin<CaltropTorch, UltimateFireSpike>(UltimateFireSpike.PlantID,
                ab.GetAsset<GameObject>("UltimateFireSpikePrefabSkin"),
                ab.GetAsset<GameObject>("UltimateFireSpikePreviewSkin"), new List<(int, int)>
                {
                    ((int)PlantType.UltimateTorch, (int)PlantType.Caltrop)
                }, 0f, 0f, 300, 300, 7.5f, 600);
            CustomCore.AddPlantAlmanacStrings(UltimateFireSpike.PlantID, $"究极地刺窝炬",
                "烧烤着迷你爆炸窝瓜的地刺树桩，点燃能力极强，还能生成弹跳的迷你火爆窝瓜。\n" +
                "<color=#0000FF>究极火爆窝炬同人亚种</color>\n\n" +
                "<color=#3D1400>贴图作者：@林秋-AutumnLin</color>\n" +
                "<color=#3D1400>转化配方：</color><color=red>地刺←→火炬树桩</color>\n" +
                "<color=#3D1400>伤害：</color><color=red>300/1秒</color>\n" +
                "<color=#3D1400>特点：</color><color=red>①点燃除迷你爆炸樱桃的低矮平射子弹：伤害x2\n" +
                "②影响经过的的迷你爆炸樱桃：变为迷你爆炸窝瓜子弹，弹跳时造成半径1格范围的樱桃爆炸\n" +
                "③每影响80发子弹，在原地生产一个300伤害的迷你火爆窝瓜，第6次落地时释放火爆辣椒效果，6次弹跳后消失</color>\n" +
                "<color=#3D1400>词条1:</color><color=red>事半功倍：生成迷你火爆窝瓜子弹需求降至40</color>\n" +
                "<color=#3D1400>词条2:</color><color=red>窝红温了：迷你火爆窝瓜伤害x3，每3次弹跳召唤一次火爆辣椒效果</color>\n" +
                "<color=#3D1400>连携词条：</color><color=red>火力全开：究极火爆窝炬地刺生成的迷你火爆窝瓜效果为全场生成火爆辣椒效果</color>\n\n" +
                "<color=#3D1400>究极地刺窝炬不喜欢出门，植物们管他叫“宅窝”，因为他极少出门。“我并不是喜欢待在家里，每次出门都会给我带来很多困扰，”究极地刺窝炬继续说到，“嘿！我在这边，我就知道！”或许你知道究极地刺窝炬为啥不喜欢出门了。</color>");
            CustomCore.TypeMgrExtra.IsFirePlant.Add(UltimateFireSpike.PlantID);
            CustomCore.AddFusion((int)PlantType.UltimateTorch, UltimateFireSpike.PlantID, (int)PlantType.TorchWood);
            CustomCore.AddUltimatePlant(UltimateFireSpike.PlantID);
            // Bullet_firePea_small_hypno
            UltimateFireSpike.jala = Resources.Load<GameObject>("items/littlesquash/LittleSquash_jala");

            CustomCore.RegisterCustomBullet<Bullet_ironPea>(UltimateFireSpike.Bullet_puffIronPea_fire, ab.GetAsset<GameObject>("Bullet_puffIronPea fire"));
            CustomCore.RegisterCustomBullet<Bullet_firePea>(UltimateFireSpike.Bullet_firePea_small_red, ab.GetAsset<GameObject>("Bullet_firePea_small_red"));
            CustomCore.RegisterCustomBullet<Bullet_cherrySquash>(UltimateFireSpike.Bullet_cherrySquash_small, ab.GetAsset<GameObject>("Bullet_cherrySquash_small"));
        }
    }

    public class UltimateFireSpike : MonoBehaviour
    {
        public static ID PlantID = 1962;
        public static ID Bullet_puffIronPea_fire = 1962;
        public static ID Bullet_firePea_small_red = 1963;
        public static ID Bullet_cherrySquash_small = 1964;

        public static GameObject? jala = null;

        public CaltropTorch plant => gameObject.GetComponent<CaltropTorch>();

        public void Awake()
        {
            if (plant == null)
            {
                Destroy(this);
                return;
            }
            plant.DisableDisMix();
            plant.isShort = true;
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision == null || collision.IsDestroyed() || collision.gameObject == null || collision.gameObject.IsDestroyed()) return;
            if (!collision.TryGetComponent<Bullet>(out var bullet) || bullet.IsDestroyed() || bullet == null) return;
            if (bullet.theBulletRow != plant.thePlantRow || bullet.Team != Team.Player || bullet.torchWood == plant) return;
            var damage = bullet.Damage;
            var type = Bullet_firePea_small_red;
            switch (bullet.theBulletType)
            {
                case BulletType.Bullet_firePea_small:
                case BulletType.Bullet_puffPea:
                    damage *= 2;
                    type = Bullet_firePea_small_red;
                    break;
                case BulletType.Bullet_puffIronPea:
                    damage *= 2;
                    type = Bullet_puffIronPea_fire;
                    break;
                case (BulletType)1929:
                    type = Bullet_cherrySquash_small;
                    break;
                case (BulletType)1922:
                    damage *= 2;
                    type = (BulletType)1923;
                    break;
                case (BulletType)1965:
                    damage *= 2;
                    type = (BulletType)1966;
                    break;
                default:
                    return;
            }
            var newBullet = CreateBullet.Instance.SetBullet(bullet.transform.position.x, bullet.transform.position.y, plant.thePlantRow, type, bullet._moveWay);
            newBullet.Damage = damage;
            newBullet.from = bullet.from;
            newBullet.fromType = bullet.fromType;
            newBullet.transform.rotation = bullet.transform.rotation;
            newBullet.theStatus = bullet.theStatus;
            newBullet.theExistTime = bullet.theExistTime;
            newBullet.torchWood = plant;
            newBullet.rb.velocity = bullet.rb.velocity;
            // 4.0 起 Bullet.detaVx / detaVy 两个分量字段合并成 Vector2 属性 acceleration（整向量赋值）
            newBullet.acceleration = bullet.acceleration;
            if (newBullet.theBulletType == Bullet_cherrySquash_small)
                newBullet.acceleration = new Vector2(newBullet.acceleration.x, 9.8f);
            newBullet.normalSpeed = bullet.normalSpeed;
            newBullet.theExistTime = bullet.theExistTime;
            bullet.Die();
            plant.attributeCount++;
            if (plant.attributeCount >= (Lawnf.TravelUltimate(CoreTools.GetUltiBuffByString("事半功倍")) ? 40 : 80))
            {
                plant.attributeCount = 0;
                if (jala == null) return;
                var littleSquash = Instantiate(jala, plant.axis.position, Quaternion.identity, plant.board.transform).GetComponent<LittleSquash>();

                if (littleSquash != null)
                {
                    littleSquash.theDamage = 300;
                    if (Lawnf.TravelUltimate(CoreTools.GetUltiBuffByString("窝红温了")))
                        littleSquash.theDamage *= 3;
                    littleSquash.theRow = plant.thePlantRow;
                    littleSquash.thePlantType = PlantType.UltimateTorch;
                    Action<int, int> action = (row, damage) =>
                    {
                        if ((littleSquash.crashCount + 1) % (Lawnf.TravelUltimate(CoreTools.GetUltiBuffByString("窝红温了")) ? 3 : 6) == 0)
                        {
                            var jalaDamage = 1800;
                            if (Lawnf.TravelUltimate(CoreTools.GetUltiBuffByString("窝红温了")))
                                jalaDamage *= 3;
                            if (Board.Instance == null) return;
                            if (Lawnf.TravelAdvanced(CoreTools.GetAdvBuffByString("火力全开")))
                            {
                                for (int i = 0; i < plant.board.rowNum; i++)
                                {
                                    Board.Instance.boardAction.CreateFireLine(i, jalaDamage, fromType: PlantID);
                                }
                            }
                            else
                            {
                                Board.Instance.boardAction.CreateFireLine(row, jalaDamage, fromType: PlantID);
                            }
                        }
                    };
                    littleSquash.crashAction = action;
                    Vector3 position = plant.axis.position;

                    CreateParticle.SetParticle(1, position, plant.thePlantRow, true);
                    GameAPP.PlaySound(22, 0.5f, 1.0f);
                }
            }
        }
    }

    [HarmonyPatch(typeof(CaltropTorch))]
    public static class CaltropTorchPatch
    {
        [HarmonyPatch(nameof(CaltropTorch.OnTriggerEnter2D))]
        [HarmonyPrefix]
        public static bool PreOnTriggerEnter2D(CaltropTorch __instance)
        {
            if (__instance.thePlantType == UltimateFireSpike.PlantID)
                return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.Die))]
    public static class PlantDieEventPatch
    {
        [HarmonyPrefix]
        public static void Prefix(Plant __instance)
        {
            if (__instance.thePlantType == UltimateFireSpike.PlantID)
                __instance.board.boardAction.CreateFireLine(__instance.thePlantRow, fromType: __instance.thePlantType);
        }
    }
}
