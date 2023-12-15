using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ADVPart.Manipulate.Chara;
using HarmonyLib;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MoreAccessoriesKOI.Patches
{
    internal static class AdvUi_Patches
    {
        private static float? _defaultHeight;//base length of 20 slots to use as reference for rebuilding height
        private static float? _defaultText;//default font size to return to after autoscaling text to fit

        [HarmonyPatch(typeof(AccessoryUICtrl), nameof(AccessoryUICtrl.UpdateUI))]
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private static class UpdateUI_Patches
        {
            private static bool Prefix(AccessoryUICtrl __instance)
            {
                __instance.isUpdateUI = true;
                UpdateAdvUi(__instance);
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
                    if (!_defaultText.HasValue)
                    {
                        _defaultText = __instance.toggles[0].toggles[0].transform.parent.parent.GetComponentInChildren<TextMeshProUGUI>().m_currentFontSize;
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
                        text.fontSize = _defaultText.Value;
                    }
                }
                #endregion
                
                __instance.isUpdateUI = false;
                return false;
            }
        }

        private static void UpdateAdvUi(AccessoryUICtrl advUI)
        {
            var advToggleTemplate = advUI.toggles[19].toggles[0].transform.parent.parent;
            var baseLength = advUI.toggles.Length;
            var count = advUI.chaControl.nowCoordinate.accessory.parts.Length - baseLength;
            if (0 < count)
            {
                var toggleUiAppend = new AccessoryUICtrl.ToggleUI[count];
                for (var i = 0; i < count; i++)
                {
                    var toggleGo = Object.Instantiate(advToggleTemplate, advToggleTemplate.parent);
                    var toggleNum = baseLength + i;

                    toggleGo.name = $"Slot {toggleNum + 1}";
                    var toggleUI = toggleUiAppend[i] = new AccessoryUICtrl.ToggleUI();
                    var toggles = toggleGo.GetComponentsInChildren<Toggle>();
                    toggleGo.GetComponentInChildren<TextMeshProUGUI>().text = $"スロット {toggleNum + 1}";
                    toggleUI.toggles = new Toggle[toggles.Length];
                    for (var j = 0; j < toggles.Length; j++)
                    {
                        var state = j - 1;
                        toggles[j].onValueChanged = new Toggle.ToggleEvent();
                        toggles[j].OnValueChangedAsObservable().Subscribe(delegate
                        {
                            if (advUI.isUpdateUI) { return; }

                            advUI.charState.accessory[toggleNum] = state;

                            if (state >= 0)
                            {
                                advUI.chaControl.SetAccessoryState(toggleNum, state == 0);
                            }
                        });

                        toggleUI.toggles[j] = toggles[j];
                    }
                    toggleGo.gameObject.SetActive(true);
                }
                advUI.toggles = advUI.toggles.Concat(toggleUiAppend).ToArray();
            }
            count = advUI.chaControl.nowCoordinate.accessory.parts.Length - advUI.charState.accessory.Length;
            if (0 < count)
            {
                var accessoryArray = new int[count];
                for (var i = 0; i < count; i++)
                {
                    accessoryArray[i] = -1;
                }
                advUI.charState.accessory = advUI.charState.accessory.Concat(accessoryArray).ToArray();
            }
            else if (count != 0)
            {
                advUI.charState.accessory = advUI.charState.accessory.Take(advUI.chaControl.nowCoordinate.accessory.parts.Length).ToArray();
            }
        }

        private static void CalculateHeight(AccessoryUICtrl advUI)
        {
            var advToggleTemplate = advUI.toggles[19].toggles[0].transform.parent.parent;
            var parent = (RectTransform)advToggleTemplate.parent.parent;
            if (!_defaultHeight.HasValue)
            {
                _defaultHeight = parent.offsetMax.y;
            }
            parent.offsetMin = new Vector2(0, _defaultHeight.Value - 66 - 34 * (advUI.chaControl.nowCoordinate.accessory.parts.Length + 1));
            LayoutRebuilder.MarkLayoutForRebuild((RectTransform)advToggleTemplate.parent.parent);
        }
    }
}
