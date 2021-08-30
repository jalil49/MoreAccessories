using ChaCustom;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Illusion.Extensions;
#if DEBUG
using System;
#endif
#if KKS
using Cysharp.Threading.Tasks;

#endif
using UniRx.Triggers;


namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public static class CvsAccessory_Patches
        {
            [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.Start))]
            private static class CvsAccessory_Start_Patches
            {
                private static bool Prefix(CvsAccessory __instance)
                {
                    _ = Test(__instance);
                    return false;
                }

                private static async UniTask Test(CvsAccessory __instance)
                {
                    __instance.enabled = false;

                    await UniTask.WaitUntil(() => CustomBase.instance.chaCtrl != null, PlayerLoopTiming.Update, default);

                    var slot = __instance.nSlotNo;

                    Singleton<CustomBase>.Instance.actUpdateCvsAccessory[slot] = (Action)Delegate.Combine(Singleton<CustomBase>.Instance.actUpdateCvsAccessory[slot], new Action(__instance.UpdateCustomUI));
                    Singleton<CustomBase>.Instance.actUpdateAcsSlotName[slot] = (Action)Delegate.Combine(Singleton<CustomBase>.Instance.actUpdateAcsSlotName[slot], new Action(__instance.UpdateSlotName));

                    __instance.tglTakeOverParent.OnValueChangedAsObservable().Subscribe(delegate (bool isOn) { Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverParent = isOn; });
                    __instance.tglTakeOverColor.OnValueChangedAsObservable().Subscribe(delegate (bool isOn) { Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverColor = isOn; });
                    __instance.ddAcsType.onValueChanged.AddListener(delegate (int idx)
                    {
                        __instance.UpdateSelectAccessoryType(idx);
                        var visible = idx != 0;
                        __instance.ChangeSettingVisible(visible);
                    });
                    __instance.tglAcsKind.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                    {
                        var num = (__instance.ddAcsType).value - 1;
                        if (0 <= num)
                        {
                            if (__instance.cgAccessoryWin[num] && 0f != __instance.cgAccessoryWin[num].alpha != isOn)
                            {
                                __instance.cgAccessoryWin[num].Enable(isOn, false);
                                if (isOn)
                                {
                                    __instance.tglAcsParent.isOn = false;
                                    __instance.tglAcsMove01.isOn = false;
                                    __instance.tglAcsMove02.isOn = false;
                                    for (var i = 0; i < __instance.cgAccessoryWin.Length; i++)
                                    {
                                        if (i != num)
                                        {
                                            __instance.cgAccessoryWin[i].Enable(false, false);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (var j = 0; j < __instance.cgAccessoryWin.Length; j++)
                            {
                                __instance.cgAccessoryWin[j].Enable(false, false);
                            }
                        }
                    });
                    __instance.tglAcsParent.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                    {
                        if (__instance.cgAcsParent)
                        {
                            if (__instance.cgAcsParent && 0f != __instance.cgAcsParent.alpha != isOn)
                            {
                                __instance.cgAcsParent.Enable(isOn, false);
                                if (isOn)
                                {
                                    __instance.tglAcsKind.isOn = false;
                                    __instance.tglAcsMove01.isOn = false;
                                    __instance.tglAcsMove02.isOn = false;
                                }
                            }
                        }
                    });
                    __instance.btnInitParent.onClick.AsObservable().Subscribe(delegate
                    {
                        var accessoryDefaultParentStr = CustomBase.Instance.chaCtrl.GetAccessoryDefaultParentStr(slot);
                        __instance.accessory.parts[slot].parentKey = accessoryDefaultParentStr;
                        CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[slot].parentKey = accessoryDefaultParentStr;

                        __instance.FuncUpdateAcsParent(false);
#if KOIKATSU
                        if (_self._hasDarkness == false)
                            BackwardCompatibility.CustomHistory_Instance_Add2(CustomBase.Instance.chaCtrl, __instance.FuncUpdateAcsParent, true);
#endif
                        __instance.UpdateCustomUI();
                    });
                    __instance.btnReverseParent.onClick.AsObservable().Subscribe(delegate
                    {
                        var part = __instance.accessory.parts[slot];
                        var reverseParent = ChaAccessoryDefine.GetReverseParent(part.parentKey);
                        if (string.Empty != reverseParent)
                        {
                            part.parentKey = reverseParent;
                            CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[slot].parentKey = reverseParent;

                            __instance.FuncUpdateAcsParent(false);
#if KOIKATSU
                            if (_self._hasDarkness == false)
                                BackwardCompatibility.CustomHistory_Instance_Add2(CustomBase.Instance.chaCtrl, __instance.FuncUpdateAcsParent, true);
#endif
                            __instance.UpdateCustomUI();
                        }
                    });
                    __instance.btnAcsColor01.OnClickAsObservable().Subscribe(delegate
                    {
                        if (__instance.cvsColor.isOpen && (int)__instance.cvsColor.connectColorKind == (slot * 4 + 124))
                        {
                            __instance.cvsColor.Close();
                        }
                        else
                        {
                            __instance.cvsColor.Setup($"スロット{slot + 1:00} カラー①", (CvsColor.ConnectColorKind)(slot * 4 + 124), __instance.accessory.parts[slot].color[0], __instance.UpdateAcsColor01, __instance.UpdateAcsColorHistory(), false);
                        }
                    });
                    __instance.btnAcsColor02.OnClickAsObservable().Subscribe(delegate
                    {
                        if (__instance.cvsColor.isOpen && (int)__instance.cvsColor.connectColorKind == (slot * 4 + 124 + 1))
                        {
                            __instance.cvsColor.Close();
                        }
                        else
                        {
                            __instance.cvsColor.Setup($"スロット{slot + 1:00} カラー②", (CvsColor.ConnectColorKind)(slot * 4 + 124 + 1), __instance.accessory.parts[slot].color[1], __instance.UpdateAcsColor02, __instance.UpdateAcsColorHistory(), false);
                        }
                    });
                    __instance.btnAcsColor03.OnClickAsObservable().Subscribe(delegate
                    {
                        if (__instance.cvsColor.isOpen && (int)__instance.cvsColor.connectColorKind == (slot * 4 + 124 + 2))
                        {
                            __instance.cvsColor.Close();
                        }
                        else
                        {
                            __instance.cvsColor.Setup($"スロット{slot + 1:00} カラー③", (CvsColor.ConnectColorKind)(slot * 4 + 124 + 2), __instance.accessory.parts[slot].color[2], __instance.UpdateAcsColor03, __instance.UpdateAcsColorHistory(), false);
                        }
                    });
                    __instance.btnAcsColor04.OnClickAsObservable().Subscribe(delegate
                    {
                        if (__instance.cvsColor.isOpen && (int)__instance.cvsColor.connectColorKind == (slot * 4 + 124 + 3))
                        {
                            __instance.cvsColor.Close();
                        }
                        else
                        {
                            __instance.cvsColor.Setup($"スロット{slot + 1:00} カラー④", (CvsColor.ConnectColorKind)(slot * 4 + 124 + 3), __instance.accessory.parts[slot].color[3], __instance.UpdateAcsColor04, __instance.UpdateAcsColorHistory(), true);
                        }
                    });
                    __instance.btnInitColor.onClick.AsObservable().Subscribe(delegate
                    {
                        __instance.SetDefaultColor();
#if KK
                        if (_self._hasDarkness == false)
                            __instance.UpdateAcsColorHistory();
#endif
                        __instance.UpdateCustomUI();
                    });
                    __instance.tglAcsMove01.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                    {
                        if (__instance.cgAcsMove01)
                        {
                            var flag = __instance.cgAcsMove01.alpha != 0f;
                            if (flag != isOn)
                            {
                                __instance.cgAcsMove01.Enable(isOn, false);
                                if (isOn)
                                {
                                    __instance.tglAcsKind.isOn = false;
                                    __instance.tglAcsParent.isOn = false;
                                    (__instance.tglAcsMove02).isOn = false;
                                }
                            }
                        }
                    });
                    __instance.tglAcsMove02.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                    {
                        if (__instance.cgAcsMove02)
                        {
                            var flag = __instance.cgAcsMove02.alpha != 0f;
                            if (flag != isOn)
                            {
                                __instance.cgAcsMove02.Enable(isOn, false);
                                if (isOn)
                                {
                                    __instance.tglAcsKind.isOn = false;
                                    __instance.tglAcsParent.isOn = false;
                                    (__instance.tglAcsMove01).isOn = false;
                                }
                            }
                        }
                    });
                    (from item in (__instance.tglAcsGroup).Select((tgl, idx) => new
                    {
                        tgl,
                        idx
                    })
                     where item.tgl != null
                     select item).ToList().ForEach(item =>
                     {
                         (from isOn in item.tgl.OnValueChangedAsObservable()
                          where isOn
                          select isOn).Subscribe(delegate
                          {
                              var part = __instance.accessory.parts[slot];
                              if (!Singleton<CustomBase>.Instance.GetUpdateCvsAccessory(slot) && item.idx != part.hideCategory)
                              {
                                  part.hideCategory = item.idx;
                                  CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[slot].hideCategory = item.idx;

#if KK || KKS
                                  __instance.cmpDrawCtrl.UpdateAccessoryDraw();
                                  if (_self._hasDarkness == false)
                                      BackwardCompatibility.CustomHistory_Instance_Add1(CustomBase.Instance.chaCtrl, null);
#endif
                              }
                          });
                     });
#if EC
                        (from item in (__instance.tglHideTiming).Select((Toggle tgl, int idx) => new { tgl, idx })
                         where item.tgl != null
                         select item).ToList().ForEach(item =>
                         {
                             item.tgl.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                             {
                                 ChaFileAccessory.PartsInfo part = __instance.accessory.parts[slot];
                                 if (!Singleton<CustomBase>.Instance.updateCustomUI && isOn && !Singleton<CustomBase>.Instance.GetUpdateCvsAccessory(slot) && item.idx != part.hideTiming)
                                 {
                                     part.hideTiming = item.idx;
                                         CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[slot].hideTiming = item.idx;
                                 }
                             });
                         });
#endif
                    if (__instance.tglDrawController01)
                    {
                        (__instance.tglDrawController01).OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                        {
                            if (!Singleton<CustomBase>.Instance.updateCustomUI)
                            {
                                Singleton<CustomBase>.Instance.customSettingSave.drawController[0] = isOn;
                            }
                        });
                    }
                    if (__instance.tglControllerType01 != null)
                    {
                        (__instance.tglControllerType01).Select((p, idx) => new
                        {
                            toggle = p,
                            index = (byte)idx
                        }).ToList().ForEach(p =>
                        {
                            p.toggle.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                            {
                                if (!Singleton<CustomBase>.Instance.updateCustomUI && isOn)
                                {
                                    Singleton<CustomBase>.Instance.customSettingSave.controllerType[0] = p.index;
                                    Singleton<CustomBase>.Instance.customCtrl.cmpGuid[0].SetMode(p.index);
                                }
                            });
                        });
                    }
                    if (__instance.sldControllerSpeed01)
                    {
                        __instance.sldControllerSpeed01.onValueChanged.AsObservable().Subscribe(delegate (float val)
                        {
                            Singleton<CustomBase>.Instance.customSettingSave.controllerSpeed[0] = val;
                            Singleton<CustomBase>.Instance.customCtrl.cmpGuid[0].speedMove = val;
                        });
                        __instance.sldControllerSpeed01.OnScrollAsObservable().Subscribe(delegate (PointerEventData scl) { __instance.sldControllerSpeed01.value = Mathf.Clamp(__instance.sldControllerSpeed01.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0.01f, 1f); });
                    }
                    if (__instance.sldControllerScale01)
                    {
                        __instance.sldControllerScale01.onValueChanged.AsObservable().Subscribe(delegate (float val)
                        {
                            Singleton<CustomBase>.Instance.customSettingSave.controllerScale[0] = val;
                            Singleton<CustomBase>.Instance.customCtrl.cmpGuid[0].scaleAxis = val;
                            Singleton<CustomBase>.Instance.customCtrl.cmpGuid[0].UpdateScale();
                        });
                        __instance.sldControllerScale01.OnScrollAsObservable().Subscribe(delegate (PointerEventData scl) { __instance.sldControllerScale01.value = Mathf.Clamp(__instance.sldControllerScale01.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0.3f, 3f); });
                    }
                    if (__instance.tglDrawController02)
                    {
                        (__instance.tglDrawController02).OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                        {
                            if (!Singleton<CustomBase>.Instance.updateCustomUI)
                            {
                                Singleton<CustomBase>.Instance.customSettingSave.drawController[1] = isOn;
                            }
                        });
                    }
                    if (__instance.tglControllerType02 != null)
                    {
                        __instance.tglControllerType02.Select((p, idx) => new
                        {
                            toggle = p,
                            index = (byte)idx
                        }).ToList().ForEach(p =>
                        {
                            p.toggle.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                            {
                                if (!Singleton<CustomBase>.Instance.updateCustomUI && isOn)
                                {
                                    Singleton<CustomBase>.Instance.customSettingSave.controllerType[1] = p.index;
                                    Singleton<CustomBase>.Instance.customCtrl.cmpGuid[1].SetMode(p.index);
                                }
                            });
                        });
                    }
                    if (__instance.sldControllerSpeed02)
                    {
                        __instance.sldControllerSpeed02.onValueChanged.AsObservable().Subscribe(delegate (float val)
                        {
                            Singleton<CustomBase>.Instance.customSettingSave.controllerSpeed[1] = val;
                            Singleton<CustomBase>.Instance.customCtrl.cmpGuid[1].speedMove = val;
                        });
                        __instance.sldControllerSpeed02.OnScrollAsObservable().Subscribe(delegate (PointerEventData scl) { __instance.sldControllerSpeed02.value = Mathf.Clamp(__instance.sldControllerSpeed02.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0.01f, 1f); });
                    }
                    if (__instance.sldControllerScale02)
                    {
                        __instance.sldControllerScale02.onValueChanged.AsObservable().Subscribe(delegate (float val)
                        {
                            Singleton<CustomBase>.Instance.customSettingSave.controllerScale[1] = val;
                            Singleton<CustomBase>.Instance.customCtrl.cmpGuid[1].scaleAxis = val;
                            Singleton<CustomBase>.Instance.customCtrl.cmpGuid[1].UpdateScale();
                        });
                        __instance.sldControllerScale02.OnScrollAsObservable().Subscribe(delegate (PointerEventData scl) { __instance.sldControllerScale02.value = Mathf.Clamp(__instance.sldControllerScale02.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0.3f, 3f); });
                    }
                    if (_self._hasDarkness)
                        __instance.tglNoShake.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                        {
                            var part = __instance.accessory.parts[slot];
                            if (!Singleton<CustomBase>.Instance.updateCustomUI && part.noShake != isOn)
                            {
                                part.noShake = isOn;
                                CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[slot].noShake = isOn;
                            }
                        });
                    __instance.enabled = true;
                }
            }

            [HarmonyPatch(typeof(CvsAccessoryChange), nameof(CvsAccessoryChange.Start))]
            internal static class CvsAccessoryChange_Start_Patches
            {
                private static CvsAccessoryChange _instance;
                private static void Postfix(CvsAccessoryChange __instance)
                {
                    _instance = __instance;
                }

                internal static void SetSourceIndex(int index)
                {
                    _instance.selSrc = index;
                }
                internal static void SetDestinationIndex(int index)
                {
                    _instance.selDst = index;
                }
            }

