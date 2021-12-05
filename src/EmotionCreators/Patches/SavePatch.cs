using HarmonyLib;
#pragma warning disable IDE0051 // Remove unused private members

namespace MoreAccessoriesKOI.Patches
{
    [HarmonyPatch(typeof(HEdit.ADVPart.CharState), MethodType.Constructor, new[] { typeof(HEdit.ADVPart.CharState) })]
    internal class CharStateConstructorPatch
    {
        private static void Prefix(HEdit.ADVPart.CharState __instance, HEdit.ADVPart.CharState _state)
        {
            __instance.accessory = new int[_state.accessory.Length];
        }
    }
}