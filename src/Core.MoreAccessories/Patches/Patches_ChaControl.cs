using HarmonyLib;
using IllusionUtility.GetUtility;
using Manager;
using MoreAccessoriesKOI.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

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

#if KKS
            [HarmonyPatch]
            internal class ChaControl_ChangeAccessoryAsync_Patches
            {
                static MethodBase methodbase;

                static MethodBase TargetMethod()
                {
                    methodbase = AccessTools.Method(AccessTools.TypeByName("ChaControl+<ChangeAccessoryAsync>d__483, Assembly-CSharp"), "MoveNext");
                    return methodbase;
                }

                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
#if DEBUG
                    _self.Logger.LogWarning($"ChaControl_ChangeAccessoryAsync_Patches\t\t{methodbase.ReflectedType}  .   {methodbase.Name}");
#endif
                    var instructionsList = instructions.ToList();
                    var end = instructionsList.FindIndex(4, x => x.opcode == OpCodes.Brtrue || x.opcode == OpCodes.Brtrue_S); //work backwards from end

#if DEBUG
                    if (end == -1)
                    {
                        _self.Logger.LogError($"Opcode not found Brtrue || Brtrue_s");
                        for (var i = 0; i < instructionsList.Count; i++)
                        {
                            _self.Logger.LogWarning($"{i:00} {instructionsList[i].opcode} {instructionsList[i].operand}");
                        }
                    }
#endif

                    var start = end - 4; //code is at least 4 lines

                    for (; start > 0; start--)
                    {
                        if (instructionsList[start].opcode == OpCodes.Ldc_I4_0)
                        {
                            break;
                        }
                    }

                    for (int i = 0, j = 0; i < instructionsList.Count; i++, j++)
                    {
                        var inst = instructionsList[i];

                        if (i == start)//instead pushing 0,slot,19 and popping RangeEqual => push Chacontrol, slotno and pop with call
                        {
                            i++;//skip pushing 0 to stack
                            yield return new CodeInstruction(OpCodes.Ldloc_1); //ldarg_0  contains chacontrol don't use this.chacontrol parts array is null for a mysterious reason
                            yield return instructionsList[i++]; //ldarg_0  this
                            yield return instructionsList[i++]; //ldfld  this.Chacontrol
                            yield return instructionsList[i++]; //ldfld  this.Chacontrol.slotno

                            //var test = new CodeInstruction(OpCodes.Call, typeof(ChaControl_ChangeAccessoryAsync_Patches).GetMethod(nameof(AccessoryCheck), AccessTools.all));
                            //_self.Logger.LogWarning($"{j++:00} {test.opcode} {test.operand}");
                            yield return new CodeInstruction(OpCodes.Call, typeof(ChaControl_ChangeAccessoryAsync_Patches).GetMethod(nameof(AccessoryCheck), AccessTools.all));
                            i += 2;//skip 19 insert and call range
                        }
                        //if (i < 35)
                        //    _self.Logger.LogWarning($"{j:00} {instructionsList[i].opcode} {instructionsList[i].operand}");
                        yield return instructionsList[i];
                    }

#if DEBUG
                    _self.Logger.LogWarning($"Transpiler worked");
#endif

                }

                private static bool AccessoryCheck(ChaControl chara, int slot)
                {
                    return MathfEx.RangeEqualOn(0, slot, chara.nowCoordinate.accessory.parts.Length - 1);
                }

            }

            [HarmonyPatch]
            internal class ChaControl_ChangeAccessoryAsync_Replace20_Patches
            {
                static MethodBase methodbase;

                static MethodBase TargetMethod()
                {
                    methodbase = AccessTools.Method(AccessTools.TypeByName("ChaControl+<ChangeAccessoryAsync>d__482, Assembly-CSharp"), "MoveNext");
                    return methodbase;
                }

                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
#if DEBUG
                    _self.Logger.LogWarning($"{nameof(ChaControl_ChangeAccessoryAsync_Replace20_Patches)} Method");
                                        var worked = false;

#endif
                    var instructionsList = instructions.ToList();
                    for (var i = 0; i < instructionsList.Count; i++)
                    {
                        var inst = instructionsList[i];
                        if (inst.opcode == OpCodes.Ldc_I4_S && inst.operand.ToString() == "20")
                        {
#if DEBUG
                            worked = true;
#endif
                            yield return new CodeInstruction(OpCodes.Ldloc_1);//feed chacontrol to method
                            yield return new CodeInstruction(OpCodes.Call, typeof(ChaControl_ChangeAccessoryAsync_Replace20_Patches).GetMethod(nameof(AccessoryCount), AccessTools.all));
                            continue;
                        }
                        yield return inst;
                    }

#if DEBUG
                    _self.Logger.Log(worked ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Error, "Transpiler finished");
#endif
                }

                private static int AccessoryCount(ChaControl chara)
                {
                    return chara.nowCoordinate.accessory.parts.Length;
                }
            }