#if true

            [HarmonyPatch]
            internal static class Replace_20_Patch
            {
#if DEBUG
                static readonly List<MethodBase> list = new List<MethodBase>
                    {
                        GetMethod(typeof(CvsAccessory), nameof(CvsAccessory.UpdateCustomUI)),
                        GetMethod(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAcsParent)),
                        GetMethod(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAcsColor)),
                        GetMethod(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAccessory)),
                        GetMethod(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.ChangeColorWindow), new[] { typeof(int) }),
                        GetMethod(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.UpdateSlotNames)),
                        GetMethod(typeof(CvsAccessoryChange), nameof(CvsAccessoryChange.CalculateUI)),
#if KKS
                        GetMethod(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.Initialize)), //possible conflict 2 references to 20, and a 21 reference
#else
                        GetMethod(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.Start)), //possible conflict
#endif
#if !EC
                        GetMethod(typeof(CvsAccessoryCopy), nameof(CvsAccessoryCopy.ChangeDstDD)),
                        GetMethod(typeof(CvsAccessoryCopy), nameof(CvsAccessoryCopy.ChangeSrcDD)),
                        GetMethod(typeof(CvsAccessoryCopy), nameof(CvsAccessoryCopy.CopyAcs)),
#endif

                    };

                static Exception Finalizer(Exception __exception)
                {
                    if (__exception != null)
                    {
                        _self.Logger.LogError(__exception);
                        __exception = null;
                    }
                    return __exception;
                }
