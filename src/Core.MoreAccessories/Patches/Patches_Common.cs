using HarmonyLib;
using Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoreAccessoriesKOI.Patches
{
    public class Common_Patches
    {
        //
        // Please do not adjust externally, please just call arraysync after adjusting nowcoordinate.accessory.parts to desired size
        //
        // Greedy code stop others from adjusting array sizes unexpectedly and breaking sync
        #region Greedy Code

#if KK || EC
        private static List<ChaControl> GetChaControls()
        {
            if (Character.instance)
                return Character.instance.dictEntryChara.Values.ToList();
            return null;
        }
#elif KKS
        private static List<ChaControl> GetChaControls()
        {
            return Character.ChaControls;
        }
#endif
        /// <summary>
        /// Please do not adjust externally, please just call arraysync after adjusting nowcoordinate.accessory.parts to desired size
        /// </summary>
        /// <param name="value"></param>
        internal static void Seal(bool value)
        {
            ShowAccessorySetterPatch.seal = value;
            CusAcsCmpSetterPatch.seal = value;
            ObjAccessorySetterPatch.seal = value;
            ObjAcsMoveSetterPatch.seal = value;
            InfoAccessorySetterPatch.seal = value;
            HideHairAcsSetterPatch.seal = value;
        }

        [HarmonyPatch(typeof(ChaFileStatus), nameof(ChaFileStatus.showAccessory), MethodType.Setter)]
        internal class ShowAccessorySetterPatch
        {
            internal static bool seal = true;
            internal static bool Prefix(ChaFileStatus __instance, bool[] value)
            {
                var control = GetChaControls()?.FirstOrDefault(x => x.fileStatus == __instance);
                if (control != null && __instance.showAccessory != null && seal && value.Length != control.nowCoordinate.accessory.parts.Length)
                {
                    MoreAccessories.Print($"Do not change showAccessory array size {System.Environment.StackTrace}", BepInEx.Logging.LogLevel.Warning);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ChaInfo), nameof(ChaInfo.cusAcsCmp), MethodType.Setter)]
        internal class CusAcsCmpSetterPatch
        {
            internal static bool seal = true;
            internal static bool Prefix(ChaInfo __instance, ChaAccessoryComponent[] value)
            {
                var control = GetChaControls()?.FirstOrDefault(x => x == __instance);
                if (control != null && __instance.cusClothesCmp != null && seal && value.Length != control.nowCoordinate.accessory.parts.Length)
                {
                    MoreAccessories.Print($"Do not change cusAcsCmp array size {System.Environment.StackTrace}", BepInEx.Logging.LogLevel.Warning);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ChaInfo), nameof(ChaInfo.objAccessory), MethodType.Setter)]
        internal class ObjAccessorySetterPatch
        {
            internal static bool seal = true;
            internal static bool Prefix(ChaInfo __instance, ChaAccessoryComponent[] value)
            {
                var control = GetChaControls()?.FirstOrDefault(x => x == __instance);
                if (control != null && __instance.objAccessory != null && seal && value.Length != control.nowCoordinate.accessory.parts.Length)
                {
                    MoreAccessories.Print($"Do not change objAccessory array size {System.Environment.StackTrace}", BepInEx.Logging.LogLevel.Warning);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ChaInfo), nameof(ChaInfo.objAcsMove), MethodType.Setter)]
        internal class ObjAcsMoveSetterPatch
        {
            internal static bool seal = true;
            internal static bool Prefix(ChaInfo __instance, GameObject[,] value)
            {
                var control = GetChaControls()?.FirstOrDefault(x => x == __instance);
                if (control != null && __instance.objAcsMove != null && seal && value.Length != control.nowCoordinate.accessory.parts.Length)
                {
                    MoreAccessories.Print($"Do not change objAcsMove array size {System.Environment.StackTrace}", BepInEx.Logging.LogLevel.Warning);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ChaInfo), nameof(ChaInfo.infoAccessory), MethodType.Setter)]
        internal class InfoAccessorySetterPatch
        {
            internal static bool seal = true;
            internal static bool Prefix(ChaInfo __instance, ListInfoBase[] value)
            {
                var control = GetChaControls()?.FirstOrDefault(x => x == __instance);
                if (control != null && __instance.infoAccessory != null && seal && value.Length != control.nowCoordinate.accessory.parts.Length)
                {
                    MoreAccessories.Print($"Do not change infoAccessory array size {System.Environment.StackTrace}", BepInEx.Logging.LogLevel.Warning);
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.hideHairAcs), MethodType.Setter)]
        internal class HideHairAcsSetterPatch
        {
            internal static bool seal = true;
            internal static bool Prefix(ChaControl __instance, bool[] value)
            {
                if (__instance.hideHairAcs != null && seal && value.Length != __instance.nowCoordinate.accessory.parts.Length)
                {
                    MoreAccessories.Print($"Please do not try to change hideHairAcs array size outside of MoreAccessories.ArraySync {System.Environment.StackTrace}", BepInEx.Logging.LogLevel.Warning);
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
