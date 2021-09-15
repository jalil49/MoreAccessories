using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MoreAccessoriesKOI.Patches.MainGame
{
    public class ChaControl_Patches
    {
        [HarmonyPatch]
        public class ChangeCoordinate_Patch
        {
            static readonly List<ChaControl> PendingNowAccessories = new List<ChaControl>();
#if KK || KKS
            [HarmonyPrefix]
            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.ChangeCoordinateType), new[] { typeof(ChaFileDefine.CoordinateType), typeof(bool) })]
            internal static void ChangeCoordPrefix(ChaControl __instance)
            {
                PendingNowAccessories.Add(__instance);
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.SetNowCoordinate), new[] { typeof(ChaFileCoordinate) })]
            internal static void SetNowCoordinatePrefix(ChaControl __instance)
            {
                PendingNowAccessories.Add(__instance);
            }
#endif

            [HarmonyPostfix]
            [HarmonyPatch(typeof(ChaFileCoordinate), nameof(ChaFileCoordinate.LoadBytes))]
            internal static void Nowcoordinatechange()
            {
                if (PendingNowAccessories.Count == 0) return;
                foreach (var item in PendingNowAccessories)
                {
                    var len = item.nowCoordinate.accessory.parts.Length;
                    if (len > item.objAccessory.Length || len > item.fileStatus.showAccessory.Length || MoreAccessories.CharaMaker)
                        MoreAccessories.ArraySync(item);
                }
                PendingNowAccessories.Clear();
                if (MoreAccessories.CharaMaker && ChaCustom.CustomBase.instance.chaCtrl != null) MoreAccessories.MakerMode.UpdateMakerUI();
            }
        }

        [HarmonyPatch]
        internal class ChaControl_ChangeAccessoryAsync_Patches
        {
            static MethodBase TargetMethod()
            {
                MethodBase methodbase;
#if KKS
                methodbase = AccessTools.Method(AccessTools.TypeByName("ChaControl+<ChangeAccessoryAsync>d__483, Assembly-CSharp"), "MoveNext");
#elif KK || EC
                methodbase = AccessTools.Method(AccessTools.TypeByName("ChaControl+<ChangeAccessoryAsync>c__Iterator12, Assembly-CSharp"), "MoveNext");
#endif
                return methodbase;
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
#if DEBUG
                MoreAccessories.Print($"ChaControl_ChangeAccessoryAsync_Patches");
#endif
                var instructionsList = instructions.ToList();
                var end = instructionsList.FindIndex(4, x => x.opcode == OpCodes.Brtrue || x.opcode == OpCodes.Brtrue_S); //work backwards from end

#if DEBUG
                if (end == -1)
                {
                    MoreAccessories.Print($"Opcode not found Brtrue || Brtrue_s", BepInEx.Logging.LogLevel.Error);
                    for (var i = 0; i < instructionsList.Count; i++)
                    {
                        MoreAccessories.Print($"{i:00} {instructionsList[i].opcode} {instructionsList[i].operand}");
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
#if KKS
                        yield return new CodeInstruction(OpCodes.Ldloc_1); //ldarg_0  contains chacontrol don't use this.chacontrol parts array is null for a mysterious reason
#elif KK || EC
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.TypeByName("ChaControl+<ChangeAccessoryAsync>c__Iterator12, Assembly-CSharp").GetField("$this", AccessTools.all));
#endif
                        yield return instructionsList[i++]; //ldarg_0  this
                        yield return instructionsList[i++]; //ldfld  this.Chacontrol
                        yield return instructionsList[i++]; //ldfld  this.Chacontrol.slotno

                        //var test = new CodeInstruction(OpCodes.Call, typeof(ChaControl_ChangeAccessoryAsync_Patches).GetMethod(nameof(AccessoryCheck), AccessTools.all));
                        //MoreAccessories._self.Logger.LogWarning($"{j++:00} {test.opcode} {test.operand}");
                        yield return new CodeInstruction(OpCodes.Call, typeof(ChaControl_ChangeAccessoryAsync_Patches).GetMethod(nameof(AccessoryCheck), AccessTools.all));
                        i += 2;//skip 19 insert and call range
                    }
                    //if (i < 35)
                    //    MoreAccessories._self.Logger.LogWarning($"{j:00} {instructionsList[i].opcode} {instructionsList[i].operand}");
                    yield return instructionsList[i];
                }

#if DEBUG
                MoreAccessories.Print($"Transpiler worked");
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
            static MethodBase TargetMethod()
            {
                MethodBase methodbase;
#if KKS
                methodbase = AccessTools.Method(AccessTools.TypeByName("ChaControl+<ChangeAccessoryAsync>d__482, Assembly-CSharp"), "MoveNext");
#elif KK || EC
                methodbase = AccessTools.Method(AccessTools.TypeByName("ChaControl+<ChangeAccessoryAsync>c__Iterator11, Assembly-CSharp"), "MoveNext");
#endif
                return methodbase;
            }

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
#if DEBUG
                MoreAccessories.Print($"{nameof(ChaControl_ChangeAccessoryAsync_Replace20_Patches)} Method");
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
#if KKS
                        yield return new CodeInstruction(OpCodes.Ldloc_1);//feed chacontrol to method
#elif KK || EC
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.TypeByName("ChaControl+<ChangeAccessoryAsync>c__Iterator11, Assembly-CSharp").GetField("$this", AccessTools.all));
#endif
                        yield return new CodeInstruction(OpCodes.Call, typeof(ChaControl_ChangeAccessoryAsync_Replace20_Patches).GetMethod(nameof(AccessoryCount), AccessTools.all));
                        continue;
                    }
                    yield return inst;
                }

