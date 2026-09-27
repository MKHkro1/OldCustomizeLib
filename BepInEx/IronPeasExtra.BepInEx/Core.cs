using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using BepInEx;
using System.Reflection;
using UnityEngine;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;

namespace IronPeasExtra.BepInEx
{
    [HarmonyPatch(typeof(SuperSnowGatling))]
    public static class SuperSnowGatlingPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GetBulletType")]
        public static void PostGetBulletType(SuperSnowGatling __instance, ref BulletType __result)
        {
            if (__instance.thePlantType is (PlantType)163)
            {
                __result = BulletType.Bullet_ironPea;
            }
        }
    }

    public class BigIronGatlingPea : MonoBehaviour
    {
        public BigIronGatlingPea() : base(ClassInjector.DerivedConstructorPointer<BigIronGatlingPea>()) => ClassInjector.DerivedConstructorBody(this);

        public BigIronGatlingPea(IntPtr i) : base(i)
        {
        }

        public void AnimRaised()
        {
            if (plant is not null)
                plant.theStatus = PlantStatus.BigGatling_raised;
        }

        public void AnimShooting()
        {
            if (plant.thePlantType is (PlantType)301)
            {
                if (plant.theStatus is not PlantStatus.BigGatling_raised) return;
                var pos = plant.shoot.transform.position;
                CreateBullet.Instance.SetBullet(pos.x, pos.y - 0.3f, plant.thePlantRow, BulletType.Bullet_ironPea, 0).Damage = plant.attackDamage;
                CreateBullet.Instance.SetBullet(pos.x, pos.y, plant.thePlantRow, BulletType.Bullet_ironPea, 0).Damage = plant.attackDamage;
                CreateBullet.Instance.SetBullet(pos.x, pos.y + 0.3f, plant.thePlantRow, BulletType.Bullet_ironPea, 0).Damage = plant.attackDamage;
            }
        }

        public void Awake()
        {
            plant.shoot = plant.gameObject.transform.GetChild(0).GetChild(3);
            var tag = plant.plantTag;
            tag.doubleBoxPlant = true;
            plant.plantTag = tag;
            plant.isConnected = true;
        }

        public BigGatling plant => gameObject.GetComponent<BigGatling>();
    }

    [BepInPlugin("inf75.ironpeas", "IronPeasExtra", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<BigIronGatlingPea>();
            ClassInjector.RegisterTypeInIl2Cpp<SuperIronGatling>();
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "ironpeas");
            CustomCore.RegisterCustomPlant<BigGatling, BigIronGatlingPea>(301, ab.GetAsset<GameObject>("BigIronGatlingPeaPrefab"),
                ab.GetAsset<GameObject>("BigIronGatlingPeaPreview"), [], 0.3f, 0, 80, 2500, 15, 1000);
            CustomCore.RegisterCustomPlant<SuperSnowGatling, SuperIronGatling>(163, ab.GetAsset<GameObject>("SuperIronGatlingPrefab"),
                ab.GetAsset<GameObject>("SuperIronGatlingPreview"), [(1168, 1020), (1020, 1168)], 0.3f, 0, 80, 2500, 15, 800);
            CustomCore.RegisterCustomUseItemOnPlantEvent(PlantType.BigGatling, BucketType.Bucket, (PlantType)301);
            CustomCore.TypeMgrExtra.DoubleBoxPlants.Add((PlantType)301);
            CustomCore.AddPlantAlmanacStrings(301, "铁桶机枪豌豆炮台(301)", "会发射铁豌豆的巨型豌豆炮台\n<color=#3D1400>贴图作者：@屑红leong</color>\n<color=#3D1400>伤害：</color><color=red>80</color>\n<color=#3D1400>融合配方：</color><color=red>巨型豌豆炮台+铁桶</color>\n<color=#3D1400>铁桶机枪豌豆炮台认为，身上的每一处缺口，每一道磨痕，都象征着一场艰苦的战斗。每一次打磨，都是为了在下一场战斗中更加无坚不摧。</color>");
            CustomCore.AddPlantAlmanacStrings(163, "超级铁豌豆机枪(163)", "会发射铁豌豆的超级机枪射手\n<color=#3D1400>贴图作者：@屑红leong</color>\n<color=#3D1400>伤害：</color><color=red>80</color>\n<color=#3D1400>融合配方：</color><color=red>超级机枪射手+铁豌豆</color>\n<color=#3D1400>词条：</color><color=red>炽热铁豆：超级铁豌豆机枪发射红色铁豆，6倍伤害(解锁条件：场上存在超级铁豌豆机枪)</color>\n<color=#3D1400>超级铁豌豆机枪站在前线，像一支军队般横扫着战场上的敌人。僵尸们或许以为自己能冲破防线，但很快就会发现，面对钢铁子弹的洪流，他们毫无胜算。</color>");
        }
    }

    public class SuperIronGatling : MonoBehaviour
    {
        public SuperIronGatling() : base(ClassInjector.DerivedConstructorPointer<SuperIronGatling>()) => ClassInjector.DerivedConstructorBody(this);

        public SuperIronGatling(IntPtr i) : base(i)
        {
        }

        public void Awake()
        {
            plant.shoot = plant.gameObject.transform.GetChild(0).GetChild(0);
        }

        public SuperSnowGatling plant => gameObject.GetComponent<SuperSnowGatling>();
    }
}