#endif
                static IEnumerable<MethodBase> TargetMethods()
                {
#if !DEBUG
                    var list = new List<MethodBase>
                    {
                        GetMethod(typeof(CvsAccessory), nameof(CvsAccessory.UpdateCustomUI)),
                        GetMethod(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAcsParent)),
                        GetMethod(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAcsColor)),
                        GetMethod(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAccessory)),
                        GetMethod(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.ChangeColorWindow), new[] { typeof(int)}),
                        GetMethod(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.UpdateSlotNames)),
                        GetMethod(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.LateUpdate)),
                        GetMethod(typeof(CvsAccessoryChange), nameof(CvsAccessoryChange.CalculateUI)),
                        GetMethod(typeof(CustomControl), nameof(CustomControl.Update)),
#if KKS
                        GetMethod(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.Initialize)), 
#else
                        GetMethod(change, nameof(CustomAcsChangeSlot.Start)), 
#endif
#if !EC
                        GetMethod(typeof(CvsAccessoryCopy), nameof(CvsAccessoryCopy.ChangeDstDD)),
                        GetMethod(typeof(CvsAccessoryCopy), nameof(CvsAccessoryCopy.ChangeSrcDD)),
                        GetMethod(typeof(CvsAccessoryCopy), nameof(CvsAccessoryCopy.CopyAcs)),
