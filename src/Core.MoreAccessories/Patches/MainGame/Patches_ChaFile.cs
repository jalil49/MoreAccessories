using HarmonyLib;

namespace MoreAccessoriesKOI.Patches
{
    public static class ChaFile_Patches
    {
        [HarmonyPatch(typeof(ChaFileStatus), nameof(ChaFileStatus.Copy))]
        private static class ChaFileControl_Copy_Patches
        {
            private static void Prefix(ChaFileStatus __instance, ChaFileStatus src)
            {
                Common_Patches.Seal(false);//will probably get sealed soon
                __instance.showAccessory = new bool[src.showAccessory.Length];
            }
        }

        [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.CopyAll))]
        private static class ChaFileCopyAllPatch
        {
            private static void Prefix() => Common_Patches.Seal(false);
            private static void Postfix(ChaFile __instance)
            {
                MoreAccessories.ArraySync(__instance);
            }
        }
#if KK || KKS
        [HarmonyPatch(typeof(ActionGame.Chara.Base), nameof(ActionGame.Chara.Base.Replace))]
        private static class Replace_Patches
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
