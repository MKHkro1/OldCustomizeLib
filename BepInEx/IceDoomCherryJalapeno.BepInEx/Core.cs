using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using BepInEx;
using Unity.VisualScripting;
using UnityEngine;
using BepInEx.Unity.IL2CPP;
using System.Reflection;
using CustomizeLib.BepInEx;

namespace IceDoomCherryJalapeno.BepInEx
{
    [HarmonyPatch(typeof(CherryJalapeno), "OnTriggerStay2D")]
    public static class CherryJalapenoPatch
    {
        public static bool Prefix(CherryJalapeno __instance, ref Collider2D collision)
        {
            if (__instance.thePlantType is (PlantType)177 && __instance.GameObject().TryGetComponent<IceDoomCherryJalapeno>(out var p) && p is not null)
            {
                if (collision.gameObject.TryGetComponent<Zombie>(out var zombie) && zombie is not null && !zombie.IsDestroyed() && zombie.theZombieRow == __instance.thePlantRow)
                {
                    var par = CreateParticle.SetParticle(203, __instance.transform.position, __instance.thePlantRow, true);
                    var bc = par.GetComponent<BombCherry>();
                    bc.bombRow = __instance.thePlantRow;
                    bc.explodeDamage = __instance.attackDamage;
                    bc.bulletFromZombie = false;
                    GameAPP.PlaySound(40, 0.2f);
                    zombie.KnockBack(0.5f, Zombie.KnockBackReason.ByJalapeno);
                    zombie.SetFreeze(10);
                    if (!p.ZombiesHash.Contains(zombie.GetHashCode()))
                    {
                        p.ZombiesHash.Add(zombie.GetHashCode() * 1);
                    }

                    if (p.ZombiesHash.Count % 5 == 0)
                    {
                        Board.Instance.SetDoom(Mouse.Instance.GetColumnFromX(__instance.transform.position.x), __instance.thePlantRow, false, true, default, 1800);
                    }
                    return false;
                }
            }
            return true;
        }
    }

    [BepInPlugin("inf75.icedoomcherryjalapeno", "IceDoomCherryJalapeno", "1.0")]
    public class Core : BasePlugin//177
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "icedoomcherryjalapeno");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<IceDoomCherryJalapeno>();
            CustomCore.RegisterCustomPlant<CherryJalapeno, IceDoomCherryJalapeno>(177, ab.GetAsset<GameObject>("IceDoomCherryJalapenoPrefab"),
                ab.GetAsset<GameObject>("IceDoomCherryJalapenoPreview"), [(1040, 1179), (1179, 1040)], 3, 0, 300, 300, 7.5f, 700);
            var par = ab.GetAsset<GameObject>("IceDoomCloudSmall");
            par.AddComponent<BombCherry>();
            CustomCore.RegisterCustomParticle((ParticleType)203, par);
            CustomCore.TypeMgrExtra.IsIcePlant.Add((PlantType)177);
            CustomCore.AddPlantAlmanacStrings(177, "冰毁爆竹(177)", "炸伤并冰冻碰到的僵尸，每碰到5个僵尸生成一次冰毁爆炸\n<color=#3D1400>贴图作者：@仨硝基甲苯_ @屑红leong </color>\n<color=#3D1400>伤害：</color><color=red>300(同烈焰爆竹),1800(全屏)</color>\n<color=#3D1400>融合配方：</color><color=red>烈焰爆竹+寒冰毁灭菇</color>\n<color=#3D1400>作为远近闻名的艺术家，冰毁爆竹的炸弹配方一直是商业机密，想要的人不在少数，“想要？站前面看着吧。”不巧的是此时他真正进行行为艺术《艺术就是……》</color>");
        }
    }

    public class IceDoomCherryJalapeno : MonoBehaviour
    {
        public IceDoomCherryJalapeno() : base(ClassInjector.DerivedConstructorPointer<IceDoomCherryJalapeno>()) => ClassInjector.DerivedConstructorBody(this);

        public IceDoomCherryJalapeno(IntPtr i) : base(i)
        {
        }

        public void OnDestroy()
        {
            if (GameAPP.theGameStatus is 0)
            {
                Board.Instance.SetDoom(Mouse.Instance.GetColumnFromX(transform.position.x), plant.thePlantRow, false, true, default, 1800);
            }
        }

        public CherryJalapeno plant => gameObject.GetComponent<CherryJalapeno>();
        public List<int> ZombiesHash { [HideFromIl2Cpp] get; [HideFromIl2Cpp] set; } = [];
    }
}