#if DEBUG
                MoreAccessories.Print("Transpiler finished", worked ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Error);
#endif
            }

            internal static void Prefix(ChaControl __instance)
            {
                var len = __instance.nowCoordinate.accessory.parts.Length;
                if (len > __instance.objAccessory.Length || len > __instance.fileStatus.showAccessory.Length)
                {
                    MoreAccessories.ArraySync(__instance);
                }
            }

            private static int AccessoryCount(ChaControl chara)
            {
                return chara.nowCoordinate.accessory.parts.Length;
            }
        }

        [HarmonyPatch]
        internal class ChaControl_CheckAdjuster_param_slot_0_Patches
        {
            static int count = 0;
#if DEBUG
            static void Finalizer(Exception __exception)
            {
                if (__exception != null)
                {
                    MoreAccessories.Print(__exception.ToString(), BepInEx.Logging.LogLevel.Error);
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
                        AccessTools.Method(ChaCon, nameof(ChaControl.IsAccessory)),     //6
                    };

#if KKS
                list.Add(AccessTools.Method(ChaCon, nameof(ChaControl.ChangeAccessoryNoAsync)));
#endif

                return list;
            }

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
#if DEBUG
                MoreAccessories.Print($"ChaControl_CheckAdjuster_param_slot_0_Patches Method {count}");
                var worked = false;

#endif
                var instructionsList = instructions.ToList();
                var end = instructionsList.FindIndex(4, x => x.opcode == OpCodes.Brtrue || x.opcode == OpCodes.Brtrue_S); //work backwards from end
#if DEBUG
                if (end == -1)
                {
                    MoreAccessories.Print($"Opcode not found Brtrue || Brtrue_s", BepInEx.Logging.LogLevel.Error);
                    for (var i = 0; i < instructionsList.Count; i++)
                    {
                        MoreAccessories.Print($"{i:00} {instructionsList[i].opcode} {instructionsList[i].operand}");
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
                MoreAccessories.Print("Transpiler finished", worked ? BepInEx.Logging.LogLevel.Debug : BepInEx.Logging.LogLevel.Error);
#endif

                count++;
            }

            private static bool AccessoryCheck(ChaControl chara, int slot)
            {
                return MathfEx.RangeEqualOn(0, slot, chara.nowCoordinate.accessory.parts.Length - 1);
            }
        }
#if KKS
        [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.ChangeAccessory), new[] { typeof(bool), typeof(bool) })]
#elif KK || EC
        [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.ChangeAccessory), new[] { typeof(bool) })]
