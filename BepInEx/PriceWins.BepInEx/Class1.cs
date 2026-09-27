using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Core;

namespace PriceWins.BepInEx
{
    [BepInPlugin("salmon.pricewins", "PriceWins", "1.0")]
    public class Core : BasePlugin
    {
        public static LevelType levelType = (LevelType)1900;
        public static int levelID = 1900;
        public static List<ZombieType> zombieList = new List<ZombieType>()
        {
            ZombieType.NormalZombie,
            ZombieType.ConeZombie,
            ZombieType.BucketZombie,
            ZombieType.WallNutZombie
        };

        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(UIMgr))]
    public static class UIMgrPatch
    {
        [HarmonyPatch(nameof(UIMgr.EnterChallengeMenu))]
        [HarmonyPostfix]
        public static void PostEnterChallengeMenu(UIMgr __instance)
        {
            GameAPP.Instance.StartCoroutine(Init().WrapToIl2Cpp());
            IEnumerator Init()
            {
                yield return null;
                var page = GameAPP.canvas.FindChild($"ChallengeMenu(Clone)/Levels/PageMiniGames/Pages");
                var targetPage = page.FindChild($"Page{page.childCount}");
                var level = targetPage.GetChild(0);
                var levelCount = 0;
                for (int i = 0; i < targetPage.childCount; i++)
                    if (targetPage.GetChild(i).gameObject.activeSelf)
                        levelCount++;
                var rowCount = levelCount / 6; // 获取有多少行
                var rowLevelCount = levelCount % 6; // 获取当前行有多少关卡
                // f(x) = 150x-300 x坐标函数
                // g(x) = -130x+160 y坐标函数
                int x = 150 * rowLevelCount - 300;
                int y = -130 * rowCount + 160;
                var pos = new Vector2(x, y);
                var newLevel = UnityEngine.Object.Instantiate(level.gameObject, targetPage);
                newLevel.transform.localPosition = pos;
                newLevel.GetComponent<Image>().sprite = Resources.Load<Sprite>("image/Almanac_GroundDay"); // 设置图标背景
                newLevel.transform.GetChild(0).GetComponent<Image>().sprite =
                    GameAPP.resourcesManager._plantPreviews[PlantType.EndoFlame][0].GetComponent<SpriteRenderer>().sprite; // 设置植物图标
                newLevel.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "优胜劣汰"; // 设置关卡名称
                newLevel.transform.GetChild(1).GetComponent<Advanture_Btn>().levelType = Core.levelType;
                newLevel.transform.GetChild(1).GetComponent<Advanture_Btn>().buttonNumber = Core.levelID;
                newLevel.SetActive(true);
                yield break;
            }
        }

        [HarmonyPatch(nameof(UIMgr.EnterGame))]
        [HarmonyPrefix]
        public static bool PreEnterGame(LevelType levelType, int levelNumber, int id, string name)
        {
            if (levelType != Core.levelType) return true;

            SynergyManager.Instance.ClearAllSynergies();
            EventManager.ClearAllEvents();
            GameAPP.UIManager.PopAll();
            CamaraFollowMouse.Instance.ResetCamera();

            Time.timeScale = GameAPP.config.gameSpeed;

            GameAPP.theBoardType = levelType;
            GameAPP.theBoardLevel = levelNumber;

            RogueManager.Instance.Clear();

            GameObject boardGO = new("Board");
            GameAPP.board = boardGO;
            Board board = boardGO.AddComponent<Board>();
            var tag = board.boardTag;
            tag.isNight = true;
            board.boardTag = tag;
            board.rowNum = 5;
            board.theMaxWave = 20;
            board.theSun = 500;
            MapData_cs.GetMap(SceneType.Night, board);
            //GameObject mapInstance = UnityEngine.Object.Instantiate(MapData_cs.GetMap(SceneType.Night, board), boardGO.transform);
            //board.ChangeMap(mapInstance);

            InitZombieList.InitZombie(levelType, levelNumber);

            // 播放音乐并开始游戏
            GameAPP.Instance.PlayMusic(MusicType.SelectCard);
            GameAPP.theGameStatus = GameStatus.InInterlude;
            board.gameObject.AddComponent<InitBoard>();
            return false;
        }
    }

    [HarmonyPatch(typeof(InGameUI))]
    public static class InGameUIPatch
    {
        [HarmonyPatch(nameof(InGameUI.SetUniqueText))]
        [HarmonyPostfix]
        public static void PostSetUniqueText(InGameUI __instance, ref Il2CppSystem.Collections.Generic.List<TextMeshProUGUI> T)
        {
            if (GameAPP.theBoardType == Core.levelType && GameAPP.theBoardLevel == Core.levelID)
            {
                __instance.ChangeString(T, "优胜劣汰");
            }
        }
    }

    [HarmonyPatch(typeof(InitZombieList))]
    public static class InitZombieListPatch
    {
        [HarmonyPatch(nameof(InitZombieList.PickZombie))]
        [HarmonyPrefix]
        public static void PrePickZombie()
        {
            if (GameAPP.theBoardType == Core.levelType && GameAPP.theBoardLevel == Core.levelID)
            {
                foreach (var zt in Core.zombieList)
                    InitZombieList.zombieToSpawns.Add(zt);
            }
        }
    }

    [HarmonyPatch(typeof(WaveManager))]
    public static class WaveManagerPatch
    {
        [HarmonyPatch(nameof(WaveManager.GetMaxWave))]
        [HarmonyPostfix]
        public static void PostGetMaxWave(ref int __result)
        {
            if (GameAPP.theBoardType == Core.levelType && GameAPP.theBoardLevel == Core.levelID)
            {
                __result = 30;
            }
        }
    }

    [HarmonyPatch(typeof(CreatePlant))]
    public static class CreatePlantPatch
    {
        [HarmonyPatch(nameof(CreatePlant.Lim))]
        [HarmonyPostfix]
        public static void PostLim(CreatePlant __instance, ref PlantType theSeedType, ref bool __result)
        {
            if (GameAPP.theBoardType == Core.levelType && GameAPP.theBoardLevel == Core.levelID)
            {
                if (Lawnf.IsSuperPlant(theSeedType))
                {
                    __result = true;
                    InGameText.Instance.ShowText("本关不能使用超级植物", 1f);
                    InGameTextPatch.disable = true;
                }
            }
        }

        [HarmonyPatch(nameof(CreatePlant.SetPlant))]
        [HarmonyPostfix]
        public static void PostSetPlant(PlantType theSeedType)
        {
            if (GameAPP.theBoardType == Core.levelType && GameAPP.theBoardLevel == Core.levelID)
            {
                var list = Lawnf.GetAllPlants().ToArray().ToList().Where(p => p != null && !TypeMgr.IsPumpkin(p.thePlantType)).ToList();
                if (list.Count > 3)
                {
                    var plants = list.OrderByDescending((p) =>
                    {
                        if (PlantDataManager.GetPlantData(p.thePlantType) != null)
                            return PlantDataManager.GetPlantData(p.thePlantType).cost;
                        return 0;
                    }).Take(3).ToList();
                    foreach (var plant in list)
                    {
                        if (!plants.Contains(plant))
                            plant.Die((Plant.DieReason)1900);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(InGameText), nameof(InGameText.ShowText))]
    public static class InGameTextPatch
    {
        public static bool disable = false;
        [HarmonyPrefix]
        public static bool Prefix(string text, float time)
        {
            if (text == "通关挑战模式解锁配方" && time == 7f && disable)
            {
                disable = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Board))]
    public static class BoardPatch
    {
        [HarmonyPatch(nameof(Board.Start))]
        [HarmonyPostfix]
        public static void PostStart(Board __instance)
        {
            if (GameAPP.theBoardType == Core.levelType && GameAPP.theBoardLevel == Core.levelID)
            {
                __instance.StartCoroutine(OnGameStart().WrapToIl2Cpp());
                IEnumerator OnGameStart()
                {
                    while (GameAPP.theGameStatus != GameStatus.InGame) yield return null;
                    InGameText.Instance.ShowText("本关不能使用超级植物\n" +
                        "场上最多存在3株植物\n" +
                        "价格低的植物会被价格高的植物顶替\n" +
                        "祝你好运！", 5f);
                    yield break;
                }
            }
        }
    }
}
