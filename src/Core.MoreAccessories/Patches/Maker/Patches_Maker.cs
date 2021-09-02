using ChaCustom;
using HarmonyLib;
using Illusion.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoreAccessoriesKOI.Patches.Maker
{
    [HarmonyPatch]
    internal static class Maker_Replace_20_Patch
    {
#if DEBUG
        static int count = 0;
        static Exception Finalizer(Exception __exception)
        {
            if (__exception != null)
            {
                MoreAccessories.LogSource.LogError(__exception);
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
                    yield return new CodeInstruction(OpCodes.Call, typeof(Maker_Replace_20_Patch).GetMethod(nameof(AccessoryCount), AccessTools.all));
                    continue;
                }
                yield return inst;
            }
#if DEBUG
            MoreAccessories.LogSource.Log(work ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Error, $"transpiler {count++} finished");
#endif
        }

        private static int AccessoryCount()//works fine for copybutton since it is equal
        {
            return CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length;
        }
    }
#if DEBUG //Seperate Updates from other patches
    [HarmonyPatch]
    internal static class Maker_Replace_20_Patch_2
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
                MoreAccessories.LogSource.LogError(__exception);
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
                    var test = new CodeInstruction(OpCodes.Call, typeof(Maker_Replace_20_Patch_2).GetMethod(nameof(AccessoryCount), AccessTools.all));

                    yield return new CodeInstruction(OpCodes.Call, typeof(Maker_Replace_20_Patch_2).GetMethod(nameof(AccessoryCount), AccessTools.all));
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
}
