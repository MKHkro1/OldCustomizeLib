using CustomizeLib.BepInEx;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using Unity.VisualScripting;
using UnityEngine;

namespace SuperUmbrellasExtra.BepInEx
{
    public class SuperHypnoUmbrella : MonoBehaviour
    {
        public SuperHypnoUmbrella() : base(ClassInjector.DerivedConstructorPointer<SuperHypnoUmbrella>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperHypnoUmbrella(IntPtr i) : base(i)
        {
        }

        [HideFromIl2Cpp]
        public static void Register(AssetBundle ab)
        {
            CustomCore.RegisterCustomPlant<SuperUmbrella, SuperHypnoUmbrella>(176, ab.GetAsset<GameObject>("SuperHypnoUmbrellaPrefab"),
              ab.GetAsset<GameObject>("SuperHypnoUmbrellaPreview"), [(916, 8), (923, 8), (170, 8), (171, 8), (172, 8), (173, 8), (175, 8), (174, 8),], 3, 0, 3600, 4000, 60f, 1800);
            CustomCore.RegisterSuperSkill(176, (plant) =>
            {
                var pos = plant.axis.position;
                LayerMask layermask = plant.zombieLayer.m_Mask;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 3f);
                int i = 1;
                foreach (var z in array)
                {
                    if (z is not null && z.gameObject.TryGetComponent<Zombie>(out var zombie) && !zombie.isMindControlled)
                    {
                        i++;
                    }
                }
                return 1000 * (10 + (int)(4 * Math.Log(i)));
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
                        zombie.SetMindControl();
                    }
                }
            });
            CustomCore.TypeMgrExtra.UmbrellaPlants.Add((PlantType)176);
            CustomCore.AddPlantAlmanacStrings(176, "魅宝石伞(176)", "魅宝石伞能放大招魅惑周围的僵尸\n<color=#3D1400>贴图作者：@仨硝基甲苯_ @林秋AutumnLin </color>\n<color=#3D1400>特点：</color><color=red>僵尸主动靠近魅宝石伞时将其魅惑，使伞扣除700血量且无法替伤，花费1000*(10+4ln(要魅惑的僵尸数+1))钱币释放大招，魅惑周围全部僵尸</color>\n<color=#3D1400>融合配方：</color><color=red>其他宝石伞+魅惑菇</color>\n<color=#3D1400>词条1：</color><color=red>彩虹伞神：当场上同时有9种宝石伞时，所有钱币花费量降为500(解锁条件：场上同时存在9种宝石伞)</color>\n<color=#3D1400>词条2：</color><color=red>保护保护伞：紫、黑、魅宝石伞触发被动时受伤大幅减少且可被替伤(解锁条件：场上存在紫、黑、魅宝石伞之一)</color>\n<color=#3D1400>据说，若有人能找到彩宝石伞最喜爱的颜色，她将短暂地从睡梦中醒来，展露自己的光辉。但她的喜好没有规律可循，就像彩虹不会为任何人停留。</color>");
        }

        [HideFromIl2Cpp]
        public static bool SuperBlockEffect(SuperUmbrella __instance, ref Zombie zombie)
        {
            if (__instance.thePlantType is (PlantType)176 && !zombie.isMindControlled)
            {
                zombie.SetMindControl();
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