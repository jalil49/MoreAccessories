using ChaCustom;
using HarmonyLib;

namespace MoreAccessoriesKOI.Patches.Maker
{
    //No external reference CvsAccessoryChange (AKA Transferwindow)
    [HarmonyPatch(typeof(CvsAccessoryChange), nameof(CvsAccessoryChange.Start))]
    internal static class CvsAccessoryChange_Start_Patches
    {
        private static void Postfix(CvsAccessoryChange __instance)
        {
            if (MoreAccessories.MakerMode.TransferWindow == null)
                MoreAccessories.MakerMode.TransferWindow = new Transfer_Window(__instance);
        }
    }
}
