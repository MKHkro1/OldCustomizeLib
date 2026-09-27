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
    public class SuperJalapenoUmbrella : MonoBehaviour
    {
        public SuperJalapenoUmbrella() : base(ClassInjector.DerivedConstructorPointer<SuperJalapenoUmbrella>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperJalapenoUmbrella(IntPtr i) : base(i)
        {
        }

        [HideFromIl2Cpp]
        public static void Register(AssetBundle ab)
        {
            CustomCore.RegisterCustomPlant<SuperUmbrella, SuperJalapenoUmbrella>(174, ab.GetAsset<GameObject>("SuperJalapenoUmbrellaPrefab"),
              ab.GetAsset<GameObject>("SuperJalapenoUmbrellaPreview"), [(916, 16), (923, 16), (170, 16), (171, 16), (172, 16), (173, 16), (175, 16), (176, 16),],
              3, 0, 100, 4000, 60f, 1400);
            CustomCore.RegisterSuperSkill(174, (_) => 7000, (plant) =>
            {
                var pos = plant.axis.position;
                LayerMask layermask = plant.zombieLayer.m_Mask;
                var array = Physics2D.OverlapCircleAll(new(pos.x, pos.y), 3f);
                foreach (var z in array)
                {
                    if (z is not null && z.GameObject().TryGetComponent<Zombie>(out var zombie) && !zombie.isMindControlled && (zombie.theZombieRow == plant.thePlantRow || zombie.theZombieRow == plant.thePlantRow - 1 || zombie.theZombieRow == plant.thePlantRow + 1))
                    {
                        zombie.SetJalaed();
                        Board.Instance.CreateFireLine(plant.thePlantRow);
                        if (plant.thePlantRow is not 0)
                        {
                            Board.Instance.CreateFireLine(plant.thePlantRow - 1);
                        }
                        if (plant.thePlantRow != Board.Instance.rowNum)
                        {
                            Board.Instance.CreateFireLine(plant.thePlantRow + 1);
                        }
                    }
                }
                Board.Instance.CreateFireLine(plant.thePlantRow);
                if (plant.thePlantRow is not 0)
                {
                    Board.Instance.CreateFireLine(plant.thePlantRow - 1);
                }
                if (plant.thePlantRow != Board.Instance.rowNum)
                {
                    Board.Instance.CreateFireLine(plant.thePlantRow + 1);
                }
            });
            CustomCore.TypeMgrExtra.UmbrellaPlants.Add((PlantType)174);
            CustomCore.AddPlantAlmanacStrings(174, "橙宝石伞(174)", "橙宝石伞能放大招点燃本行及相邻两行，并使周围的僵尸红温\n<color=#3D1400>贴图作者：@仨硝基甲苯_ @林秋AutumnLin </color>\n<color=#3D1400>特点：</color><color=red>僵尸主动靠近橙宝石伞时将其弹开并使其红温，花费7000钱币释放大招，使周围的所有僵尸红温，并在本行及领路造成火爆辣椒效果</color>\n<color=#3D1400>融合配方：</color><color=red>其他宝石伞+火爆辣椒</color>\n<color=#3D1400>词条1：</color><color=red>彩虹伞神：当场上同时有9种宝石伞时，所有钱币花费量降为500(解锁条件：场上同时存在9种宝石伞)</color>\n<color=#3D1400>每天想和橙宝石伞交易的人多了去了，“你知道我推辞了多少会议才来这里吗？”他来是因为听说这里的僵尸个个僵傻钱多，但他们只想要脑子，连白送的辣椒都不要。</color>");
        }

        [HideFromIl2Cpp]
        public static bool SuperBlockEffect(SuperUmbrella __instance, ref Zombie zombie)
        {
            if (__instance.thePlantType is (PlantType)174 && !zombie.isMindControlled)
            {
                zombie.SetJalaed();
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