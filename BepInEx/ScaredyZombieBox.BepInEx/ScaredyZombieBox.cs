using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using Il2CppInterop.Runtime.Injection;
using System.Reflection;
using UnityEngine;

namespace ScaredyZombieBox.BepInEx
{
    [BepInPlugin("inf75.scaredyzombiebox", "ScaredyZombieBox", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<ScaredyZombieBox>();
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "scaredyzombiebox");
            CustomCore.RegisterCustomPlant<ScaredyShroom, ScaredyZombieBox>(305, ab.GetAsset<GameObject>("ScaredyZombieBoxPrefab"),
                ab.GetAsset<GameObject>("ScaredyZombieBoxPreview"), [(250, 1024)], 0.8f, 0, 40, 300, 7.5f, 300);
            CustomCore.AddPlantAlmanacStrings(305, "僵尸盒子胆小菇(305)", "发射孢子，害怕时会缩头并生成一个魅惑黄金盲盒。\n<color=#3D1400>贴图作者：@林秋AutumnLin </color>\n<color=#3D1400>伤害：</color><color=red>40</color>\n<color=#3D1400>融合配方：</color><color=red>僵尸礼盒+魅惑胆小菇(有序)</color>\n<color=#3D1400>每当礼物胆小菇缩头时，身后都会传来一阵嘲笑，“我们知道是你” 。但这次却没有了笑声，因为在他的周围，全是僵尸礼盒...</color>");
        }
    }

    public class ScaredyZombieBox : MonoBehaviour
    {
        public ScaredyZombieBox() : base(ClassInjector.DerivedConstructorPointer<ScaredyZombieBox>()) => ClassInjector.DerivedConstructorBody(this);

        public ScaredyZombieBox(IntPtr i) : base(i)
        {
        }

        public void AnimScared()
        {
            CreateZombie.Instance.SetZombieWithMindControl(plant.thePlantRow, ZombieType.RandomPlusZombie, transform.position.x);
            Instantiate(GameAPP.particlePrefab[11], transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, plant.board.transform);
        }

        public ScaredyShroom plant => gameObject.GetComponent<ScaredyShroom>();
    }
}