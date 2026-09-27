using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using CustomizeLib.BepInEx.ExtensionData.Basic;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using UnityEngine;

namespace UltimateCherryTorch.BepInEx
{
    [BepInPlugin("salmon.ultimatecherrytorch", "UltimateCherryTorch", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<UltimateCherryTorch>();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "ultimatecherrytorch");
            CustomCore.RegisterCustomPlant<SuperTorch, UltimateCherryTorch>((int)UltimateCherryTorch.PlantID, ab.GetAsset<GameObject>("UltimateCherryTorchPrefab"),
                ab.GetAsset<GameObject>("UltimateCherryTorchPreview"), new List<(int, int)>
                {
                    ((int)PlantType.CherryTorch, (int)PlantType.NuclearDoomCherry),
                    ((int)PlantType.NuclearDoomCherry, (int)PlantType.CherryTorch)
                }, 0.5f, 0f, 300, 300, 7.5f, 750);
            CustomCore.AddPlantAlmanacStrings((int)UltimateCherryTorch.PlantID,
                $"终极核爆火炬",
                "蕴含着极致核能力量的火炬，经过的特定子弹会引起高能裂变\n\n" +
                "<color=#3D1400>贴图作者：@林秋-AutumnLin</color>\n" +
                "<color=#3D1400>融合配方：</color><color=red>樱桃火炬+核爆樱桃</color>\n" +
                "<color=#3D1400>伤害：</color><color=red>1200/0.5秒</color>\n" +
                "<color=#3D1400>特点：</color><color=red>①免疫樱桃爆炸\n" +
                "②特点同超级火炬，影响的子弹伤害额外x4\n" +
                "③经过的毁灭豌豆子弹，爆炸樱桃子弹，爆炸窝瓜子弹，爆炸星星子弹，毁灭菇子弹，会转化为核能樱桃子弹，伤害x4\n" +
                "④3x3范围释放辐射，伤害僵尸，对无防具的僵尸伤害x3\n" +
                "<color=#3D1400>词条1:</color><color=red>高能辐射：终极聚能压缩火炬影响的子弹伤害x3</color>\n" +
                "<color=#3D1400>词条2:</color><color=red>伽马射线：终极聚能压缩火炬的影响范围增加至5x5</color>\n\n" +
                "<color=#3D1400>终极核爆火炬对于穿着有一套独特的见解，他的发型是熊熊燃烧的火焰，他的脸上是独特的熔岩面具，他经营着一家理发店，“你想拥有一套靓丽的时装么？你想拥有一种不拘一格的发型么？还在为自己的形象而困扰？《这个店长不太冷》理发店，时刻欢迎大家来改变自己！”终极核爆火炬的理发店生意火爆，据终极核爆火炬透露，预约的队伍已经排到明年年底，大部分客户都是小豌豆。</color>");
            UltimateCherryTorch.Buff1 = CustomCore.RegisterCustomBuff("高能辐射：终极聚能压缩火炬影响的子弹伤害x3", BuffType.AdvancedBuff, () => Board.Instance.ObjectExist<UltimateCherryTorch>(),
                5000, UltimateCherryTorch.PlantID);
            UltimateCherryTorch.Buff2 = CustomCore.RegisterCustomBuff("伽马射线：终极聚能压缩火炬的影响范围增加至5x5", BuffType.AdvancedBuff, () => Board.Instance.ObjectExist<UltimateCherryTorch>(),
                5000, UltimateCherryTorch.PlantID);
            CustomCore.TypeMgrExtra.IsFirePlant.Add(UltimateCherryTorch.PlantID);
            CustomCore.AddUltimatePlant(UltimateCherryTorch.PlantID);
        }
    }

    public class UltimateCherryTorch : MonoBehaviour
    {
        public static PlantType PlantID = (PlantType)1951;
        public static BuffID Buff1 = -1;
        public static BuffID Buff2 = -1;

        public SuperTorch plant => gameObject.GetComponent<SuperTorch>();
        public Radiation radiation = null;

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent<Bullet>(out var bullet) && bullet != null && bullet.torchWood != plant && bullet.Team == Team.Player)
            {
                if (bullet.theBulletType == BulletType.Bullet_doom || bullet.theBulletType == BulletType.Bullet_doom_big ||
                    bullet.theBulletType == BulletType.Bullet_superCherry || bullet.theBulletType == BulletType.Bullet_cherrySquash ||
                    bullet.theBulletType == BulletType.Bullet_cherryStar || bullet.theBulletType == BulletType.Bullet_pea_doom || 
                    bullet.theBulletType == BulletType.Bullet_doom_big_ulti || bullet.theBulletType == BulletType.Bullet_doom_ulti)
                {
                    if (bullet.theBulletRow != plant.thePlantRow)
                        return;
                    var moveType = BulletMoveWay.MoveRight;
                    if (bullet.MoveWay != BulletMoveWay.Sin && bullet.MoveWay != BulletMoveWay.Track)
                        moveType = (BulletMoveWay)bullet.MoveWay;
                    var nuclear = CreateBullet.Instance.SetBullet(bullet.transform.position.x, bullet.transform.position.y, bullet.theBulletRow, BulletType.Bullet_nuclear, moveType, false);
                    nuclear.Damage = bullet.Damage * 4;
                    if (Lawnf.TravelAdvanced(Buff1))
                        nuclear.Damage *= 3;
                    nuclear.torchWood = plant;
                    nuclear.SetData("UltimateCherryTorch_fireByTorch", true);
                    bullet.Die();
                }
            }
        }

        public void Start()
        {
            InstantiateRadiation();
        }

        public void Update()
        {
            if (radiation == null)
                InstantiateRadiation();
            radiation.lifeTimer = 3f;
            if (Lawnf.TravelAdvanced(Buff2))
                radiation.transform.localScale = new Vector3(7f, 7f, 7f);
            else
                radiation.transform.localScale = new Vector3(4f, 4f, 4f);
        }

        public void InstantiateRadiation()
        {
            var origin = Radiation.radiation;
            Radiation.radiation = null;
            radiation = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("plants/cherrybomb/nucleardoomcherry/Radiation"), plant.axis.transform.position, Quaternion.identity, plant.transform).GetComponent<Radiation>();
            Radiation.radiation = origin;
            radiation.lifeTimer = 3f;
            radiation.damage = 400;
            radiation.transform.localScale = new Vector3(4f, 4f, 4f);
            radiation.GetComponent<ParticleSystem>().Simulate(0.02f, true);
        }
    }

    [HarmonyPatch(typeof(SuperTorch), nameof(SuperTorch.OnTriggerEnter2D))]
    public static class SuperTorch_OnTriggerEnter2D_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(SuperTorch __instance, ref Collider2D collision)
        {
            if (__instance.thePlantType == UltimateCherryTorch.PlantID && collision.gameObject.TryGetComponent<Bullet>(out var bullet) && bullet != null && bullet.torchWood == __instance &&
                ((bullet.GetData("UltimateCherryTorch_fireByTorch") is not null && bullet.GetData("UltimateCherryTorch_fireByTorch") is false) || bullet.GetData("UltimateCherryTorch_fireByTorch") is null))
            {
                bullet.Damage *= 4;
                if (Lawnf.TravelAdvanced(UltimateCherryTorch.Buff1))
                    bullet.Damage *= 3;
                bullet.torchWood = __instance;
            }
        }
    }

    [HarmonyPatch(typeof(BombCherry), nameof(BombCherry.PlantTakeDamage))]
    public static class BombCherry_PlantTakeDamage_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Plant plant)
        {
            if (plant.thePlantType == UltimateCherryTorch.PlantID)
                return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(SuperTorch), nameof(SuperTorch.DieEvent))]
    public static class SuperTorch_DieEvent_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(SuperTorch __instance, ref Plant.DieReason reason)
        {
            if (__instance.thePlantType == UltimateCherryTorch.PlantID)
            {
                __instance.board.boardAction.SetDoom(__instance.thePlantColumn, __instance.thePlantRow, false, false, default, __instance.attackDamage);

                Doom.SetDoom(__instance.board, __instance.axis.transform.position, DoomType.Nuclear);

                var origin = Radiation.radiation;
                Radiation.radiation = null;
                var radiation = UnityEngine.Object.Instantiate(
                    Resources.Load<GameObject>("plants/cherrybomb/nucleardoomcherry/Radiation"),
                    __instance.axis.transform.position,
                    Quaternion.identity, __instance.board.transform).GetComponent<Radiation>();
                Radiation.radiation = origin;
                radiation.damage = 3600;

                return false;
            }
            return true;
        }
    }
}