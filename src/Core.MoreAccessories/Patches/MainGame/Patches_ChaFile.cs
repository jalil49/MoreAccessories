using HarmonyLib;
#if EC
using ADVPart.Manipulate.Chara;
using HEdit;
using HPlay;
#endif

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
                    MoreAccessories.ArraySync(ChaCustom.CustomBase.instance.chaCtrl);
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
    }

}