#endif
                };
#endif
                    return list;
                }

                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var instructionsList = instructions.ToList();
                    for (var i = 0; i < instructionsList.Count; i++)
                    {
                        var inst = instructionsList[i];
                        if (inst.opcode == OpCodes.Ldc_I4_S && inst.operand.ToString() == "20")
                        {
                            var test = new CodeInstruction(OpCodes.Call, typeof(Replace_20_Patch).GetMethod(nameof(AccessoryCount), AccessTools.all));

                            yield return new CodeInstruction(OpCodes.Call, typeof(Replace_20_Patch).GetMethod(nameof(AccessoryCount), AccessTools.all));
                            continue;
                        }
                        if (inst.opcode == OpCodes.Ldc_I4_S && inst.operand.ToString() == "21")//resolve conflict in CustomAcsChangeSlot.Initialize/start
                        {
                            var test = new CodeInstruction(OpCodes.Call, typeof(Replace_20_Patch).GetMethod(nameof(AccessoryCount), AccessTools.all));

                            yield return new CodeInstruction(OpCodes.Call, typeof(Replace_20_Patch).GetMethod(nameof(ChangeButton), AccessTools.all));
                            continue;
                        }
                        yield return inst;
                    }
                }

                private static int AccessoryCount()//works fine for copybutton since it is equal
                {
                    return CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length;
                }
                private static int ChangeButton()
                {
                    return CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length + 1;
                }
            }

