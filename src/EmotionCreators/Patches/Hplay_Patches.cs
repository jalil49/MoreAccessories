using HarmonyLib;
using HPlay;
using MoreAccessoriesKOI.Extensions;
using UnityEngine.UI;

namespace MoreAccessoriesKOI.Patches
{
    internal class Hplay_Patches
    {
        [HarmonyPatch(typeof(HPlayHPartAccessoryCategoryUI), nameof(HPlayHPartAccessoryCategoryUI.Start))]
        internal static class HPlayHPartAccessoryCategoryUI_Start_Postfix
        {
            internal static void Postfix(HPlayHPartAccessoryCategoryUI __instance)
            {
                MoreAccessories.PlayMode = new PlayMode(__instance);
            }
        }

        [HarmonyPatch(typeof(HPlayHPartAccessoryCategoryUI), nameof(HPlayHPartAccessoryCategoryUI.Init))]
        internal static class HPlayHPartAccessoryCategoryUI_Init_Postfix
        {
            internal static void Postfix()
            {
                MoreAccessories._self.ExecuteDelayed(MoreAccessories.PlayMode.UpdatePlayUI, 2);
            }
        }

        //[HarmonyPatch(typeof(HPlayHPartClothMenuUI), "Init")]
        //internal static class HPlayHPartClothMenuUI_Init_Postfix
        //{
        //    internal static void Postfix(HPlayHPartClothMenuUI __instance, Button[] ___btnClothMenus)
        //    {
        //        ___btnClothMenus[1].gameObject.SetActive(___btnClothMenus[1].gameObject.activeSelf /*|| MoreAccessories._self._accessoriesByChar[__instance.selectChara.chaFile].objAccessory.Any(o => o != null)*/);
        //    }
        //}
    }
}
