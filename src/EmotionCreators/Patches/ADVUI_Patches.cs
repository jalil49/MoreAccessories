using ADVPart.Manipulate.Chara;
using HarmonyLib;
using HarmonyLib;
using HEdit;
using HPlay;
using MoreAccessoriesKOI.Extensions;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Text;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAccessoriesKOI.Patches
{
    internal class ADVUI_Patches
    {
        [HarmonyPatch(typeof(AccessoryUICtrl), "Init")]
        internal static class AccessoryUICtrl_Init_Patches
        {
            private static void Postfix(AccessoryUICtrl __instance)
            {
                MoreAccessories.ADVMode = new ADVMode(__instance);
            }
        }

        [HarmonyPatch(typeof(AccessoryUICtrl), "UpdateUI")]
        internal static class AccessoryUICtrl_UpdateUI_Patches
        {
            private static void Postfix()
            {
                MoreAccessories.ADVMode.UpdateADVUI();
            }
        }
    }
}
