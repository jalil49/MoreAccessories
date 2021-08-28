using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
#if EMOTIONCREATORS
using ADVPart.Manipulate.Chara;
using HEdit;
using HPlay;
#endif
using UniRx;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        #region Patches
#if KOIKATSU
        [HarmonyPatch]
        internal static class VRHScene_Start_Patches
        {
            private static bool Prepare()
            {
                return Type.GetType("VRHScene,Assembly-CSharp.dll") != null;
            }

            private static MethodInfo TargetMethod()
            {
                return Type.GetType("VRHScene,Assembly-CSharp.dll").GetMethod("Start", AccessTools.all);
            }

            private static void Postfix(List<ChaControl> ___lstFemale, HSprite[] ___sprites)
            {
                _self.SpawnHUI(___lstFemale, ___sprites[0]);
            }
        }

        [HarmonyPatch(typeof(HSceneProc), nameof(HSceneProc.Start))]
        internal static class HSceneProc_Start_Patches
        {
            private static void Postfix(List<ChaControl> ___lstFemale, HSprite ___sprite)
            {
                _self.SpawnHUI(___lstFemale, ___sprite);
            }
        }

        [HarmonyPatch(typeof(HSprite), nameof(HSprite.Update))]
        public static class HSprite_Update_Patches
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var instructionsList = instructions.ToList();
                for (var i = 0; i < instructionsList.Count; i++)
                {
                    var inst = instructionsList[i];
                    yield return inst;
                    if (inst.opcode == OpCodes.Stloc_S && inst.operand != null && inst.operand.ToString().Equals("System.Int32 (5)"))
                    {
                        var num2Operand = inst.operand;
                        yield return new CodeInstruction(OpCodes.Ldloc_2); //i
                        yield return new CodeInstruction(OpCodes.Call, typeof(HSprite_Update_Patches).GetMethod(nameof(GetFixedAccessoryCount), BindingFlags.NonPublic | BindingFlags.Static));
                        yield return new CodeInstruction(OpCodes.Stloc_S, num2Operand);

                        while (instructionsList[i].opcode != OpCodes.Blt)
                            ++i;
                    }
                }
            }


            private static int GetFixedAccessoryCount(int i)
            {
                var female = _self._hSceneFemales[i];
                var res = 0;
                for (var k = 0; k < 20; k++)
                    if (female.IsAccessory(k))
                        res++;
                CharAdditionalData data;
                if (_self._accessoriesByChar.TryGetValue(female.chaFile, out data) == false)
                    return res;
                foreach (var part in data.nowAccessories)
                {
                    if (part.type != 120)
                        res++;
                }
                return res;
            }
        }

#elif EMOTIONCREATORS
    [HarmonyPatch(typeof(HPlayHPartAccessoryCategoryUI), nameof(Start))]
    internal static class HPlayHPartAccessoryCategoryUI_Start_Postfix
    {
        private static void Postfix(HPlayHPartAccessoryCategoryUI __instance)
        {
            _self.SpawnPlayUI(__instance);
        }
    }

    [HarmonyPatch(typeof(HPlayHPartAccessoryCategoryUI), nameof(Init))]
    internal static class HPlayHPartAccessoryCategoryUI_Init_Postfix
    {
        private static void Postfix()
        {
            _self.ExecuteDelayed(_self.UpdatePlayUI, 2);
        }
    }

    [HarmonyPatch(typeof(HPlayHPartClothMenuUI), nameof(Init))]
    internal static class HPlayHPartClothMenuUI_Init_Postfix
    {
        private static void Postfix(HPlayHPartClothMenuUI __instance, Button[] ___btnClothMenus)
        {
            ___btnClothMenus[1].gameObject.SetActive(___btnClothMenus[1].gameObject.activeSelf || _self._accessoriesByChar[__instance.selectChara.chaFile].objAccessory.Any(o => o != null));
        }
    }

    [HarmonyPatch(typeof(PartInfoClothSetUI), nameof(Start))]
    internal static class PartInfoClothSetUI_Start_Patches
    {
        internal static WeakKeyDictionary<ChaControl, CharAdditionalData> _originalAdditionalData = new WeakKeyDictionary<ChaControl, CharAdditionalData>();
        private static void Postfix(PartInfoClothSetUI.CoordinateUIInfo[] ___coordinateUIs)
        {
            _originalAdditionalData.Purge();
            for (int i = 0; i < ___coordinateUIs.Length; i++)
            {
                PartInfoClothSetUI.CoordinateUIInfo ui = ___coordinateUIs[i];
                Button.ButtonClickedEvent originalOnClick = ui.btnEntry.onClick;
                ui.btnEntry.onClick = new Button.ButtonClickedEvent();
                int i1 = i;
                ui.btnEntry.onClick.AddListener(() =>
                {
                    ChaControl chaControl = Singleton<HEditData>.Instance.charas[i1];
                    if (_originalAdditionalData.ContainsKey(chaControl) == false && _self._accessoriesByChar.TryGetValue(chaControl.chaFile, out CharAdditionalData originalData))
                    {
                        CharAdditionalData newData = new CharAdditionalData();
                        newData.LoadFrom(originalData);
                        _originalAdditionalData.Add(chaControl, newData);
                    }
                    originalOnClick.Invoke();
                });
            }
        }
    }

    [HarmonyPatch(typeof(PartInfoClothSetUI), nameof(BackToCoordinate))]
    internal static class PartInfoClothSetUI_BackToCoordinate_Patches
    {
        private static void Prefix(int _charaID)
        {
            ChaControl chara = Singleton<HEditData>.Instance.charas[_charaID];
            CharAdditionalData originalData;
            if (PartInfoClothSetUI_Start_Patches._originalAdditionalData.TryGetValue(chara, out originalData) && 
                _self._accessoriesByChar.TryGetValue(chara.chaFile, out CharAdditionalData data))
                data.LoadFrom(originalData);
        }
    }

    [HarmonyPatch(typeof(AccessoryUICtrl), nameof(Init))]
    internal static class AccessoryUICtrl_Init_Patches
    {
        private static void Postfix(AccessoryUICtrl __instance)
        {
            _self.SpawnADVUI(__instance);
        }
    }

    [HarmonyPatch(typeof(AccessoryUICtrl), nameof(UpdateUI))]
    internal static class AccessoryUICtrl_UpdateUI_Patches
    {
        private static void Postfix()
        {
            _self.UpdateADVUI();
        }
    }

#endif


#if false
    [HarmonyPatch(typeof(CustomAcsMoveWindow), nameof(CustomAcsMoveWindow.UpdateHistory))]
    internal static class CustomAcsMoveWindow_UpdateHistory_Patches
    {
        private static bool Prepare()
        {
            return (_self._hasDarkness == false);
        }
        private static bool Prefix(CustomAcsMoveWindow __instance)
        {
            _self.GetCvsAccessory(__instance.nSlotNo).UpdateAcsMoveHistory();
            return false;
        }
    }
#endif
        #endregion
    }
}
