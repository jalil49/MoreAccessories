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
using UnityEngine.UI;
using Illusion.Extensions;
#if DEBUG

#endif
#if KKS
using Cysharp.Threading.Tasks;

#endif
using UniRx.Triggers;


namespace MoreAccessoriesKOI.Patches.Maker
{
    public static class CvsAccessory_Patches
    {
        [HarmonyPatch(typeof(CustomBase), nameof(CustomBase.SetUpdateCvsAccessory))]
        private static class CvsAccessory_UpdateSlotName_Patches
        {
            private static void Postfix()
            {
                MoreAccessories._self.MakerMode.AccessoriesWindow.FixWindowScroll();
            }
        }


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
                MoreAccessories.LogSource.LogWarning($"{nameof(CustomAcsChangeSlot_Patch)} Method");
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
                    //MoreAccessories._self.Logger.LogWarning($"{j:00} {instructionsList[i].opcode} {instructionsList[i].operand}");
                    yield return inst;
                }

#if DEBUG
                MoreAccessories.LogSource.Log(worked && worked2 ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Error, "Transpiler finished");
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
    }
}
