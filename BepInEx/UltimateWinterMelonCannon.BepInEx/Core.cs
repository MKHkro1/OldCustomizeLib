using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using BepInEx;
using System.Reflection;
using UnityEngine;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;

namespace UltimateWinterMelonCannon.BepInEx
{
    [HarmonyPatch(typeof(Bullet_cannon))]
    public static class Bullet_cannonPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitLand")]
        public static bool PreHitLand(Bullet_cannon __instance)
        {
            if (__instance.theBulletType == (BulletType)UltimateWinterMelonCannon.BulletId)
            {
                CreateParticle.SetParticle(200, new(__instance.cannonPos.x, __instance.cannonPos.y), __instance.theBulletRow);
                var pos = __instance.transform.position;
                LayerMask layermask = __instance.zombieLayer.m_Mask;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 3f);
                foreach (var z in array)
                {
                    if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zombie) && !zombie.isMindControlled)
                    {
                        zombie.TakeDamage(DmgType.IceAll, __instance.Damage * (Lawnf.TravelUltimate((UltiBuff)14) ? 3 : 1));
                        zombie.SetFreeze(10);
                        zombie.AddfreezeLevel(400);
                        zombie.SetCold(30);
                        if (Lawnf.TravelAdvanced(UltimateWinterMelonCannon.Buff1))
                        {
                            zombie.TakeDamage(DmgType.IceAll, (int)(0.05 * (zombie.theHealth + zombie.theFirstArmorHealth + zombie.theSecondArmorHealth)));
                        }
                    }
                }

                GameAPP.PlaySound(UnityEngine.Random.RandomRangeInt(104, 106));
                if (Lawnf.TravelAdvanced(UltimateWinterMelonCannon.Buff2))
                {
                    Board.Instance.boardAction.SetDoom(Mouse.Instance.GetColumnFromX(__instance.transform.position.x), __instance.theBulletRow, false, true, default, 3600);
                }
                __instance.Die();
                return false;
            }
            return true;
        }
    }

    [BepInPlugin("inf75.ultimatewintermeloncannon", "UltimateWinterMelonCannon", "1.0")]
    public class Core : BasePlugin//168
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<UltimateWinterMelonCannon>();
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "ultimatewintermeloncannon");
            CustomCore.RegisterCustomPlant<MelonCannon, UltimateWinterMelonCannon>(168, ab.GetAsset<GameObject>("UltimateWinterMelonCannonPrefab"),
                ab.GetAsset<GameObject>("UltimateWinterMelonCannonPreview"), [(915, 32)], 24, 24, 450, 1000, 60f, 1200);
            CustomCore.RegisterCustomBullet<Bullet_melonCannon>((BulletType)UltimateWinterMelonCannon.BulletId, ab.GetAsset<GameObject>("ProjectileCannon_UltimateWinterMelon"));
            CustomCore.RegisterCustomParticle((ParticleType)200, ab.GetAsset<GameObject>("CannonUltimateWinterMelonSplat"));
            CustomCore.TypeMgrExtra.IsIcePlant.Add((PlantType)168);
            CustomCore.TypeMgrExtra.DoubleBoxPlants.Add((PlantType)168);
            CustomCore.AddFusion(915, 168, 28);
            CustomCore.AddPlantAlmanacStrings(168, "冰毁瓜加农炮(168)", "手动发射寒冰毁灭西瓜，范围全屏\n<color=#3D1400>贴图作者：@林秋AutumnLin </color>\n<color=#3D1400>特点：</color><color=red>究极加农炮亚种，使用西瓜投手、玉米投手切换。点击发射60个伤害450的寒冰毁灭西瓜子弹，范围全屏，受击僵尸冻结15s</color>\n<color=#3D1400>融合配方：</color><color=red>究极加农炮+西瓜投手</color>\n<color=#3D1400>词条1：</color><color=red>兵贵神速：装填时间降为10秒</color>\n<color=#3D1400>词条2：</color><color=red>中心爆破：单个子弹伤害提升至1350</color>\n<color=#3D1400>词条3：</color><color=red>瓜里藏刀：每个冰毁瓜子弹对僵尸额外造成5%血量伤害(解锁条件：解锁了词条1、2且场上存在冰毁瓜加农炮)</color>\n<color=#3D1400>词条4：</color><color=red>真正的冰毁瓜：每个冰毁瓜子弹落地时直接生成3600伤害的寒冰毁灭菇爆炸，范围全屏(解锁条件：解锁了词条1、2、3且场上存在冰毁瓜加农炮)</color>\n<color=#3D1400>“包装自己的最有效策略？”冰毁西瓜炮低声说道：“首先要冷静，谦逊……然后，从沉默中醒来——轰！整个世界都会记住你的名字。”</color>");
            UltimateWinterMelonCannon.Buff1 = CustomCore.RegisterCustomBuff("瓜里藏刀：每个冰毁瓜子弹对僵尸额外造成5%血量伤害", BuffType.AdvancedBuff, () => Board.Instance.ObjectExist<UltimateWinterMelonCannon>() && Lawnf.TravelUltimate((UltiBuff)14) && Lawnf.TravelUltimate((UltiBuff)15), 5400, null, (PlantType)168);
            UltimateWinterMelonCannon.Buff2 = CustomCore.RegisterCustomBuff("真正的冰毁瓜：每个冰毁瓜子弹落地时直接生成3600伤害的寒冰毁灭菇爆炸，范围全屏", BuffType.AdvancedBuff, () => Board.Instance.ObjectExist<UltimateWinterMelonCannon>() && Lawnf.TravelUltimate((UltiBuff)14) && Lawnf.TravelUltimate((UltiBuff)15) && Lawnf.TravelAdvanced(UltimateWinterMelonCannon.Buff1), 28800, "red", (PlantType)168);
        }
    }

    public class UltimateWinterMelonCannon : MonoBehaviour
    {
        public UltimateWinterMelonCannon() : base(ClassInjector.DerivedConstructorPointer<UltimateWinterMelonCannon>()) => ClassInjector.DerivedConstructorBody(this);

        public UltimateWinterMelonCannon(IntPtr i) : base(i)
        {
        }

        public void AnimShooting()
        {
            GameAPP.PlaySound(4, 1.0f);
            var RowFromY = Mouse.Instance.GetRowFromY(plant.cannonTarget.x, plant.cannonTarget.y);
            var bullet = plant.board.GetComponent<CreateBullet>().SetBullet(plant.shoot.transform.position.x, plant.shoot.transform.position.y, RowFromY, (BulletType)BulletId, (BulletMoveWay)14);
            var pos2 = bullet.cannonPos;
            pos2.x = plant.cannonTarget.x;
            pos2.y = plant.cannonTarget.y;
            bullet.cannonPos = pos2;
            bullet.rb.velocity = new(1.5f, 0);
            bullet.theStatus = BulletStatus.GoldMelon_cannon;
            bullet.Damage = plant.attackDamage;
        }

        public void Awake()
        {
            plant.DisableDisMix();
            plant.shoot = gameObject.transform.FindChild("Shoot");
        }

        public void Update()
        {
            if (GameAPP.theGameStatus is 0 && Lawnf.TravelUltimate((UltiBuff)15) && plant is not null && plant.attributeCountdown > 10f)
            {
                plant.attributeCountdown = 10f;
            }
        }

        public static BuffID Buff1 { get; set; } = -1;
        public static BuffID Buff2 { get; set; } = -1;

        public static int BulletId { get; set; } = 901;

        public MelonCannon plant => gameObject.GetComponent<MelonCannon>();
    }
}