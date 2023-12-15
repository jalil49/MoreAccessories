#if KK || KKS
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;

namespace MoreAccessoriesKOI.Patches
{
    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Harmony Patches - Used Externally")]
    [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Harmony Patches - Used Externally")]
    internal class VR_Patches
    {
        [HarmonyPatch]
        private static class VrHSceneStart_Patches
        {
            private static bool Prepare()
            {
                return Type.GetType("VRHScene,Assembly-CSharp.dll") != null;
            }

            private static MethodInfo TargetMethod()
            {
                return Type.GetType("VRHScene,Assembly-CSharp.dll")?.GetMethod("Start", AccessTools.all);
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
#endif