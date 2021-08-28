using System.Linq;
using ChaCustom;
using HarmonyLib;
#if EMOTIONCREATORS
using ADVPart.Manipulate.Chara;
using HEdit;
using HPlay;
#endif
using Illusion.Extensions;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using MoreAccessoriesKOI.Extensions;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public static class CustomAcs_Patches
        {
            #region CustomAcsChangeSlot
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

            [HarmonyPatch(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.UpdateSlotNames))]
            internal static class CustomAcsChangeSlot_UpdateSlotNames_Patches
            {
                private static void Postfix()
                {
                    if (_self._charaMakerData.nowAccessories == null || _self._charaMakerData.nowAccessories.Count == 0)
                        return;
                    for (var i = 0; i < _self._additionalCharaMakerSlots.Count; i++)
                    {
                        var slot = _self._additionalCharaMakerSlots[i];
                        if (slot.toggle.isOn == false || _self._charaMakerData.nowAccessories.Count >= i)
                            continue;
                        if (_self._charaMakerData.nowAccessories[i].type == 120)
                        {
                            slot.text.text = $"スロット{i + 21:00}";
                        }
                        else if (_self._charaMakerData.infoAccessory[i] != null)
                        {
                            slot.text.text = _self._charaMakerData.infoAccessory[i].Name;
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.ChangeColorWindow), new[] { typeof(int) })]
            internal static class CustomAcsChangeSlot_ChangeColorWindow_Patches
            {
                private static CvsColor cvsColor;

                private static bool Prefix(CustomAcsChangeSlot __instance, int no)
                {
                    if (cvsColor == null)
                        cvsColor = __instance.cvsColor;
                    if (null == cvsColor)
                        return false;
                    if (!cvsColor.isOpen)
                        return false;
                    if (no < 20)
                    {
                        var accessory = _self.GetCvsAccessory(no);
                        if (accessory)
                            accessory.SetDefaultColorWindow(no);
                    }
                    else
                        cvsColor.Close();
                    return false;
                }
            }

            [HarmonyPatch(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.LateUpdate))]
            internal static class CustomAcsChangeSlot_LateUpdate_Patches
            {
                private static bool Prefix(CustomAcsChangeSlot __instance)
                {
                    var array = new bool[2];
                    if (__instance.cgAccessoryTop.alpha == 1f)
                    {
                        var selectIndex = _self.GetSelectedMakerIndex();
                        if (selectIndex != -1)
                        {
                            var accessory = _self.GetCvsAccessory(selectIndex);
                            if (accessory.isController01Active && Singleton<CustomBase>.Instance.customSettingSave.drawController[0])
                            {
                                array[0] = true;
                            }
                            if (accessory.isController02Active && Singleton<CustomBase>.Instance.customSettingSave.drawController[1])
                            {
                                array[1] = true;
                            }
                        }
                    }
                    for (var i = 0; i < 2; i++)
                    {
                        Singleton<CustomBase>.Instance.customCtrl.cmpGuid[i].gameObject.SetActiveIfDifferent(array[i]);
                    }
                    return false;
                }
            }

            #endregion

            #region CustomAcsMoveWindow
            [HarmonyPatch(typeof(CustomAcsMoveWindow), nameof(CustomAcsMoveWindow.Initialize))]
            internal static class CustomAcsMoveWindow_Start_Patches
            {
                private static bool Prefix(CustomAcsMoveWindow __instance)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        Singleton<CustomBase>.Instance.lstTmpInputField.Add((__instance.inpPos)[i]);
                        Singleton<CustomBase>.Instance.lstTmpInputField.Add((__instance.inpRot)[i]);
                        Singleton<CustomBase>.Instance.lstTmpInputField.Add((__instance.inpScl)[i]);
                    }
                    (__instance._slotNo).TakeUntilDestroy(__instance).Subscribe(delegate
                    {
                        __instance.UpdateWindow();
                    });
                    var btnClose = __instance.btnClose;
                    if (btnClose)
                    {
                        btnClose.OnClickAsObservable().Subscribe(delegate
                        {
                            var tglReference = __instance.tglReference;
                            if (tglReference)
                            {
                                tglReference.isOn = false;
                            }
                        });
                    }
                    (__instance.tglPosRate).Select((p, idx) => new
                    {
                        toggle = p,
                        index = (byte)idx
                    }).ToList().ForEach(p =>
                    {
                        (from isOn in p.toggle.OnValueChangedAsObservable()
                         where isOn
                         select isOn).Subscribe(delegate
                         {
                             Singleton<CustomBase>.Instance.customSettingSave.acsCorrectPosRate[__instance.correctNo] = p.index;
                         });
                    });
                    (__instance.tglRotRate).Select((p, idx) => new
                    {
                        toggle = p,
                        index = (byte)idx
                    }).ToList().ForEach(p =>
                    {
                        (from isOn in p.toggle.OnValueChangedAsObservable()
                         where isOn
                         select isOn).Subscribe(delegate
                         {
                             Singleton<CustomBase>.Instance.customSettingSave.acsCorrectRotRate[__instance.correctNo] = p.index;
                         });
                    });
                    (__instance.tglSclRate).Select((p, idx) => new
                    {
                        toggle = p,
                        index = (byte)idx
                    }).ToList().ForEach(p =>
                    {
                        (from isOn in p.toggle.OnValueChangedAsObservable()
                         where isOn
                         select isOn).Subscribe(delegate
                         {
                             Singleton<CustomBase>.Instance.customSettingSave.acsCorrectSclRate[__instance.correctNo] = p.index;
                         });
                    });
                    var downTimeCnt = 0f;
                    var loopTimeCnt = 0f;
                    var change = false;
                    (__instance.btnPos).Select((p, idx) => new
                    {
                        btn = p,
                        index = idx
                    }).ToList().ForEach(p =>
                    {
                        p.btn.OnClickAsObservable().Subscribe(delegate
                        {
                            if (!change)
                            {
                                var num = p.index / 2;
                                var num2 = (p.index % 2 != 0) ? 1 : -1;
                                if (num == 0)
                                {
                                    num2 *= -1;
                                }
                                var val = num2 * __instance.movePosValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectPosRate[__instance.correctNo]];
                                _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsPosAdd(__instance.correctNo, num, true, val);
                                if (_self._hasDarkness == false)
                                    _self.GetCvsAccessory(__instance.nSlotNo).UpdateAcsMoveHistory();
                                (__instance.inpPos)[num].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 0][num].ToString();
                                _self.GetCvsAccessory(__instance.nSlotNo).SetControllerTransform(__instance.correctNo);
                            }
                        });
                        p.btn.UpdateAsObservable().SkipUntil(p.btn.OnPointerDownAsObservable().Do(delegate
                        {
                            downTimeCnt = 0f;
                            loopTimeCnt = 0f;
                            change = false;
                        })).TakeUntil(p.btn.OnPointerUpAsObservable().Do(delegate
                        {
                            if (_self._hasDarkness == false)
                                if (change)
                                {
                                    _self.GetCvsAccessory(__instance.nSlotNo).UpdateAcsMoveHistory();
                                }
                        })
                        ).RepeatUntilDestroy(__instance).Subscribe(delegate
                        {
                            var num = p.index / 2;
                            var num2 = (p.index % 2 != 0) ? 1 : -1;
                            if (num == 0)
                            {
                                num2 *= -1;
                            }
                            var num3 = num2 * __instance.movePosValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectPosRate[__instance.correctNo]];
                            var num4 = 0f;
                            downTimeCnt += Time.deltaTime;
                            if (downTimeCnt > 0.3f)
                            {
                                for (loopTimeCnt += Time.deltaTime; loopTimeCnt > 0.05f; loopTimeCnt -= 0.05f)
                                {
                                    num4 += num3;
                                }
                                if (num4 != 0f)
                                {
                                    _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsPosAdd(__instance.correctNo, num, true, num4);
                                    (__instance.inpPos)[num].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 0][num].ToString();
                                    change = true;
                                    _self.GetCvsAccessory(__instance.nSlotNo).SetControllerTransform(__instance.correctNo);
                                }
                            }
                        }).AddTo(__instance);
                    });
                    (__instance.inpPos).Select((p, idx) => new
                    {
                        inp = p,
                        index = idx
                    }).ToList().ForEach(p =>
                    {
                        p.inp.onEndEdit.AsObservable().Subscribe(delegate (string value)
                        {
                            var xyz = p.index % 3;
                            var val = CustomBase.ConvertValueFromTextLimit(-100f, 100f, 1, value);
                            p.inp.text = val.ToString();
                            _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsPosAdd(__instance.correctNo, xyz, false, val);
                            if (_self._hasDarkness == false)
                                _self.GetCvsAccessory(__instance.nSlotNo).UpdateAcsMoveHistory();
                            _self.GetCvsAccessory(__instance.nSlotNo).SetControllerTransform(__instance.correctNo);
                        });
                    });
                    (__instance.btnPosReset).Select((p, idx) => new
                    {
                        btn = p,
                        index = idx
                    }).ToList().ForEach(p =>
                    {
                        p.btn.OnClickAsObservable().Subscribe(delegate
                        {
                            (__instance.inpPos)[p.index].text = "0";
                            _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsPosAdd(__instance.correctNo, p.index, false, 0f);
                            if (_self._hasDarkness == false)
                                _self.GetCvsAccessory(__instance.nSlotNo).UpdateAcsMoveHistory();
                            _self.GetCvsAccessory(__instance.nSlotNo).SetControllerTransform(__instance.correctNo);
                        });
                    });
                    (__instance.btnRot).Select((p, idx) => new
                    {
                        btn = p,
                        index = idx
                    }).ToList().ForEach(p =>
                    {
                        p.btn.OnClickAsObservable().Subscribe(delegate
                        {
                            if (!change)
                            {
                                var num = p.index / 2;
                                var num2 = (p.index % 2 != 0) ? 1 : -1;
                                var val = num2 * __instance.moveRotValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectRotRate[__instance.correctNo]];
                                _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsRotAdd(__instance.correctNo, num, true, val);
                                if (_self._hasDarkness == false)
                                    _self.GetCvsAccessory(__instance.nSlotNo).UpdateAcsMoveHistory();
                                (__instance.inpRot)[num].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 1][num].ToString();
                                _self.GetCvsAccessory(__instance.nSlotNo).SetControllerTransform(__instance.correctNo);
                            }
                        });
                        p.btn.UpdateAsObservable().SkipUntil(p.btn.OnPointerDownAsObservable().Do(delegate
                        {
                            downTimeCnt = 0f;
                            loopTimeCnt = 0f;
                            change = false;
                        })).TakeUntil(p.btn.OnPointerUpAsObservable().Do(delegate
                        {
                            if (_self._hasDarkness == false)
                                if (change)
                                {
                                    _self.GetCvsAccessory(__instance.nSlotNo).UpdateAcsMoveHistory();
                                }
                        })
                        ).RepeatUntilDestroy(__instance).Subscribe(delegate
                        {
                            var num = p.index / 2;
                            var num2 = (p.index % 2 != 0) ? 1 : -1;
                            var num3 = num2 * __instance.moveRotValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectRotRate[__instance.correctNo]];
                            var num4 = 0f;
                            downTimeCnt += Time.deltaTime;
                            if (downTimeCnt > 0.3f)
                            {
                                for (loopTimeCnt += Time.deltaTime; loopTimeCnt > 0.05f; loopTimeCnt -= 0.05f)
                                {
                                    num4 += num3;
                                }
                                if (num4 != 0f)
                                {
                                    _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsRotAdd(__instance.correctNo, num, true, num4);
                                    (__instance.inpRot)[num].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 1][num].ToString();
                                    change = true;
                                    _self.GetCvsAccessory(__instance.nSlotNo).SetControllerTransform(__instance.correctNo);
                                }
                            }
                        }).AddTo(__instance);
                    });
                    (__instance.inpRot).Select((p, idx) => new
                    {
                        inp = p,
                        index = idx
                    }).ToList().ForEach(p =>
                    {
                        p.inp.onEndEdit.AsObservable().Subscribe(delegate (string value)
                        {
                            var xyz = p.index % 3;
                            var val = CustomBase.ConvertValueFromTextLimit(0f, 360f, 0, value);
                            p.inp.text = val.ToString();
                            _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsRotAdd(__instance.correctNo, xyz, false, val);
                            if (_self._hasDarkness == false)
                                _self.GetCvsAccessory(__instance.nSlotNo).UpdateAcsMoveHistory();
                            _self.GetCvsAccessory(__instance.nSlotNo).SetControllerTransform(__instance.correctNo);
                        });
                    });
                    (__instance.btnRotReset).Select((p, idx) => new
                    {
                        btn = p,
                        index = idx
                    }).ToList().ForEach(p =>
                    {
                        p.btn.OnClickAsObservable().Subscribe(delegate
                        {
                            (__instance.inpRot)[p.index].text = "0";
                            _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsRotAdd(__instance.correctNo, p.index, false, 0f);
                            if (_self._hasDarkness == false)
                                _self.GetCvsAccessory(__instance.nSlotNo).UpdateAcsMoveHistory();
                            _self.GetCvsAccessory(__instance.nSlotNo).SetControllerTransform(__instance.correctNo);
                        });
                    });
                    (__instance.btnScl).Select((p, idx) => new
                    {
                        btn = p,
                        index = idx
                    }).ToList().ForEach(p =>
                    {
                        p.btn.OnClickAsObservable().Subscribe(delegate
                        {
                            if (!change)
                            {
                                var num = p.index / 2;
                                var num2 = (p.index % 2 != 0) ? 1 : -1;
                                var val = num2 * __instance.moveSclValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectSclRate[__instance.correctNo]];
                                _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsSclAdd(__instance.correctNo, num, true, val);
                                if (_self._hasDarkness == false)
                                    _self.GetCvsAccessory(__instance.nSlotNo).UpdateAcsMoveHistory();
                                (__instance.inpScl)[num].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 2][num].ToString();
                            }
                        });
                        p.btn.UpdateAsObservable().SkipUntil(p.btn.OnPointerDownAsObservable().Do(delegate
                        {
                            downTimeCnt = 0f;
                            loopTimeCnt = 0f;
                            change = false;
                        })).TakeUntil(p.btn.OnPointerUpAsObservable().Do(delegate
                        {
                            if (_self._hasDarkness == false)
                                if (change)
                                {
                                    _self.GetCvsAccessory(__instance.nSlotNo).UpdateAcsMoveHistory();
                                }
                        })
                        ).RepeatUntilDestroy(__instance).Subscribe(delegate
                        {
                            var num = p.index / 2;
                            var num2 = (p.index % 2 != 0) ? 1 : -1;
                            var num3 = num2 * __instance.moveSclValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectSclRate[__instance.correctNo]];
                            var num4 = 0f;
                            downTimeCnt += Time.deltaTime;
                            if (downTimeCnt > 0.3f)
                            {
                                for (loopTimeCnt += Time.deltaTime; loopTimeCnt > 0.05f; loopTimeCnt -= 0.05f)
                                {
                                    num4 += num3;
                                }
                                if (num4 != 0f)
                                {
                                    _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsSclAdd(__instance.correctNo, num, true, num4);
                                    (__instance.inpScl)[num].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 2][num].ToString();
                                    change = true;
                                }
                            }
                        }).AddTo(__instance);
                    });
                    (__instance.inpScl).Select((p, idx) => new
                    {
                        inp = p,
                        index = idx
                    }).ToList().ForEach(p =>
                    {
                        p.inp.onEndEdit.AsObservable().Subscribe(delegate (string value)
                        {
                            var xyz = p.index % 3;
                            var val = CustomBase.ConvertValueFromTextLimit(0.01f, 100f, 2, value);
                            p.inp.text = val.ToString();
                            _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsSclAdd(__instance.correctNo, xyz, false, val);
                            if (_self._hasDarkness == false)
                                _self.GetCvsAccessory(__instance.nSlotNo).UpdateAcsMoveHistory();
                        });
                    });
                    (__instance.btnSclReset).Select((p, idx) => new
                    {
                        btn = p,
                        index = idx
                    }).ToList().ForEach(p =>
                    {
                        p.btn.OnClickAsObservable().Subscribe(delegate
                        {
                            (__instance.inpScl)[p.index].text = "1";
                            _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsSclAdd(__instance.correctNo, p.index, false, 1f);
                            if (_self._hasDarkness == false)
                                _self.GetCvsAccessory(__instance.nSlotNo).UpdateAcsMoveHistory();
                        });
                    });
                    (__instance.btnCopy).OnClickAsObservable().Subscribe(delegate
                    {
                        Singleton<CustomBase>.Instance.vecAcsClipBord[0] = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 0];
                        Singleton<CustomBase>.Instance.vecAcsClipBord[1] = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 1];
                        Singleton<CustomBase>.Instance.vecAcsClipBord[2] = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 2];
                    });
                    (__instance.btnPaste).OnClickAsObservable().Subscribe(delegate
                    {
                        _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsMovePaste(__instance.correctNo, Singleton<CustomBase>.Instance.vecAcsClipBord);
                        if (_self._hasDarkness == false)
                            _self.GetCvsAccessory(__instance.nSlotNo).UpdateAcsMoveHistory();
                        (__instance.inpPos)[0].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 0].x.ToString();
                        (__instance.inpPos)[1].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 0].y.ToString();
                        (__instance.inpPos)[2].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 0].z.ToString();
                        (__instance.inpRot)[0].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 1].x.ToString();
                        (__instance.inpRot)[1].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 1].y.ToString();
                        (__instance.inpRot)[2].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 1].z.ToString();
                        (__instance.inpScl)[0].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 2].x.ToString();
                        (__instance.inpScl)[1].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 2].y.ToString();
                        (__instance.inpScl)[2].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 2].z.ToString();
                        _self.GetCvsAccessory(__instance.nSlotNo).SetControllerTransform(__instance.correctNo);
                    });
                    (__instance.btnAllReset).OnClickAsObservable().Subscribe(delegate
                    {
                        for (var j = 0; j < 3; j++)
                        {
                            (__instance.inpPos)[j].text = "0";
                            (__instance.inpRot)[j].text = "0";
                            (__instance.inpScl)[j].text = "1";
                        }
                        _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsAllReset(__instance.correctNo);
                        if (_self._hasDarkness == false)
                            _self.GetCvsAccessory(__instance.nSlotNo).UpdateAcsMoveHistory();
                        _self.GetCvsAccessory(__instance.nSlotNo).SetControllerTransform(__instance.correctNo);
                    });



                    return false;
                }
            }

            [HarmonyPatch(typeof(CustomAcsMoveWindow), nameof(CustomAcsMoveWindow.ChangeSlot))]
            internal static class CustomAcsMoveWindow_ChangeSlot_Patches
            {
                private static bool Prefix(CustomAcsMoveWindow __instance, int _no, bool open)
                {
                    var tglReference = __instance.tglReference;
                    __instance.slotNo = (CustomAcsMoveWindow.AcsSlotNo)_no;
                    var isOn = tglReference.isOn;
                    tglReference.isOn = false;
                    if (__instance.correctNo == 0)
                        tglReference = _self.GetCvsAccessory(__instance.nSlotNo).tglAcsMove01;
                    else
                        tglReference = _self.GetCvsAccessory(__instance.nSlotNo).tglAcsMove02;
                    __instance.SetPrivate("tglReference", tglReference);
                    if (open && isOn)
                        tglReference.isOn = true;
                    return false;
                }
            }

            [HarmonyPatch(typeof(CustomAcsMoveWindow), nameof(CustomAcsMoveWindow.UpdateCustomUI))]
            internal static class CustomAcsMoveWindow_UpdateCustomUI_Patches
            {
                private static bool Prefix(CustomAcsMoveWindow __instance)
                {
                    var part = _self.GetPart(__instance.nSlotNo);
                    for (var i = 0; i < 3; i++)
                    {
                        __instance.inpPos[i].text = part.addMove[__instance.correctNo, 0][i].ToString();
                        __instance.inpRot[i].text = part.addMove[__instance.correctNo, 1][i].ToString();
                        __instance.inpScl[i].text = part.addMove[__instance.correctNo, 2][i].ToString();
                    }
                    return false;
                }
            }


            [HarmonyPatch(typeof(CustomAcsMoveWindow), nameof(CustomAcsMoveWindow.UpdateDragValue))]
            internal static class CustomAcsMoveWindow_UpdateDragValue_Patches
            {
                private static bool Prefix(CustomAcsMoveWindow __instance, int type, int xyz, float move)
                {
                    switch (type)
                    {
                        case 0:
                            {
                                var val = move * __instance.movePosValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectPosRate[__instance.correctNo]];
                                _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsPosAdd(__instance.correctNo, xyz, true, val);
                                (__instance.inpPos)[xyz].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 0][xyz].ToString();
                                break;
                            }
                        case 1:
                            {
                                var val2 = move * __instance.moveRotValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectRotRate[__instance.correctNo]];
                                _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsRotAdd(__instance.correctNo, xyz, true, val2);
                                (__instance.inpRot)[xyz].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 1][xyz].ToString();
                                break;
                            }
                        case 2:
                            {
                                var val3 = move * __instance.moveSclValue[Singleton<CustomBase>.Instance.customSettingSave.acsCorrectSclRate[__instance.correctNo]];
                                _self.GetCvsAccessory(__instance.nSlotNo).FuncUpdateAcsSclAdd(__instance.correctNo, xyz, true, val3);
                                (__instance.inpScl)[xyz].text = _self.GetPart(__instance.nSlotNo).addMove[__instance.correctNo, 2][xyz].ToString();
                                break;
                            }
                    }
                    _self.GetCvsAccessory(__instance.nSlotNo).SetControllerTransform(__instance.correctNo);
                    return false;
                }
            }

            #endregion

            #region CustomAcsSelectKind
            [HarmonyPatch(typeof(CustomAcsSelectKind), nameof(CustomAcsSelectKind.ChangeSlot))]
            internal static class CustomAcsSelectKind_ChangeSlot_Patches
            {
                private static bool Prefix(CustomAcsSelectKind __instance, int _no, bool open)
                {
                    var selWin = __instance.selWin;
                    __instance.slotNo = _no;
                    var isOn = selWin.tglReference.isOn;
                    selWin.tglReference.isOn = false;
                    selWin.tglReference = _self.GetCvsAccessory(__instance.slotNo).tglAcsKind;
                    if (open && isOn)
                        selWin.tglReference.isOn = true;
                    return false;
                }
            }

            [HarmonyPatch(typeof(CustomAcsSelectKind), nameof(CustomAcsSelectKind.UpdateCustomUI))]
            internal static class CustomAcsSelectKind_UpdateCustomUI_Patches
            {
                private static bool Prefix(CustomAcsSelectKind __instance, int param)
                {
                    __instance.listCtrl.SelectItem(_self.GetPart(__instance.slotNo).id);
                    return false;
                }
            }

            [HarmonyPatch(typeof(CustomAcsSelectKind), nameof(CustomAcsSelectKind.OnSelect))]
            internal static class CustomAcsSelectKind_OnSelect_Patches
            {
                private static bool Prefix(CustomAcsSelectKind __instance, int index)
                {
                    var selectInfoFromIndex = __instance.listCtrl.GetSelectInfoFromIndex(index);
                    if (selectInfoFromIndex != null)
                        _self.GetCvsAccessory(__instance.slotNo).UpdateSelectAccessoryKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
                    return false;
                }
            }

            #endregion
        }
    }
}
