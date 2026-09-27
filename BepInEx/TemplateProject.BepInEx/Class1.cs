using BepInEx;
using BepInEx.Unity.IL2CPP;
using CustomizeLib.BepInEx;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System.Collections.Generic;
using UnityEngine;

namespace TemplateProject.BepInEx
{
    [BepInPlugin("salmon.", "", "1.0")]
    public class Core : CorePlugin
    {
        // 模板占位：下面 7 个量上游只写了名字没给定义（CS0103），这里补上可直接改的默认值。
        // fusions 是"材料有序对"，本植物即其产物；不需要融合就保持空表。
        public static List<(int, int)> fusions = new();
        public static float attackInterval = 1.5f;   // 攻击间隔（秒）
        public static float produceInterval = 7.5f;  // 产阳光间隔（秒）
        public static int attackDamage = 20;          // 单发伤害
        public static int maxHealth = 300;            // 血量上限
        public static float cd = 7.5f;                // 卡牌冷却（秒）
        public static int sun = 100;                  // 种植阳光

        public override void OnStart()
        {
            var ab = CustomCore.GetAssetBundle(Tools.GetAssembly(), "abname");
            // 第一个泛型参数 TBase 是要继承的游戏植物基类，替换成自己用的那个
            // （4.0 里常见的有 PeaShooter / ThreePeater / WallNut / Chomper 等，都在全局命名空间）。
            // 上游模板这里写的是一个根本没定义的 TBase，所以永远编译不过（CS0246）。
            CustomCore.RegisterCustomPlant<PeaShooter, TemplatePlant>(TemplatePlant.PlantID, ab.GetAsset<GameObject>("Prefab"),
                ab.GetAsset<GameObject>("Preview"), fusions, attackInterval, produceInterval, attackDamage, maxHealth, cd, sun);
            CustomCore.AddPlantAlmanacStrings(TemplatePlant.PlantID, $"AlmanacName",
                "xxx\n\n" +
                "<color=#3D1400>贴图作者：@林秋-AutumnLin</color>\n" +
                "<color=#3D1400>韧性：</color><color=red>xxx</color>\n" +
                "<color=#3D1400>特点：</color><color=red>xxx</color>\n\n" +
                "<color=#3D1400>宝开鱼</color>");
        }
    }

    public class TemplatePlant : MonoBehaviour
    {
        public static ID PlantID;
    }
}
