using System.Collections.Generic;
using System.Reflection;
using System;
using HarmonyLib;
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
using System.Reflection.Emit;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public static class ChaControl_Patches
        {
#if true
            [HarmonyPatch]
            internal class ChaControl_CheckAdjuster_Patches
            {
#if DEBUG
                static int current = 0;

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
                        GetMethod(typeof(ChaControl), nameof(ChaControl.ChangeAccessoryAsync), new[] { typeof(int), typeof(int), typeof(int), typeof(string), typeof(bool), typeof(bool) }),
#if KKS
                        GetMethod(typeof(ChaControl), nameof(ChaControl.ChangeAccessoryNoAsync)),
#endif
                    };

                static void Finalizer(Exception __exception)
                {
                    current %= list.Count;
                    if (__exception != null)
                    {
                        _self.Logger.LogError($"Post  Method {current} {list[current].Name}\n" + __exception);
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
