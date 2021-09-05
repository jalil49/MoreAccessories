using HarmonyLib;
using System;
using System.Linq;

namespace MoreAccessoriesKOI.Patches
{
#if KK || KKS
    [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.GetCoordinateBytes), new Type[0])]
    internal class SavePatch
    {
        [HarmonyPriority(Priority.First)]
        internal static void Prefix(ChaFile __instance, out ChaFileCoordinate[] __state)
        {
            MoreAccessories.Print("Prefixing Save");
            var coord = __instance.coordinate;
            __state = coord.ToArray();
            for (int outfitnum = 0, n = coord.Length; outfitnum < n; outfitnum++)
            {
                var accessories = coord[outfitnum].accessory.parts.ToList();
                for (var slot = accessories.Count - 1; accessories.Count > 20; slot--)
                {
                    if (accessories[slot].type != 120) break;
                    accessories.RemoveAt(slot);
                }
                coord[outfitnum].accessory.parts = accessories.ToArray();
            }
        }

        [HarmonyPriority(Priority.First)]
        internal static void Postfix(ChaFile __instance, ChaFileCoordinate[] __state)
        {
            MoreAccessories.Print("Postfix Save");

            __instance.coordinate = __state;
        }
    }
#else
    [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.GetCoordinateBytes), new Type[0])]
    internal class SavePatch
    {
        [HarmonyPriority(Priority.First)]
        internal static void Prefix(ChaFile __instance, out int __state)
        {
            var coord = __instance.coordinate;
            __state = coord.accessory.parts.Length;
            var accessories = coord.accessory.parts.ToList();
            for (var slot = accessories.Count - 1; accessories.Count > 20; slot--)
            {
                if (accessories[slot].type != 120) break;
                accessories.RemoveAt(slot);
            }
            coord.accessory.parts = accessories.ToArray();

        }

        [HarmonyPriority(Priority.First)]
        internal static void Postfix(ChaFile __instance, int __state)
        {
            var delta = __state - __instance.coordinate.accessory.parts.Length;
            if (delta > 0)
            {
                var newarray = new ChaFileAccessory.PartsInfo[delta];
                for (var i = 0; i < delta; i++)
                {
                    newarray[i] = new ChaFileAccessory.PartsInfo();
                }
                __instance.coordinate.accessory.parts = __instance.coordinate.accessory.parts.Concat(newarray).ToArray();
            }
        }
    }

#endif
}
