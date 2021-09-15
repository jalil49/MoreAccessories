using HarmonyLib;

namespace MoreAccessoriesKOI.Patches
{
    public static class ChaFile_Patches
    {
#if KKS
        [HarmonyPatch(typeof(ChaFileControl), nameof(ChaFileControl.LoadFileLimited), new[] { typeof(string), typeof(byte), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool) })]
#elif KK || EC
        [HarmonyPatch(typeof(ChaFileControl), nameof(ChaFileControl.LoadFileLimited), new[] { typeof(string), typeof(byte), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool) })]
#endif
        private static class ChaFileControl_LoadFileLimited_Patches
        {
            private static void Postfix()
            {
                if (MoreAccessories.CharaMaker)
                {
                    ChaCustom.CustomBase.Instance.selectSlot = -1;//Fixes issue with a bow appearing in first slot
                    //MoreAccessories.ArraySync(ChaCustom.CustomBase.instance.chaCtrl);
                }
            }
        }

        [HarmonyPatch(typeof(ChaFileStatus), nameof(ChaFileStatus.Copy))]
        private static class ChaFileControl_Copy_Patches
        {
            private static void Prefix(ChaFileStatus __instance, ChaFileStatus src)
            {
                __instance.showAccessory = new bool[src.showAccessory.Length];
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
