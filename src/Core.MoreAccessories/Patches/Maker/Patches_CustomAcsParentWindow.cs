#if EC
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
#endif
#if KK || KKS
#endif
using ChaCustom;
using HarmonyLib;

namespace MoreAccessoriesKOI.Patches.Maker
{
    public static class CustomAcsParentWindowdow_Patches
    {
        [HarmonyPatch(typeof(CustomAcsParentWindow), nameof(CustomAcsParentWindow.Start))]
        internal static class CustomAcsParentWindow_Start_Patches
        {
            private static void Postfix(CustomAcsParentWindow __instance)
            {
                MoreAccessories.MakerMode.AccessoriesWindow.ParentWin = __instance;
            }
        }
    }
}
