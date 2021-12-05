using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using TMPro;
using UnityEngine.UI;

namespace MoreAccessoriesKOI.Patches.MainGame
{
    public static class H_Patches
    {
        [HarmonyPatch(typeof(HSprite), nameof(HSprite.Update))]
        private static class HSpriteUpdate_patch
        {
#if DEBUG
            static int current = 0;

            static void Finalizer(Exception __exception)
            {
                current %= 300;
                if (__exception != null && current == 0)
                {
                    MoreAccessories.Print($"Hsprite Update" + __exception, BepInEx.Logging.LogLevel.Error);
                }
                current++;
            }
#endif
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

                        yield return new CodeInstruction(OpCodes.Call, typeof(HSpriteUpdate_patch).GetMethod(nameof(FemaleAccessoryCount), AccessTools.all));
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

                        yield return new CodeInstruction(OpCodes.Call, typeof(HSpriteUpdate_patch).GetMethod(nameof(MaleAccessoryCount), AccessTools.all));
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

                        yield return new CodeInstruction(OpCodes.Call, typeof(HSpriteUpdate_patch).GetMethod(nameof(FemaleAccessoryCount), AccessTools.all));
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
        private static class HSpriteAccessoryProc_patch
        {
            private static void Prefix(HSprite __instance, HSceneSpriteCategory ___categoryAccessory, List<ChaControl> ___females) => AddAccessoryButtons(__instance, ___categoryAccessory, ___females, 0);
            private static void Postfix(HSceneSpriteCategory ___categoryAccessory, List<ChaControl> ___females)
            {
                try { HideExcessButtons(___categoryAccessory, ___females[0]); }
                catch (Exception ex)
                {
                    MoreAccessories.Print($"Failed to hide buttons to H #0 {ex}", BepInEx.Logging.LogLevel.Error);
                }
            }
        }

        [HarmonyPatch(typeof(HSprite), nameof(HSprite.FemaleDressSubMenuAccessoryProc))]
        private static class HSpriteFemaleDressSubMenuAccessoryProc_patch
        {
            private static void Prefix(HSprite __instance, List<HSprite.FemaleDressButtonCategory> ___lstMultipleFemaleDressButton, List<ChaControl> ___females, int _female)
            {
                try
                {
                    AddAccessoryButtons(__instance, ___lstMultipleFemaleDressButton[_female].accessory, ___females, _female);
                }
                catch (Exception ex)
                {
                    MoreAccessories.Print($"Failed to make Multiple H #{_female} AddAccessoryButtons {ex}", BepInEx.Logging.LogLevel.Error);
                }
            }

            private static void Postfix(List<HSprite.FemaleDressButtonCategory> ___lstMultipleFemaleDressButton, List<ChaControl> ___females, int _female)
            {
                try { HideExcessButtons(___lstMultipleFemaleDressButton[_female].accessory, ___females[_female]); }
                catch (Exception ex)
                {
                    MoreAccessories.Print($"Failed to hide excess buttons to H #{_female} {ex}", BepInEx.Logging.LogLevel.Error);
                }
            }
        }

        [HarmonyPatch(typeof(HSceneProc), nameof(HSceneProc.Start))]
        private static class HSceneProc_Start_Patches
        {
            private static void Postfix(List<ChaControl> ___lstFemale, HSprite ___sprite)
            {
                MoreAccessories.HMode = new HScene(___lstFemale, new[] { ___sprite });
            }
        }

        internal static void AddAccessoryButtons(HSprite sprite, HSceneSpriteCategory accessorycategory, List<ChaControl> ___females, int femalenum)
        {
            try
            {
                var list = accessorycategory.lstButton;
                var slot = list[0].transform;
                var parent = slot.parent;
                var delta = ___females[femalenum].nowCoordinate.accessory.parts.Length - list.Count;
                if (delta > 0)
                {
                    for (var i = 0; i < delta; i++)
                    {
                        var transform = UnityEngine.Object.Instantiate(slot, parent);
                        transform.name = $"{list.Count + 1}";
                        var button = transform.GetComponent<Button>();
                        button.onClick = new Button.ButtonClickedEvent();
                        MakeButton(sprite, button, femalenum, list.Count);
                        list.Add(button);
                    }
                }
            }
            catch (Exception ex)
            {
                MoreAccessories.Print($"Failed to add buttons to H #{femalenum} {ex}", BepInEx.Logging.LogLevel.Error);
            }
        }

        private static void HideExcessButtons(HSceneSpriteCategory accessorycategory, ChaControl female)
        {
            var accessorycount = female.nowCoordinate.accessory.parts.Length;
            var i = 20;
            for (; i < accessorycount && i < accessorycategory.lstButton.Count; i++)
            {
                var show = female.IsAccessory(i);
                accessorycategory.SetActive(show, i);
                if (show)
                {
                    var component = female.objAccessory[i].GetComponent<ListInfoComponent>();
                    var accslot = accessorycategory.GetObject(i);
                    TextMeshProUGUI textMeshProUGUI = null;
                    if (accslot)
                    {
                        textMeshProUGUI = accslot.GetComponentInChildren<TextMeshProUGUI>(true);
                    }
                    if (component && textMeshProUGUI)
                    {
                        textMeshProUGUI.text = component.data.Name;
                    }
                }
            }
            for (var n = accessorycategory.lstButton.Count; i < n; i++)
            {
                accessorycategory.SetActive(false, i);
            }
        }

        private static void MakeButton(HSprite sprite, Button button, int _female, int slot)
        {
            button.onClick.AddListener(() =>
            {
                sprite.OnClickAccessory(_female, slot);
            });
        }
    }
}