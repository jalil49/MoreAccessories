using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
#pragma warning disable IDE0051 // Remove unused private members

namespace MoreAccessoriesKOI.Patches
{
    internal class VR_Patches
    {
        [HarmonyPatch]
        private static class VRHScene_Start_Patches
        {
            private static bool Prepare()
            {
                return Type.GetType("VRHScene,Assembly-CSharp.dll") != null;
            }

            private static MethodInfo TargetMethod()
            {
                return Type.GetType("VRHScene,Assembly-CSharp.dll").GetMethod("Start", AccessTools.all);
            }
#if KK
            private static void Postfix(List<ChaControl> ___lstFemale, HSprite[] ___sprites)
            {
                MoreAccessories.HMode = new HScene(___lstFemale, ___sprites);
            }
#elif KKS
            private static void Postfix(List<ChaControl> ___lstFemale, HSprite ___sprite)
            {
                MoreAccessories.HMode = new HScene(___lstFemale, new[] { ___sprite });
            }
#endif
        }
    }
}