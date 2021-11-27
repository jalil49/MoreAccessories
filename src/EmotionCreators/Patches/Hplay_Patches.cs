using HarmonyLib;
using HPlay;
using Illusion.Extensions;
using MoreAccessoriesKOI.Extensions;
using System;
using UniRx;

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

        [HarmonyPriority(Priority.Last), HarmonyPatch(typeof(HPlayHPartAccessoryCategoryUI), nameof(HPlayHPartAccessoryCategoryUI.Init))]
        internal static class HPlayHPartAccessoryCategoryUI_Init_Postfix
        {
            internal static void Postfix()
            {
                MoreAccessories._self.ExecuteDelayed(MoreAccessories.PlayMode.UpdatePlayUI, 2);
            }
        }

        [HarmonyPatch(typeof(HPlayHPartClothMenuUI), nameof(HPlayHPartClothMenuUI.Start))]
        internal static class HPlayHPartClothMenuUI_Start_Postfix
        {
            internal static void Postfix(HPlayHPartClothMenuUI __instance)
            {
                for (var i = 0; i < __instance.btnClothMenus.Length; i++)
                {
                    var num = i;
                    __instance.btnClothMenus[i].OnClickAsObservable().Subscribe(delegate (Unit _)
                    {
                        if (num == 1)
                        {
                            MoreAccessories.PlayMode.SetScrollViewActive(__instance.accessoryCategoryUI.Active);
                        }
                        else
                        {
                            MoreAccessories.PlayMode.SetScrollViewActive(false);
                        }
                    });
                }
            }
        }
    }
}
