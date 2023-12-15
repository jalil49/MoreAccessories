using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace MoreAccessoriesKOI.Patches
{
    public static class ChaFile_Patches
    {
        /// <summary>
        /// make sure arrays are equal size, copy uses Array.Copy method
        /// </summary>
        [HarmonyPatch(typeof(ChaFileStatus), nameof(ChaFileStatus.Copy))]
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Harmony Patches - Used Externally")]
        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Harmony Patches - Used Externally")]
        private static class ChaFileControlCopy_Patch
        {
            private static void Prefix(ChaFileStatus __instance, ChaFileStatus src)
            {
                Common_Patches.Seal(false); //will probably get sealed soon
                __instance.showAccessory = new bool[src.showAccessory.Length];
            }
        }

        /// <summary>
        /// sync copied status to current coordinate array length
        /// </summary>
        [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.CopyAll))]
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Harmony Patches - Used Externally")]
        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Harmony Patches - Used Externally")]
        private static class ChaFileCopyAllPatch
        {
            private static void Prefix() => Common_Patches.Seal(false);
            private static void Postfix(ChaFile __instance) => MoreAccessories.ArraySync(__instance);
        }
#if KK || KKS
        /// <summary>
        /// Array sync when replacing Characters
        /// </summary>
        [HarmonyPatch(typeof(ActionGame.Chara.Base), nameof(ActionGame.Chara.Base.Replace))]
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Harmony Patches - Used Externally")]
        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Harmony Patches - Used Externally")]
        private static class Replace_Patch
        {
            private static void Postfix(ActionGame.Chara.Base __instance)
            {
                if (__instance.chaCtrl != null)
                {
                    MoreAccessories.ArraySync(__instance.chaCtrl);
                }
            }
        }
#endif
    }
}