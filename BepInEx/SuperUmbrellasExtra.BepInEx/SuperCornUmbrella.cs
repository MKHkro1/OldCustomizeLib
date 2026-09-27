using CustomizeLib.BepInEx;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using Unity.VisualScripting;
using UnityEngine;

namespace SuperUmbrellasExtra.BepInEx
{
    public class SuperCornUmbrella : MonoBehaviour
    {
        public SuperCornUmbrella() : base(ClassInjector.DerivedConstructorPointer<SuperCornUmbrella>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperCornUmbrella(IntPtr i) : base(i)
        {
        }

        [HideFromIl2Cpp]
        public static void Register(AssetBundle ab)
        {
            CustomCore.RegisterCustomPlant<SuperUmbrella, SuperCornUmbrella>(175, ab.GetAsset<GameObject>("SuperCornUmbrellaPrefab"),
                ab.GetAsset<GameObject>("SuperCornUmbrellaPreview"), [(916, 28), (923, 28), (170, 28), (171, 28), (172, 28), (173, 28), (174, 28), (176, 28),], 3, 0, 80, 4000, 60f, 800);
            CustomCore.RegisterSuperSkill(175, (_) => 6000, (plant) =>
            {
                var pos = plant.axis.position;
                LayerMask layermask = plant.zombieLayer.m_Mask;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 3f);
                foreach (var z in array)
                {
                    if (z is not null && z.GameObject().TryGetComponent<Zombie>(out var zombie) && !zombie.isMindControlled && (zombie.theZombieRow == plant.thePlantRow || zombie.theZombieRow == plant.thePlantRow - 1 || zombie.theZombieRow == plant.thePlantRow + 1))
                    {
                        zombie.KnockBack(1.5f * (plant.Cast<SuperUmbrella>().UmbrellaPot is not null ? 2 : 1));
                        zombie.Buttered(10);
                    }
                }
            });
            CustomCore.TypeMgrExtra.UmbrellaPlants.Add((PlantType)175);
            CustomCore.AddFusion(916, 175, 26);
            CustomCore.AddPlantAlmanacStrings(175, "黄宝石伞(175)", "黄宝石伞能用黄油黏住靠近的僵尸，又能放出大招黏住一定范围的僵尸\n<color=#3D1400>贴图作者：@仨硝基甲苯_ @林秋AutumnLin </color>\n<color=#3D1400>特点：</color><color=red>绿宝石伞亚种，使用玉米投手、卷心菜投手切换。僵尸主动靠近黄宝石伞时特性同黄油伞，花费6000钱币释放大招，对周围僵尸施加黄油效果并击退</color>\n<color=#3D1400>融合配方：</color><color=red>其他宝石伞+玉米投手</color>\n<color=#3D1400>词条1：</color><color=red>彩虹伞神：当场上同时有9种宝石伞时，所有钱币花费量降为500(解锁条件：场上同时存在9种宝石伞)</color>\n<color=#3D1400>作为餐厅的主厨，黄宝石伞做的菜一直饱受好评，“这要归功于师傅娴熟的按摩技术，以及作为主要厨具的自我修养。”</color>");
        }

        [HideFromIl2Cpp]
        public static bool SuperBlockEffect(SuperUmbrella __instance, ref Zombie zombie)
        {
            if (__instance.thePlantType is (PlantType)175 && !zombie.isMindControlled)
            {
                zombie.TakeDamage(DmgType.Normal, 80);
                zombie.KnockBack(1.5f * (__instance.UmbrellaPot is not null ? 2 : 1));
                zombie.Buttered(6);
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