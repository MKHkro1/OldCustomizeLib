using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using BepInEx;
using System.Reflection;
using UnityEngine;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;

namespace IceMelonCannon.BepInEx
{
    [HarmonyPatch(typeof(Bullet_cannon))]
    public static class CannonBulletPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("HitLand")]
        public static bool PostHitLand(Bullet_cannon __instance)
        {
            if (__instance.theBulletType == (BulletType)IceMelonCannon.BulletId)
            {
                CreateParticle.SetParticle(56, new(__instance.cannonPos.x, __instance.cannonPos.y), __instance.theBulletRow);
                var pos = __instance.transform.position;
                LayerMask layermask = __instance.zombieLayer.m_Mask;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 1.5f);
                foreach (var z in array)
                {
                    if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zombie) && !zombie.isMindControlled)
                    {
                        zombie.TakeDamage(DmgType.IceAll, __instance.Damage);
                        zombie.AddfreezeLevel(50);
                        zombie.SetCold(15);
                    }
                }
                GameAPP.PlaySound(UnityEngine.Random.RandomRangeInt(104, 106));
                __instance.Die();
                return false;
            }
            return true;
        }
    }

    [BepInPlugin("inf75.icemeloncannon", "IceMelonCannon", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "icemeloncannon");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<IceMelonCannon>();
            CustomCore.RegisterCustomPlant<MelonCannon, IceMelonCannon>(303, ab.GetAsset<GameObject>("IceMelonCannonPrefab"),
                ab.GetAsset<GameObject>("IceMelonCannonPreview"), [(1132, 10)], 24, 30, 120, 1000, 30f, 900);
            CustomCore.TypeMgrExtra.IsIcePlant.Add((PlantType)303);
            CustomCore.TypeMgrExtra.DoubleBoxPlants.Add((PlantType)303);
            CustomCore.RegisterCustomBullet<Bullet_melonCannon>((BulletType)IceMelonCannon.BulletId, ab.GetAsset<GameObject>("ProjectileCannon_wintermelon"));
            CustomCore.AddPlantAlmanacStrings(303, "冰瓜加农炮(303)", "手动发射大量冰瓜砸向僵尸\n<color=#3D1400>贴图作者：@屑红leong @仨硝基甲苯_ </color>\n<color=#3D1400>伤害：</color><color=red>120*40，3×3无衰减，受到伤害的僵尸增加50点冻结值，子弹落点为5×3随机</color>\n<color=#3D1400>装填时间：</color><color=red>24秒</color>\n<color=#3D1400>融合配方：</color><color=red>西瓜加农炮+寒冰菇</color>\n<color=#3D1400>“冰瓜炮曾经是我最好的战友，直到他讲出比寒冰毁灭菇还要冷的笑话，他离我而去，我则被他冰冻。”说完冰炮跑回战场，而冰瓜炮正在举办冰西瓜大胃王比赛。其他植物表示，为什么冰瓜炮的西瓜比其他的还要更冷，但实际上，冰瓜炮是不小心喝到液氮，才变成这副模样。</color>");
        }
    }

    public class IceMelonCannon : MonoBehaviour
    {
        public IceMelonCannon() : base(ClassInjector.DerivedConstructorPointer<IceMelonCannon>()) => ClassInjector.DerivedConstructorBody(this);

        public IceMelonCannon(IntPtr i) : base(i)
        {
        }

        public void AnimShooting()
        {
            GameAPP.PlaySound(4, 1.0f);
            var RowFromY = Mouse.Instance.GetRowFromY(plant.cannonTarget.x, plant.cannonTarget.y);
            var bullet = plant.board.GetComponent<CreateBullet>().SetBullet(plant.shoot.transform.position.x, plant.shoot.transform.position.y, RowFromY, (BulletType)BulletId, 14);
            var pos2 = bullet.cannonPos;
            pos2.x = plant.cannonTarget.x;
            pos2.y = plant.cannonTarget.y;
            bullet.cannonPos = pos2;
            bullet.rb.velocity = new(1.5f, 0);
            bullet.theStatus = BulletStatus.Melon_cannon;
            bullet.Damage = plant.attackDamage;
        }

        public void Awake()
        {
            plant.shoot = gameObject.transform.FindChild("Shoot");
        }

        public static int BulletId { get; set; } = 902;
        public MelonCannon plant => gameObject.GetComponent<MelonCannon>();
    }
}