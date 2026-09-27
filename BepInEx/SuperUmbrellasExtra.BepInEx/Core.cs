using HarmonyLib;
using BepInEx;
using System.Reflection;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;
using CustomizeLib.BepInEx;

namespace SuperUmbrellasExtra.BepInEx
{
    [HarmonyPatch(typeof(Money), "UsedEvent")]
    public static class MoneyPatch
    {
        public static void Prefix(ref int cost)
        {
            if (Lawnf.TravelAdvanced(Core.Buff1) && Board.Instance.ObjectExist<SuperChomperUmbrella>() && Board.Instance.ObjectExist<SuperHypnoUmbrella>()
                && Board.Instance.ObjectExist<SuperCornUmbrella>() && Board.Instance.ObjectExist<SuperDoomUmbrella>() && Board.Instance.ObjectExist<SuperGarlicUmbrella>()
                && Board.Instance.ObjectExist<SuperIceUmbrella>() && Board.Instance.ObjectExist<SuperJalapenoUmbrella>() && Board.Instance.ObjectExist<EmeraldUmbrella>()
                && Board.Instance.ObjectExist<RedEmeraldUmbrella>())
            {
                cost = 500;
            }
        }
    }

    [HarmonyPatch(typeof(SuperUmbrella), "BlockEffect")]
    public static class SuperUmbrellaPatch
    {
        public static bool Prefix(SuperUmbrella __instance, ref Zombie zombie)
        {
            return SuperCornUmbrella.SuperBlockEffect(__instance, ref zombie) &&
                SuperHypnoUmbrella.SuperBlockEffect(__instance, ref zombie) &&
                SuperChomperUmbrella.SuperBlockEffect(__instance, ref zombie) &&
                SuperDoomUmbrella.SuperBlockEffect(__instance, ref zombie) &&
                SuperGarlicUmbrella.SuperBlockEffect(__instance, ref zombie) &&
                SuperIceUmbrella.SuperBlockEffect(__instance, ref zombie) &&
                SuperJalapenoUmbrella.SuperBlockEffect(__instance, ref zombie);
        }
    }

    [BepInPlugin("inf75.superumbrellasextra", "SuperUmbrellasExtra", "1.0")]
    public class Core : BasePlugin
    {
        public override void Load()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var ab = CustomCore.GetAssetBundle(Assembly.GetExecutingAssembly(), "superumbrellasextra");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ClassInjector.RegisterTypeInIl2Cpp<SuperCornUmbrella>();
            ClassInjector.RegisterTypeInIl2Cpp<SuperHypnoUmbrella>();
            ClassInjector.RegisterTypeInIl2Cpp<SuperChomperUmbrella>();
            ClassInjector.RegisterTypeInIl2Cpp<SuperDoomUmbrella>();
            ClassInjector.RegisterTypeInIl2Cpp<SuperGarlicUmbrella>();
            ClassInjector.RegisterTypeInIl2Cpp<SuperIceUmbrella>();
            ClassInjector.RegisterTypeInIl2Cpp<SuperJalapenoUmbrella>();
            SuperCornUmbrella.Register(ab);                             //175
            SuperHypnoUmbrella.Register(ab);                            //176
            SuperChomperUmbrella.Register(ab);                          //170
            SuperDoomUmbrella.Register(ab);                             //171
            SuperGarlicUmbrella.Register(ab);                           //172
            SuperIceUmbrella.Register(ab);                              //173
            SuperJalapenoUmbrella.Register(ab);                         //174
            for (int i = 170; i <= 176; i++)
            {
                CustomCore.AddFusion(916, i, 26);
                CustomCore.AddFusion(923, i, 32);
            }
            Buff1 = CustomCore.RegisterCustomBuff("彩虹伞神：当场上同时有9种宝石伞时，所有钱币花费量降为500", BuffType.AdvancedBuff,
                () => Board.Instance.ObjectExist<SuperChomperUmbrella>() && Board.Instance.ObjectExist<SuperHypnoUmbrella>()
                && Board.Instance.ObjectExist<SuperCornUmbrella>() && Board.Instance.ObjectExist<SuperDoomUmbrella>() && Board.Instance.ObjectExist<SuperGarlicUmbrella>()
                && Board.Instance.ObjectExist<SuperIceUmbrella>() && Board.Instance.ObjectExist<SuperJalapenoUmbrella>() && Board.Instance.ObjectExist<EmeraldUmbrella>()
                && Board.Instance.ObjectExist<RedEmeraldUmbrella>(), 36100, "red", (PlantType)176);
            Buff2 = CustomCore.RegisterCustomBuff("保护保护伞：紫、黑、魅宝石伞触发被动时受伤大幅减少且可被替伤", BuffType.AdvancedBuff, () => Board.Instance.ObjectExist<SuperHypnoUmbrella>()
                || Board.Instance.ObjectExist<SuperChomperUmbrella>() || Board.Instance.ObjectExist<SuperDoomUmbrella>(), 10700, "#DA64FF", (PlantType)171);
        }

        public static int Buff1 { get; set; }
        public static int Buff2 { get; set; }
    }
}