#endif

            [HarmonyPatch]
            internal class ChaControl_CheckAdjuster_param_slot_0_Patches
            {
                static int count = 0;
#if DEBUG
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
                    var ChaCon = typeof(ChaControl);
                    var list = new List<MethodBase>
                    {
                        AccessTools.Method(ChaCon, nameof(ChaControl.ChangeAccessoryParent)),        //0
                        AccessTools.Method(ChaCon, nameof(ChaControl.SetAccessoryPos)),              //1
                        AccessTools.Method(ChaCon, nameof(ChaControl.SetAccessoryRot)),              //2
                        AccessTools.Method(ChaCon, nameof(ChaControl.SetAccessoryScl)),              //3
                        AccessTools.Method(ChaCon, nameof(ChaControl.UpdateAccessoryMoveFromInfo)),  //4
                        AccessTools.Method(ChaCon, nameof(ChaControl.ChangeAccessoryColor)),         //5
                        AccessTools.Method(ChaCon, nameof(ChaControl.SetAccessoryDefaultColor)),     //6
                    };

#if KKS
                    list.Add(AccessTools.Method(ChaCon, nameof(ChaControl.ChangeAccessoryNoAsync)));
#endif

                    return list;
                }

                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
#if DEBUG
                    _self.Logger.LogWarning($"ChaControl_CheckAdjuster_param_slot_0_Patches Method {count}");
                                        var worked = false;

#endif
                    var instructionsList = instructions.ToList();
                    var end = instructionsList.FindIndex(4, x => x.opcode == OpCodes.Brtrue || x.opcode == OpCodes.Brtrue_S); //work backwards from end
#if DEBUG
                    if (end == -1)
                    {
                        _self.Logger.LogError($"Opcode not found Brtrue || Brtrue_s");
                        for (var i = 0; i < instructionsList.Count; i++)
                        {
                            _self.Logger.LogWarning($"{i:00} {instructionsList[i].opcode} {instructionsList[i].operand}");
                        }
                    }
#endif

                    var start = end - 4; //code is at least 4 lines

                    for (; start > 0; start--)
                    {
                        if (instructionsList[start].opcode == OpCodes.Ldc_I4_0)
                        {
                            break;
                        }
                    }

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
#if DEBUG
worked = true;
#endif
                            yield return new CodeInstruction(OpCodes.Ldarg_0);
                            yield return new CodeInstruction(OpCodes.Ldarg_1);

                            yield return new CodeInstruction(OpCodes.Call, typeof(ChaControl_CheckAdjuster_param_slot_0_Patches).GetMethod(nameof(AccessoryCheck), AccessTools.all));
                            i = end;
                            inst = instructionsList[i];
                        }

                        yield return inst;
                    }

#if DEBUG
                    _self.Logger.Log(worked ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Error, "Transpiler finished");
