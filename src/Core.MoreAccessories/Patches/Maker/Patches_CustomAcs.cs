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


        //[HarmonyPatch(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.LateUpdate))]
        //internal static class CustomAcsChangeSlot_LateUpdate_Patches
        //{
        //    private static bool Prefix(CustomAcsChangeSlot __instance)
        //    {
        //        var array = new bool[2];
        //        if (__instance.cgAccessoryTop.alpha == 1f)
        //        {
        //            var selectIndex = MoreAccessories._self.GetSelectedMakerIndex();
        //            if (selectIndex != -1)
        //            {
        //                var accessory = MoreAccessories._self.GetCvsAccessory(selectIndex);
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
