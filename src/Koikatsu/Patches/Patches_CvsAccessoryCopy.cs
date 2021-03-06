using ChaCustom;
using HarmonyLib;

namespace MoreAccessoriesKOI.Patches.Maker
{
    [HarmonyPatch(typeof(CvsAccessoryCopy), nameof(CvsAccessoryCopy.Start))]
    internal static class CvsAccessoryCopy_Start_Patches
    {
        private static void Postfix(CvsAccessoryCopy __instance)
        {
            if (MoreAccessories.MakerMode.CopyWindow == null)
                MoreAccessories.MakerMode.CopyWindow = new Copy_Window(__instance);
        }
    }
}