#endif

                    count++;
                }

                private static bool AccessoryCheck(ChaControl chara, int slot)
                {
                    return MathfEx.RangeEqualOn(0, slot, chara.nowCoordinate.accessory.parts.Length - 1);
                }
            }

            [HarmonyPatch]
            internal class ChaControl_CheckAdjuster_param_slot_1_Patches
            {
#if DEBUG
                static int count = 0;
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
                    var ChaCon = typeof(ChaControl);
                    var list = new List<MethodBase>
                    {
                        AccessTools.Method(ChaCon, nameof(ChaControl.GetAccessoryDefaultColor)),
                    };
                    return list;
                }

                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
#if DEBUG
                    _self.Logger.LogWarning($"ChaControl_CheckAdjuster_param_slot_1_Patches Method {count}");
#endif
                    var instructionsList = instructions.ToList();
                    var end = instructionsList.FindIndex(4, x => x.opcode == OpCodes.Brtrue); //work backwards from end

#if DEBUG
                    var worked = false;
                    if (end == -1)
                    {
                        _self.Logger.LogError($"Opcode not found OpCodes.Brtrue_S");
                        for (var i = 0; i < instructionsList.Count; i++)
                        {
                            _self.Logger.LogWarning($"{i:00} {instructionsList[i].opcode} {instructionsList[i].operand}");
                        }
                    }
#endif

                    var start = end - 4; //code is at least 4 lines

                    for (; start > 0; start--)
                    {
                        if (instructionsList[start].opcode == OpCodes.Ldc_I4_0)
                        {
                            break;
                        }
                    }

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
#if DEBUG
                            worked = true;
#endif
                            yield return new CodeInstruction(OpCodes.Ldarg_0);
                            yield return new CodeInstruction(OpCodes.Ldarg_2);
                            yield return new CodeInstruction(OpCodes.Call, typeof(ChaControl_CheckAdjuster_param_slot_1_Patches).GetMethod(nameof(AccessoryCheck), AccessTools.all));
                            i = end;
                            inst = instructionsList[i];
                        }

                        yield return inst;
                    }
#if DEBUG
                    _self.Logger.Log(worked ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Error, "Transpiler finished");
                    count++;
#endif

                }

                private static bool AccessoryCheck(ChaControl chara, int slot)
                {
                    return MathfEx.RangeEqualOn(0, slot, chara.nowCoordinate.accessory.parts.Length - 1);
                }
            }

            [HarmonyPatch]
            internal static class ChaControl_Replace_20_Patch
            {
#if DEBUG
                static int count = 0;
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
                    var ChaCon = typeof(ChaControl);
                    var list = new List<MethodBase>
                    {
                        AccessTools.Method(ChaCon, nameof(ChaControl.UpdateAccessoryMoveAllFromInfo)),
                        AccessTools.Method(ChaCon, nameof(ChaControl.ChangeAccessory), new[] { typeof(bool), typeof(bool) }),
                    };
                    return list;
                }

                public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
#if DEBUG
                    _self.Logger.LogWarning($"{nameof(ChaControl_Replace_20_Patch)} Method {count++}");
                                        var worked = false;
#endif
                    var instructionsList = instructions.ToList();
                    for (var i = 0; i < instructionsList.Count; i++)
                    {
                        var inst = instructionsList[i];
                        if (inst.opcode == OpCodes.Ldc_I4_S && inst.operand.ToString() == "20")
                        {
#if DEBUG
                            worked = true;
#endif
                            yield return new CodeInstruction(OpCodes.Ldarg_0);//feed chacontrol to method
                            yield return new CodeInstruction(OpCodes.Call, typeof(ChaControl_Replace_20_Patch).GetMethod(nameof(AccessoryCount), AccessTools.all));
                            continue;
                        }
                        yield return inst;
                    }

#if DEBUG
                    _self.Logger.Log(worked ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Error, "Transpiler finished");
#endif

                }

                private static int AccessoryCount(ChaControl chara)
                {
                    return chara.nowCoordinate.accessory.parts.Length;
                }
            }
        }
    }
}
