using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ChaCustom;
using HarmonyLib;

namespace MoreAccessoriesKOI.Patches.Maker
{
    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Harmony Patches - Used Externally")]
    [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Harmony Patches - Used Externally")]
    public static class CvsAccessory_Patches
    {
        [HarmonyPatch(typeof(CustomBase), nameof(CustomBase.SetUpdateCvsAccessory))]
        private static class FixScroll_Patch
        {
            private static void Postfix()
            {
                MoreAccessories.MakerMode?.AccessoriesWindow?.FixWindowScroll();
            }
        }

        [HarmonyPatch]
        private static class CustomAcsChangeSlot_Patch
        {
            private static MethodBase TargetMethod()
            {
#if KKS
                return AccessTools.Method(AccessTools.TypeByName("ChaCustom.CustomAcsChangeSlot+<>c__DisplayClass18_1, Assembly-CSharp"), "<Initialize>b__4");
#elif KK || EC
                return AccessTools.Method(AccessTools.TypeByName("ChaCustom.CustomAcsChangeSlot+<Start>c__AnonStorey0+<Start>c__AnonStorey1, Assembly-CSharp"), "<>m__0"); //insert disgust sounds
#endif
            }

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var worked = false;

                var instructionsList = instructions.ToList();

                foreach (var inst in instructionsList)
                {
                    if (inst.opcode == OpCodes.Ldc_I4_S)
                    {
                        switch (inst.operand.ToString()) //pop after the first replacement as the subsequent are labelled
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
                                yield return inst;
                                yield return new CodeInstruction(OpCodes.Pop);
                                yield return new CodeInstruction(OpCodes.Call, typeof(CustomAcsChangeSlot_Patch).GetMethod(nameof(ChangeButton), AccessTools.all));
                                continue;
                        }
                    }

                    yield return inst;
                }
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

        /// <summary>
        /// Crashes in KKS if info accessory is not found and type is not 120
        /// Fails to update slot name in KK if info accessory is not found and type is not 120
        /// </summary>
        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.UpdateSlotName))]
        private static class UpdateSlotNamePatch
        {
            [HarmonyPriority(Priority.Last)]
            private static bool Prefix(CvsAccessory __instance, out int __state)
            {
                var invalid = __instance.accessory.parts.Length <= __instance.nSlotNo;
                if (invalid)
                {
                    __state = 120;
                    return false;
                }

                __state = __instance.accessory.parts[__instance.nSlotNo].type;
                if (__instance.chaCtrl.infoAccessory[__instance.nSlotNo] == null)
                {
                    __instance.accessory.parts[__instance.nSlotNo].type = 120;
                }

                return true;
            }

            [HarmonyPriority(Priority.First)]
            private static void Postfix(CvsAccessory __instance, int __state)
            {
                if (__state == 120) return;
                __instance.accessory.parts[__instance.nSlotNo].type = __state;
            }
        }
    }
}