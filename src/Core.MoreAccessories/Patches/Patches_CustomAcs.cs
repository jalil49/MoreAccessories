using ChaCustom;
using HarmonyLib;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public static class CustomAcs_Patches
        {
            #region CustomAcsChangeSlot
#if KKSf
            [HarmonyPatch(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.Initialize))]
            internal static class CustomAcsChangeSlot_Start_Patches
            {
                private static void Postfix(CustomAcsChangeSlot __instance)
                {
                    _self._customAcsChangeSlot = __instance;
                    _self._customAcsParentWin = __instance.customAcsParentWin;
                    _self._customAcsMoveWin = __instance.customAcsMoveWin;
                    _self._customAcsSelectKind = __instance.customAcsSelectKind;
                    _self.SpawnMakerUI();
                }
            }
#endif


#if KK || EC || KKS
            [HarmonyPatch(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.Start))]
            internal static class CustomAcsChangeSlot_KKS_Start_Patches
            {
                private static void Postfix(CustomAcsChangeSlot __instance)
                {
                    _self._customAcsChangeSlot = __instance;
                    _self.SpawnMakerUI();
                }
            }
#endif


            //[HarmonyPatch(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.LateUpdate))]
            //internal static class CustomAcsChangeSlot_LateUpdate_Patches
            //{
            //    private static bool Prefix(CustomAcsChangeSlot __instance)
            //    {
            //        var array = new bool[2];
            //        if (__instance.cgAccessoryTop.alpha == 1f)
            //        {
            //            var selectIndex = _self.GetSelectedMakerIndex();
            //            if (selectIndex != -1)
            //            {
            //                var accessory = _self.GetCvsAccessory(selectIndex);
            //                if (accessory.isController01Active && Singleton<CustomBase>.Instance.customSettingSave.drawController[0])
            //                {
            //                    array[0] = true;
            //                }
            //                if (accessory.isController02Active && Singleton<CustomBase>.Instance.customSettingSave.drawController[1])
            //                {
            //                    array[1] = true;
            //                }
            //            }
            //        }
            //        for (var i = 0; i < 2; i++)
            //        {
            //            Singleton<CustomBase>.Instance.customCtrl.cmpGuid[i].gameObject.SetActiveIfDifferent(array[i]);
            //        }
            //        return false;
            //    }
            //}
            #endregion
        }
    }
}
