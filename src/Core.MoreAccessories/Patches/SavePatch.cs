using HarmonyLib;
using System.IO;
using System.Linq;

namespace MoreAccessoriesKOI.Patches
{
    internal class SavePatch
    {
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.SaveFile), typeof(BinaryWriter), typeof(bool))]
        internal static void Chafile_Save_Patch(ChaFile __instance)
        {
            var coord = __instance.coordinate;
            for (int outfitnum = 0, n = coord.Length; outfitnum < n; outfitnum++)
            {
                var accessories = coord[outfitnum].accessory.parts.ToList();
                for (var slot = accessories.Count - 1; accessories.Count > 20; slot++)
                {
                    if (accessories[slot].type != 120) break;
                    accessories.RemoveAt(slot);
                }
                coord[outfitnum].accessory.parts = accessories.ToArray();
            }
        }
    }
}
