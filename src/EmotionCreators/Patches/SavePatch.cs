using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

#pragma warning disable IDE0051 // Remove unused private members

namespace MoreAccessoriesKOI.Patches
{
    [HarmonyPatch(typeof(HEdit.ADVPart.CharState), MethodType.Constructor, typeof(HEdit.ADVPart.CharState))]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    internal class CharStateConstructor_Patch
    {
        // ReSharper disable once InconsistentNaming
        private static void Prefix(HEdit.ADVPart.CharState __instance, HEdit.ADVPart.CharState _state)
        {
            __instance.accessory = new int[_state.accessory.Length];
        }
    }
}