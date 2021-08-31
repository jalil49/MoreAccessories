#if EC
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
#endif
#if KK || KKS
#endif
using ChaCustom;
using HarmonyLib;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public static class CustomAcsParentWindowdow_Patches
        {
            [HarmonyPatch(typeof(CustomAcsParentWindow), nameof(CustomAcsParentWindow.Start))]
            internal static class CustomAcsParentWindow_Start_Patches
            {
                private static void Postfix(CustomAcsParentWindow __instance)
                {
                    _self.ParentWin = __instance;
                }
            }

            //[HarmonyPatch(typeof(CustomAcsParentWindow), nameof(CustomAcsParentWindow.Initialize))]
            //internal static class CustomAcsParentWindow_Initialize_Patches
            //{
            //    private static bool Prefix(CustomAcsParentWindow __instance)
            //    {
            //        __instance._slotNo.TakeUntilDestroy(__instance).Subscribe(delegate
            //        {
            //            __instance.UpdateWindow();
            //        });
            //        if (__instance.btnClose)
            //        {
            //            (__instance.btnClose).OnClickAsObservable().Subscribe(delegate
            //            {
            //                if (__instance.tglReference)
            //                {
            //                    (__instance.tglReference).isOn = false;
            //                }
            //            });
            //        }
            //        __instance.tglParent.Select((p, idx) => new
            //        {
            //            toggle = p,
            //            index = (byte)idx
            //        }).ToList().ForEach(p =>
            //        {
            //            p.toggle.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
            //            {
            //                if (!__instance.updateWin && isOn)
            //                {
            //                    _self.GetCvsAccessory((int)__instance.slotNo).UpdateSelectAccessoryParent(p.index);
            //                }
            //            });
            //        });

            //        if (_self._accessoriesByChar.TryGetValue(CustomBase.Instance.chaCtrl.chaFile, out _self._charaMakerData) == false)
            //        {
            //            _self._charaMakerData = new CharAdditionalData();
            //            _self._accessoriesByChar.Add(CustomBase.Instance.chaCtrl.chaFile, _self._charaMakerData);
            //        }
            //        if (_self._charaMakerData.nowAccessories == null)
            //        {
            //            _self._charaMakerData.nowAccessories = new List<ChaFileAccessory.PartsInfo>();
            //            _self._charaMakerData.rawAccessoriesInfos.Add(CustomBase.Instance.chaCtrl.fileStatus.GetCoordinateType(), _self._charaMakerData.nowAccessories);
            //        }
            //        __instance.enabled = true;
            //        return false;
            //    }
            //}

            //[HarmonyPatch(typeof(CustomAcsParentWindow), nameof(CustomAcsParentWindow.ChangeSlot))]
            //internal static class CustomAcsParentWindow_ChangeSlot_Patches
            //{
            //    private static bool Prefix(CustomAcsParentWindow __instance, int _no, bool open)
            //    {
            //        var tglReference = __instance.tglReference;
            //        __instance.slotNo = (CustomAcsParentWindow.AcsSlotNo)_no;
            //        var isOn = tglReference.isOn;
            //        tglReference.isOn = false;
            //        tglReference = _self.GetCvsAccessory(_no).tglAcsParent;
            //        __instance.SetPrivate("tglReference", tglReference);
            //        if (open && isOn)
            //            tglReference.isOn = true;

            //        return false;
            //    }
            //}

            //[HarmonyPatch(typeof(CustomAcsParentWindow), nameof(CustomAcsParentWindow.UpdateCustomUI))]
            //internal static class CustomAcsParentWindow_UpdateCustomUI_Patches
            //{
            //    private static bool Prefix(CustomAcsParentWindow __instance, ref int __result)
            //    {
            //        __instance.updateWin = true;
            //        var index = (int)__instance.slotNo;
            //        __result = __instance.SelectParent(_self.GetPart(index).parentKey);
            //        __instance.updateWin = false;
            //        return false;
            //    }
            //}

            //[HarmonyPatch(typeof(CustomAcsParentWindow), nameof(CustomAcsParentWindow.UpdateWindow))]
            //internal static class CustomAcsParentWindow_UpdateWindow_Patches
            //{
            //    private static TextMeshProUGUI textTitle;

            //    private static bool Prefix(CustomAcsParentWindow __instance)
            //    {
            //        if (textTitle == null)
            //            textTitle = __instance.textTitle;
            //        __instance.updateWin = true;
            //        if (textTitle)
            //        {
            //            textTitle.text = $"スロット{(int)__instance.slotNo + 1:00}の親を選択";
            //        }
            //        var index = (int)__instance.slotNo;
            //        __instance.SelectParent(_self.GetPart(index).parentKey);
            //        __instance.updateWin = false;
            //        return false;
            //    }
            //}

        }
    }
}
