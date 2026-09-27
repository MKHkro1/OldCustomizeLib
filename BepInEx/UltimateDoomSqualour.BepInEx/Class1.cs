using BepInEx;
using CustomizeLib.BepInEx;
using HarmonyLib;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Internal;

namespace UltimateDoomSqualour.BepInEx
{
    [BepInPlugin("salmon.ultimatedoomsqualour", "UltimateDoomSqualour", "1.0")]
    public class Core : CorePlugin
    {
        public override void OnStart()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Tools.Assembly, "ultimatedoomsqualour");
            CustomCore.RegisterCustomPlant<Squalour, UltimateDoomSqualour>(
                (int)UltimateDoomSqualour.PlantID,
                ab.GetAsset<GameObject>("UltimateDoomSqualourPrefab"),
                ab.GetAsset<GameObject>("UltimateDoomSqualourPreview"),
                new List<(int, int)>
                {
                    ((int)PlantType.NuclearDoomCherry, (int)PlantType.Squalour)
                },
                0f, 0f, 1800, 300, 7.5f, 425
            );
            CustomCore.AddPlantAlmanacStrings((int)UltimateDoomSqualour.PlantID,
                $"核爆猫瓜({(int)UltimateDoomSqualour.PlantID})",
                "蕴含着核能力量的红温猫瓜，可千万不要招惹她…\n" +
                "<color=#0000FF>核爆樱桃同人亚种</color>\n\n" +
                "<color=#3D1400>贴图作者：@林秋-AutumnLin</color>\n" +
                "<color=#3D1400>转化配方：</color><color=red>樱桃炸弹←→猫瓜</color>\n" +
                "<color=#3D1400>伤害：</color><color=red>1800（压扁）</color>\n" +
                "<color=#3D1400>索敌范围：</color><color=red>左侧2格～右侧3格</color>\n" +
                "<color=#3D1400>特点：</color><color=red>①落地后，每碾压1个僵尸，威力增加1800，至多7200\n" +
                "②落地压到僵尸后，释放伤害等同于自身攻击力的毁灭菇效果，生成一片半径5.4格的辐射区域，其持续时间为（6+压到僵尸数量）秒，至多10秒。最后在全屏释放18次毁灭菇爆炸\n" +
                "③右侧一格有火红莲时，碾压火红莲并造成7200的毁灭菇爆炸，不留坑，并返还卡片\n" +
                "④右侧一格有僵尸火红莲时，碾压僵尸火红莲并造成等同于核爆樱桃的爆炸，但是不生成子弹，不留坑，并返还卡片\n" +
                "⑤若未压到僵尸，则返还卡片</color>\n\n" +
                "<color=#3D1400>核爆猫瓜超级暴躁，最好不要招惹她，至于原因嘛，或许是看见了僵尸火红莲？</color>"
            );
            CustomCore.AddFusion((int)PlantType.NuclearDoomCherry, (int)UltimateDoomSqualour.PlantID, (int)PlantType.CherryBomb);
        }
    }

    public class UltimateDoomSqualour : MonoBehaviour
    {
        public static PlantType PlantID = (PlantType)1952;

        public void Awake()
        {
            gameObject.AddComponent<EndoFlameSaver>().endoFlame = null;
        }

        public Squalour plant => gameObject.GetComponent<Squalour>();
    }

    public class EndoFlameSaver : MonoBehaviour { public Plant? endoFlame = null; }

    [HarmonyPatch(typeof(Squalour))]
    public class SqualourPatch
    {
        [HarmonyPatch(nameof(Squalour.LourDie))]
        [HarmonyPrefix]
        public static bool Prefix(Squalour __instance)
        {
            if (__instance.thePlantType == UltimateDoomSqualour.PlantID && !__instance.squashed)
            {
                Lawnf.SetDroppedCard(__instance.axis.transform.position, __instance.thePlantType);
                __instance.Die();
                return false;
            }
            return true;
        }

        [HarmonyPatch(nameof(Squalour.ActionOnZombie))]
        [HarmonyPrefix]
        public static bool Prefix(Squalour __instance, ref Zombie zombie)
        {
            if (__instance.thePlantType == UltimateDoomSqualour.PlantID)
            {
                if (zombie == null) return false;
                zombie.TakeDamage(DmgType.Squash, 1800);
                __instance.squashCount++;
                __instance.squashed = true;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Squash))]
    public class SquashPatch
    {
        [HarmonyPatch(nameof(Squash.Range), MethodType.Getter)]
        [HarmonyPrefix]
        public static bool PreGetRange(Squash __instance, ref Vector2 __result)
        {
            if (__instance.thePlantType == UltimateDoomSqualour.PlantID)
            {
                __result = new Vector2(CoreTools.ColumnX * 6f, CoreTools.ColumnX * 6f);
                return false;
            }
            return true;
        }

        [HarmonyPatch(nameof(Squash.AttackZombie))]
        [HarmonyPrefix]
        public static bool Prefix(Squash __instance)
        {
            if (__instance.thePlantType == UltimateDoomSqualour.PlantID)
            {
                {
                    Vector3 position = __instance.axis.position;
                    Vector2 centerPos = new Vector2(position.x, position.y);

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
                            if (collider.TryGetComponent<Zombie>(out var zombie))
                            {
                                bool attacked = __instance.AttackLandZombie(zombie);

                                if (attacked && (zombie.theZombieRow == __instance.thePlantRow || zombie.theZombieType == ZombieType.DancePolZombie2))
                                {
                                    __instance.ActionOnZombie(zombie);
                                }
                            }
                        }
                    }

                    int col = Lawnf.GetColumnFromX(position.x);
                    int row = __instance.thePlantRow;

                    if (__instance.board.GetBoxType(col, row) != BoxType.Water)
                    {
                        GameAPP.PlaySound(74, 0.5f, 1f);
                        ScreenShake.shakeDuration = 0.05f;
                    }
                    else
                    {
                        GameObject effectPrefab = Resources.Load<GameObject>("Particle/Anim/Water/WaterSplashPrefab");
                        Vector3 spawnPos = new Vector3(position.x, position.y - 1.75f, 0f);
                        Transform boardTransform = __instance.board.transform;
                        GameObject.Instantiate(effectPrefab, spawnPos, Quaternion.identity, boardTransform);

                        GameAPP.PlaySound(75, 0.5f, 1f);
                        __instance.Die();
                    }
                }
                var mouse = Mouse.Instance;
                if (__instance.TryGetComponent<EndoFlameSaver>(out var saver_p) && saver_p != null && !saver_p.IsDestroyed() && saver_p.endoFlame != null &&
                    saver_p.endoFlame.thePlantType is PlantType.EndoFlame) // 普通飘飘
                {
                    var endoFlame = saver_p.endoFlame;
                    __instance.board.boardAction.SetDoom(endoFlame.thePlantColumn, endoFlame.thePlantRow, false, false, damage: 7200, existParticle: false, 
                        fromType: __instance.thePlantType);
                    float x = mouse.GetBoxXFromColumn(endoFlame.thePlantColumn), y = mouse.GetBoxYFromRow(endoFlame.thePlantRow);
                    Doom.SetDoom(__instance.board, new Vector2(x, y), DoomType.Nuclear2);
                    Lawnf.SetDroppedCard(__instance.axis.transform.position, UltimateDoomSqualour.PlantID, 0);
                    endoFlame.Crashed();
                    __instance.Die();
                }
                else if (__instance.TryGetComponent<EndoFlameSaver>(out var saver_z) && saver_z != null && !saver_z.IsDestroyed() && saver_z.endoFlame != null &&
                    saver_z.endoFlame.thePlantType is PlantType.ZombieEndoFlame) // 僵飘飘
                {
                    var endoFlame = saver_z.endoFlame;
                    __instance.board.boardAction.SetDoom(endoFlame.thePlantColumn, endoFlame.thePlantRow, false, damage: 3600, 
                        fromType: __instance.thePlantType, existParticle: false);
                    float x = mouse.GetBoxXFromColumn(endoFlame.thePlantColumn), y = mouse.GetBoxYFromRow(endoFlame.thePlantRow);
                    Doom.SetDoom(__instance.board, new Vector2(x, y), DoomType.Nuclear);

                    UnityEngine.Object.Instantiate(
                        Resources.Load<GameObject>("plants/cherrybomb/nucleardoomcherry/Radiation"),
                        __instance.axis.transform.position,
                        Quaternion.identity,
                        __instance.board.transform
                    ).GetComponent<Radiation>().damage = 3600;

                    saver_z.endoFlame.Crashed();
                    Lawnf.SetDroppedCard(__instance.axis.transform.position, UltimateDoomSqualour.PlantID, 0);
                    __instance.Die();
                }
                else if (__instance.GetComponent<Squalour>().squashed) // 正常
                {
                    int damage = 1800 + 1800 * Mathf.Min(__instance.GetComponent<Squalour>().squashCount, 3);
                    var position = __instance.axis.transform.position;
                    position.y += 0.65f;
                    int column = mouse.GetColumnFromX(__instance.axis.transform.position.x), 
                        row = mouse.GetRowFromY(__instance.axis.transform.position.x, __instance.axis.transform.position.y + 0.5f);
                    if (row != 0)
                        row++;
                    __instance.board.boardAction.SetDoom(column, row, false, false, damage: damage, existParticle: false, fromType: __instance.thePlantType);
                    if (!CoreTools.TravelAdvanced("可控核聚变"))
                        __instance.board.boardAction.SetPit(column, row);
                    __instance.board.StartCoroutine(CreateDoom(damage, __instance.board));
                    Doom.SetDoom(__instance.board, new Vector2(mouse.GetBoxXFromColumn(column), mouse.GetBoxYFromRow(row)), DoomType.Nuclear);
                    var radiation = UnityEngine.Object.Instantiate(
                        Resources.Load<GameObject>("plants/cherrybomb/nucleardoomcherry/Radiation"),
                        position,
                        Quaternion.identity, __instance.board.transform).GetComponent<Radiation>();
                    radiation.damage = damage;
                    radiation.transform.localScale = new Vector3(CoreTools.ColumnX * 5.4f / 0.5f, CoreTools.ColumnX * 5.4f / 0.5f);
                    radiation.lifeTimer = Mathf.Min(10, 6 + __instance.GetComponent<Squalour>().squashCount);
                    __instance.Die();
                }
                return false;
            }
            return true;
        }

        public static IEnumerator CreateDoom(int damage, Board board)
        {
            for (int i = 0; i < 18; i++)
            {
                if (board == null)
                {
                    yield break;
                }
                int column = UnityEngine.Random.Range(0, board.columnNum), row = UnityEngine.Random.Range(0, board.rowNum);
                board.boardAction.SetDoom(column, row, false, damage: damage, existParticle: false, fromType: UltimateDoomSqualour.PlantID);
                Doom.SetDoom(board, new Vector2(Mouse.Instance.GetBoxXFromColumn(column), Mouse.Instance.GetBoxYFromRow(row)), DoomType.Nuclear2);
                yield return new WaitForSeconds(0.05f);
            }
            yield break;
        }
    }

    [HarmonyPatch(typeof(Squash))]
    public class SqualourUpdatePatch
    {
        [HarmonyPatch(nameof(Squash.SquashUpdate))]
        [HarmonyPrefix]
        public static bool Prefix(Squash __instance)
        {
            if (__instance.thePlantType == UltimateDoomSqualour.PlantID)
            {
                List<Plant> nearPlant = Lawnf.Get1x1Plants(__instance.thePlantColumn + 1, __instance.thePlantRow).ToArray().ToList();

                foreach (Plant plant in nearPlant)
                {
                    if (plant != null && (plant.thePlantType == PlantType.EndoFlame || plant.thePlantType == PlantType.ZombieEndoFlame))
                    {
                        bool success = __instance.TryGetComponent<EndoFlameSaver>(out var component);
                        if (success)
                        {
                            component.endoFlame = plant;

                            // 设置攻击状态
                            __instance.isJump = true;

                            // 确定攻击方向（左/右）
                            __instance.anim.SetTrigger("lookleft"); // 向右看

                            // 设置父级对象（跟随棋盘）
                            __instance.transform.SetParent(__instance.board.transform);

                            // 记录攻击开始时间
                            __instance.startTime = Time.time;

                            // 保存目标位置
                            var position = plant.axis.transform.position;
                            position.y -= 0.5f;
                            __instance.endPos = position;
                            __instance.startJumpPos = position;

                            __instance.targetZombie = null;

                            // 执行攻击动作

                            // 鼠标选择特殊处理
                            __instance.invincible = true;

                            // 禁用碰撞器
                            foreach (var boxCol in __instance.boxCols)
                            {
                                boxCol.enabled = false;
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        [HarmonyPatch(nameof(Squash.AnimMove))]
        [HarmonyPrefix]
        public static bool Prefix_(Squash __instance)
        {
            if ((__instance.thePlantType == PlantType.Squalour) || ((int)__instance.thePlantType == 1913) || ((int)__instance.thePlantType == 1914))
            {
                bool findEndoFlame = false;
                Plant endoFlame = null;
                foreach (Plant plant in Lawnf.Get1x1Plants(__instance.thePlantColumn + 1, __instance.thePlantRow).ToArray().ToList())
                    if (plant.thePlantType == PlantType.EndoFlame || plant.thePlantType == PlantType.ZombieEndoFlame)
                    {
                        findEndoFlame = true;
                        endoFlame = plant;
                        bool success = __instance.TryGetComponent<EndoFlameSaver>(out var component);
                        if (success)
                            component.endoFlame = plant;
                        break;
                    }

                if (findEndoFlame)
                {
                    SpriteRenderer renderer = __instance.axis.GetComponent<SpriteRenderer>();
                    UnityEngine.Object.Destroy(renderer);

                    __instance.freeMoving = false;
                    __instance.RemoveFromList();  // 从植物列表中移除自身


                    Vector2 targetPoint = new Vector2(endoFlame.axis.transform.position.x, endoFlame.axis.transform.position.y + 1.75f);
                    __instance.startPos = __instance.axis.transform.position;
                    __instance.endPos = targetPoint;

                    // 启动跳跃协程
                    __instance.StartCoroutine(__instance.MoveToZombie(targetPoint, 5f));
                }
                return !findEndoFlame;
            }
            return true;
        }
    }
}
