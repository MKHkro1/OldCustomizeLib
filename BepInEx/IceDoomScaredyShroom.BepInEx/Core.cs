using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using BepInEx;
using UnityEngine;
using BepInEx.Unity.IL2CPP;
using System.Reflection;
using CustomizeLib.BepInEx;

namespace IceDoomScaredyShroom.BepInEx
{
    [BepInPlugin("inf75.icedoomscaredyshroom", "IceDoomScaredyShroom", "1.0")]
    public class Core : BasePlugin//304
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<IceDoomScaredyShroom>();
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "icedoomscaredyshroom");
            CustomCore.RegisterCustomPlant<IceScaredyShroom, IceDoomScaredyShroom>(304, ab.GetAsset<GameObject>("IceDoomScaredyShroomPrefab"),
                ab.GetAsset<GameObject>("IceDoomScaredyShroomPreview"), [(9, 1040), (1040, 9), (1038, 11), (11, 1038), (1042, 10), (10, 1042)], 0.8f, 0, 20, 300, 7.5f, 300);
            CustomCore.TypeMgrExtra.IsIcePlant.Add((PlantType)304);
            CustomCore.AddPlantAlmanacStrings(304, "冰毁胆小菇(304)", "发射冰毁子弹，害怕时会缩头并造成冰毁爆炸，自身血量变为原来的三分之一。\n<color=#3D1400>贴图作者：@仨硝基甲苯_ @屑红leong </color>\n<color=#3D1400>伤害：</color><color=red>20(同超喷),1800(全屏)</color>\n<color=#3D1400>融合配方：</color><color=red>胆小菇+寒冰菇+毁灭菇</color>\n<color=#3D1400>地下的咚咚声，从天而降的喊叫声都把冰毁胆小菇吓得不轻，“没事的，想象他们不存在……”等她起身时，他们果真不在了。</color>");
        }
    }

    public class IceDoomScaredyShroom : MonoBehaviour
    {
        public IceDoomScaredyShroom() : base(ClassInjector.DerivedConstructorPointer<IceDoomScaredyShroom>()) => ClassInjector.DerivedConstructorBody(this);

        public IceDoomScaredyShroom(IntPtr i) : base(i)
        {
        }

        public void AnimDoom()
        {
            Board.Instance.SetDoom(plant.thePlantColumn, plant.thePlantRow, false, true, default, 1800);
            plant.thePlantHealth = (int)(plant.thePlantHealth / 3f);
            plant.UpdateText();
        }

        public void SuperAnimShoot()
        {
            var t = plant.transform.Find("Shoot");
            CreateBullet.Instance.SetBullet(t.position.x + 0.1f, t.position.y, plant.thePlantRow, BulletType.Bullet_iceDoom, 0).Damage = plant.attackDamage;
            GameAPP.PlaySound(68);
        }

        public IceScaredyShroom plant => gameObject.GetComponent<IceScaredyShroom>();
    }
}