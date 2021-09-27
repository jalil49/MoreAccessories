using ChaCustom;
using HarmonyLib;

namespace MoreAccessoriesKOI.Patches.Maker
{
    public static class CustomAcs_Patches
    {
        #region CustomAcsChangeSlot

        [HarmonyPatch(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.Start))]
        internal static class CustomAcsChangeSlot_KKS_Start_Patches
        {
            private static void Postfix(CustomAcsChangeSlot __instance)
            {
                MoreAccessories.MakerMode.AccessoriesWindow = new Accessories(__instance);
            }
        }
        #endregion
    }
}
