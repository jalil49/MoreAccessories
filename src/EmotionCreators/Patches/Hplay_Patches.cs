using ADVPart.Manipulate.Chara;
using HarmonyLib;
using HEdit;
using HPlay;
using MoreAccessoriesKOI.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAccessoriesKOI.Patches
{
    internal class Hplay_Patches
    {
        [HarmonyPatch(typeof(HPlayHPartAccessoryCategoryUI), "Start")]
        internal static class HPlayHPartAccessoryCategoryUI_Start_Postfix
        {
            internal static void Postfix(HPlayHPartAccessoryCategoryUI __instance)
            {
                MoreAccessories.PlayMode = new PlayMode(__instance);
            }
        }

        [HarmonyPatch(typeof(HPlayHPartAccessoryCategoryUI), "Init")]
        internal static class HPlayHPartAccessoryCategoryUI_Init_Postfix
        {
            internal static void Postfix()
            {
                MoreAccessories._self.ExecuteDelayed(MoreAccessories.PlayMode.UpdatePlayUI, 2);
            }
        }

        [HarmonyPatch(typeof(HPlayHPartClothMenuUI), "Init")]
        internal static class HPlayHPartClothMenuUI_Init_Postfix
        {
            internal static void Postfix(HPlayHPartClothMenuUI __instance, Button[] ___btnClothMenus)
            {
                ___btnClothMenus[1].gameObject.SetActive(___btnClothMenus[1].gameObject.activeSelf /*|| MoreAccessories._self._accessoriesByChar[__instance.selectChara.chaFile].objAccessory.Any(o => o != null)*/);
            }
        }

        //[HarmonyPatch(typeof(PartInfoClothSetUI), "Start")]
        //internal static class PartInfoClothSetUI_Start_Patches
        //{
        //    internal static WeakKeyDictionary<ChaControl, MoreAccessories.CharAdditionalData> _originalAdditionalData = new WeakKeyDictionary<ChaControl, MoreAccessories.CharAdditionalData>();
        //    private static void Postfix(PartInfoClothSetUI.CoordinateUIInfo[] ___coordinateUIs)
        //    {
        //        _originalAdditionalData.Purge();
        //        for (var i = 0; i < ___coordinateUIs.Length; i++)
        //        {
        //            var ui = ___coordinateUIs[i];
        //            var originalOnClick = ui.btnEntry.onClick;
        //            ui.btnEntry.onClick = new Button.ButtonClickedEvent();
        //            var i1 = i;
        //            ui.btnEntry.onClick.AddListener(() =>
        //            {
        //                var chaControl = Singleton<HEditData>.Instance.charas[i1];
        //                if (_originalAdditionalData.ContainsKey(chaControl) == false && MoreAccessories._self._accessoriesByChar.TryGetValue(chaControl.chaFile, out MoreAccessories.CharAdditionalData originalData))
        //                {
        //                    var newData = new MoreAccessories.CharAdditionalData();
        //                    newData.LoadFrom(originalData);
        //                    _originalAdditionalData.Add(chaControl, newData);
        //                }
        //                originalOnClick.Invoke();
        //            });
        //        }
        //    }
        //}

        //[HarmonyPatch(typeof(PartInfoClothSetUI), "BackToCoordinate")]
        //internal static class PartInfoClothSetUI_BackToCoordinate_Patches
        //{
        //    private static void Prefix(int _charaID)
        //    {
        //        var chara = Singleton<HEditData>.Instance.charas[_charaID];
        //        MoreAccessories.CharAdditionalData originalData;
        //        if (PartInfoClothSetUI_Start_Patches._originalAdditionalData.TryGetValue(chara, out originalData) &&
        //            MoreAccessories._self._accessoriesByChar.TryGetValue(chara.chaFile, out MoreAccessories.CharAdditionalData data))
        //            data.LoadFrom(originalData);
        //    }
        //}
    }
}