#endif
        internal class ChacontrolChangeAccessory_Patch
        {
            internal static void Prefix(ChaControl __instance)
            {
                var len = __instance.nowCoordinate.accessory.parts.Length;
                if (len > __instance.objAccessory.Length || len > __instance.fileStatus.showAccessory.Length)
                {
                    MoreAccessories.ArraySync(__instance);
                }
            }

            internal static void Postfix(ChaControl __instance)
            {
                var obj = __instance.objAccessory;
                var info = __instance.infoAccessory;
                var cusacscmp = __instance.cusAcsCmp;
                var objAcsMove = __instance.objAcsMove;
                for (int i = __instance.nowCoordinate.accessory.parts.Length, n = obj.Length; i < n; i++)
                {
                    if (obj[i])
                    {
                        __instance.SafeDestroy(obj[i]);
                        info[i] = null;
                        cusacscmp[i] = null;
                        for (var j = 0; j < 2; j++)
                        {
                            objAcsMove[i, j] = null;
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.ChangeAccessoryAsync), new[] { typeof(bool) })]
        internal class ChacontrolChangeAccessoryAsync_Patch
        {
            internal static void Prefix(ChaControl __instance)
            {
                var len = __instance.nowCoordinate.accessory.parts.Length;
                if (len > __instance.objAccessory.Length || len > __instance.fileStatus.showAccessory.Length)
                {
                    MoreAccessories.ArraySync(__instance);
                }
            }

            internal static void Postfix(ChaControl __instance)
            {
                var obj = __instance.objAccessory;
                var info = __instance.infoAccessory;
                var cusacscmp = __instance.cusAcsCmp;
                var objAcsMove = __instance.objAcsMove;
                for (int i = __instance.nowCoordinate.accessory.parts.Length, n = obj.Length; i < n; i++)
                {
                    if (obj[i])
                    {
                        __instance.SafeDestroy(obj[i]);
                        info[i] = null;
                        cusacscmp[i] = null;
                        for (var j = 0; j < 2; j++)
                        {
                            objAcsMove[i, j] = null;
                        }
                    }
                }
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
                    MoreAccessories.Print(__exception.ToString(), BepInEx.Logging.LogLevel.Error);
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
                MoreAccessories.Print($"ChaControl_CheckAdjuster_param_slot_1_Patches Method {count}");
#endif
                var instructionsList = instructions.ToList();
                var end = instructionsList.FindIndex(4, x => x.opcode == OpCodes.Brtrue); //work backwards from end

#if DEBUG
                var worked = false;
                if (end == -1)
                {
                    MoreAccessories.Print($"Opcode not found OpCodes.Brtrue_S", worked ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Error);
                    for (var i = 0; i < instructionsList.Count; i++)
                    {
                        MoreAccessories.Print($"{i:00} {instructionsList[i].opcode} {instructionsList[i].operand}");
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
                MoreAccessories.Print("Transpiler finished", worked ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Error);
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
                    MoreAccessories.Print(__exception.ToString(), BepInEx.Logging.LogLevel.Error);
                }
            }
#endif
            static IEnumerable<MethodBase> TargetMethods()
            {
                var list = new List<MethodBase>
                {
                    AccessTools.Method(typeof(ChaControl), nameof(ChaControl.UpdateAccessoryMoveAllFromInfo)),
#if KKS
                    AccessTools.Method(typeof(ChaControl), nameof(ChaControl.ChangeAccessory), new[] { typeof(bool), typeof(bool) }),
#elif KK || EC
                    AccessTools.Method(typeof(ChaControl), nameof(ChaControl.ChangeAccessory), new[] { typeof(bool) }),
#endif
                };
                return list;
            }

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
#if DEBUG
                MoreAccessories.Print($"{nameof(ChaControl_Replace_20_Patch)} Method {count++}");
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
                MoreAccessories.Print("Transpiler finished", worked ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Error);
#endif

            }

            private static int AccessoryCount(ChaControl chara)
            {
                return chara.nowCoordinate.accessory.parts.Length;
            }
        }

        [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.SetAccessoryState))]
        internal class SetAccessory_Patch
        {
            static bool Prefix(ChaControl __instance, int slotNo, bool show)
            {
                if (__instance.nowCoordinate.accessory.parts.Length <= slotNo)
                {
                    return false;
                }
                __instance.fileStatus.showAccessory[slotNo] = show;
                return false;
            }
        }

        [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.SetAccessoryStateAll))]
        internal class SetAccessoryStateAll_Patch
        {
            static bool Prefix(ChaControl __instance, bool show)
            {
                var length = __instance.nowCoordinate.accessory.parts.Length;
                for (var i = 0; i < length; i++)
                {
                    __instance.fileStatus.showAccessory[i] = show;
                }
                return false;
            }
        }

