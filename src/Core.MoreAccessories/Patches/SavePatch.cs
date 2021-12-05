using HarmonyLib;
using System;
using System.Linq;

namespace MoreAccessoriesKOI.Patches
{
#if KK || KKS
    /// <summary>
    /// Trim slot length to last slot that is not empty or 20 length
    /// </summary>
    [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.GetCoordinateBytes), new Type[0])]
    internal class CharaSavePatch
    {
        [HarmonyPriority(Priority.Last)]
        private static void Prefix(ChaFile __instance)
        {
            try
            {
                var coord = __instance.coordinate;
                for (int outfitnum = 0, n = coord.Length; outfitnum < n; outfitnum++)
                {
                    if (coord[outfitnum].accessory.parts.Length == 20) continue; //array is natural size don't do extra work

                    var lastvalidslot = Array.FindLastIndex(coord[outfitnum].accessory.parts, x => x.type != 120) + 1;
                    if (lastvalidslot < 20) lastvalidslot = 20; //Make sure to trim if list is completely empty thanks IDontHaveIdea for catching this

                    coord[outfitnum].accessory.parts = coord[outfitnum].accessory.parts.Take(lastvalidslot).ToArray();
                }
            }
            catch (Exception ex)
            {
                MoreAccessories.Print($"Error occurred while saving chafile coordinates {ex}", BepInEx.Logging.LogLevel.Fatal);
            }
        }

        [HarmonyPriority(Priority.First)]
        private static void Postfix(ChaFile __instance) => MoreAccessories.NowCoordinateTrimAndSync(Common_Patches.GetChaControls().FirstOrDefault(x => __instance == x.chaFile));
    }
#else
    [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.GetCoordinateBytes), new Type[0])]
    internal class SavePatch
    {
        [HarmonyPriority(Priority.Last)]
        private static void Prefix(ChaFile __instance)
        {
            try
            {
                var accessory = __instance.coordinate.accessory;
                if (accessory.parts.Length == 20)//Don't do extra work
                {
                    return;
                }

                var lastvalidslot = Array.FindLastIndex(accessory.parts, x => x.type != 120) + 1;
                if (lastvalidslot < 20) lastvalidslot = 20;
                if (lastvalidslot == accessory.parts.Length) return;//don't do below since nothing changed
                accessory.parts = accessory.parts.Take(lastvalidslot).ToArray();
            }
            catch (Exception ex)
            {
                MoreAccessories.Print($"Error occurred while saving chafile coordinates {ex}", BepInEx.Logging.LogLevel.Fatal);
            }
        }

        [HarmonyPriority(Priority.First)]
        private static void Postfix(ChaFile __instance) => MoreAccessories.NowCoordinateTrimAndSync(Common_Patches.GetChaControls().FirstOrDefault(x => __instance == x.chaFile));
    }

#endif
    [HarmonyPatch(typeof(ChaFileCoordinate), nameof(ChaFileCoordinate.SaveFile))]
    internal class CoordSavePatch
    {
        [HarmonyPriority(Priority.Last)]
        private static void Prefix(ChaFileCoordinate __instance)
        {
            try
            {
                if (__instance.accessory.parts.Length == 20)//Don't do extra work
                {
                    return;
                }


                var lastvalidslot = Array.FindLastIndex(__instance.accessory.parts, x => x.type != 120) + 1;
                if (lastvalidslot < 20) lastvalidslot = 20;
                if (lastvalidslot == __instance.accessory.parts.Length) return;//don't do below since nothing changed
                __instance.accessory.parts = __instance.accessory.parts.Take(lastvalidslot).ToArray();
            }
            catch (Exception ex)
            {
                MoreAccessories.Print($"Error occurred while saving coordinate {ex}", BepInEx.Logging.LogLevel.Fatal);
            }
        }

        [HarmonyPriority(Priority.First)]
        private static void Postfix(ChaFileCoordinate __instance) => MoreAccessories.NowCoordinateTrimAndSync(Common_Patches.GetChaControls().FirstOrDefault(x => __instance == x.nowCoordinate));
    }

    //Don't save with manipulated showAccessory array. Will cause issues when plugin is not installed or when used with moreaccessories by joan
    [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.GetStatusBytes), typeof(ChaFileStatus))]
    internal static class ChaFileStatusPatch
    {
        [HarmonyPriority(Priority.Last)]
        private static void Prefix(ChaFileStatus _status, out bool[] __state)
        {
            Common_Patches.Seal(false);
            try
            {
                __state = _status.showAccessory;
                _status.showAccessory = __state.Take(20).ToArray();
            }
            catch (Exception ex)
            {
                MoreAccessories.Print($"Error occurred while saving chafile coordinates {ex}", BepInEx.Logging.LogLevel.Fatal);
                __state = null;
            }
        }

        [HarmonyPriority(Priority.First)]
        private static void Postfix(ChaFile __instance, ChaFileStatus _status, bool[] __state)
        {
            if (__state != null)
            {
                _status.showAccessory = __state;
            }
            MoreAccessories.NowCoordinateTrimAndSync(Common_Patches.GetChaControls().FirstOrDefault(x => __instance == x.chaFile));
            Common_Patches.Seal(true);
        }
    }
}
