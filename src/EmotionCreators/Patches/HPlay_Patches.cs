using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using HPlay;
using Illusion.Extensions;
using UniRx;

namespace MoreAccessoriesKOI.Patches
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    internal class HPlay_Patches
    {
        [HarmonyPatch(typeof(HPlayHPartAccessoryCategoryUI), nameof(HPlayHPartAccessoryCategoryUI.Start))]
        private static class PlayModeStart_Patch
        {
            private static void Postfix(HPlayHPartAccessoryCategoryUI __instance)
            {
                MoreAccessories.PlayMode = new PlayMode(__instance);
            }
        }

        [HarmonyPriority(Priority.Last), HarmonyPatch(typeof(HPlayHPartAccessoryCategoryUI), nameof(HPlayHPartAccessoryCategoryUI.Init))]
        private static class HPlayHPartAccessoryCategoryUI_Patches
        {
            private static bool Prefix(HPlayHPartAccessoryCategoryUI __instance)
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
                    var showButton = __instance.selectChara.IsAccessory(i);
                    __instance.accessoryCategoryUIs[i].btn.gameObject.SetActiveIfDifferent(showButton);
                    if (showButton)
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
        private static class ClothMenuStart_Patch
        {
            private static void Postfix(HPlayHPartClothMenuUI __instance)
            {
                for (var i = 0; i < __instance.btnClothMenus.Length; i++)
                {
                    var num = i;
                    __instance.btnClothMenus[i].OnClickAsObservable().Subscribe(delegate { MoreAccessories.PlayMode.SetScrollViewActive(num == 1 && __instance.accessoryCategoryUI.Active); });
                }
            }
        }
    }
}
