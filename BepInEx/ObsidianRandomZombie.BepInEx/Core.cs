using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using BepInEx;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using Unity.VisualScripting;
using UnityEngine;
using BepInEx.Unity.IL2CPP;
using System.Reflection;
using CustomizeLib.BepInEx;

namespace ObsidianRandomZombie.BepInEx
{
    [HarmonyPatch(typeof(DiamondRandomZombie))]
    public static class DiamondRandomZombiePatch
    {
        [HarmonyPatch("SetRandomZombie")]
        [HarmonyPrefix]
        public static bool PreSetRandomZombie(DiamondRandomZombie __instance, ref GameObject __result)
        {
            if (__instance is not null && __instance.theZombieType is (ZombieType)98)
            {
                Vector3 position = __instance.axis.position;
                List<int> ids = [];
                if (Lawnf.TravelDebuff(ObsidianRandomZombie.Debuff))
                {
                    for (int i = 0; i < GameAPP.resourcesManager.allZombieTypes.Count; i++)
                    {
                        if (TypeMgr.IsBossZombie((ZombieType)i) && GameAPP.resourcesManager.zombiePrefabs[GameAPP.resourcesManager.allZombieTypes[i]] is not null)
                        {
                            ids.Add(i);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < GameAPP.resourcesManager.allZombieTypes.Count; i++)
                    {
                        if (GameAPP.resourcesManager.zombiePrefabs[GameAPP.resourcesManager.allZombieTypes[i]] is not null && !TypeMgr.IsBossZombie((ZombieType)i) && !TypeMgr.NotRandomZombie((ZombieType)i))
                        {
                            ids.Add(i);
                        }
                    }
                }
                if (!__instance.isMindControlled)
                {
                    __result = CreateZombie.Instance.SetZombie(__instance.theZombieRow, (ZombieType)ids[UnityEngine.Random.RandomRangeInt(0, ids.Count)], __instance.transform.position.x);
                }
                else
                {
                    __result = CreateZombie.Instance.SetZombieWithMindControl(__instance.theZombieRow, (ZombieType)ids[UnityEngine.Random.RandomRangeInt(0, ids.Count)], __instance.transform.position.x);
                }
                UnityEngine.Object.Instantiate(GameAPP.particlePrefab[11], new Vector3(__instance.transform.position.x, position.y + 1f, 0f), Quaternion.identity).transform.SetParent(GameAPP.board.transform);
                __instance.summoned = true;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(RandomZombie))]
    public static class RandomZombiePatch
    {
        [HarmonyPatch(nameof(RandomZombie.FirstArmorFall))]
        [HarmonyPrefix]
        public static bool PreFirstArmorBroken(RandomZombie __instance)
        {
            if (__instance.theZombieType is (ZombieType)98)
            {
                if (__instance.theFirstArmorHealth < __instance.theFirstArmorMaxHealth * 2 / 3)
                {
                    __instance.theFirstArmorBroken = 1;
                    __instance.theFirstArmor.GetComponent<SpriteRenderer>().sprite = GameAPP.spritePrefab[204];
                    return false;
                }
                if (__instance.theFirstArmorHealth < __instance.theFirstArmorMaxHealth / 3)
                {
                    __instance.theFirstArmorBroken = 2;
                    __instance.theFirstArmor.GetComponent<SpriteRenderer>().sprite = GameAPP.spritePrefab[205];
                    return false;
                }
                if (__instance.theFirstArmorHealth >= __instance.theFirstArmorMaxHealth * 2 / 3)
                {
                    __instance.theFirstArmorBroken = 0;
                    __instance.theFirstArmor.GetComponent<SpriteRenderer>().sprite = GameAPP.spritePrefab[207];
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Zombie))]
    public static class ZombiePatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void PostStart(Zombie __instance)
        {
            if (__instance.TryCast<UltimateGargantuar>() is not null)
            {
                if (!__instance.isMindControlled)
                {
                    CreateZombie.Instance.SetZombie(__instance.theZombieRow, (ZombieType)98, __instance.transform.position.x);
                }
                else
                {
                    CreateZombie.Instance.SetZombieWithMindControl(__instance.theZombieRow, (ZombieType)98, __instance.transform.position.x);
                }
            }
        }

        [HarmonyPatch("FindAndDestoryZombieHead")]
        [HarmonyPatch("SetCold")]
        [HarmonyPatch("SetFreeze")]
        [HarmonyPatch("Warm")]
        [HarmonyPatch("KnockBack")]
        [HarmonyPrefix]
        public static bool PreKnockBack(Zombie __instance) => __instance.theZombieType is not (ZombieType)98;
    }

    [BepInPlugin("inf75.obsidianrandomzombie", "ObsidianRandomZombie", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<ObsidianRandomZombie>();
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "obsidianrandomzombie");
            CustomCore.RegisterCustomZombie<DiamondRandomZombie, ObsidianRandomZombie>((ZombieType)98,
                ab.GetAsset<GameObject>("ObsidianRandomZombie"), 206, 50, 40000, 9000, 0);
            CustomCore.RegisterCustomSprite(204, ab.GetAsset<Sprite>("ObsidianRandomZombie_head2"));
            CustomCore.RegisterCustomSprite(205, ab.GetAsset<Sprite>("ObsidianRandomZombie_head3"));
            CustomCore.RegisterCustomSprite(206, ab.GetAsset<Sprite>("ObsidianRandomZombie_0"));
            CustomCore.RegisterCustomSprite(207, ab.GetAsset<Sprite>("ObsidianRandomZombie_head1"));
            ObsidianRandomZombie.Debuff = CustomCore.RegisterCustomBuff("黑曜石盲盒僵尸只开出领袖僵尸", BuffType.Debuff, () => true, 0);
            CustomCore.AddZombieAlmanacStrings(98, "黑曜石盲盒僵尸", "?????!!!!!\n\n<color=#3D1400>头套贴图作者：@林秋AutumnLin @E杯芒果奶昔 </color>\n<color=#3D1400>韧性：</color><color=red>9000</color>\n<color=#3D1400>特点：</color><color=red>究极黑曜石巨人生成时有50%概率伴生。免疫击退，每隔一段时间自动换行，受到攻击时扣除与减伤前伤害等量钱币，究极机械保龄球替伤无效，死亡时变成随机非领袖僵尸</color>\n<color=#3D1400>词条：</color><color=red>黑曜石盲盒僵尸只开出领袖僵尸</color>\n<color=#3D1400>“小植物们，快来看我的另一个新发明，黑曜石盲盒，看起来很棒对不对，我觉得非常好，他不但无比坚硬，还很看运气。不过有也给了一个小小的礼物，让你一定玩的「开心」，还有，不要再用大嘴花解决我的发明了！！“ \n(埃德加博士留的)</color>");
        }
    }

