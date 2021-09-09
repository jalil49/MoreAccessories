#if false
using ADVPart.Manipulate.Chara;
using HarmonyLib;

namespace MoreAccessoriesKOI.Patches
{
    internal class ADVUI_Patches//no idea if there is anything for me to do here
    {
        [HarmonyPatch(typeof(AccessoryUICtrl), "Init")]
        internal static class AccessoryUICtrl_Init_Patches
        {
            private static void Postfix(AccessoryUICtrl __instance)
            {
                MoreAccessories.ADVMode = new ADVMode(__instance);
            }
        }

        [HarmonyPatch(typeof(AccessoryUICtrl), "UpdateUI")]
        internal static class AccessoryUICtrl_UpdateUI_Patches
        {
            private static void Postfix()
            {
                MoreAccessories.ADVMode.UpdateADVUI();
            }
        }
    }
}
#endif