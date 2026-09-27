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
    public class SuperIceUmbrella : MonoBehaviour
    {
        public SuperIceUmbrella() : base(ClassInjector.DerivedConstructorPointer<SuperIceUmbrella>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperIceUmbrella(IntPtr i) : base(i)
        {
        }

        [HideFromIl2Cpp]
        public static void Register(AssetBundle ab)
        {
            CustomCore.RegisterCustomPlant<SuperUmbrella, SuperIceUmbrella>(173, ab.GetAsset<GameObject>("SuperIceUmbrellaPrefab"),
              ab.GetAsset<GameObject>("SuperIceUmbrellaPreview"), [(916, 10), (923, 10), (170, 10), (171, 10), (172, 10), (174, 10), (175, 10), (176, 10),],
              3, 0, 100, 4000, 60f, 900);
            CustomCore.RegisterSuperSkill(173, (_) => 6000, (plant) =>
            {
                var pos = plant.axis.position;
                LayerMask layermask = plant.zombieLayer.m_Mask;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 3f);
                foreach (var z in array)
                {
                    if (z is not null && z.GameObject().TryGetComponent<Zombie>(out var zombie) && !zombie.isMindControlled && (zombie.theZombieRow == plant.thePlantRow || zombie.theZombieRow == plant.thePlantRow - 1 || zombie.theZombieRow == plant.thePlantRow + 1))
                    {
                        zombie.SetFreeze(20);
                    }
                }
            });
            CustomCore.TypeMgrExtra.UmbrellaPlants.Add((PlantType)173);
            CustomCore.TypeMgrExtra.IsIcePlant.Add((PlantType)173);
            CustomCore.AddPlantAlmanacStrings(173, "蓝宝石伞(173)", "蓝宝石伞能放大招冻结周围的僵尸\n<color=#3D1400>贴图作者：@仨硝基甲苯_ @林秋AutumnLin </color>\n<color=#3D1400>特点：</color><color=red>僵尸主动靠近蓝宝石伞时将其弹开并使其减速，花费4000钱币释放大招，使周围的僵尸冻结20s</color>\n<color=#3D1400>融合配方：</color><color=red>其他宝石伞+寒冰菇</color>\n<color=#3D1400>词条1：</color><color=red>彩虹伞神：当场上同时有9种宝石伞时，所有钱币花费量降为500(解锁条件：场上同时存在9种宝石伞)</color>\n<color=#3D1400>请你们看看我的商品，然而别靠我太近，我怕会冻住你，害你留在原地，或者头盔黏在头上，还有可以帮我找找橙宝石伞吗，或是其他火热的东西，火热才能融化我的冰</color>");
        }

        [HideFromIl2Cpp]
        public static bool SuperBlockEffect(SuperUmbrella __instance, ref Zombie zombie)
        {
            if (__instance.thePlantType is (PlantType)173 && !zombie.isMindControlled)
            {
                zombie.SetCold(15);
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