#if DEBUG
            //[HarmonyPatch(typeof(CvsAccessoryChange), nameof(CvsAccessoryChange.Start))]
            //internal static class CvsAccessoryChange_Start_Patches
            //{
            //    private static CvsAccessoryChange _instance;
            //    private static void Postfix(CvsAccessoryChange __instance)
            //    {
            //        _instance = __instance;
            //    }

            //    internal static void SetSourceIndex(int index)
            //    {
            //        _instance.selSrc = index;
            //    }
            //    internal static void SetDestinationIndex(int index)
            //    {
            //        _instance.selDst = index;
            //    }
            //}

            [HarmonyPatch]
            internal static class Replace_20_Patch_2
            {
                static readonly List<MethodBase> list = new List<MethodBase>
                    {
                        GetMethod(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.LateUpdate)),
                        GetMethod(typeof(CustomControl), nameof(CustomControl.Update)),
                    };

                static void Finalizer(Exception __exception)
                {
                    if (__exception != null)
                    {
                        _self.Logger.LogError(__exception);
                    }
                }
                static IEnumerable<MethodBase> TargetMethods()
                {
                    return list;
                }

                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var instructionsList = instructions.ToList();
                    for (var i = 0; i < instructionsList.Count; i++)
                    {
                        var inst = instructionsList[i];
                        if (inst.opcode == OpCodes.Ldc_I4_S && inst.operand.ToString() == "20")
                        {
                            var test = new CodeInstruction(OpCodes.Call, typeof(Replace_20_Patch_2).GetMethod(nameof(AccessoryCount), AccessTools.all));

                            yield return new CodeInstruction(OpCodes.Call, typeof(Replace_20_Patch_2).GetMethod(nameof(AccessoryCount), AccessTools.all));
                            continue;
                        }
                        yield return inst;
                    }
                }

                private static int AccessoryCount()
                {
                    return CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length;
                }
            }
#endif
#endif
        }
    }
}
