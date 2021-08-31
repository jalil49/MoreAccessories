#if EC
using ADVPart.Manipulate.Chara;
using HEdit;
using HPlay;
using MoreAccessoriesKOI.Extensions;
using UnityEngine.UI;
#endif
using ChaCustom;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        #region Patches
#if KK
        [HarmonyPatch]
        private static class VRHScene_Start_Patches
        {
            internal static bool Prepare()
            {
                return Type.GetType("VRHScene,Assembly-CSharp.dll") != null;
            }

            internal static MethodInfo TargetMethod()
            {
                return Type.GetType("VRHScene,Assembly-CSharp.dll").GetMethod("Start", AccessTools.all);
            }

            internal static void Postfix(List<ChaControl> ___lstFemale, HSprite[] ___sprites)
            {
                _self.SpawnHUI(___lstFemale, ___sprites[0]);
            }
        }
#elif EC
        [HarmonyPatch(typeof(HPlayHPartAccessoryCategoryUI), nameof(HPlayHPartAccessoryCategoryUI.Start))]
        internal static class HPlayHPartAccessoryCategoryUI_Start_Postfix
        {
            private static void Postfix(HPlayHPartAccessoryCategoryUI __instance)
            {
                _self.SpawnPlayUI(__instance);
            }
        }

        [HarmonyPatch(typeof(HPlayHPartAccessoryCategoryUI), nameof(HPlayHPartAccessoryCategoryUI.Init))]
        internal static class HPlayHPartAccessoryCategoryUI_Init_Postfix
        {
            private static void Postfix()
            {
                _self.ExecuteDelayed(_self.UpdatePlayUI, 2);
            }
        }

        [HarmonyPatch(typeof(HPlayHPartClothMenuUI), nameof(HPlayHPartClothMenuUI.Init))]
        internal static class HPlayHPartClothMenuUI_Init_Postfix
        {
            private static void Postfix(HPlayHPartClothMenuUI __instance, Button[] ___btnClothMenus)
            {
                ___btnClothMenus[1].gameObject.SetActive(___btnClothMenus[1].gameObject.activeSelf);
            }
        }

        [HarmonyPatch(typeof(PartInfoClothSetUI), nameof(PartInfoClothSetUI.Start))]
        internal static class PartInfoClothSetUI_Start_Patches
        {
            internal static WeakKeyDictionary<ChaControl, CharAdditionalData> _originalAdditionalData = new WeakKeyDictionary<ChaControl, CharAdditionalData>();
            private static void Postfix(PartInfoClothSetUI.CoordinateUIInfo[] ___coordinateUIs)
            {
                _originalAdditionalData.Purge();
                for (var i = 0; i < ___coordinateUIs.Length; i++)
                {
                    var ui = ___coordinateUIs[i];
                    var originalOnClick = ui.btnEntry.onClick;
                    ui.btnEntry.onClick = new Button.ButtonClickedEvent();
                    var i1 = i;
                    ui.btnEntry.onClick.AddListener(() =>
                    {
                        var chaControl = Singleton<HEditData>.Instance.charas[i1];
                        //if (_originalAdditionalData.ContainsKey(chaControl) == false && _self._accessoriesByChar.TryGetValue(chaControl.chaFile, out CharAdditionalData originalData))
                        //{
                        //    var newData = new CharAdditionalData();
                        //    newData.LoadFrom(originalData);
                        //    _originalAdditionalData.Add(chaControl, newData);
                        //}
                        originalOnClick.Invoke();
                    });
                }
            }
        }


        [HarmonyPatch(typeof(AccessoryUICtrl), nameof(AccessoryUICtrl.Init))]
        internal static class AccessoryUICtrl_Init_Patches
        {
            private static void Postfix(AccessoryUICtrl __instance)
            {
                _self.SpawnADVUI(__instance);
            }
        }

        [HarmonyPatch(typeof(AccessoryUICtrl), nameof(UpdateUI))]
        internal static class AccessoryUICtrl_UpdateUI_Patches
        {
            private static void Postfix()
            {
                _self.UpdateADVUI();
            }
        }

#endif
        #endregion
    }
}
