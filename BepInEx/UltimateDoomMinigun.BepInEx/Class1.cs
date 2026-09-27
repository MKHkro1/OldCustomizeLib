using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using BepInEx;
using UnityEngine;
using BepInEx.Unity.IL2CPP;
using System.Reflection;
using CustomizeLib.BepInEx;

namespace UltimateDoomMinigun.BepInEx
{
    [BepInPlugin("salmon.ultimatedoomminigun", "UltimateDoomMinigun", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<UltimateDoomMinigun>();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "ultimatedoomminigun");
            CustomCore.RegisterCustomPlant<UltimateMinigun, UltimateDoomMinigun>(UltimateDoomMinigun.PlantID, ab.GetAsset<GameObject>("UltimateDoomMinigunPrefab"),
                ab.GetAsset<GameObject>("UltimateDoomMinigunPreview"), new List<(int, int)> (), 0.5f, 0f, 300, 300, 90f, 1000);
            CustomCore.AddPlantAlmanacStrings(UltimateDoomMinigun.PlantID, $"究级速射毁灭机枪射手({UltimateDoomMinigun.PlantID})",
                "发射毁灭子弹的加特林速射机枪\n" +
                "<color=#0000FF>毁灭菇机枪射手的限定形态</color>\n\n" +
                "<color=#3D1400>贴图作者：@林秋-AutumnLin</color>\n" +
                "<color=#3D1400>使用条件：</color><color=red>①融合或转化毁灭机枪射手时有2%概率变异\n" +
                "②神秘模式\n" +
                "*可使用豌豆射手切回毁灭机枪射手\n" +
                "*可使用胆小菇切换究极速射毁灭机枪胆小菇</color>\n" +
                "<color=#3D1400>伤害：</color><color=red>300x6/0.5秒</color>\n" +
                "<color=#3D1400>特点：</color><color=red>①每次发射有5%概率发射大毁灭菇子弹，每第16发发射伤害1800的大毁灭菇子弹，造成半径3格无衰减溅射。\n" +
                "②启动射击需要预热1.5秒。</color>\n\n" +
                "<color=#3D1400>“你问我成功的秘卷是什么？是看到机会就立马抓住的反应，是成功路上不断披荆斩棘的勇气，是成功后与他人分享到喜悦！”究极速射毁灭菇机枪射手整理了下衣领，接着说道，“我们都曾遇到过困境，经历过迷惘，那是什么带领我们击败困境穿越迷惘的呢？是曾经的自己，所以为了感谢曾经的自己，好好对待当下的每一天吧。哦对了！别感冒！”</color>");
            CustomCore.TypeMgrExtra.LevelPlants.Add((PlantType)UltimateDoomMinigun.PlantID, CardLevel.Red);
            CustomCore.AddFusion((int)PlantType.DoomGatling, UltimateDoomMinigun.PlantID, (int)PlantType.Peashooter);
            CustomCore.AddFusion((int)PlantType.DoomGatling, (int)PlantType.Peashooter, UltimateDoomMinigun.PlantID);
            CustomCore.AddFusion((int)UltimateDoomMinigun.PlantID, 1953, (int)PlantType.Peashooter);
            CustomCore.AddFusion((int)UltimateDoomMinigun.PlantID, (int)PlantType.Peashooter, 1953);
        }
    }

    public class UltimateDoomMinigun : MonoBehaviour
    {
        public static int PlantID = 1933;
        public int shootCount = 0;
        public void Start()
        {
            plant.shoot = plant.gameObject.transform.FindChild("GatlingPea_head/Shoot");
            if (plant.board != null)
                plant.board.OnPlantCreate(plant);
            plant.UpdateText();
            plant.ReplaceSprite();
        }

        public Bullet AnimShoot_Doom()
        {
            shootCount++;

            BulletType bulletType = BulletType.Bullet_doom;
            int damage = plant.attackDamage;
            bool isBigBullet = false;
            if (shootCount % ((Lawnf.TravelAdvanced((AdvBuff)3) ? 4 : 16)) == 0)
            {
                shootCount = 0;
                bulletType = BulletType.Bullet_doom_big;
                damage = 6 * plant.attackDamage;
                isBigBullet = true;
            }
            if (UnityEngine.Random.Range(0, 100) <= 5)
            {
                bulletType = BulletType.Bullet_doom_big;
                damage = 6 * plant.attackDamage;
                isBigBullet = true;
            }

            // 创建子弹
            Bullet bullet = CreateBullet.Instance.SetBullet(
                plant.shoot.transform.position.x,
                plant.shoot.transform.position.y,
                plant.thePlantRow,
                bulletType,
                BulletMoveWay.MoveRight,
                false);

            if (bullet != null)
            {
                // 设置子弹伤害
                bullet.Damage = damage;

                // 加倍子弹速度
                bullet.normalSpeed *= 2f; // 猜测字段名为speed
                if (isBigBullet)
                    bullet.theStatus = BulletStatus.Doom_big;
                // 播放随机射击音效
                GameAPP.PlaySound(UnityEngine.Random.Range(3, 5), 0.5f, 1.0f); // 猜测音效参数
            }

            return bullet;
        }

        public UltimateMinigun plant => gameObject.GetComponent<UltimateMinigun>();
    }

    [HarmonyPatch(typeof(CreatePlant), nameof(CreatePlant.CheckMix))]
    public static class CreatePlant_CheckMix_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(CreatePlant __instance, ref Plant __result)
        {
            if (__result != null && __result.GetComponent<Plant>().thePlantType == PlantType.DoomGatling && UnityEngine.Random.Range(0, 100) <= 1 &&
                GameAPP.theGameStatus == GameStatus.InGame)
            {
                var row = __result.thePlantRow;
                var column = __result.thePlantColumn;
                var first = __result.firstParent;
                var second = __result.secondParent;
                __result.Die(Plant.DieReason.ByShovel);
                var plant = __instance.SetPlant(column, row, (PlantType)UltimateDoomMinigun.PlantID, null, default, true);
                plant.firstParent = first;
                plant.secondParent = second;
            }
        }
    }

    [HarmonyPatch(typeof(Plant), nameof(Plant.Shootable))]
    public static class Plant_Shootable_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Plant __instance, ref bool __result)
        {
            if (__instance != null && __instance.thePlantType == (PlantType)UltimateDoomMinigun.PlantID)
            {
                if (Lawnf.TravelAdvanced((AdvBuff)2) && __result)
                    __instance.anim.Play("shooting");
            }
        }
    }
}