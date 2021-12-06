using ADVPart.Manipulate.Chara;
using HarmonyLib;
using Illusion.Extensions;
using MoreAccessoriesKOI.Extensions;
using System;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAccessoriesKOI.Patches
{
    internal class ADVUI_Patches
    {
        private static float? defaultheight;//base length of 20 slots to use as reference for rebuilding height
        private static float? defaulttext;//default font size to return to after autoscaling text to fit

        [HarmonyPatch(typeof(AccessoryUICtrl), nameof(AccessoryUICtrl.UpdateUI))]
        private static class AccessoryUICtrl_UpdateUI_Patches
        {
            private static bool Prefix(AccessoryUICtrl __instance)
            {
                __instance.isUpdateUI = true;
                UpdateADVUI(__instance);
                var range = Math.Min(__instance.chaControl.nowCoordinate.accessory.parts.Length, __instance.toggles.Length);

                #region Adjust Visible Slots
                {
                    var i = 0;
                    for (; i < range; i++)
                    {
                        __instance.toggles[i].toggles[0].transform.parent.parent.gameObject.SetActive(true);
                        __instance.toggles[i].select = __instance.charState.accessory[i] + 1;
                        if (i < __instance.chaControl.objAccessory.Length)
                        {
                            var interact = __instance.chaControl.objAccessory[i] != null;
                            __instance.toggles[i].interactable = interact;
                            foreach (var toggle in __instance.toggles[i].toggles)
                            {
                                toggle.interactable = interact;
                            }
                        }
                    }
                    for (; i < __instance.toggles.Length; i++)
                    {
                        __instance.toggles[i].toggles[0].transform.parent.parent.gameObject.SetActive(false);
                    }
                }
                #endregion

                CalculateHeight(__instance);

                #region NameSlots
                {
                    if (!defaulttext.HasValue)
                    {
                        defaulttext = __instance.toggles[0].toggles[0].transform.parent.parent.GetComponentInChildren<TextMeshProUGUI>().m_currentFontSize;
                    }

                    var i = 0;
                    for (; i < range && i < __instance.chaControl.infoAccessory.Length; i++)
                    {
                        var text = __instance.toggles[i].toggles[0].transform.parent.parent.GetComponentInChildren<TextMeshProUGUI>();
                        var info = __instance.chaControl.infoAccessory[i];
                        if (info != null && MoreAccessories.SceneCreateAccessoryNames.Value)
                        {
                            text.text = $"{i + 1} {info.Name}";
                            text.enableAutoSizing = true;
                            continue;
                        }
                        text.text = $"スロット {i + 1}";
                        text.enableAutoSizing = false;
                        text.fontSize = defaulttext.Value;
                    }
                }
                #endregion

                __instance.isUpdateUI = false;
                return false;
            }
        }

        private static void UpdateADVUI(AccessoryUICtrl _advUI)
        {
            var _advToggleTemplate = _advUI.toggles[19].toggles[0].transform.parent.parent;
            var baselength = _advUI.toggles.Length;
            var count = _advUI.chaControl.nowCoordinate.accessory.parts.Length - baselength;
            if (0 < count)
            {
                var toggleuiappend = new AccessoryUICtrl.ToggleUI[count];
                for (var i = 0; i < count; i++)
                {
                    var toggleGO = UnityEngine.Object.Instantiate(_advToggleTemplate, _advToggleTemplate.parent);
                    var togglenum = baselength + i;

                    toggleGO.name = $"Slot {togglenum + 1}";
                    var toggleUI = toggleuiappend[i] = new AccessoryUICtrl.ToggleUI();
                    var toggles = toggleGO.GetComponentsInChildren<Toggle>();
                    toggleGO.GetComponentInChildren<TextMeshProUGUI>().text = $"スロット {togglenum + 1}";
                    toggleUI.toggles = new Toggle[toggles.Length];
                    for (var j = 0; j < toggles.Length; j++)
                    {
                        var state = j - 1;
                        toggles[j].onValueChanged = new Toggle.ToggleEvent();
                        toggles[j].OnValueChangedAsObservable().Subscribe(delegate (bool _)
                        {
                            if (_advUI.isUpdateUI) { return; }

                            _advUI.charState.accessory[togglenum] = state;

                            if (state >= 0)
                            {
                                _advUI.chaControl.SetAccessoryState(togglenum, state == 0);
                            }
                        });

                        toggleUI.toggles[j] = toggles[j];
                    }
                    toggleGO.gameObject.SetActive(true);
                }
                _advUI.toggles = _advUI.toggles.Concat(toggleuiappend).ToArray();
            }
            count = _advUI.chaControl.nowCoordinate.accessory.parts.Length - _advUI.charState.accessory.Length;
            if (0 < count)
            {
                var accessoryarray = new int[count];
                for (var i = 0; i < count; i++)
                {
                    accessoryarray[i] = -1;
                }
                _advUI.charState.accessory = _advUI.charState.accessory.Concat(accessoryarray).ToArray();
            }
            else if (count != 0)
            {
                _advUI.charState.accessory = _advUI.charState.accessory.Take(_advUI.chaControl.nowCoordinate.accessory.parts.Length).ToArray();
            }
        }

        private static void CalculateHeight(AccessoryUICtrl _advUI)
        {
            var _advToggleTemplate = _advUI.toggles[19].toggles[0].transform.parent.parent;
            var parent = (RectTransform)_advToggleTemplate.parent.parent;
            if (!defaultheight.HasValue)
            {
                defaultheight = parent.offsetMax.y;
            }
            parent.offsetMin = new Vector2(0, defaultheight.Value - 66 - 34 * (_advUI.chaControl.nowCoordinate.accessory.parts.Length + 1));
            LayoutRebuilder.MarkLayoutForRebuild((RectTransform)_advToggleTemplate.parent.parent);
            //MoreAccessories._self.ExecuteDelayed(() =>
            //{
            //    LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_advToggleTemplate.parent.parent);
            //    LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_advToggleTemplate.parent.parent.parent);
            //});
        }
    }
}
