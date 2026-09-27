using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using BepInEx;
using UnityEngine;
using System.Reflection;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;

namespace ObsidianDollZombie.BepInEx
{
    [HarmonyPatch(typeof(DollZombie))]
    public static class DollZombiePatch
    {
        [HarmonyPatch("FirstArmorBroken")]
        [HarmonyPrefix]
        public static bool PreFirstArmorBroken(DollZombie __instance)
        {
            if (__instance.theZombieType is (ZombieType)99)
            {
                if (__instance.theFirstArmorHealth < __instance.theFirstArmorMaxHealth * 2 / 3)
                {
                    __instance.theFirstArmorBroken = 1;
                    __instance.theFirstArmor.GetComponent<SpriteRenderer>().sprite = GameAPP.spritePrefab[200];
                    return false;
                }
                if (__instance.theFirstArmorHealth < __instance.theFirstArmorMaxHealth / 3)
                {
                    __instance.theFirstArmorBroken = 2;
                    __instance.theFirstArmor.GetComponent<SpriteRenderer>().sprite = GameAPP.spritePrefab[201];
                    return false;
                }
                if (__instance.theFirstArmorHealth >= __instance.theFirstArmorMaxHealth * 2 / 3)
                {
                    __instance.theFirstArmorBroken = 0;
                    __instance.theFirstArmor.GetComponent<SpriteRenderer>().sprite = GameAPP.spritePrefab[203];
                    return false;
                }
            }
            return true;
        }

        [HarmonyPatch("FirstArmorFall")]
        [HarmonyPrefix]
        public static bool PreFirstArmorFall(DollZombie __instance)
        {
            if (__instance.theZombieType is (ZombieType)99)
            {
                Vector3 position = __instance.axis.position;

                if (!__instance.isMindControlled)
                {
                    CreateZombie.Instance.SetZombie(__instance.theZombieRow, (ZombieType)21, __instance.axis.position.x, false);
                    CreateZombie.Instance.SetZombie(__instance.theZombieRow, (ZombieType)21, __instance.axis.position.x, false);
                    CreateZombie.Instance.SetZombie(__instance.theZombieRow, (ZombieType)21, __instance.axis.position.x, false);
                }
                else
                {
                    CreateZombie.Instance.SetZombieWithMindControl(__instance.theZombieRow, (ZombieType)21, __instance.axis.position.x, false);
                    CreateZombie.Instance.SetZombieWithMindControl(__instance.theZombieRow, (ZombieType)21, __instance.axis.position.x, false);
                    CreateZombie.Instance.SetZombieWithMindControl(__instance.theZombieRow, (ZombieType)21, __instance.axis.position.x, false);
                }
                UnityEngine.Object.Instantiate(GameAPP.particlePrefab[11], new Vector3(__instance.transform.position.x, position.y + 1f, 0f), Quaternion.identity).transform.SetParent(GameAPP.board.transform);
                __instance.Die(2);
                return false;
            }
            return true;
        }
    }

