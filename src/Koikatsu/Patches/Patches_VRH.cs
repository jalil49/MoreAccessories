using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MoreAccessoriesKOI.Patches
{
    public class VR_Patches
    {
        [HarmonyPatch]
        internal static class VRHScene_Start_Patches
        {
            internal static bool Prepare()
            {
                return Type.GetType("VRHScene,Assembly-CSharp.dll") != null;
            }

            internal static MethodInfo TargetMethod()
            {
                return Type.GetType("VRHScene,Assembly-CSharp.dll").GetMethod("Start", AccessTools.all);
            }
#if KK
            internal static void Postfix(List<ChaControl> ___lstFemale, HSprite[] ___sprites)
            {
                MoreAccessories.HMode = new HScene(___lstFemale, ___sprites);
            }
#elif KKS
            internal static void Postfix(List<ChaControl> ___lstFemale, HSprite ___sprite)
            {
                MoreAccessories.HMode = new HScene(___lstFemale, new[] { ___sprite });
            }
#endif
        }
    }
}