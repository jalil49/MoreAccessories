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
        [HarmonyPatch(typeof(ChaFileControl), nameof(ChaFileControl.LoadFileLimited), new[] { typeof(string), typeof(byte), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool) })]
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
    }

}
