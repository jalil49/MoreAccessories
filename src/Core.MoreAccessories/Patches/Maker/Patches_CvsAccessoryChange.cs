using System.Diagnostics.CodeAnalysis;
using ChaCustom;
using HarmonyLib;

namespace MoreAccessoriesKOI.Patches.Maker
{
    //No external reference CvsAccessoryChange (AKA Transfer Window)
    [HarmonyPatch(typeof(CvsAccessoryChange), nameof(CvsAccessoryChange.Start))]
    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Harmony Patches - Used Externally")]
    [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Harmony Patches - Used Externally")]
    internal static class TransferWindowStart_Patches
    {
        private static void Postfix(CvsAccessoryChange __instance)
        {
            if (MoreAccessories.MakerMode.TransferWindow == null)
                MoreAccessories.MakerMode.TransferWindow = new Transfer_Window(__instance);
        }
    }
}
