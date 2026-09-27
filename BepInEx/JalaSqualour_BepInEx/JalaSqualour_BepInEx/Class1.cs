using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using BepInEx;
using UnityEngine;
using BepInEx.Unity.IL2CPP;
using System.Reflection;
using CustomizeLib.BepInEx;
using Il2CppSystem;

namespace JalaSqualourBepInEx
{
    [BepInPlugin("salmon.jalasqualour", "JalaSqualour", "1.0")]
    public class Core : BasePlugin//304
    {
        public override void Load()
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<JalaSqualour>();

            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "jalasqualour");
            CustomCore.RegisterCustomPlant<Squalour, JalaSqualour>(
                JalaSqualour.PlantID,
                ab.GetAsset<GameObject>("JalaSqualourPrefab"),
                ab.GetAsset<GameObject>("JalaSqualourPreview"),
                new List<(int, int)>
                {
                ((int)PlantType.Squalour, (int)PlantType.Jalapeno),
                ((int)PlantType.Jalapeno, (int)PlantType.Squalour)
                },
                0f, 0f, 1800, 300, 0f, 250
            );
            CustomCore.RegisterCustomPlantSkin<Squalour, JalaSqualour>(
                JalaSqualour.PlantID,
                ab.GetAsset<GameObject>("JalaSqualourSkinPrefab"),
                ab.GetAsset<GameObject>("JalaSqualourSkinPreview"),
                new List<(int, int)>
                {
                ((int)PlantType.Squalour, (int)PlantType.Jalapeno),
                ((int)PlantType.Jalapeno, (int)PlantType.Squalour)
                },
                0f, 0f, 1800, 300, 0f, 250
            );
            CustomCore.AddPlantAlmanacStrings(JalaSqualour.PlantID,
                $"火爆猫瓜({JalaSqualour.PlantID})",
                "脸上略有红晕的红温窝瓜，根据压到僵尸数量，生成窝瓜卡片及火爆辣椒效果。\n\n<color=#3D1400>贴图作者：@林秋-AutumnLin</color>\n<color=#3D1400>伤害：</color><color=red>1800</color>\n<color=#3D1400>特点：</color><color=red>拥有窝瓜的特点，每压到1个僵尸，生成1张窝瓜卡片和本行造成一次火爆辣椒效果；压到2个僵尸，生成两个窝瓜卡片和本行释放两次火爆辣椒效果；至少压到3个时，掉落3张窝瓜卡片和1张火爆窝瓜卡片，同时在本行和本行及邻行释放火爆辣椒效果。</color>\n<color=#3D1400>融合配方：</color><color=red>猫瓜+火爆辣椒</color>\n\n<color=#3D1400>猫咪为何而生气？是打破了一个微妙的平衡？又或者是猜谜题却大败而归？不过有一个事情成为了绝佳的导火索：她刚刚可以唾手可得的僵尸被手套移走了。这次失败令她恼火，现在的状态用一个词来完美描述：“窝红温啦！”</color>"
            );
            CustomCore.TypeMgrExtra.IsFirePlant.Add((PlantType)JalaSqualour.PlantID);
        }
    }


    public class JalaSqualour : MonoBehaviour
    {
        public static int PlantID = 1913;

        public Squalour plant => gameObject.GetComponent<Squalour>();

        public void LourDie_JalaSqualour()
        {
        }
    }

    [HarmonyPatch(typeof(Squalour), nameof(Squalour.LourDie))]
    public class SqualourLourDiePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Squalour __instance)
        {
            if ((int)__instance.thePlantType == JalaSqualour.PlantID)
            {
                // 获取位置并调整
                Vector3 position = __instance.axis.position;
                Vector2 adjustedPos = new Vector2(position.x, position.y - 0.5f);

                // 根据状态生成不同的掉落卡牌
                if (__instance.squashed)
                {

                    if (__instance.squashCount >= 3)
                    {
                        Lawnf.SetDroppedCard(adjustedPos, PlantType.JalaSquash, 0);
                    }

                    int loopCount = Mathf.Min(__instance.squashCount, 3);
                    for (int i = 0; i < loopCount; i++)
                    {
                        Lawnf.SetDroppedCard(adjustedPos, PlantType.Squash, 0);
                    }
                }
                else
                {
                    Lawnf.SetDroppedCard(adjustedPos, (PlantType)JalaSqualour.PlantID, 0);
                }

                // 调用死亡方法
                __instance.Die();
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Squash), nameof(Squash.AttackZombie))]
    public class SquashAttackZombiePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Squash __instance)
        {
            if ((int)__instance.thePlantType == JalaSqualour.PlantID)
            {
                Vector3 position = __instance.axis.position;
                Vector2 centerPos = new Vector2(position.x, position.y);

                // 检测区域内的僵尸
                int zombieLayer = LayerMask.GetMask("Zombie");
                Collider2D[] colliders = Physics2D.OverlapBoxAll(
                    centerPos,
                    new Vector2(1f, 3f),
                    0f,
                    zombieLayer
                );

                if (colliders != null)
                {
                    foreach (var collider in colliders)
                    {
                        Zombie zombie;
                        if (collider.TryGetComponent<Zombie>(out zombie))
                        {
                            // 尝试攻击僵尸
                            bool attacked = __instance.AttackLandZombie(zombie);

                            // 检查是否符合特殊攻击条件
                            if (attacked && (zombie.theZombieRow == __instance.thePlantRow || zombie.theZombieType == ZombieType.DancePolZombie2))
                            {
                                __instance.ActionOnZombie(zombie);
                            }
                        }
                    }
                }

                // 检查当前位置的格子类型
                int col = Lawnf.GetColumnFromX(position.x);
                int row = __instance.thePlantRow;

                if (__instance.board.GetBoxType(col, row) != BoxType.Water)
                {
                    GameAPP.PlaySound(74, 0.5f, 1f);
                    ScreenShake.shakeDuration = 0.05f;
                }
                else // 正常格子
                {
                    // 创建特效
                    GameObject effectPrefab = Resources.Load<GameObject>("Particle/Anim/Water/WaterSplashPrefab");
                    Vector3 spawnPos = new Vector3(position.x, position.y - 1.75f, 0f);
                    Transform boardTransform = __instance.board.transform;
                    GameObject.Instantiate(effectPrefab, spawnPos, Quaternion.identity, boardTransform);

                    GameAPP.PlaySound(75, 0.5f, 1f);
                    __instance.Die();
                }

                Board board = GameAPP.board.GetComponent<Board>();

                switch (__instance.GetComponent<JalaSqualour>().plant.squashCount)
                {
                    case 0:
                        break;
                    case 1:
                        board.boardAction.CreateFireLine(__instance.thePlantRow, 1800, false, false, true, null);
                        break;
                    case 2:
                        board.boardAction.CreateFireLine(__instance.thePlantRow, 1800, false, false, true, null);
                        board.boardAction.CreateFireLine(__instance.thePlantRow, 1800, false, false, true, null);
                        break;
                    default:
                        board.boardAction.CreateFireLine(__instance.thePlantRow, 1800, false, false, true, null);
                        board.boardAction.CreateFireLine(__instance.thePlantRow, 1800, false, false, true, null);
                        if (__instance.thePlantRow - 1 >= 0 && __instance.thePlantRow - 1 < GameAPP.board.GetComponent<Board>().rowNum)
                            board.boardAction.CreateFireLine(__instance.thePlantRow - 1, 1800, false, false, true, null);
                        if (__instance.thePlantRow + 1 >= 0 && __instance.thePlantRow + 1 < GameAPP.board.GetComponent<Board>().rowNum)
                            board.boardAction.CreateFireLine(__instance.thePlantRow + 1, 1800, false, false, true, null);
                        break;
                }
                return false;
            }
            return true;
        }
    }
    [HarmonyLib.HarmonyPatch(typeof(SquashPumpkin), nameof(SquashPumpkin.AttributeEvent))]
    public class SquashPumpkinAttributeEventPatch
    {
        [HarmonyLib.HarmonyPrefix]
        public static bool Prefix(SquashPumpkin __instance)
        {
            List<Plant> plants = Lawnf.Get1x1Plants(__instance.thePlantColumn, __instance.thePlantRow).ToArray().ToList();
            bool find = false;
            Plant squalour = null;
            foreach (Plant plant in plants)
                if (plant.TryGetComponent<JalaSqualour>(out var component))
                {
                    find = true;
                    squalour = plant;
                }
            if (find)
            {
                __instance.attributeCountdown = 3f;

                if (__instance.littleSquash == null)
                {
                    // 验证种植位置是否有效
                    BoxType gridType = __instance.board.GetBoxType(__instance.thePlantRow, __instance.thePlantColumn);
                    if (gridType != BoxType.Water) // 非水路格子
                    {
                        // 实例化预制体
                        GameObject instance = UnityEngine.Object.Instantiate(
                            __instance.squashPrefab_lour,
                            __instance.axis.position,
                            Quaternion.identity,
                            __instance.board.transform
                        );

                        // 获取LittleSquash组件
                        __instance.littleSquash = instance.GetComponent<LittleSquash>();

                        // 设置伤害值
                        if (__instance.littleSquash != null)
                        {
                            __instance.littleSquash.theDamage = squalour.attackDamage / 8;
                        }

                        // 设置小型Squash的位置
                        if (__instance.littleSquash != null)
                        {
                            __instance.littleSquash.theRow = __instance.thePlantRow;
                            Vector3 position = __instance.axis.position;

                            // 播放特效和音效
                            CreateParticle.SetParticle(1, position, __instance.thePlantRow, true);
                            GameAPP.PlaySound(22, 0.5f, 1.0f);
                        }
                    }
                }
                return false;
            }
            return true;
        }
    }
}