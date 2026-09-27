using CustomizeLib.BepInEx;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace SuperUmbrellasExtra.BepInEx
{
    public class SuperGarlicUmbrella : MonoBehaviour
    {
        public SuperGarlicUmbrella() : base(ClassInjector.DerivedConstructorPointer<SuperGarlicUmbrella>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperGarlicUmbrella(IntPtr i) : base(i)
        {
        }

        [HideFromIl2Cpp]
        public static void Register(AssetBundle ab)
        {
            CustomCore.RegisterCustomPlant<SuperUmbrella, SuperGarlicUmbrella>(172, ab.GetAsset<GameObject>("SuperGarlicUmbrellaPrefab"),
              ab.GetAsset<GameObject>("SuperGarlicUmbrellaPreview"), [(916, 29), (923, 29), (170, 29), (171, 29), (174, 29), (173, 29), (175, 29), (176, 29),],
              3, 0, 100, 4000, 60f, 900);
            CustomCore.RegisterSuperSkill(172, (_) => 6000, (plant) =>
            {
                var pos = plant.axis.position;
                LayerMask layermask = plant.zombieLayer.m_Mask;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 3f);
                int health = 1;
                int count = 0;
                foreach (var z in array)
                {
                    if (z is not null && z.GameObject().TryGetComponent<Zombie>(out var zombie) && !zombie.isMindControlled && (zombie.theZombieRow == plant.thePlantRow || zombie.theZombieRow == plant.thePlantRow - 1 || zombie.theZombieRow == plant.thePlantRow + 1))
                    {
                        health += zombie.GetTotalHealth();
                        count++;
                        for (int i = 0; i < 30; i++)
                        {
                            zombie.SetPoison(10);
                        }
                        if (count * Math.Log(health) < 10)
                        {
                            zombie.Die(2);
                            CreateZombie.Instance.SetZombie(zombie.theZombieRow, ZombieType.GoldZombie, zombie.transform.position.x);
                        }
                    }
                }
            });
            CustomCore.TypeMgrExtra.UmbrellaPlants.Add((PlantType)172);
            CustomCore.AddPlantAlmanacStrings(172, "白宝石伞(172)", "白宝石伞能放大招使周围的僵尸中毒\n<color=#3D1400>贴图作者：@仨硝基甲苯_ @林秋AutumnLin </color>\n<color=#3D1400>特点：</color><color=red>僵尸主动靠近白宝石伞时将其弹开并叠加10点蒜值，花费6000钱币释放大招，使周围的僵尸增加20点蒜值，部分僵尸在被附加蒜值后变成金雕像僵尸</color>\n<color=#3D1400>融合配方：</color><color=red>其他宝石伞+大蒜</color>\n<color=#3D1400>词条1：</color><color=red>彩虹伞神：当场上同时有9种宝石伞时，所有钱币花费量降为500(解锁条件：场上同时存在9种宝石伞)</color>\n<color=#3D1400>作为一个快退休的商人来说，你应该是我最后一个顾客，我也赚够了，商业界也少一个商人了，尽管他们多希望我留下來，但时代变了，我这个老植物，快跟不上时代变化了，现在是年轻人的时代，总之，你想买什么，我这里什么都有卖</color>");
        }

        [HideFromIl2Cpp]
        public static bool SuperBlockEffect(SuperUmbrella __instance, ref Zombie zombie)
        {
            if (__instance.thePlantType is (PlantType)172 && !zombie.isMindControlled)
            {
                for (int i = 0; i < 10; i++)
                {
                    zombie.SetPoison(10);
                }

                zombie.KnockBack(1.5f * (__instance.UmbrellaPot is not null ? 2 : 1));
                return false;
            }
            return true;
        }

        public void Awake()
        {
            plant.DisableDisMix();
        }

        public SuperUmbrella plant => gameObject.GetComponent<SuperUmbrella>();
    }
}