#if KK || KKS
        [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.SetAccessoryStateCategory))]
        internal class SetAccessoryStateCategoryPatch
        {
            static bool Prefix(ChaControl __instance, int cateNo, bool show)
            {
                if (cateNo != 0 && 1 != cateNo)
                {
                    return false;
                }
                var length = __instance.nowCoordinate.accessory.parts.Length;
                for (var i = 0; i < length; i++)
                {
                    if (__instance.nowCoordinate.accessory.parts[i].hideCategory == cateNo)
                    {
                        __instance.fileStatus.showAccessory[i] = show;
                    }
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.GetAccessoryCategoryCount))]
        internal class GetAccessoryCategoryCountPatch
        {
            static bool Prefix(ChaControl __instance, int cateNo, ref int __result)
            {
                if (cateNo != 0 && 1 != cateNo)
                {
                    __result = -1;
                    return false;
                }
                __result = 0;
                var length = __instance.nowCoordinate.accessory.parts.Length;

                for (var i = 0; i < length; i++)
                {
                    if (__instance.nowCoordinate.accessory.parts[i].hideCategory == cateNo)
                    {
                        __result++;
                    }
                }
                return false;
            }
        }
#endif


        [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.UpdateVisible))]
        internal class UpdateVisible_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
#if DEBUG
                MoreAccessories.Print($"{nameof(UpdateVisible_Patch)} Method");
                var worked = false;
#endif
                var instructionsList = instructions.ToList();
                //var end = instructionsList.FindLastIndex(x => x.opcode == OpCodes.Conv_I4);
                var end = instructionsList.Count - 1;

                var endcount = 0;
#if EC
                endcount = 1;
#endif

                var findcount = 0;
                for (; end > 0; end--)
                {
                    if (instructionsList[end].opcode == OpCodes.Conv_I4)
                    {
                        if (findcount++ == endcount)
                        {
                            break;
                        }
                    }
                }

                var start = end - 1;
                while (instructionsList[start].opcode != OpCodes.Call)
                {
                    start--;
                }
                for (var i = 0; i < instructionsList.Count; i++)
                {
                    var inst = instructionsList[i];
                    if (i == start)
                    {
#if DEBUG
                        worked = true;
#endif
                        //yield return instructionsList[start - 2];
                        yield return new CodeInstruction(OpCodes.Call, typeof(UpdateVisible_Patch).GetMethod(nameof(AccessoryCount), AccessTools.all));
                        i = end;
                        continue;
                    }
                    yield return inst;
                }

#if DEBUG
                MoreAccessories.Print("Transpiler finished", worked ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Error);
#endif
            }
            //private static void Print(int k)
            //{
            //    MoreAccessories.Print(k.ToString());
            //}
            private static int AccessoryCount(ChaControl chara/*, int k*/)
            {
                //MoreAccessories.Print($"{k:00}/{chara.objAccessory.Length}  {chara.nowCoordinate.accessory.parts.Length} {chara.fileStatus.showAccessory.Length}");
                var length = chara.nowCoordinate.accessory.parts.Length;
                //if (chara.fileStatus.showAccessory.Length <= length)
                //    MoreAccessories.ArraySync(chara);
                return length;
            }
        }
#if KK || KKS
        [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.AssignCoordinate), typeof(ChaFileDefine.CoordinateType))]
#elif EC
        [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.AssignCoordinate), new Type[0])]
#endif
        internal class AssignCoordinate_Patch
        {
            internal static void Prefix(ChaControl __instance)
            {
                MoreAccessories.ArraySync(__instance);
            }
        }
    }
}
