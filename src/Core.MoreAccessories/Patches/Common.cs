using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {

        //remove if (!MathfEx.RangeEqualOn<int>(0, slotNo, 19)) return; checks
        public static IEnumerable<CodeInstruction> AccessoryCheckTranspiler(IEnumerable<CodeInstruction> instructions, int parameter = 0)
        {
            var instructionsList = instructions.ToList();
            var i = 0;
            while (instructionsList[i++].opcode != OpCodes.Ret) { }
            _self.Logger.LogWarning($"exited at line {i}");

            yield return new CodeInstruction(OpCodes.Ldarga_S, parameter);//female count
            yield return new CodeInstruction(OpCodes.Call, typeof(MoreAccessories).GetMethod(nameof(AccessoryCheck), BindingFlags.NonPublic | BindingFlags.Static));
            yield return new CodeInstruction(OpCodes.Brtrue_S);
            yield return new CodeInstruction(OpCodes.Ret);


            for (; i < instructionsList.Count; i++)
            {
                var inst = instructionsList[i];

                yield return inst;
            }
        }

        private static bool AccessoryCheck(ChaControl chara, int slot)
        {
            return MathfEx.RangeEqualOn(0, slot, chara.nowCoordinate.accessory.parts.Length);
        }

    }
}
