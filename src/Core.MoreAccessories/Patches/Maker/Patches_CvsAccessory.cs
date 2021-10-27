using ChaCustom;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UniRx;

namespace MoreAccessoriesKOI.Patches.Maker
{
    public static class CvsAccessory_Patches
    {
        [HarmonyPatch(typeof(CustomBase), nameof(CustomBase.SetUpdateCvsAccessory))]
        private static class CvsAccessory_UpdateSlotName_Patches
        {
            private static void Postfix()
            {
                MoreAccessories.MakerMode?.AccessoriesWindow?.FixWindowScroll();
            }
        }

        [HarmonyPatch]
        internal static class CustomAcsChangeSlot_Patch
        {
            static MethodBase TargetMethod()
            {
                MethodBase methodbase;
#if KKS
                methodbase = AccessTools.Method(AccessTools.TypeByName("ChaCustom.CustomAcsChangeSlot+<>c__DisplayClass18_1, Assembly-CSharp"), "<Initialize>b__4");
#elif KK || EC
                methodbase = AccessTools.Method(AccessTools.TypeByName("ChaCustom.CustomAcsChangeSlot+<Start>c__AnonStorey0+<Start>c__AnonStorey1, Assembly-CSharp"), "<>m__0");//insert disgust sounds
#endif
                return methodbase;
            }

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
#if DEBUG
                MoreAccessories.Print($"{nameof(CustomAcsChangeSlot_Patch)} Method");
                var worked2 = false;
#if EC
                worked2 = true;
#endif
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
                MoreAccessories.Print("Transpiler finished", worked && worked2 ? BepInEx.Logging.LogLevel.Warning : BepInEx.Logging.LogLevel.Error);
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

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.UpdateSlotName))]
        internal static class UpdateSlotNamePatch
        {
            internal static void Prefix(CvsAccessory __instance, out int __state)
            {
                __state = __instance.accessory.parts[__instance.nSlotNo].type;
                if (__instance.chaCtrl.infoAccessory[__instance.nSlotNo] == null)
                {
                    __instance.accessory.parts[__instance.nSlotNo].type = 120;
                }
            }

            internal static void Postfix(CvsAccessory __instance, int __state)
            {
                __instance.accessory.parts[__instance.nSlotNo].type = __state;
            }
        }
    }
}
