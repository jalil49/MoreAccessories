using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MoreAccessoriesKOI.Patches.MainGame
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public static class H_Patches
    {
        [HarmonyPatch(typeof(HSprite), nameof(HSprite.Update))]
        private static class HSpriteUpdate_Patch
        {
#if KKS
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)//Male was added in KKS
            {
                var instructionsList = instructions.ToList();
                var i = 0;
                for (; i < instructionsList.Count; i++)
                {
                    var inst = instructionsList[i];
                    if (inst.opcode == OpCodes.Ldc_I4_S && inst.operand.ToString() == "20")//female list
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);

                        yield return new CodeInstruction(OpCodes.Ldfld, typeof(HSprite).GetField(nameof(HSprite.females), AccessTools.all));

                        yield return new CodeInstruction(OpCodes.Ldloc_1);

                        yield return new CodeInstruction(OpCodes.Call, typeof(HSpriteUpdate_Patch).GetMethod(nameof(FemaleAccessoryCount), AccessTools.all));
                        i++;
                        break;
                    }
                    yield return inst;
                }

                for (; i < instructionsList.Count; i++)//male
                {
                    var inst = instructionsList[i];
                    if (inst.opcode == OpCodes.Ldc_I4_S && inst.operand.ToString() == "20")
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);

                        yield return new CodeInstruction(OpCodes.Ldfld, typeof(HSprite).GetField(nameof(HSprite.male), AccessTools.all));

                        yield return new CodeInstruction(OpCodes.Call, typeof(HSpriteUpdate_Patch).GetMethod(nameof(MaleAccessoryCount), AccessTools.all));
                        continue;
                    }
                    yield return inst;
                }
            }
            private static int MaleAccessoryCount(ChaControl male)
            {
                return male.nowCoordinate.accessory.parts.Length;
            }
#elif KK
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var instructionsList = instructions.ToList();
                var i = 0;
                for (; i < instructionsList.Count; i++)
                {
                    var inst = instructionsList[i];
                    if (inst.opcode == OpCodes.Ldc_I4_S && inst.operand.ToString() == "20")//female list
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);

                        yield return new CodeInstruction(OpCodes.Ldfld, typeof(HSprite).GetField(nameof(HSprite.females), AccessTools.all));

                        yield return new CodeInstruction(OpCodes.Ldloc_2);

                        yield return new CodeInstruction(OpCodes.Call, typeof(HSpriteUpdate_Patch).GetMethod(nameof(FemaleAccessoryCount), AccessTools.all));
                        continue;
                    }
                    yield return inst;
                }
            }
#endif

            private static int FemaleAccessoryCount(List<ChaControl> femaleList, int current)
            {
                return femaleList[current].nowCoordinate.accessory.parts.Length;
            }
        }

        [HarmonyPatch(typeof(HSprite), nameof(HSprite.AccessoryProc))]
        private static class HSpriteAccessoryProc_Patch
        {
            private static void Prefix(HSprite __instance, HSceneSpriteCategory ___categoryAccessory, List<ChaControl> ___females) => AddAccessoryButtons(__instance, ___categoryAccessory, ___females, 0);
            private static void Postfix(HSceneSpriteCategory ___categoryAccessory, List<ChaControl> ___females)
            {
                try { HideExcessButtons(___categoryAccessory, ___females[0]); }
                catch (Exception ex)
                {
                    MoreAccessories.Print($"Failed to hide buttons to H #0 {ex}", LogLevel.Error);
                }
            }
        }

        [HarmonyPatch(typeof(HSprite), nameof(HSprite.FemaleDressSubMenuAccessoryProc))]
        private static class HSpriteFemaleDressSubMenuAccessoryProc_Patch
        {
            // ReSharper disable once InconsistentNaming
            private static void Prefix(HSprite __instance, List<HSprite.FemaleDressButtonCategory> ___lstMultipleFemaleDressButton, List<ChaControl> ___females, int _female)
            {
                try
                {
                    AddAccessoryButtons(__instance, ___lstMultipleFemaleDressButton[_female].accessory, ___females, _female);
                }
                catch (Exception ex)
                {
                    MoreAccessories.Print($"Failed to make Multiple H #{_female} AddAccessoryButtons {ex}", LogLevel.Error);
                }
            }

            // ReSharper disable once InconsistentNaming
            private static void Postfix(List<HSprite.FemaleDressButtonCategory> ___lstMultipleFemaleDressButton, List<ChaControl> ___females, int _female)
            {
                try { HideExcessButtons(___lstMultipleFemaleDressButton[_female].accessory, ___females[_female]); }
                catch (Exception ex)
                {
                    MoreAccessories.Print($"Failed to hide excess buttons to H #{_female} {ex}", LogLevel.Error);
                }
            }
        }

        [HarmonyPatch(typeof(HSceneProc), nameof(HSceneProc.Start))]
        private static class HSceneProcStart_Patches
        {
            private static void Postfix(List<ChaControl> ___lstFemale, HSprite ___sprite)
            {
                MoreAccessories.HMode = new HScene(___lstFemale, new[] { ___sprite });
            }
        }

        internal static void AddAccessoryButtons(HSprite sprite, HSceneSpriteCategory accessoryCategory, List<ChaControl> ___females, int femaleNum)
        {
            try
            {
                var list = accessoryCategory.lstButton;
                var slot = list[0].transform;
                var parent = slot.parent;
                var delta = ___females[femaleNum].nowCoordinate.accessory.parts.Length - list.Count;
                if (delta > 0)
                {
                    for (var i = 0; i < delta; i++)
                    {
                        var transform = Object.Instantiate(slot, parent);
                        transform.name = $"{list.Count + 1}";
                        var button = transform.GetComponent<Button>();
                        button.onClick = new Button.ButtonClickedEvent();
                        MakeButton(sprite, button, femaleNum, list.Count);
                        list.Add(button);
                    }
                }
            }
            catch (Exception ex)
            {
                MoreAccessories.Print($"Failed to add buttons to H #{femaleNum} {ex}", LogLevel.Error);
            }
        }

        private static void HideExcessButtons(HSceneSpriteCategory accessoryCategory, ChaControl female)
        {
            var accessoryCount = female.nowCoordinate.accessory.parts.Length;
            var i = 20;
            for (; i < accessoryCount && i < accessoryCategory.lstButton.Count; i++)
            {
                var show = female.IsAccessory(i);
                accessoryCategory.SetActive(show, i);
                if (show)
                {
                    var component = female.objAccessory[i].GetComponent<ListInfoComponent>();
                    var accessorySlot = accessoryCategory.GetObject(i);
                    TextMeshProUGUI textMeshProUGUI = null;
                    if (accessorySlot)
                    {
                        textMeshProUGUI = accessorySlot.GetComponentInChildren<TextMeshProUGUI>(true);
                    }
                    if (component && textMeshProUGUI)
                    {
                        textMeshProUGUI.text = component.data.Name;
                    }
                }
            }
            for (var n = accessoryCategory.lstButton.Count; i < n; i++)
            {
                accessoryCategory.SetActive(false, i);
            }
        }

        private static void MakeButton(HSprite sprite, Button button, int femaleNumber, int slot)
        {
            button.onClick.AddListener(() =>
            {
                sprite.OnClickAccessory(femaleNumber, slot);
            });
        }
    }
}