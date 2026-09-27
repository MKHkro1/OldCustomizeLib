using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using BepInEx;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;

namespace SuperGarlicFume.BepInEx
{
    [BepInPlugin("inf75.supergarlicfume", "SuperGarlicFume", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<SuperGarlicFume>();
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "supergarlicfume");
            CustomCore.RegisterCustomPlant<UltimateFume, SuperGarlicFume>(165, ab.GetAsset<GameObject>("SuperGarlicFumePrefab"),
                ab.GetAsset<GameObject>("SuperGarlicFumePreview"), [(904, 29)], 3, 0, 150, 300, 30, 700);
            CustomCore.AddFusion(904, 165, 8);
            CustomCore.TypeMgrExtra.IsIcePlant.Add((PlantType)165);
            SuperGarlicFume.Buff1 = CustomCore.RegisterCustomBuff("满屋蒜香：究极蒜大喷菇攻击时取消僵尸蒜值上限", BuffType.AdvancedBuff, () => Lawnf.TravelUltimate(4) && Lawnf.TravelUltimate(5) && Board.Instance.ObjectExist<SuperGarlicFume>(), 5400, "#DA64FF", (PlantType)165);
            SuperGarlicFume.Buff2 = CustomCore.RegisterCustomBuff("超剧毒：究极蒜大喷菇可以对僵尸额外造成总血量×蒜值%的伤害", BuffType.AdvancedBuff, () => Lawnf.TravelUltimate(4) && Lawnf.TravelUltimate(5) && Lawnf.TravelAdvanced(SuperGarlicFume.Buff1) && Board.Instance.ObjectExist<SuperGarlicFume>(), 17900, "red", (PlantType)165);
            SuperGarlicFume.Buff3 = CustomCore.RegisterCustomBuff("冰蒜天：究极蒜大喷菇可以在僵尸被冻结时使僵尸蒜值增加e的(本行究极投手总数+1)次方，对僵尸本体额外造成(蒜值×(本行究极投手总数+1)的三次方)伤害", BuffType.AdvancedBuff, () => Lawnf.TravelUltimate(4) && Lawnf.TravelUltimate(5) && Lawnf.TravelAdvanced(SuperGarlicFume.Buff2) && Board.Instance.ObjectExist<SuperGarlicFume>(), 17900, "red", (PlantType)165);
            CustomCore.AddPlantAlmanacStrings(165, "究极蒜大喷菇(165)", "究极蒜大喷菇的孢子能同时造成减速和中毒效果。\n<color=#3D1400>贴图作者：@林秋AutumnLin</color>\n<color=#3D1400>特点：</color><color=red>究极大喷菇亚种，使用大蒜、魅惑菇切换。持续攻击，每0.5s对本行所有僵尸造成150+蒜值*20伤害并减速，同时对每个受到攻击的僵尸附加10点冻结值和1点蒜值</color>\n<color=#3D1400>融合配方：</color><color=red>究极大喷菇+大蒜</color>\n<color=#3D1400>\n<color=#3D1400>词条1：</color><color=red>凛风刺骨：攻击力×3</color>\n<color=#3D1400>词条2：</color><color=red>三尺之寒：冻结值积累速度×5</color>\n<color=#3D1400>词条3：</color><color=red>满屋蒜香：究极蒜大喷菇攻击时取消僵尸蒜值上限(解锁条件：解锁了词条1、2且场上存在究极蒜大喷菇)</color>\n<color=#3D1400>词条4：</color><color=red>超剧毒：究极蒜大喷菇可以对僵尸额外造成总血量*蒜值%的真实伤害(解锁条件：词条3的所有条件+解锁词条3)</color>\n<color=#3D1400>词条5：</color><color=red>冰蒜天：究极蒜大喷菇可以在僵尸被冻结时使僵尸蒜值增加e的(本行究极投手总数+1)次方，对僵尸本体额外造成(蒜值×(本行究极投手总数+1)的三次方)伤害(解锁条件：词条4的所有条件+解锁词条4)</color>\n<color=#3D1400>在经历一切的事情后，他决定退休，看一些老电影，欣赏向日葵歌剧，甚至是安排退休生活，不过僵尸不会给他机会。</color>");
        }
    }

    public class SuperGarlicFume : MonoBehaviour
    {
        public SuperGarlicFume() : base(ClassInjector.DerivedConstructorPointer<SuperGarlicFume>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperGarlicFume(IntPtr i) : base(i)
        {
        }

        public void Awake()
        {
            plant.emission.enabled = false;
            plant.DisableDisMix();
            var tag = plant.plantTag;
            tag.icePlant = true;
            plant.plantTag = tag;
        }

        public void StartShoot()
        {
            plant.emission.enabled = true;
        }

        public void StopShoot()
        {
            plant.emission.enabled = false;
        }

        public void SuperAttackZombie()
        {
            plant.zombieList.Clear();
            foreach (var z in Board.Instance.zombieArray)
            {
                if (z is not null && !z.IsDestroyed() && !z.isMindControlled && !TypeMgr.IsAirZombie(z.theZombieType) && z.theZombieRow == plant.thePlantRow && z.axis.position.x > plant.axis.position.x)
                {
                    plant.zombieList.Add(z);
                }
            }
            GameAPP.PlaySound(58);
            foreach (var z in plant.zombieList)
            {
                if (z is not null && !z.IsDestroyed() && !z.isMindControlled)
                {
                    z.TakeDamage(DmgType.IceAll, (150 + z.poisonLevel * 20 + z.theMaxHealth * (Lawnf.TravelAdvanced(Buff2) ? z.poisonLevel / 100 : 0)) * (Lawnf.TravelUltimate(4) ? 3 : 1));
                    GameAPP.PlaySound(UnityEngine.Random.RandomRangeInt(0, 3));
                    if (Lawnf.TravelAdvanced(Buff2))
                    {
                        z.TakeDamage(DmgType.IceAll, z.theMaxHealth * z.poisonLevel / 100, PlantType.Nothing, true);
                    }
                    if (Lawnf.TravelAdvanced(Buff1))
                    {
                        z.SetPoison(10);
                    }
                    else
                    {
                        z.AddPoisonLevel();
                    }
                    z.SetCold(10);
                    z.AddfreezeLevel(10 * (Lawnf.TravelUltimate(5) ? 5 : 1));

                    if (z.poisonLevel <= 1000000 && Lawnf.TravelAdvanced(Buff3) && z.freezeTimer > 3)
                    {
                        double multiplier = 1;
                        foreach (var p in Board.Instance.boardEntity.plantArray)
                        {
                            if (p is not null && p.thePlantRow == plant.thePlantRow && p.TryCast<UltimateMelon>() is not null)
                            {
                                multiplier++;
                            }
                        }
                        double newPoison = z.poisonLevel + Math.Pow(Math.E, multiplier);
                        if (newPoison > int.MaxValue / 2) newPoison = int.MaxValue / 2;
                        z.freezeTimer = 3;
                        z.poisonLevel = (int)newPoison;
                        z.BodyTakeDamage((int)(multiplier * multiplier * multiplier * z.poisonLevel));
                    }
                }
            }
        }

        public void SuperAttackZombie2() => SuperAttackZombie();

        public static int Buff1 { get; set; } = -1;
        public static int Buff2 { get; set; } = -1;
        public static int Buff3 { get; set; } = -1;
        public UltimateFume plant => gameObject.GetComponent<UltimateFume>();
    }
}