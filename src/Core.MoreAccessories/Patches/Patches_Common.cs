using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BepInEx.Logging;
using HarmonyLib;
using Manager;
using UnityEngine;
#if KK || EC
using System.Linq;
#endif

namespace MoreAccessoriesKOI.Patches
{
    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Harmony Patches - Used Externally")]
    [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Harmony Patches - Used Externally")]
    internal class Common_Patches
    {
        // Please do not adjust externally, please just call array sync after adjusting accessory.parts to desired size
        //
        // Greedy code stop others from adjusting array sizes unexpectedly and breaking sync

        #region Greedy Code

        internal static List<ChaControl> GetChaControls()
        {
#if KK || EC
            return Character.instance ? Character.instance.dictEntryChara.Values.ToList() : null;
#elif KKS
            return Character.ChaControls;
#endif
        }

        /// <summary>
        /// Please do not adjust externally, please just call array sync after adjusting accessory.parts to desired size
        /// </summary>
        /// <param name="value">stops modification if true</param>
        internal static void Seal(bool value)
        {
            ShowAccessorySetter_Patch.SealPatch = value;
            CusAcsCmpSetter_Patch.SealPatch = value;
            ObjAccessorySetter_Patch.SealPatch = value;
            ObjAcsMoveSetter_Patch.SealPatch = value;
            InfoAccessorySetter_Patch.SealPatch = value;
            HideHairAcsSetter_Patch.SealPatch = value;
        }

        [HarmonyPatch(typeof(ChaFileStatus), nameof(ChaFileStatus.showAccessory), MethodType.Setter)]
        internal class ShowAccessorySetter_Patch
        {
            internal static bool SealPatch = true;

            private static bool Prefix(ChaFileStatus __instance, bool[] value)
            {
                if (!SealPatch) return true;

                ChaControl control = null;
                foreach (var x in GetChaControls())
                {
                    if (x.fileStatus == __instance)
                    {
                        control = x;
                        break;
                    }
                }

                if (control != null && __instance.showAccessory != null && control.nowCoordinate != null && value.Length != control.nowCoordinate.accessory.parts.Length)
                {
                    MoreAccessories.Print($"Do not change showAccessory array size {Environment.StackTrace}", LogLevel.Warning);
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(ChaInfo), nameof(ChaInfo.cusAcsCmp), MethodType.Setter)]
        internal class CusAcsCmpSetter_Patch
        {
            internal static bool SealPatch = true;

            private static bool Prefix(ChaInfo __instance, ChaAccessoryComponent[] value)
            {
                if (!SealPatch) return true;

                var control = __instance as ChaControl;

                if (control != null && __instance.cusClothesCmp != null && control.nowCoordinate != null && value.Length != control.nowCoordinate.accessory.parts.Length)
                {
                    MoreAccessories.Print($"Do not change cusAcsCmp array size {Environment.StackTrace}", LogLevel.Warning);
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(ChaInfo), nameof(ChaInfo.objAccessory), MethodType.Setter)]
        internal class ObjAccessorySetter_Patch
        {
            internal static bool SealPatch = true;

            private static bool Prefix(ChaInfo __instance, ChaAccessoryComponent[] value)
            {
                if (!SealPatch) return true;

                var control = __instance as ChaControl;

                if (control != null && __instance.objAccessory != null && control.nowCoordinate != null && value.Length != control.nowCoordinate.accessory.parts.Length)
                {
                    MoreAccessories.Print($"Do not change objAccessory array size {Environment.StackTrace}", LogLevel.Warning);
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(ChaInfo), nameof(ChaInfo.objAcsMove), MethodType.Setter)]
        internal class ObjAcsMoveSetter_Patch
        {
            internal static bool SealPatch = true;

            private static bool Prefix(ChaInfo __instance, GameObject[,] value)
            {
                if (!SealPatch) return true;

                var control = __instance as ChaControl;

                if (control != null && __instance.objAcsMove != null && control.nowCoordinate != null && value.Length != control.nowCoordinate.accessory.parts.Length)
                {
                    MoreAccessories.Print($"Do not change objAcsMove array size {Environment.StackTrace}", LogLevel.Warning);
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(ChaInfo), nameof(ChaInfo.infoAccessory), MethodType.Setter)]
        internal class InfoAccessorySetter_Patch
        {
            internal static bool SealPatch = true;

            private static bool Prefix(ChaInfo __instance, ListInfoBase[] value)
            {
                if (!SealPatch) return true;

                var control = __instance as ChaControl;

                if (control != null && __instance.infoAccessory != null && control.nowCoordinate != null && value.Length != control.nowCoordinate.accessory.parts.Length)
                {
                    MoreAccessories.Print($"Do not change infoAccessory array size {Environment.StackTrace}", LogLevel.Warning);
                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.hideHairAcs), MethodType.Setter)]
        internal class HideHairAcsSetter_Patch
        {
            internal static bool SealPatch = true;

            private static bool Prefix(ChaControl __instance, bool[] value)
            {
                if (!SealPatch) return true;

                if (__instance.hideHairAcs != null && __instance.nowCoordinate != null && value.Length != __instance.nowCoordinate.accessory.parts.Length)
                {
                    MoreAccessories.Print($"Please do not try to change hideHairAcs array size outside of MoreAccessories.ArraySync {Environment.StackTrace}", LogLevel.Warning);
                    return false;
                }

                return true;
            }
        }

        #endregion

        //native code that triggers greed allow it to bypass
        //Let this do its thing, not an issue... so far
        [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.ReleaseObject))]
        internal class ReleaseObjectPatch
        {
            private static void Prefix() => Seal(false);
            private static void Postfix() => Seal(true);
        }
    }
}