#if !EC
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
namespace MoreAccessoriesKOI.Patches.MainGame
{
    public static class H_Patches
    {
        [HarmonyPatch(typeof(HSprite), nameof(HSprite.Update))]
        internal static class Replace_20_Patch
        {
#if DEBUG
            static int current = 0;

            static void Finalizer(Exception __exception)
            {
                current %= 300;
                if (__exception != null && current == 0)
                {
                    MoreAccessories.LogSource.LogError($"Hsprite Update" + __exception);
                }
                current++;
            }
#endif

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var instructionsList = instructions.ToList();
                var i = 0;
                for (; i < instructionsList.Count; i++)
                {
                    var inst = instructionsList[i];
                    if (inst.opcode == OpCodes.Ldc_I4_S && inst.operand.ToString() == "20")//female list
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);

                        yield return new CodeInstruction(OpCodes.Ldfld, typeof(HSprite).GetField(nameof(HSprite.females), AccessTools.all));

                        yield return new CodeInstruction(OpCodes.Ldloc_1);

                        yield return new CodeInstruction(OpCodes.Call, typeof(Replace_20_Patch).GetMethod(nameof(FemaleAccessoryCount), AccessTools.all));
                        i++;
                        break;
                    }
                    yield return inst;
                }

                for (; i < instructionsList.Count; i++)//male
                {
                    var inst = instructionsList[i];
                    if (inst.opcode == OpCodes.Ldc_I4_S && inst.operand.ToString() == "20")
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);

                        yield return new CodeInstruction(OpCodes.Ldfld, typeof(HSprite).GetField(nameof(HSprite.male), AccessTools.all));

                        yield return new CodeInstruction(OpCodes.Call, typeof(Replace_20_Patch).GetMethod(nameof(MaleAccessoryCount), AccessTools.all));
                        continue;
                    }
                    yield return inst;
                }
            }

            private static int FemaleAccessoryCount(List<ChaControl> femaleList, int current)
            {
                return femaleList[current].nowCoordinate.accessory.parts.Length;
            }

            private static int MaleAccessoryCount(ChaControl male)
            {
                return male.nowCoordinate.accessory.parts.Length;
            }
        }


        [HarmonyPatch(typeof(HSceneProc), nameof(HSceneProc.Start))]
        internal static class HSceneProc_Start_Patches
        {
            private static void Postfix(List<ChaControl> ___lstFemale, HSprite ___sprite)
            {
                MoreAccessories._self.HMode = new HScene(___lstFemale, ___sprite);
            }
        }
    }
}
#endif