    [BepInPlugin("inf75.obsidiandollzombie", "ObsidianDollZombie", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<ObsidianDollZombie>();
            ClassInjector.RegisterTypeInIl2Cpp<ObsidianRandomZombie>();
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "obsidiandollzombie");
            CustomCore.RegisterCustomZombie<DollZombie, ObsidianDollZombie>((ZombieType)99,
                ab.GetAsset<GameObject>("ObsidianDollZombie"), 202, 50, 40000, 18000, 0);
            CustomCore.RegisterCustomSprite(200, ab.GetAsset<Sprite>("ObsidianDollZombie_head2"));
            CustomCore.RegisterCustomSprite(201, ab.GetAsset<Sprite>("ObsidianDollZombie_head3"));
            CustomCore.RegisterCustomSprite(202, ab.GetAsset<Sprite>("ObsidianDollZombie_0"));
            CustomCore.RegisterCustomSprite(203, ab.GetAsset<Sprite>("ObsidianDollZombie_head1"));
            CustomCore.AddZombieAlmanacStrings(99, "黑曜石套娃僵尸", "一个很很很很肉的路障僵尸?????(似乎对小推车有着深入研究)\n\n<color=#3D1400>头套贴图作者：@林秋AutumnLin @E杯芒果奶昔 </color>\n<color=#3D1400>韧性：</color><color=red>18000</color>\n<color=#3D1400>特点：</color><color=red>钻石盲盒僵尸生成时有10%伴生，死亡时生成3个钻石套娃僵尸。免疫击退，遇到小推车时会将其拾起并回满血，此后啃咬植物直接代码杀，此状态下若再次遇到小推车则将所有小推车变成黑曜石套娃僵尸</color>\n<color=#3D1400>黑曜石套娃僵尸对自己的头套十分满意。这不仅是因为在外观上无可挑剔，更是因为层层嵌套让他无懈可击。</color>");

            CustomCore.RegisterCustomZombie<DiamondRandomZombie, ObsidianRandomZombie>((ZombieType)98,
                ab.GetAsset<GameObject>("ObsidianRandomZombie"), 206, 50, 40000, 12000, 0);
            CustomCore.RegisterCustomSprite(204, ab.GetAsset<Sprite>("ObsidianRandomZombie_head2"));
            CustomCore.RegisterCustomSprite(205, ab.GetAsset<Sprite>("ObsidianRandomZombie_head3"));
            CustomCore.RegisterCustomSprite(206, ab.GetAsset<Sprite>("ObsidianRandomZombie_0"));
            CustomCore.RegisterCustomSprite(207, ab.GetAsset<Sprite>("ObsidianRandomZombie_head1"));
            ObsidianRandomZombie.Debuff = CustomCore.RegisterCustomBuff("黑曜石盲盒僵尸只开出领袖僵尸", BuffType.Debuff, () => true, 0);
            CustomCore.AddZombieAlmanacStrings(98, "黑曜石盲盒僵尸", "?????!!!!!\n\n<color=#3D1400>头套贴图作者：@林秋AutumnLin @E杯芒果奶昔 </color>\n<color=#3D1400>韧性：</color><color=red>12000</color>\n<color=#3D1400>特点：</color><color=red>究极黑曜石巨人生成时伴生。免疫击退、冰冻、红温，遇到小推车时会将其拾起并回满血，此后啃咬植物直接代码杀，受到攻击时扣除与减伤前伤害等量钱币，究极机械保龄球替伤无效，死亡时变成随机非领袖僵尸</color>\n<color=#3D1400>词条：</color><color=red>黑曜石盲盒僵尸只开出领袖僵尸</color>\n<color=#3D1400>“小植物们，快来看我的另一个新发明，黑曜石盲盒，看起来很棒对不对，我觉得非常好，他不但无比坚硬，还很看运气。不过有也给了一个小小的礼物，让你一定玩的「开心」，还有，不要再用大嘴花解决我的发明了！！“ \n(埃德加博士留的)</color>");
        }
    }

    public class ObsidianDollZombie : MonoBehaviour
    {
        public ObsidianDollZombie() : base(ClassInjector.DerivedConstructorPointer<ObsidianDollZombie>()) => ClassInjector.DerivedConstructorBody(this);

        public ObsidianDollZombie(IntPtr i) : base(i)
        {
        }

        public void Awake()
        {
            if (GameAPP.theGameStatus is 0)
            {
                zombie.theFirstArmor = gameObject.transform.FindChild("Zombie_head").GetChild(0).gameObject;
                zombie.butterHead = zombie.theFirstArmor;
            }
        }

        public void PickUpMower()
        {
            zombie.theHealth = zombie.theMaxHealth;
            zombie.theFirstArmorHealth = zombie.theFirstArmorMaxHealth;
            zombie.FirstArmorBroken();
            zombie.UpdateHealthText();
            if (HasMower)
            {
                foreach (var m in Board.Instance.mowerArray)
                {
                    if (m is not null && m.TryGetComponent<Mower>(out var mower))
                    {
                        var row = mower.theMowerRow;
                        var x = m.transform.position.x;
                        mower.Die();
                        Destroy(mower.gameObject);
                        CreateZombie.Instance.SetZombie(row, (ZombieType)99, x);
                    }
                }
            }
            HasMower = true;
        }

        public void Start()
        {
            if (GameAPP.theGameStatus is 0 && zombie is not null)
            {
                zombie!.theFirstArmorType = Zombie.FirstArmorType.Doll;
            }
        }

        public bool HasMower { get; set; } = false;
        public DollZombie zombie => gameObject.GetComponent<DollZombie>();
    }
}