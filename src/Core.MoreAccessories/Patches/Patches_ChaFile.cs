using HarmonyLib;
#if EC
using ADVPart.Manipulate.Chara;
using HEdit;
using HPlay;
#endif

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public static class ChaFile_Patches
        {
            [HarmonyPatch(typeof(ChaFileControl), nameof(ChaFileControl.LoadFileLimited), new[] { typeof(string), typeof(byte), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool) })]
            private static class ChaFileControl_LoadFileLimited_Patches
            {
                private static void Postfix()
                {
                    if (_self._inCharaMaker)
                        _self.ArraySync(ChaCustom.CustomBase.instance.chaCtrl);
                }
            }
        }
    }
}
