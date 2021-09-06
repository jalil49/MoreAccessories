#if !EC
using HarmonyLib;
using Illusion.Extensions;
using Illusion.Game;
using IllusionUtility;
using MoreAccessoriesKOI.Extensions;
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
        internal static class HSpriteUpdate_patch
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

                        yield return new CodeInstruction(OpCodes.Ldloc_1);

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
#if KKS
            private static int MaleAccessoryCount(ChaControl male)
            {
                return male.nowCoordinate.accessory.parts.Length;
            }
#endif
        }

        [HarmonyPatch(typeof(HSprite), nameof(HSprite.AccessoryProc))]
        internal static class HSpriteAccessoryProc_patch
        {
            internal static void Prefix(HSprite __instance, HSceneSpriteCategory ___categoryAccessory, List<ChaControl> ___females)
            {
                var list = ___categoryAccessory.lstButton;
                var slot = list[0].transform;
                var parent = slot.parent;
                var delta = ___females[0].nowCoordinate.accessory.parts.Length - list.Count;
                if (delta > 0)
                {
                    for (var i = 0; i < delta; i++)
                    {
                        var transform = UnityEngine.Object.Instantiate(slot, parent);
                        transform.name = $"{list.Count}";
                        var button = transform.GetComponent<Button>();
                        button.onClick = new Button.ButtonClickedEvent();
                        MakeButton(__instance, button, list.Count);
                        transform.gameObject.SetActive(true);
                        ___categoryAccessory.lstButton.Add(button);
                    }
                    MoreAccessories._self.ExecuteDelayed(__instance.AccessoryProc, 5);
                }
            }
            private static void MakeButton(HSprite sprite, Button button, int slot)
            {
                button.onClick.AddListener(() =>
                {
                    sprite.OnClickAccessory(slot);
                });
            }

            public static void Postfix(HSceneSpriteCategory ___categoryAccessory, List<ChaControl> ___females)
            {
                var female = ___females[0];
                var accessorycount = female.nowCoordinate.accessory.parts.Length;
                var i = 20;
                for (; i < accessorycount; i++)
                {
                    var flag = female.IsAccessory(i);
                    ___categoryAccessory.SetActive(flag, i);
                    if (flag)
                    {
                        var component = female.objAccessory[i].GetComponent<ListInfoComponent>();
                        var @object = ___categoryAccessory.GetObject(i);
                        TextMeshProUGUI textMeshProUGUI = null;
                        if (@object)
                        {
                            textMeshProUGUI = @object.GetComponentInChildren<TextMeshProUGUI>(true);
                        }
                        if (component && textMeshProUGUI)
                        {
                            textMeshProUGUI.text = component.data.Name;
                        }
                    }
                }
                for (var n = ___categoryAccessory.lstButton.Count; i < n; i++)
                {
                    ___categoryAccessory.SetActive(false, i);
                }
            }
        }

        [HarmonyPatch(typeof(HSprite), nameof(HSprite.FemaleDressSubMenuAccessoryProc))]
        internal static class HSpriteFemaleDressSubMenuAccessoryProc_patch
        {
            internal static void Prefix(HSprite __instance, List<HSprite.FemaleDressButtonCategory> ___lstMultipleFemaleDressButton, List<ChaControl> ___females, int _female)
            {
                var list = ___lstMultipleFemaleDressButton[_female].accessory.lstButton;
                var slot = list[0].transform;
                var parent = slot.parent;
                var delta = ___females[_female].nowCoordinate.accessory.parts.Length - list.Count;
                if (delta > 0)
                {
                    for (var i = 0; i < delta; i++)
                    {
                        var transform = UnityEngine.Object.Instantiate(slot, parent);
                        transform.name = $"{list.Count}";
                        var button = transform.GetComponent<Button>();
                        button.onClick = new Button.ButtonClickedEvent();
                        MakeButton(__instance, button, _female, list.Count);
                        list.Add(button);
                    }
                }
            }

            private static void MakeButton(HSprite sprite, Button button, int _female, int slot)
            {
                button.onClick.AddListener(() =>
                {
                    sprite.OnClickAccessory(_female, slot);
                });
            }

            public static void Postfix(HSceneSpriteCategory ___categoryAccessory, List<ChaControl> ___females)
            {
                var female = ___females[0];
                var accessorycount = female.nowCoordinate.accessory.parts.Length;
                var i = 20;
                for (; i < accessorycount; i++)
                {
                    var flag = female.IsAccessory(i);
                    ___categoryAccessory.SetActive(flag, i);
                    if (flag)
                    {
                        var component = female.objAccessory[i].GetComponent<ListInfoComponent>();
                        var @object = ___categoryAccessory.GetObject(i);
                        TextMeshProUGUI textMeshProUGUI = null;
                        if (@object)
                        {
                            textMeshProUGUI = @object.GetComponentInChildren<TextMeshProUGUI>(true);
                        }
                        if (component && textMeshProUGUI)
                        {
                            textMeshProUGUI.text = component.data.Name;
                        }
                    }
                }
                for (var n = ___categoryAccessory.lstButton.Count; i < n; i++)
                {
                    ___categoryAccessory.SetActive(false, i);
                }
            }
        }

        [HarmonyPatch(typeof(HSceneProc), nameof(HSceneProc.Start))]
        internal static class HSceneProc_Start_Patches
        {
            private static void Postfix(List<ChaControl> ___lstFemale, HSprite ___sprite)
            {
                MoreAccessories.Print("Hstarted");
                MoreAccessories.HMode = new HScene(___lstFemale, new[] { ___sprite });
            }
        }
    }
}
#endif