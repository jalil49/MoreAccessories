using HarmonyLib;

namespace MoreAccessoriesKOI.Patches
{
    [HarmonyPatch(typeof(HEdit.ADVPart.CharState), MethodType.Constructor, new[] { typeof(HEdit.ADVPart.CharState) })]
    internal class CharStateConstructorPatch
    {
        internal static void Prefix(HEdit.ADVPart.CharState __instance, HEdit.ADVPart.CharState _state)
        {
            __instance.accessory = new int[_state.accessory.Length];
        }
    }
}