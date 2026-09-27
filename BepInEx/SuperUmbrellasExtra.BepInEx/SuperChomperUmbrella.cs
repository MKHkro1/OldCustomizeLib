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
    public class SuperChomperUmbrella : MonoBehaviour
    {
        public SuperChomperUmbrella() : base(ClassInjector.DerivedConstructorPointer<SuperChomperUmbrella>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperChomperUmbrella(IntPtr i) : base(i)
        {
        }

        [HideFromIl2Cpp]
        public static void Register(AssetBundle ab)
        {
            CustomCore.RegisterCustomPlant<SuperUmbrella, SuperChomperUmbrella>(170, ab.GetAsset<GameObject>("SuperChomperUmbrellaPrefab"),
              ab.GetAsset<GameObject>("SuperChomperUmbrellaPreview"), [(916, 5), (923, 5), (174, 5), (171, 5), (172, 5), (173, 5), (175, 5), (176, 5),],
              3, 0, 100, 4000, 60f, 1200);
            CustomCore.RegisterSuperSkill(170, (plant) =>
            {
                var pos = plant.axis.position;
                LayerMask layermask = plant.zombieLayer.m_Mask;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 3f);
                long health = 1;
                foreach (var z in array)
                {
                    if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zombie) && !zombie.isMindControlled)
                    {
                        health += (long)(zombie.theHealth + zombie.theFirstArmorHealth + zombie.theSecondArmorHealth);
                    }
                }
                return (int)(50000 * (1 - Math.Pow(Math.E, -0.00003d * health)) - 1);
            },
            (plant) =>
            {
                var pos = plant.axis.position;
                LayerMask layermask = plant.zombieLayer.m_Mask;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 3f);
                foreach (var z in array)
                {
                    if (z is not null && z.GameObject().TryGetComponent<Zombie>(out var zombie) && !zombie.isMindControlled && (zombie.theZombieRow == plant.thePlantRow || zombie.theZombieRow == plant.thePlantRow - 1 || zombie.theZombieRow == plant.thePlantRow + 1))
                    {
                        zombie.Die(2);
                    }
                }
                plant.flashCountDown = 45;
                GameAPP.PlaySound(49);
            });
            CustomCore.TypeMgrExtra.UmbrellaPlants.Add((PlantType)170);
            CustomCore.AddPlantAlmanacStrings(170, "紫宝石伞(170)", "紫宝石伞能放大招吞噬周围的僵尸\n<color=#3D1400>贴图作者：@仨硝基甲苯_ @林秋AutumnLin </color>\n<color=#3D1400>特点：</color><color=red>僵尸主动靠近紫宝石伞时将其吞下，使伞扣除900血量/3999血量(吞下领袖僵尸时)且无法替伤，花费{50000*[1-e^(-0.00003*周围僵尸总血量)]-1}钱币释放大招，无视一切判定秒杀周围全部僵尸，冷却45s</color>\n<color=#3D1400>融合配方：</color><color=red>其他宝石伞+大嘴花</color>\n<color=#3D1400>词条1：</color><color=red>彩虹伞神：当场上同时有9种宝石伞时，所有钱币花费量降为500(解锁条件：场上同时存在9种宝石伞)</color>\n<color=#3D1400>词条2：</color><color=red>保护保护伞：紫、黑、魅宝石伞触发被动时受伤大幅减少且可被替伤(解锁条件：场上存在紫、黑、魅宝石伞之一)</color>\n<color=#3D1400>紫宝石伞是一名出色的魔术师，他变的魔术从来不会少人，每次他的魔术秀都会爆满，但是没有哪个僵尸会发现大变活僵的演员其实在他肚子里</color>");
        }

        [HideFromIl2Cpp]
        public static bool SuperBlockEffect(SuperUmbrella __instance, ref Zombie zombie)
        {
            if (__instance.thePlantType is (PlantType)170 && !zombie.isMindControlled)
            {
                if (Lawnf.TravelAdvanced(Core.Buff2))
                {
                    __instance.TakeDamage(100);
                }
                else
                {
                    if (TypeMgr.IsBossZombie(zombie.theZombieType))
                    {
                        __instance.thePlantHealth -= 3899;
                        __instance.UpdateText();
                    }
                    else
                    {
                        __instance.thePlantHealth -= 900;
                        __instance.UpdateText();
                    }
                }
                zombie.KnockBack(1.5f * (__instance.UmbrellaPot is not null ? 2 : 1));
                zombie.Die(2);
                GameAPP.PlaySound(49);
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