    public class ObsidianRandomZombie : MonoBehaviour
    {
        public ObsidianRandomZombie() : base(ClassInjector.DerivedConstructorPointer<ObsidianRandomZombie>()) => ClassInjector.DerivedConstructorBody(this);

        public ObsidianRandomZombie(IntPtr i) : base(i)
        {
        }

        public void Awake()
        {
            if (GameAPP.theGameStatus is 0 && zombie is not null)
            {
                zombie.theFirstArmor = gameObject.transform.FindChild("Zombie_head").GetChild(0).gameObject;
                zombie.butterHead = zombie.theFirstArmor;
                this.StartCoroutine(ChangeRoad());
            }
        }

        [HideFromIl2Cpp]
        public IEnumerator ChangeRoad()
        {
            do
            {
                yield return new WaitForSeconds(3);
                try
                {
                    if (Board.Instance is not null && zombie is not null && !zombie.isPreview && gameObject is not null
                        && !gameObject.IsDestroyed() && zombie is not null)
                    {
                        zombie.Garliced();
                    }
                    else
                    {
                        break;
                    }
                }
                catch { break; }
            } while (Board.Instance is not null && zombie is not null && zombie.isActiveAndEnabled);
        }

        public void Start()
        {
            if (GameAPP.theGameStatus is 0 && zombie is not null)
            {
                zombie!.theFirstArmorType = Zombie.FirstArmorType.Doll;
                zombie.theZombieType = (ZombieType)98;
            }
        }

        public static int Debuff { get; set; } = -1;

        public DiamondRandomZombie? zombie => gameObject.TryGetComponent<DiamondRandomZombie>(out var z) ? z : null;
    }
}