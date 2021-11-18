using HarmonyLib;
using System;
using System.Linq;

namespace MoreAccessoriesKOI.Patches
{
#if KK || KKS
    [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.GetCoordinateBytes), new Type[0])]
    internal class CharaSavePatch
    {
        [HarmonyPriority(Priority.First)]
        internal static void Prefix(ChaFile __instance, out ChaFileAccessory.PartsInfo[][] __state)
        {
            try
            {
                var coord = __instance.coordinate;
                __state = new ChaFileAccessory.PartsInfo[coord.Length][];
                for (int outfitnum = 0, n = coord.Length; outfitnum < n; outfitnum++)
                {
                    __state[outfitnum] = coord[outfitnum].accessory.parts;
                    if (__state[outfitnum].Length == 20) continue; //array is natural size don't do extra work

                    var lastvalidslot = Array.FindLastIndex(coord[outfitnum].accessory.parts, x => x.type != 120);
                    if (lastvalidslot < 20) lastvalidslot = 19; //Make sure to trim if list is completely empty thanks IDontHaveIdea for catching this

                    coord[outfitnum].accessory.parts = coord[outfitnum].accessory.parts.Take(lastvalidslot + 1).ToArray();
                }
            }
            catch (Exception ex)
            {
                __state = null;
                MoreAccessories.Print($"Error occurred while saving chafile coordinates {ex}", BepInEx.Logging.LogLevel.Fatal);
            }
        }

        [HarmonyPriority(Priority.First)]
        internal static void Postfix(ChaFile __instance, ChaFileAccessory.PartsInfo[][] __state)
        {
            if (__state == null) return;

            for (var i = 0; i < __state.Length; i++)
            {
                __instance.coordinate[i].accessory.parts = __state[i];
            }
        }
    }
#else
    [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.GetCoordinateBytes), new Type[0])]
    internal class SavePatch
    {
        [HarmonyPriority(Priority.First)]
        internal static void Prefix(ChaFile __instance, out ChaFileAccessory.PartsInfo[] __state)
        {
            try
            {
                var accessory = __instance.coordinate.accessory;
                if (accessory.parts.Length == 20)//Don't do extra work
                {
                    __state = null;
                    return;
                }

                __state = accessory.parts;
                if (__state.Length == 20) return;
                var lastvalidslot = Array.FindLastIndex(accessory.parts, x => x.type != 120);
                if (lastvalidslot < 20) lastvalidslot = 19;
                if (lastvalidslot + 1 == __state.Length) return;//don't do below since nothing changed
                accessory.parts = accessory.parts.Take(lastvalidslot + 1).ToArray();
            }
            catch (Exception ex)
            {
                __state = null;
                MoreAccessories.Print($"Error occurred while saving chafile coordinates {ex}", BepInEx.Logging.LogLevel.Fatal);
            }
        }

        [HarmonyPriority(Priority.First)]
        internal static void Postfix(ChaFile __instance, ChaFileAccessory.PartsInfo[] __state)
        {
            if (__state == null) return;

            __instance.coordinate.accessory.parts = __state;
        }
    }

#endif
    [HarmonyPatch(typeof(ChaFileCoordinate), nameof(ChaFileCoordinate.SaveFile))]
    internal class CoordSavePatch
    {
        [HarmonyPriority(Priority.First)]
        internal static void Prefix(ChaFileCoordinate __instance, out ChaFileAccessory.PartsInfo[] __state)
        {
            try
            {
                if (__instance.accessory.parts.Length == 20)//Don't do extra work
                {
                    __state = null;
                    return;
                }

                __state = __instance.accessory.parts.ToArray();

                var lastvalidslot = Array.FindLastIndex(__instance.accessory.parts, x => x.type != 120);
                if (lastvalidslot < 20) lastvalidslot = 19;
                if (lastvalidslot + 1 == __state.Length) return;//don't do below since nothing changed
                __instance.accessory.parts = __instance.accessory.parts.Take(lastvalidslot + 1).ToArray();
            }
            catch (Exception ex)
            {
                MoreAccessories.Print($"Error occurred while saving coordinate {ex}", BepInEx.Logging.LogLevel.Fatal);
                __state = null;
            }
        }

        [HarmonyPriority(Priority.First)]
        internal static void Postfix(ChaFileCoordinate __instance, ChaFileAccessory.PartsInfo[] __state)
        {
            if (__state == null) return;

            __instance.accessory.parts = __state;
        }
    }

    [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.GetStatusBytes), typeof(ChaFileStatus))]
    internal static class ChaFileStatusPatch
    {
        [HarmonyPriority(Priority.First)]
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
        private static void Postfix(ChaFileStatus _status, bool[] __state)
        {
            if (__state != null)
            {
                _status.showAccessory = __state;
            }
            Common_Patches.Seal(true);
        }
    }
}
