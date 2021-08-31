using ChaCustom;
using System;
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
            [HarmonyPatch(typeof(CustomBase), nameof(CustomBase.SetUpdateCvsAccessory))]
            private static class CvsAccessory_UpdateSlotName_Patches
            {
                private static void Postfix()
                {
                    _self.FixWindowScroll();
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
            internal static class CustomAcsChangeSlot_Patch
            {
                static MethodBase methodbase;

                static MethodBase TargetMethod()
                {
                    methodbase = AccessTools.Method(AccessTools.TypeByName("ChaCustom.CustomAcsChangeSlot+<>c__DisplayClass18_1, Assembly-CSharp"), "<Initialize>b__4");
                    return methodbase;
                }

                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
#if DEBUG
                    _self.Logger.LogWarning($"{nameof(CustomAcsChangeSlot_Patch)} Method");
                                        var worked2 = false;

#endif
                    var worked = false;

                    var instructionsList = instructions.ToList();

                    for (int i = 0, j = 0; i < instructionsList.Count; j++, i++)
                    {
                        var inst = instructionsList[i];
                        if (inst.opcode == OpCodes.Ldc_I4_S)
                        {
                            switch (inst.operand.ToString())//pop after the first replacement as the subsequent are labelled
                            {
                                case "20":
                                    if (worked)
                                    {
                                        yield return inst;
                                        yield return new CodeInstruction(OpCodes.Pop);
                                    }
                                    worked = true;
                                    yield return new CodeInstruction(OpCodes.Call, typeof(CustomAcsChangeSlot_Patch).GetMethod(nameof(AccessoryCount), AccessTools.all));
                                    continue;
                                case "21":
#if DEBUG
                                    worked2 = true;
#endif
                                    yield return inst;
                                    yield return new CodeInstruction(OpCodes.Pop);
                                    yield return new CodeInstruction(OpCodes.Call, typeof(CustomAcsChangeSlot_Patch).GetMethod(nameof(ChangeButton), AccessTools.all));
                                    continue;

                                default:
                                    break;
                            }
                        }
                        //_self.Logger.LogWarning($"{j:00} {instructionsList[i].opcode} {instructionsList[i].operand}");
                        yield return inst;
                    }

#if DEBUG
                    _self.Logger.Log(worked && worked2 ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Error, "Transpiler finished");
#endif
                }

                private static int AccessoryCount()
                {
                    return CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length;
                }
                private static int ChangeButton()
                {
                    return CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length + 1;
                }
            }

            [HarmonyPatch]
            internal static class CVS_Replace_20_Patch
            {
#if DEBUG
                static int count = 0;
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
                    var list = new List<MethodBase>
                    {
                        AccessTools.Method(typeof(CvsAccessory), nameof(CvsAccessory.UpdateCustomUI)),//0
                        AccessTools.Method(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAcsParent)),//1
                        AccessTools.Method(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAcsColor)),//2
                        AccessTools.Method(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAccessory)),//3
                        AccessTools.Method(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.ChangeColorWindow), new[] { typeof(int)}),//4
                        AccessTools.Method(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.UpdateSlotNames)),//5
                        AccessTools.Method(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.LateUpdate)),//6
                        AccessTools.Method(typeof(CvsAccessoryChange), nameof(CvsAccessoryChange.CalculateUI)),//7
                        AccessTools.Method(typeof(CustomControl), nameof(CustomControl.Update)),//8
#if !KKS
                        AccessTools.Method(change, nameof(CustomAcsChangeSlot.Start)), 
#endif
#if !EC
                        AccessTools.Method(typeof(CvsAccessoryCopy), nameof(CvsAccessoryCopy.ChangeDstDD)),//9
                        AccessTools.Method(typeof(CvsAccessoryCopy), nameof(CvsAccessoryCopy.ChangeSrcDD)),//10
                        AccessTools.Method(typeof(CvsAccessoryCopy), nameof(CvsAccessoryCopy.CopyAcs)),//11
#endif
                };
                    return list;
                }

                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var instructionsList = instructions.ToList();
#if DEBUG
                    var work = false;
#endif
                    for (var i = 0; i < instructionsList.Count; i++)
                    {
                        var inst = instructionsList[i];
                        if (inst.opcode == OpCodes.Ldc_I4_S && inst.operand.ToString() == "20")
                        {
#if DEBUG
                            work = true;
#endif
                            yield return new CodeInstruction(OpCodes.Call, typeof(CVS_Replace_20_Patch).GetMethod(nameof(AccessoryCount), AccessTools.all));
                            continue;
                        }
                        yield return inst;
                    }
#if DEBUG
                    _self.Logger.Log(work ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Error, $"transpiler {count++} finished");
#endif
                }

                private static int AccessoryCount()//works fine for copybutton since it is equal
                {
                    return CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length;
                }
            }
#if DEBUG
            [HarmonyPatch]
            internal static class Replace_20_Patch_2
            {
                static readonly List<MethodBase> list = new List<MethodBase>
                    {
                        AccessTools.Method(typeof(CustomAcsChangeSlot), nameof(CustomAcsChangeSlot.LateUpdate)),
                        AccessTools.Method(typeof(CustomControl), nameof(CustomControl.Update)),
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
