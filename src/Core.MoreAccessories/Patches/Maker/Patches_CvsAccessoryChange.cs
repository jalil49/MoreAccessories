﻿using ChaCustom;
using HarmonyLib;

namespace MoreAccessoriesKOI.Patches.Maker
{
    [HarmonyPatch(typeof(CvsAccessoryChange), nameof(CvsAccessoryChange.Start))]
    internal static class CvsAccessoryChange_Start_Patches
    {
        private static void Postfix(CvsAccessoryChange __instance)
        {
            MoreAccessories.MakerMode.TransferWindow = new Transfer_Window(__instance);
        }
    }
}
