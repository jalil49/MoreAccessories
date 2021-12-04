using HarmonyLib;
using HPlay;
using Illusion.Extensions;
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
            internal static bool Prefix(HPlayHPartAccessoryCategoryUI __instance)
            {
                if (__instance.selectChara == null)
                {
                    return false;
                }
                MoreAccessories.PlayMode.UpdatePlayUI();
                var limit = Math.Min(__instance.accessoryCategoryUIs.Length, __instance.selectChara.nowCoordinate.accessory.parts.Length);
                var i = 0;
                for (; i < limit; i++)
                {
                    var showbutton = __instance.selectChara.IsAccessory(i);
                    __instance.accessoryCategoryUIs[i].btn.gameObject.SetActiveIfDifferent(showbutton);
                    if (showbutton)
                    {
                        var component = __instance.selectChara.objAccessory[i].GetComponent<ListInfoComponent>();
                        __instance.accessoryCategoryUIs[i].text.text = component.data.Name;
                    }
                }
                for (; i < __instance.accessoryCategoryUIs.Length; i++)
                {
                    __instance.accessoryCategoryUIs[i].btn.gameObject.SetActiveIfDifferent(false);
                }
                return false;
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
