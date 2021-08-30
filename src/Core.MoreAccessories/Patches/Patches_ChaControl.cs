using System.Collections.Generic;
using System.Reflection;
using System;
using HarmonyLib;
using System.Reflection.Emit;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using MoreAccessoriesKOI.Extensions;
using IllusionUtility.GetUtility;
#if EC
using ADVPart.Manipulate.Chara;
using HEdit;
using HPlay;
#endif
using System.Linq;
#if EC
using ADVPart.Manipulate.Chara;
using HEdit;
using HPlay;
#endif

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public static class ChaControl_Patches
        {
#if true
            static readonly object[] _params = new object[10];
            static MethodInfo _loadCharaFbxData;

            [HarmonyPrefix]
            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.ChangeAccessoryAsync), new[] { typeof(int), typeof(int), typeof(int), typeof(string), typeof(bool), typeof(bool) })]
            private static bool ChangeAccessory(ChaControl __instance, int slotNo, int type, int id, string parentKey, bool forceChange = false, bool update = true)
            {
                _self.Logger.LogWarning($"Changing slot {slotNo} to {type} with id of {id}");
                ListInfoBase lib = null;
                var load = true;
                var release = true;
                bool typerelease;
                if (Game.isAddH)
                {
                    typerelease = (120 == type || !MathfEx.RangeEqualOn(121, type, 130));
                }
                else
                {
                    typerelease = (120 == type || !MathfEx.RangeEqualOn(121, type, 129));
                }

                if (typerelease)
                {
                    release = true;
                    load = false;
                }
                else
                {
                    if (id == -1)
                    {
                        release = false;
                        load = false;
                    }
                    var num = (__instance.infoAccessory[slotNo] != null) ? __instance.infoAccessory[slotNo].Category : -1;
                    var num2 = (__instance.infoAccessory[slotNo] != null) ? __instance.infoAccessory[slotNo].Id : -1;
                    if (!forceChange && null != __instance.objAccessory[slotNo] && type == num && id == num2)
                    {
                        load = false;
                        release = false;
                    }
                    if (-1 != id)
                    {
                        if (!__instance.lstCtrl.ContainsCategoryInfo((ChaListDefine.CategoryNo)type))
                        {
                            release = true;
                            load = false;
                        }
                        else
                        {
                            lib = __instance.lstCtrl.GetInfo((ChaListDefine.CategoryNo)type, id);
                            if (lib == null)
                            {
                                release = true;
                                load = false;
                            }
                            else if (!__instance.hiPoly)
                            {
                                var flag4 = true;
                                if (123 == type && 1 == lib.Kind)
                                {
                                    flag4 = false;
                                }
                                if (122 == type && 1 == lib.GetInfoInt(ChaListDefine.KeyType.HideHair))
                                {
                                    flag4 = false;
                                }
                                if (Manager.Config.EtcData.loadHeadAccessory && 122 == type && 1 == lib.Kind)
                                {
                                    flag4 = false;
                                }
                                if (Manager.Config.EtcData.loadAllAccessory)
                                {
                                    flag4 = false;
                                }
                                if (flag4)
                                {
                                    release = true;
                                    load = false;
                                }
                            }
                        }
                    }
                }
                if (release)
                {
                    if (!load)
                    {
                        __instance.nowCoordinate.accessory.parts[slotNo].MemberInit();
                        __instance.nowCoordinate.accessory.parts[slotNo].type = 120;
                    }
                    if (__instance.objAccessory[slotNo])
                    {
                        __instance.SafeDestroy(__instance.objAccessory[slotNo]);
                        __instance.objAccessory[slotNo] = null;
                        __instance.infoAccessory[slotNo] = null;
                        __instance.cusAcsCmp[slotNo] = null;
                        for (var i = 0; i < 2; i++)
                        {
                            __instance.objAcsMove[slotNo, i] = null;
                        }
                    }
                }
                if (load)
                {
                    byte weight = 0;
                    Transform trfParent = null;
                    if ("null" == lib.GetInfo(ChaListDefine.KeyType.Parent))
                    {
                        weight = 2;
                        trfParent = __instance.objTop.transform;
                    }
                    if (_loadCharaFbxData == null)
                        _loadCharaFbxData = __instance.GetType().GetMethod("LoadCharaFbxData", AccessTools.all);
#if KK || KKS

                    _params[0] = new Action<ListInfoBase>(delegate (ListInfoBase l) { __instance.infoAccessory[slotNo] = l; });
                    _params[1] = true;
                    _params[2] = type;
                    _params[3] = id;
                    _params[4] = "ca_slot" + (slotNo + 20).ToString("00");
                    _params[5] = false;
                    _params[6] = weight;
                    _params[7] = trfParent;
                    _params[8] = -1;
                    _params[9] = false;
#elif EMOTIONCREATORS
                _params[0] = type;
                _params[1] = id;
                _params[2] = "ca_slot" + (slotNo + 20).ToString("00");
                _params[3] = false;
                _params[4] = weight;
                _params[5] = trfParent;
                _params[6] = -1;
                _params[7] = false;
#endif

                    __instance.objAccessory[slotNo] = (GameObject)_loadCharaFbxData.Invoke(__instance, _params); // I'm doing this the reflection way in order to be compatible with other plugins (like RimRemover)
                    if (__instance.objAccessory[slotNo])
                    {
                        var component = __instance.objAccessory[slotNo].GetComponent<ListInfoComponent>();
                        lib = (__instance.infoAccessory[slotNo] = component.data);
                        __instance.cusAcsCmp[slotNo] = __instance.objAccessory[slotNo].GetComponent<ChaAccessoryComponent>();
                        __instance.nowCoordinate.accessory.parts[slotNo].type = type;
                        __instance.nowCoordinate.accessory.parts[slotNo].id = lib.Id;
                        __instance.objAcsMove[slotNo, 0] = __instance.objAccessory[slotNo].transform.FindLoop("N_move");
                        __instance.objAcsMove[slotNo, 1] = __instance.objAccessory[slotNo].transform.FindLoop("N_move2");
                    }
                }
                if (__instance.objAccessory[slotNo])
                {
                    if (__instance.loadWithDefaultColorAndPtn)
                    {
                        __instance.SetAccessoryDefaultColor(slotNo);
                    }
                    __instance.ChangeAccessoryColor(slotNo + 20);
                    if (string.Empty == parentKey)
                    {
                        parentKey = lib.GetInfo(ChaListDefine.KeyType.Parent);
                    }
                    __instance.ChangeAccessoryParent(slotNo + 20, parentKey);
                    __instance.UpdateAccessoryMoveFromInfo(slotNo + 20);
                    __instance.nowCoordinate.accessory.parts[slotNo].partsOfHead = ChaAccessoryDefine.CheckPartsOfHead(parentKey);
#if KOIKATSU
                        if (!__instance.hiPoly && !Manager.Config.EtcData.loadAllAccessory)
                        {
                            var componentsInChildren = __instance.objAccessory[slotNo].GetComponentsInChildren<DynamicBone>(true);
                            foreach (var dynamicBone in componentsInChildren)
                            {
                                dynamicBone.enabled = false;
                            }
                        }
#endif
                    if (_self._hasDarkness)
                        __instance.ChangeShakeAccessory(slotNo + 20);
                }
                __instance.SetHideHairAccessory();
                return false;
            }

            [HarmonyPatch]
            internal class ChaControl_CheckAdjuster_Patches
            {
#if DEBUG
                static readonly List<MethodBase> list = new List<MethodBase>
                    {
                        GetMethod(typeof(ChaControl), nameof(ChaControl.ChangeAccessoryParent)),
                        GetMethod(typeof(ChaControl), nameof(ChaControl.SetAccessoryPos)),
                        GetMethod(typeof(ChaControl), nameof(ChaControl.SetAccessoryRot)),
                        GetMethod(typeof(ChaControl), nameof(ChaControl.SetAccessoryScl)),
                        GetMethod(typeof(ChaControl), nameof(ChaControl.UpdateAccessoryMoveFromInfo)),
                        GetMethod(typeof(ChaControl), nameof(ChaControl.ChangeAccessoryColor)),
                        GetMethod(typeof(ChaControl), nameof(ChaControl.GetAccessoryDefaultColor)),
                        GetMethod(typeof(ChaControl), nameof(ChaControl.SetAccessoryDefaultColor)),
                        //GetMethod(typeof(ChaControl), nameof(ChaControl.ChangeAccessoryAsync), new[] { typeof(int), typeof(int), typeof(int), typeof(string), typeof(bool), typeof(bool) }),
#if KKS
                        GetMethod(typeof(ChaControl), nameof(ChaControl.ChangeAccessoryNoAsync)),
#endif
                    };

                static void Finalizer(Exception __exception)
                {
                    if (__exception != null)
                    {
                        _self.Logger.LogError(__exception);
                    }
                }

#endif

                static IEnumerable<MethodBase> TargetMethods()
                {
#if !DEBUG
                    var ChaCon = typeof(ChaControl);
                    var list = new List<MethodBase>
                    {
                        GetMethod(ChaCon, nameof(ChaControl.ChangeAccessoryParent)),
                        GetMethod(ChaCon, nameof(ChaControl.SetAccessoryPos)),
                        GetMethod(ChaCon, nameof(ChaControl.SetAccessoryRot)),
                        GetMethod(ChaCon, nameof(ChaControl.SetAccessoryScl)),
                        GetMethod(ChaCon, nameof(ChaControl.UpdateAccessoryMoveFromInfo)),
                        GetMethod(ChaCon, nameof(ChaControl.ChangeAccessoryColor)),
                        GetMethod(ChaCon, nameof(ChaControl.GetAccessoryDefaultColor)),
                        GetMethod(ChaCon, nameof(ChaControl.SetAccessoryDefaultColor)),
                        GetMethod(ChaCon, nameof(ChaControl.ChangeAccessoryAsync), new[] { typeof(int), typeof(int), typeof(int), typeof(string), typeof(bool), typeof(bool) }),
#if KKS
                        GetMethod(ChaCon, nameof(ChaControl.ChangeAccessoryNoAsync)),
#endif
                    };
#endif
                    return list;
                }

                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var instructionsList = instructions.ToList();
                    var end = instructionsList.FindIndex(4, x => x.opcode == OpCodes.Brtrue_S); //work backwards from end
                    var start = end - 4; //code is at least 4 lines
                    for (; start > 0; start--)
                    {
                        if (instructionsList[start].opcode == OpCodes.Ldc_I4_0)
                        {
                            break;
                        }
                    }

                    for (var i = 0; i < instructionsList.Count; i++)
                    {
                        var inst = instructionsList[i];

                        if (i == start)
                        {
                            yield return new CodeInstruction(OpCodes.Call, typeof(ChaControl_CheckAdjuster_Patches).GetMethod(nameof(AccessoryCheck), BindingFlags.NonPublic | BindingFlags.Static));
                            i = end;
                            inst = instructionsList[i];
                        }

                        yield return inst;
                    }
                }

                private static bool AccessoryCheck(ChaControl chara, int slot)
                {
                    return MathfEx.RangeEqualOn(0, slot, chara.nowCoordinate.accessory.parts.Length - 1);
                }
            }

            [HarmonyPatch]
            internal static class Replace_20_Patch
            {
#if DEBUG
                static int current = 0;

                static readonly List<MethodBase> list = new List<MethodBase>
                    {
                        GetMethod(typeof(ChaControl), nameof(ChaControl.UpdateAccessoryMoveAllFromInfo)),
                        GetMethod(typeof(ChaControl), nameof(ChaControl.ChangeAccessory), new[] { typeof(bool), typeof(bool) }),
                        GetMethod(typeof(ChaControl), nameof(ChaControl.ChangeAccessoryAsync), new[] { typeof(bool) }),
                    };

                static void Finalizer(Exception __exception)
                {
                    current %= list.Count;

                    if (__exception != null)
                    {
                        _self.Logger.LogError($"Post Method {current} {list[current].Name}\n" + __exception);
                    }
                    current++;
                }

#endif


                static IEnumerable<MethodBase> TargetMethods()
                {
#if !DEBUG
                    var ChaCon = typeof(ChaControl);
                    var list = new List<MethodBase>
                    {
                        GetMethod(ChaCon, nameof(ChaControl.UpdateAccessoryMoveAllFromInfo)),
                        GetMethod(ChaCon, nameof(ChaControl.ChangeAccessory), new[] { typeof(bool), typeof(bool) }),
                        GetMethod(ChaCon, nameof(ChaControl.ChangeAccessoryAsync), new[] { typeof(bool) }),
                    };
#endif
                    return list;
                }

                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var instructionsList = instructions.ToList();
                    print("test");
                    for (var i = 0; i < instructionsList.Count; i++)
                    {
                        var inst = instructionsList[i];
                        if (inst.opcode == OpCodes.Ldc_I4_S && inst.operand.ToString() == "20")
                        {
                            yield return new CodeInstruction(OpCodes.Ldarg_0);//feed chacontrol to method
                            yield return new CodeInstruction(OpCodes.Call, typeof(Replace_20_Patch).GetMethod(nameof(AccessoryCount), AccessTools.all));

                            continue;
                        }
                        yield return inst;
                    }
                }

                private static int AccessoryCount(ChaControl chara)
                {
                    return chara.nowCoordinate.accessory.parts.Length;
                }
            }
#endif
        }
    }
}
