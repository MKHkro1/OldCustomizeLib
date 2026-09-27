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
    public class SuperDoomUmbrella : MonoBehaviour
    {
        public SuperDoomUmbrella() : base(ClassInjector.DerivedConstructorPointer<SuperDoomUmbrella>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperDoomUmbrella(IntPtr i) : base(i)
        {
        }

        [HideFromIl2Cpp]
        public static void Register(AssetBundle ab)
        {
            CustomCore.RegisterCustomPlant<SuperUmbrella, SuperDoomUmbrella>(171, ab.GetAsset<GameObject>("SuperDoomUmbrellaPrefab"),
              ab.GetAsset<GameObject>("SuperDoomUmbrellaPreview"), [(916, 11), (923, 11), (170, 11), (174, 11), (172, 11), (173, 11), (175, 11), (176, 11),],
              3, 0, 100, 4000, 60f, 1000);
            CustomCore.RegisterSuperSkill(171, (_) => 6000, (plant) => Board.Instance.SetDoom(plant.thePlantColumn, plant.thePlantRow, false, false, default, 5400));
            CustomCore.TypeMgrExtra.UmbrellaPlants.Add((PlantType)171);
            CustomCore.AddPlantAlmanacStrings(171, "黑宝石伞(171)", "黑宝石伞能放大招生成毁灭菇爆炸\n<color=#3D1400>贴图作者：@仨硝基甲苯_ @林秋AutumnLin </color>\n<color=#3D1400>特点：</color><color=red>僵尸主动靠近黑宝石伞时会触发毁灭菇爆炸，使伞扣除700血量且无法替伤，花费6000钱币释放大招，造成5400伤害的毁灭菇爆炸</color>\n<color=#3D1400>融合配方：</color><color=red>其他宝石伞+毁灭菇</color>\n<color=#3D1400>词条1：</color><color=red>彩虹伞神：当场上同时有9种宝石伞时，所有钱币花费量降为500(解锁条件：场上同时存在9种宝石伞)</color>\n<color=#3D1400>词条2：</color><color=red>保护保护伞：紫、黑、魅宝石伞触发被动时受伤大幅减少且可被替伤(解锁条件：场上存在紫、黑、魅宝石伞之一)</color>\n<color=#3D1400>你看完我商品就快走吧，我做事低调，都是在半夜才会出现，所以你早上完全找不到我，你別跟其他人说你看過我，也別担心我，我的商品是生活必需品，比如浓缩肥料，这个你就收下，下次见面就是要收钱，拜拜</color>");
        }

        [HideFromIl2Cpp]
        public static bool SuperBlockEffect(SuperUmbrella __instance, ref Zombie zombie)
        {
            if (__instance.thePlantType is (PlantType)171 && !zombie.isMindControlled)
            {
                Board.Instance.SetDoom(__instance.thePlantColumn, __instance.thePlantRow, false, false, default, 1800, 1);
                if (Lawnf.TravelAdvanced(Core.Buff2))
                {
                    __instance.TakeDamage(100);
                }
                else
                {
                    __instance.thePlantHealth -= 700;
                    __instance.UpdateText();
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