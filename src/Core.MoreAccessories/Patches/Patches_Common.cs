using HarmonyLib;
using UnityEngine;

namespace MoreAccessoriesKOI.Patches
{
    public class Common_Patches
    {
        #region Greedy Code
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
                if (__instance.showAccessory != null && seal && value.Length != __instance.showAccessory.Length)
                {
                    MoreAccessories.Print($"Please do not try to change showAccessory array size outside of MoreAccessories.ArraySync ", BepInEx.Logging.LogLevel.Warning);
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
                if (__instance.cusAcsCmp != null && seal && value.Length != __instance.cusAcsCmp.Length)
                {
                    MoreAccessories.Print($"Please do not try to change cusAcsCmp array size outside of MoreAccessories.ArraySync ", BepInEx.Logging.LogLevel.Warning);
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
                if (__instance.objAccessory != null && seal && value.Length != __instance.objAccessory.Length)
                {
                    MoreAccessories.Print($"Please do not try to change objAccessory array size outside of MoreAccessories.ArraySync ", BepInEx.Logging.LogLevel.Warning);
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
                if (__instance.objAcsMove != null && seal && value.Length != __instance.objAcsMove.Length)
                {
                    MoreAccessories.Print($"Please do not try to change objAcsMove array size outside of MoreAccessories.ArraySync ", BepInEx.Logging.LogLevel.Warning);
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
                if (__instance.infoAccessory != null && seal && value.Length != __instance.infoAccessory.Length)
                {
                    MoreAccessories.Print($"Please do not try to change infoAccessory array size outside of MoreAccessories.ArraySync ", BepInEx.Logging.LogLevel.Warning);
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
                if (__instance.hideHairAcs != null && seal && value.Length != __instance.hideHairAcs.Length)
                {
                    MoreAccessories.Print($"Please do not try to change hideHairAcs array size outside of MoreAccessories.ArraySync ", BepInEx.Logging.LogLevel.Warning);
                    return false;
                }
                return true;
            }
        }
        #endregion
    }
}
