using ChaCustom;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using Illusion.Extensions;
using MessagePack;
using MoreAccessoriesKOI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoreAccessoriesKOI
{
    public static class CvsAccessory_Patches
    {

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.CalculateUI))]
        private static class CvsAccessory_CalculateUI_Patches
        {
            private static bool Prefix(CvsAccessory __instance)
            {
                __instance.tglTakeOverParent.isOn = Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverParent;
                __instance.tglTakeOverColor.isOn = Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverColor;

                ChaFileAccessory.PartsInfo part = MoreAccessories._self.GetPart((int)__instance.slotNo);
                __instance.tglNoShake.isOn = part.noShake;
                __instance.imgAcsColor01.color = part.color[0];
                __instance.imgAcsColor02.color = part.color[1];
                __instance.imgAcsColor03.color = part.color[2];
                __instance.imgAcsColor04.color = part.color[3];
                Toggle[] tglAcsGroup = __instance.tglAcsGroup;
                for (int i = 0; i < tglAcsGroup.Length; i++)
                {
                    tglAcsGroup[i].isOn = i == part.hideCategory;
                }
#if EMOTIONCREATORS
                Toggle[] tglHideTiming = __instance.tglHideTiming");
                for (int j = 0; j < tglHideTiming.Length; j++)
                {
                    tglHideTiming[j].isOn = (j == part.hideTiming);
                }
#endif
                for (int j = 0; j < 2; j++)
                {
                    __instance.UpdateDrawControllerState(j);
                }
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.UpdateCustomUI))]
        private static class CvsAccessory_UpdateCustomUI_Patches
        {
            private static bool Prefix(CvsAccessory __instance)
            {
                int selectIndex = MoreAccessories._self.GetSelectedMakerIndex();
                if (selectIndex < 0)
                    return false;
                __instance.CalculateUI();
#if KOIKATSU
                __instance.cmpDrawCtrl.UpdateAccessoryDraw();
#endif
                int num = MoreAccessories._self.GetPart(selectIndex).type - 120;
                __instance.ddAcsType.value = num;
                __instance.UpdateAccessoryKindInfo();
                __instance.UpdateAccessoryParentInfo();
                __instance.UpdateAccessoryMoveInfo();
                __instance.ChangeSettingVisible(0 != num);
                bool flag = false;
                const int offset = 124;
                CvsColor cvsColor = __instance.cvsColor;
                for (int i = 0; i < MoreAccessories._self.GetPartsLength(); i++)
                {
                    ChaFileAccessory.PartsInfo part = MoreAccessories._self.GetPart(i);
                    for (int j = 0; j < 4; j++)
                    {
                        if ((int)cvsColor.connectColorKind == i * 4 + j + offset)
                        {
                            cvsColor.SetColor(part.color[j]);
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.UpdateSlotName))]
        private static class CvsAccessory_UpdateSlotName_Patches
        {
            private static bool Prefix(CvsAccessory __instance)
            {
                TextMeshProUGUI textSlotName = __instance.textSlotName;

                if (null == textSlotName)
                {
                    return false;
                }
                int index = (int)__instance.slotNo;
                if (MoreAccessories._self.GetPartsLength() <= index || MoreAccessories._self.GetPart(index).type == 120)
                {
                    textSlotName.text = MoreAccessories._self._isParty ? $"Slot {index + 1:00}" : $"スロット{index + 1:00}";
                }
                else
                {
                    if (index < 20)
                    {
                        if (CustomBase.Instance.chaCtrl.infoAccessory[index] != null)
                            textSlotName.text = CustomBase.Instance.chaCtrl.infoAccessory[index].Name;
                    }
                    else
                    {
                        if (MoreAccessories._self._charaMakerData.infoAccessory[index - 20] != null)
                            textSlotName.text = MoreAccessories._self._charaMakerData.infoAccessory[index - 20].Name;
                    }
                }
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.UpdateSelectAccessoryType))]
        private static class CvsAccessory_UpdateSelectAccessoryType_Patches
        {
            private static bool Prefix(CvsAccessory __instance, int index)
            {
                ChaFileAccessory.PartsInfo part = MoreAccessories._self.GetPart((int)__instance.slotNo);
                if (part.type - 120 != index)
                {
                    part.type = 120 + index;
                    part.id = 0;
                    ChaFileAccessory.PartsInfo setPart = null;
                    if ((int)__instance.slotNo < 20)
                    {
                        setPart = CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo];
                        setPart.type = part.type;
                        setPart.id = 0;
                    }
                    if (!Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverParent)
                        part.parentKey = string.Empty;
                    __instance.FuncUpdateAccessory(false, false);
                    if ((int)__instance.slotNo < 20)
                    {
                        setPart.parentKey = part.parentKey;
                        for (int i = 0; i < 2; i++)
                        {
                            setPart.addMove[i, 0] = part.addMove[i, 0];
                            setPart.addMove[i, 1] = part.addMove[i, 1];
                            setPart.addMove[i, 2] = part.addMove[i, 2];
                        }
                    }
                    __instance.UpdateAccessoryKindInfo();
                    __instance.UpdateAccessoryParentInfo();
                    __instance.UpdateAccessoryMoveInfo();
                    if (!Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverColor)
                        __instance.SetDefaultColor();
#if KOIKATSU
                    if (MoreAccessories._self._hasDarkness == false)
                        BackwardCompatibility.CustomHistory_Instance_Add3(CustomBase.Instance.chaCtrl, __instance.FuncUpdateAccessory, false, true);
#endif
                    __instance.UpdateSlotName();
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.UpdateSelectAccessoryKind))]
        private static class CvsAccessory_UpdateSelectAccessoryKind_Patches
        {
            private static bool Prefix(CvsAccessory __instance, string name, Sprite sp, int index)
            {
                TextMeshProUGUI textAcsKind = __instance.textAcsKind;
                if (textAcsKind)
                {
                    textAcsKind.text = name;
                }
                Image imgAcsKind = __instance.imgAcsKind;
                if (imgAcsKind)
                {
                    imgAcsKind.sprite = sp;
                }
                ChaFileAccessory.PartsInfo part = MoreAccessories._self.GetPart((int)__instance.slotNo);
                if (part.id != index)
                {
                    part.id = index;
                    ChaFileAccessory.PartsInfo setPart = null;
                    if ((int)__instance.slotNo < 20)
                    {
                        setPart = CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo];
                        setPart.id = index;
                    }
                    Dictionary<int, ListInfoBase> categoryInfo = CustomBase.Instance.chaCtrl.lstCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)part.type);
                    ListInfoBase listInfoBase = null;
                    categoryInfo.TryGetValue(part.id, out listInfoBase);
                    bool flag = listInfoBase != null && 1 == listInfoBase.GetInfoInt(ChaListDefine.KeyType.HideHair);
                    if (!Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverParent || flag)
                    {
                        part.parentKey = string.Empty;
                    }
                    __instance.FuncUpdateAccessory(!Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverColor, false);
                    if ((int)__instance.slotNo < 20)
                        setPart.parentKey = part.parentKey;
                    __instance.UpdateAccessoryParentInfo();
#if KOIKATSU
                    if (MoreAccessories._self._hasDarkness == false)
                        BackwardCompatibility.CustomHistory_Instance_Add3(CustomBase.Instance.chaCtrl, __instance.FuncUpdateAccessory, false, true);
#endif
                }

                __instance.ChangeUseColorVisible();
                __instance.ChangeParentAndMoveSettingVisible();
                __instance.UpdateSlotName();
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.UpdateSelectAccessoryParent))]
        private static class CvsAccessory_UpdateSelectAccessoryParent_Patches
        {
            private static bool Prefix(CvsAccessory __instance, int index)
            {
                TextMeshProUGUI textAcsParent = __instance.textAcsParent;
                if (textAcsParent)
                {
                    string empty = string.Empty;
                    if (!ChaAccessoryDefine.dictAccessoryParent.TryGetValue(index + 1, out empty))
                    {
                        empty = string.Empty;
                    }
                    textAcsParent.text = empty;
                }
                string[] array = (from key in Enum.GetNames(typeof(ChaAccessoryDefine.AccessoryParentKey))
                                  where key != "none"
                                  select key).ToArray();
                string text = array[index];
                ChaFileAccessory.PartsInfo part = MoreAccessories._self.GetPart((int)__instance.slotNo);
                if (part.parentKey != text)
                {
                    part.parentKey = text;
                    if ((int)__instance.slotNo < 20)
                        CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo].parentKey = text;

                    __instance.FuncUpdateAcsParent(false);
#if KOIKATSU
                    if (MoreAccessories._self._hasDarkness == false)
                        BackwardCompatibility.CustomHistory_Instance_Add2(CustomBase.Instance.chaCtrl, __instance.FuncUpdateAcsParent, true);
#endif
                }
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAccessory), new[] { typeof(bool), typeof(bool) })]
        private static class CvsAccessory_FuncUpdateAccessory_Patches
        {
            private static bool Prefix(CvsAccessory __instance, bool setDefaultColor, bool history, ref bool __result)
            {
                if (history)
                {
                    for (int i = 0; i < MoreAccessories._self.GetPartsLength(); i++)
                    {
                        ChaFileAccessory.PartsInfo part = MoreAccessories._self.GetPart(i);
                        CustomBase.Instance.chaCtrl.ChangeAccessory(i, part.type, part.id, part.parentKey, true);
                    }
                }
                else
                {
                    ChaFileAccessory.PartsInfo part = MoreAccessories._self.GetPart((int)__instance.slotNo);
                    CustomBase.Instance.chaCtrl.ChangeAccessory((int)__instance.slotNo, part.type, part.id, part.parentKey, true);
                }
                if (setDefaultColor)
                {
                    __instance.SetDefaultColor();
                }
                __result = true;
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.SetDefaultColor))]
        private static class CvsAccessory_SetDefaultColor_Patches
        {
            private static bool Prefix(CvsAccessory __instance)
            {
                Image[] array =
                {
                    __instance.imgAcsColor01,
                    __instance.imgAcsColor02,
                    __instance.imgAcsColor03,
                    __instance.imgAcsColor04
                };
                ChaFileAccessory.PartsInfo part = MoreAccessories._self.GetPart((int)__instance.slotNo);
                ChaFileAccessory.PartsInfo setPart = null;
                if ((int)__instance.slotNo < 20)
                    setPart = CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo];
                for (int i = 0; i < 4; i++)
                {
                    Color white = Color.white;
                    CustomBase.Instance.chaCtrl.GetAccessoryDefaultColor(ref white, (int)__instance.slotNo, i);
                    part.color[i] = white;
                    if ((int)__instance.slotNo < 20)
                        setPart.color[i] = white;

                    array[i].color = white;
                }
                __instance.FuncUpdateAcsColor(false);
                return false;
            }
        }
        //[HarmonyPatch(typeof(CvsAccessory), "UpdateAccessoryKindInfo"), HarmonyPrefix]
        //private static bool UpdateAccessoryKindInfo(CvsAccessory __instance)
        //{
        //    int num = (__instance.ddAcsType).value - 1;
        //    if (0 <= num)
        //    {
        //        if (__instance.tglAcsKind.isOn)
        //        {
        //            CanvasGroup[] cgAccessoryWin = __instance.cgAccessoryWin;
        //            for (int i = 0; i < cgAccessoryWin.Length; i++)
        //            {
        //                cgAccessoryWin[i].Enable(num == i, false);
        //            }
        //        }
        //        if (null != ((CustomAcsSelectKind[])__instance.customAccessory)[num])
        //        {
        //            ((CustomAcsSelectKind[])__instance.customAccessory)[num].UpdateCustomUI(0);
        //        }
        //    }
        //    else
        //    {
        //        __instance.tglAcsKind.isOn = false;
        //    }
        //    return false;
        //}

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.UpdateAccessoryParentInfo))]
        private static class CvsAccessory_UpdateAccessoryParentInfo_Patches
        {
            private static bool Prefix(CvsAccessory __instance)
            {
                int num = (__instance.ddAcsType).value - 1;
                if (0 <= num)
                {
                    if (__instance.tglAcsParent.isOn)
                        __instance.cgAcsParent.Enable(true, false);
                    if (null != __instance.cusAcsParentWin)
                    {
                        int num2 = __instance.cusAcsParentWin.UpdateCustomUI();
                        if (__instance.textAcsParent != null)
                        {
                            string empty = string.Empty;
                            if (!ChaAccessoryDefine.dictAccessoryParent.TryGetValue(num2 + 1, out empty))
                                empty = string.Empty;
                            __instance.textAcsParent.text = empty;
                        }
                    }
                }
                else
                    __instance.tglAcsParent.isOn = false;
                return false;
            }
        }

        //[HarmonyPatch(typeof(CvsAccessory), "UpdateAccessoryMoveInfo"), HarmonyPrefix]
        //private static bool UpdateAccessoryMoveInfo(CvsAccessory __instance)
        //{
        //    int num = (__instance.ddAcsType).value - 1;
        //    if (0 <= num)
        //    {
        //        if ((__instance.tglAcsMove01).isOn)
        //        {
        //            ((CanvasGroup)__instance.cgAcsMove01).Enable(true, false);
        //        }
        //        if (null != __instance.cusAcsMove01Win)
        //        {
        //            __instance.cusAcsMove01Win.UpdateCustomUI(0);
        //        }
        //        if ((__instance.tglAcsMove02).isOn)
        //        {
        //            ((CanvasGroup)__instance.cgAcsMove02).Enable(true, false);
        //        }
        //        if (null != __instance.cusAcsMove02Win)
        //        {
        //            __instance.cusAcsMove02Win.UpdateCustomUI(0);
        //        }
        //    }
        //    else
        //    {
        //        (__instance.tglAcsMove01).isOn = false;
        //        (__instance.tglAcsMove02).isOn = false;
        //    }
        //    return false;
        //}

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAcsParent))]
        private static class CvsAccessory_FuncUpdateAcsParent_Patches
        {
            private static bool Prefix(CvsAccessory __instance, bool history, ref bool __result)
            {
                if (history)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        CustomBase.Instance.chaCtrl.ChangeAccessoryParent(i, MoreAccessories._self.GetPart(i).parentKey);
                    }
                    __result = true;
                    return false;
                }
                __result = CustomBase.Instance.chaCtrl.ChangeAccessoryParent((int)__instance.slotNo, MoreAccessories._self.GetPart((int)__instance.slotNo).parentKey);
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.UpdateAcsColor01))]
        private static class CvsAccessory_UpdateAcsColor01_Patches
        {
            private static bool Prefix(CvsAccessory __instance, Color color)
            {
                MoreAccessories._self.GetPart((int)__instance.slotNo).color[0] = color;
                if ((int)__instance.slotNo < 20)
                    CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo].color[0] = color;

                (__instance.imgAcsColor01).color = color;
                __instance.FuncUpdateAcsColor(false);
                return false;
            }
        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.UpdateAcsColor02))]
        private static class CvsAccessory_UpdateAcsColor02_Patches
        {
            private static bool Prefix(CvsAccessory __instance, Color color)
            {
                MoreAccessories._self.GetPart((int)__instance.slotNo).color[1] = color;
                if ((int)__instance.slotNo < 20)
                    CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo].color[1] = color;

                (__instance.imgAcsColor02).color = color;
                __instance.FuncUpdateAcsColor(false);
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.UpdateAcsColor03))]
        private static class CvsAccessory_UpdateAcsColor03_Patches
        {
            private static bool Prefix(CvsAccessory __instance, Color color)
            {
                MoreAccessories._self.GetPart((int)__instance.slotNo).color[2] = color;
                if ((int)__instance.slotNo < 20)
                    CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo].color[2] = color;

                (__instance.imgAcsColor03).color = color;
                __instance.FuncUpdateAcsColor(false);
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.UpdateAcsColor04))]
        private static class CvsAccessory_UpdateAcsColor04_Patches
        {
            private static bool Prefix(CvsAccessory __instance, Color color)
            {
                MoreAccessories._self.GetPart((int)__instance.slotNo).color[3] = color;
                if ((int)__instance.slotNo < 20)
                    CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo].color[3] = color;

                (__instance.imgAcsColor04).color = color;
                __instance.FuncUpdateAcsColor(false);
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAcsColor))]
        private static class CvsAccessory_FuncUpdateAcsColor_Patches
        {
            private static bool Prefix(CvsAccessory __instance, bool history, ref bool __result)
            {
                if (history)
                {
                    for (int i = 0; i < MoreAccessories._self.GetPartsLength(); i++)
                    {
                        CustomBase.Instance.chaCtrl.ChangeAccessoryColor(i);
                    }
                }
                else
                {
                    CustomBase.Instance.chaCtrl.ChangeAccessoryColor((int)__instance.slotNo);
                }
                __result = true;
                return false;
            }

        }

        //#if KOIKATSU
        //        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.UpdateAcsColorHistory))]
        //        private static class CvsAccessory_UpdateAcsColorHistory_Patches
        //        {
        //            private static bool Prepare()
        //            {
        //                return (MoreAccessories._self._hasDarkness == false);
        //            }

        //            private static bool Prefix(CvsAccessory __instance)
        //            {
        //                BackwardCompatibility.CustomHistory_Instance_Add2(CustomBase.Instance.chaCtrl, __instance.FuncUpdateAcsColor, true);
        //                return false;
        //            }

        //        }
        //#endif

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAcsPosAdd))]
        private static class CvsAccessory_FuncUpdateAcsPosAdd_Patches
        {
            private static bool Prefix(CvsAccessory __instance, int correctNo, int xyz, bool add, float val)
            {
                int[] array =
                {
                    1,
                    2,
                    4
                };
                CustomBase.Instance.chaCtrl.SetAccessoryPos((int)__instance.slotNo, correctNo, val, add, array[xyz]);
                if ((int)__instance.slotNo < 20)
                    CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo].addMove[correctNo, 0] = MoreAccessories._self.GetPart((int)__instance.slotNo).addMove[correctNo, 0];
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAcsRotAdd))]
        private static class CvsAccessory_FuncUpdateAcsRotAdd_Patches
        {
            private static bool Prefix(CvsAccessory __instance, int correctNo, int xyz, bool add, float val)
            {
                int[] array =
                {
                    1,
                    2,
                    4
                };
                CustomBase.Instance.chaCtrl.SetAccessoryRot((int)__instance.slotNo, correctNo, val, add, array[xyz]);
                if ((int)__instance.slotNo < 20)
                    CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo].addMove[correctNo, 1] = MoreAccessories._self.GetPart((int)__instance.slotNo).addMove[correctNo, 1];
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAcsSclAdd))]
        private static class CvsAccessory_FuncUpdateAcsSclAdd_Patches
        {
            private static bool Prefix(CvsAccessory __instance, int correctNo, int xyz, bool add, float val)
            {
                int[] array =
                {
                    1,
                    2,
                    4
                };
                CustomBase.Instance.chaCtrl.SetAccessoryScl((int)__instance.slotNo, correctNo, val, add, array[xyz]);
                if ((int)__instance.slotNo < 20)
                    CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo].addMove[correctNo, 2] = MoreAccessories._self.GetPart((int)__instance.slotNo).addMove[correctNo, 2];
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAcsMovePaste))]
        private static class CvsAccessory_FuncUpdateAcsMovePaste_Patches
        {
            private static bool Prefix(CvsAccessory __instance, int correctNo, Vector3[] value)
            {
                CustomBase.Instance.chaCtrl.SetAccessoryPos((int)__instance.slotNo, correctNo, value[0].x, false, 1);
                CustomBase.Instance.chaCtrl.SetAccessoryPos((int)__instance.slotNo, correctNo, value[0].y, false, 2);
                CustomBase.Instance.chaCtrl.SetAccessoryPos((int)__instance.slotNo, correctNo, value[0].z, false, 4);

                CustomBase.Instance.chaCtrl.SetAccessoryRot((int)__instance.slotNo, correctNo, value[1].x, false, 1);
                CustomBase.Instance.chaCtrl.SetAccessoryRot((int)__instance.slotNo, correctNo, value[1].y, false, 2);
                CustomBase.Instance.chaCtrl.SetAccessoryRot((int)__instance.slotNo, correctNo, value[1].z, false, 4);

                CustomBase.Instance.chaCtrl.SetAccessoryScl((int)__instance.slotNo, correctNo, value[2].x, false, 1);
                CustomBase.Instance.chaCtrl.SetAccessoryScl((int)__instance.slotNo, correctNo, value[2].y, false, 2);
                CustomBase.Instance.chaCtrl.SetAccessoryScl((int)__instance.slotNo, correctNo, value[2].z, false, 4);
                if ((int)__instance.slotNo < 20)
                {
                    ChaFileAccessory.PartsInfo part = MoreAccessories._self.GetPart((int)__instance.slotNo);
                    ChaFileAccessory.PartsInfo setPart = CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo];
                    setPart.addMove[correctNo, 0] = part.addMove[correctNo, 0];
                    setPart.addMove[correctNo, 1] = part.addMove[correctNo, 1];
                    setPart.addMove[correctNo, 2] = part.addMove[correctNo, 2];
                }
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.FuncUpdateAcsAllReset))]
        private static class CvsAccessory_FuncUpdateAcsAllReset_Patches
        {
            private static bool Prefix(CvsAccessory __instance, int correctNo)
            {
                CustomBase.Instance.chaCtrl.ResetAccessoryMove((int)__instance.slotNo, correctNo, 7);
                if ((int)__instance.slotNo < 20)
                {
                    ChaFileAccessory.PartsInfo setPart = CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo];
                    ChaFileAccessory.PartsInfo part = MoreAccessories._self.GetPart((int)__instance.slotNo);
                    setPart.addMove[correctNo, 0] = part.addMove[correctNo, 0];
                    setPart.addMove[correctNo, 1] = part.addMove[correctNo, 1];
                    setPart.addMove[correctNo, 2] = part.addMove[correctNo, 2];
                }
                return false;
            }

        }

        //#if KOIKATSU
        //        //[HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.UpdateAcsMoveHistory))]
        //        [HarmonyPatch(typeof(CvsAccessory), "updateacsmovehistory")] //extension nameof(CvsAccessory.UpdateAcsMoveHistory)
        //        private static class CvsAccessory_UpdateAcsMoveHistory_Patches
        //        {
        //            private static bool Prepare()
        //            {
        //                return (MoreAccessories._self._hasDarkness == false);
        //            }

        //            private static bool Prefix(CvsAccessory __instance)
        //            {
        //                BackwardCompatibility.CustomHistory_Instance_Add1(CustomBase.Instance.chaCtrl, CustomBase.Instance.chaCtrl.UpdateAccessoryMoveAllFromInfo);
        //                return false;
        //            }
        //        }
        //#endif

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.ChangeSettingVisible))]
        private static class CvsAccessory_ChangeSettingVisible_Patches
        {
            private static bool Prefix(CvsAccessory __instance, bool visible)
            {
                __instance.tglAcsKind.transform.parent.gameObject.SetActiveIfDifferent(visible);
                __instance.ChangeParentAndMoveSettingVisible();
                __instance.ChangeUseColorVisible();
                __instance.btnInitColor.transform.parent.gameObject.SetActiveIfDifferent(visible);
                __instance.separateGroup.SetActiveIfDifferent(visible);
                __instance.tglAcsGroup[0].transform.parent.gameObject.SetActiveIfDifferent(visible);
#if EMOTIONCREATORS
                __instance.tglHideTiming[0].transform.parent.gameObject.SetActiveIfDifferent(visible);
#endif
                __instance.objExplanation.SetActiveIfDifferent(visible);
                return false;
            }
        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.ChangeParentAndMoveSettingVisible))]
        private static class CvsAccessory_ChangeParentAndMoveSettingVisible_Patches
        {
            private static bool Prefix(CvsAccessory __instance)
            {
                ChaFileAccessory.PartsInfo part = MoreAccessories._self.GetPart((int)__instance.slotNo);
                bool flag = (__instance.ddAcsType).value != 0;
                Dictionary<int, ListInfoBase> categoryInfo = CustomBase.Instance.chaCtrl.lstCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)part.type);
                ListInfoBase listInfoBase = null;
                categoryInfo.TryGetValue(part.id, out listInfoBase);
                if (listInfoBase != null && (listInfoBase.GetInfoInt(ChaListDefine.KeyType.HideHair) == 1 || "null" == listInfoBase.GetInfo(ChaListDefine.KeyType.Parent)))
                {
                    flag = false;
                }
                __instance.tglAcsParent.transform.parent.gameObject.SetActiveIfDifferent(flag);
                (__instance.btnInitParent).transform.parent.gameObject.SetActiveIfDifferent(flag);
                (__instance.btnReverseParent).transform.parent.gameObject.SetActiveIfDifferent(flag);
                (__instance.separateCorrect).SetActiveIfDifferent(flag);
                (__instance.tglAcsMove01).transform.parent.gameObject.SetActiveIfDifferent(flag);
                GameObject objAcsMove;
                if ((int)__instance.slotNo < 20)
                    objAcsMove = CustomBase.Instance.chaCtrl.objAcsMove[(int)__instance.slotNo, 1];
                else
                    objAcsMove = MoreAccessories._self._charaMakerData.objAcsMove[(int)__instance.slotNo - 20][1];
                (__instance.tglAcsMove02).transform.parent.gameObject.SetActiveIfDifferent(!(null == objAcsMove) && flag);
                if (__instance.objControllerTop01)
                {
                    (__instance.objControllerTop01).SetActiveIfDifferent(flag);
                }
                if (__instance.objControllerTop02)
                {
                    (__instance.objControllerTop02).SetActiveIfDifferent(!(null == objAcsMove) && flag);
                }
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.ChangeUseColorVisible))]
        private static class CvsAccessory_ChangeUseColorVisible_Patches
        {
            private static bool Prefix(CvsAccessory __instance)
            {
                bool[] array = new bool[4];
                bool active = false;
                if ((__instance.ddAcsType).value != 0)
                {
                    ChaAccessoryComponent cmp;
                    if ((int)__instance.slotNo < 20)
                        cmp = CustomBase.Instance.chaCtrl.cusAcsCmp[(int)__instance.slotNo];
                    else
                        cmp = MoreAccessories._self._accessoriesByChar[CustomBase.Instance.chaCtrl.chaFile].cusAcsCmp[(int)__instance.slotNo - 20];
                    if (cmp != null)
                    {
                        if (cmp.useColor01)
                        {
                            array[0] = true;
                            active = true;
                        }
                        if (cmp.useColor02)
                        {
                            array[1] = true;
                            active = true;
                        }
                        if (cmp.useColor03)
                        {
                            array[2] = true;
                            active = true;
                        }
                        if (cmp.rendAlpha != null && 0 < cmp.rendAlpha.Length)
                        {
                            array[3] = true;
                            active = true;
                        }
                    }
                }
                (__instance.separateColor).SetActiveIfDifferent(active);
                (__instance.btnAcsColor01).transform.parent.gameObject.SetActiveIfDifferent(array[0]);
                (__instance.btnAcsColor02).transform.parent.gameObject.SetActiveIfDifferent(array[1]);
                (__instance.btnAcsColor03).transform.parent.gameObject.SetActiveIfDifferent(array[2]);
                (__instance.btnAcsColor04).transform.parent.gameObject.SetActiveIfDifferent(array[3]);
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.SetDefaultColorWindow))]
        private static class CvsAccessory_SetDefaultColorWindow_Patches
        {
            private static bool Prefix(CvsAccessory __instance, int no)
            {
                __instance.cvsColor.Setup($"スロット{no + 1:00} カラー①", (CvsColor.ConnectColorKind)(no * 4 + 124), MoreAccessories._self.GetPart(no).color[0], __instance.UpdateAcsColor01, __instance.UpdateAcsColorHistory(), false);
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.SetControllerTransform))]
        private static class CvsAccessory_SetControllerTransform_Patches
        {
            private static bool Prefix(CvsAccessory __instance, int guidNo)
            {
                int index = (int)__instance.slotNo;
                GameObject gameObject;
                if (index < 20)
                    gameObject = CustomBase.Instance.chaCtrl.objAcsMove[index, guidNo];
                else
                    gameObject = MoreAccessories._self._charaMakerData.objAcsMove[index - 20][guidNo];
                if (null == gameObject)
                {
                    return false;
                }
                Singleton<CustomBase>.Instance.customCtrl.cmpGuid[guidNo].amount.position = gameObject.transform.position;
                Singleton<CustomBase>.Instance.customCtrl.cmpGuid[guidNo].amount.rotation = gameObject.transform.eulerAngles;
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.SetAccessoryTransform))]
        private static class CvsAccessory_SetAccessoryTransform_Patches
        {
            private static bool Prefix(CvsAccessory __instance, int guidNo, bool updateInfo)
            {
                int index = (int)__instance.slotNo;
                GameObject gameObject;
                if (index < 20)
                    gameObject = CustomBase.Instance.chaCtrl.objAcsMove[index, guidNo];
                else
                    gameObject = MoreAccessories._self._charaMakerData.objAcsMove[index - 20][guidNo];
                if (null == gameObject)
                {
                    return false;
                }
                if (__instance.tglControllerType01 == null || (__instance.tglControllerType01).Length == 0)
                {
                    return false;
                }
                if (__instance.tglControllerType02 == null || (__instance.tglControllerType02).Length == 0)
                {
                    return false;
                }
                Toggle[] array =
                {
                    guidNo != 0 ? (__instance.tglControllerType02)[0] : (__instance.tglControllerType01)[0],
                    guidNo != 0 ? (__instance.tglControllerType02)[1] : (__instance.tglControllerType01)[1]
                };
                if (array[0].isOn)
                {
                    gameObject.transform.position = Singleton<CustomBase>.Instance.customCtrl.cmpGuid[guidNo].amount.position;
                    if (updateInfo)
                    {
                        Vector3 localPosition = gameObject.transform.localPosition;
                        localPosition.x = Mathf.Clamp(localPosition.x * 100f, -100f, 100f);
                        localPosition.y = Mathf.Clamp(localPosition.y * 100f, -100f, 100f);
                        localPosition.z = Mathf.Clamp(localPosition.z * 100f, -100f, 100f);
                        CustomBase.Instance.chaCtrl.SetAccessoryPos((int)__instance.slotNo, guidNo, localPosition.x, false, 1);
                        CustomBase.Instance.chaCtrl.SetAccessoryPos((int)__instance.slotNo, guidNo, localPosition.y, false, 2);
                        CustomBase.Instance.chaCtrl.SetAccessoryPos((int)__instance.slotNo, guidNo, localPosition.z, false, 4);
                        if ((int)__instance.slotNo < 20)
                            CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo].addMove[guidNo, 0] = MoreAccessories._self.GetPart((int)__instance.slotNo).addMove[guidNo, 0];

                        CustomBase.Instance.chaCtrl.UpdateAccessoryMoveFromInfo((int)__instance.slotNo);
                        Singleton<CustomBase>.Instance.customCtrl.cmpGuid[guidNo].amount.position = gameObject.transform.position;
                    }
                }
                else
                {
                    gameObject.transform.eulerAngles = Singleton<CustomBase>.Instance.customCtrl.cmpGuid[guidNo].amount.rotation;
                    if (updateInfo)
                    {
                        Vector3 localEulerAngles = gameObject.transform.localEulerAngles;
                        CustomBase.Instance.chaCtrl.SetAccessoryRot((int)__instance.slotNo, guidNo, localEulerAngles.x, false, 1);
                        CustomBase.Instance.chaCtrl.SetAccessoryRot((int)__instance.slotNo, guidNo, localEulerAngles.y, false, 2);
                        CustomBase.Instance.chaCtrl.SetAccessoryRot((int)__instance.slotNo, guidNo, localEulerAngles.z, false, 4);
                        if ((int)__instance.slotNo < 20)
                            CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo].addMove[guidNo, 1] = MoreAccessories._self.GetPart((int)__instance.slotNo).addMove[guidNo, 1];

                        CustomBase.Instance.chaCtrl.UpdateAccessoryMoveFromInfo((int)__instance.slotNo);
                        Singleton<CustomBase>.Instance.customCtrl.cmpGuid[guidNo].amount.rotation = gameObject.transform.eulerAngles;
                    }
                }
                __instance.UpdateCustomUI();
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.UpdateDrawControllerState))]
        private static class CvsAccessory_UpdateDrawControllerState_Patches
        {
            private static bool Prefix(CvsAccessory __instance, int guidNo)
            {
                if (__instance.tglControllerType01 == null || (__instance.tglControllerType01).Length == 0)
                {
                    return false;
                }
                if (__instance.tglControllerType02 == null || (__instance.tglControllerType02).Length == 0)
                {
                    return false;
                }
                Toggle toggle = guidNo != 0 ? __instance.tglDrawController02 : __instance.tglDrawController01;
                Toggle[] array =
                {
                    guidNo != 0 ? (__instance.tglControllerType02)[0] : (__instance.tglControllerType01)[0],
                    guidNo != 0 ? (__instance.tglControllerType02)[1] : (__instance.tglControllerType01)[1]
                };
                Slider slider = guidNo != 0 ? __instance.sldControllerSpeed02 : __instance.sldControllerSpeed01;
                Slider slider2 = guidNo != 0 ? __instance.sldControllerScale02 : __instance.sldControllerScale01;
                toggle.isOn = Singleton<CustomBase>.Instance.customSettingSave.drawController[guidNo];
                if (Singleton<CustomBase>.Instance.customSettingSave.controllerType[guidNo] == 0)
                {
                    if (array[0])
                    {
                        array[0].isOn = true;
                    }
                    if (array[1])
                    {
                        array[1].isOn = false;
                    }
                }
                else
                {
                    if (array[0])
                    {
                        array[0].isOn = false;
                    }
                    if (array[1])
                    {
                        array[1].isOn = true;
                    }
                }
                slider.value = Singleton<CustomBase>.Instance.customSettingSave.controllerSpeed[guidNo];
                slider2.value = Singleton<CustomBase>.Instance.customSettingSave.controllerScale[guidNo];
                return false;
            }
        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.Start))]
        private static class CvsAccessory_Start_Patches
        {
            private static bool Prefix(CvsAccessory __instance)
            {
                _ = Test(__instance);
                return false;
            }

            private static async UniTask Test(CvsAccessory __instance)
            {
                __instance.enabled = false;

                await UniTask.WaitUntil(() => CustomBase.instance.chaCtrl != null, PlayerLoopTiming.Update, default);

                if (MoreAccessories._self._accessoriesByChar.TryGetValue(CustomBase.Instance.chaCtrl.chaFile, out MoreAccessories._self._charaMakerData) == false)
                {
                    MoreAccessories._self._charaMakerData = new MoreAccessories.CharAdditionalData();
                    MoreAccessories._self._accessoriesByChar.Add(CustomBase.Instance.chaCtrl.chaFile, MoreAccessories._self._charaMakerData);
                }
                if (MoreAccessories._self._charaMakerData.nowAccessories == null)
                {
                    MoreAccessories._self._charaMakerData.nowAccessories = new List<ChaFileAccessory.PartsInfo>();
                    MoreAccessories._self._charaMakerData.rawAccessoriesInfos.Add(CustomBase.Instance.chaCtrl.fileStatus.GetCoordinateType(), MoreAccessories._self._charaMakerData.nowAccessories);
                }

                int nSlotNo = (int)__instance.slotNo;
                if (nSlotNo < 20)
                {
                    Singleton<CustomBase>.Instance.actUpdateCvsAccessory[nSlotNo] = (Action)Delegate.Combine(Singleton<CustomBase>.Instance.actUpdateCvsAccessory[nSlotNo], new Action(__instance.UpdateCustomUI));
                    Singleton<CustomBase>.Instance.actUpdateAcsSlotName[nSlotNo] = (Action)Delegate.Combine(Singleton<CustomBase>.Instance.actUpdateAcsSlotName[nSlotNo], new Action(__instance.UpdateSlotName));
                }
                __instance.tglTakeOverParent.OnValueChangedAsObservable().Subscribe(delegate (bool isOn) { Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverParent = isOn; });
                __instance.tglTakeOverColor.OnValueChangedAsObservable().Subscribe(delegate (bool isOn) { Singleton<CustomBase>.Instance.customSettingSave.acsTakeOverColor = isOn; });
                __instance.ddAcsType.onValueChanged.AddListener(delegate (int idx)
                {
                    __instance.UpdateSelectAccessoryType(idx);
                    bool visible = idx != 0;
                    __instance.ChangeSettingVisible(visible);
                });
                __instance.tglAcsKind.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                {
                    int num = (__instance.ddAcsType).value - 1;
                    if (0 <= num)
                    {
                        if (__instance.cgAccessoryWin[num] && 0f != __instance.cgAccessoryWin[num].alpha != isOn)
                        {
                            __instance.cgAccessoryWin[num].Enable(isOn, false);
                            if (isOn)
                            {
                                __instance.tglAcsParent.isOn = false;
                                __instance.tglAcsMove01.isOn = false;
                                __instance.tglAcsMove02.isOn = false;
                                for (int i = 0; i < __instance.cgAccessoryWin.Length; i++)
                                {
                                    if (i != num)
                                    {
                                        __instance.cgAccessoryWin[i].Enable(false, false);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < __instance.cgAccessoryWin.Length; j++)
                        {
                            __instance.cgAccessoryWin[j].Enable(false, false);
                        }
                    }
                });
                __instance.tglAcsParent.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                {
                    if (__instance.cgAcsParent)
                    {
                        if (__instance.cgAcsParent && 0f != __instance.cgAcsParent.alpha != isOn)
                        {
                            __instance.cgAcsParent.Enable(isOn, false);
                            if (isOn)
                            {
                                __instance.tglAcsKind.isOn = false;
                                __instance.tglAcsMove01.isOn = false;
                                __instance.tglAcsMove02.isOn = false;
                            }
                        }
                    }
                });
                __instance.btnInitParent.onClick.AsObservable().Subscribe(delegate
                {
                    string accessoryDefaultParentStr = CustomBase.Instance.chaCtrl.GetAccessoryDefaultParentStr((int)__instance.slotNo);
                    MoreAccessories._self.GetPart((int)__instance.slotNo).parentKey = accessoryDefaultParentStr;
                    if ((int)__instance.slotNo < 20)
                        CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo].parentKey = accessoryDefaultParentStr;

                    __instance.FuncUpdateAcsParent(false);
#if KOIKATSU
                    if (MoreAccessories._self._hasDarkness == false)
                        BackwardCompatibility.CustomHistory_Instance_Add2(CustomBase.Instance.chaCtrl, __instance.FuncUpdateAcsParent, true);
#endif
                    __instance.UpdateCustomUI();
                });
                __instance.btnReverseParent.onClick.AsObservable().Subscribe(delegate
                {
                    ChaFileAccessory.PartsInfo part = MoreAccessories._self.GetPart((int)__instance.slotNo);
                    string reverseParent = ChaAccessoryDefine.GetReverseParent(part.parentKey);
                    if (string.Empty != reverseParent)
                    {
                        part.parentKey = reverseParent;
                        if ((int)__instance.slotNo < 20)
                            CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo].parentKey = reverseParent;

                        __instance.FuncUpdateAcsParent(false);
#if KOIKATSU
                        if (MoreAccessories._self._hasDarkness == false)
                            BackwardCompatibility.CustomHistory_Instance_Add2(CustomBase.Instance.chaCtrl, __instance.FuncUpdateAcsParent, true);
#endif
                        __instance.UpdateCustomUI();
                    }
                });
                __instance.btnAcsColor01.OnClickAsObservable().Subscribe(delegate
                {
                    if (__instance.cvsColor.isOpen && (int)__instance.cvsColor.connectColorKind == ((int)__instance.slotNo * 4 + 124))
                    {
                        __instance.cvsColor.Close();
                    }
                    else
                    {
                        __instance.cvsColor.Setup($"スロット{(int)__instance.slotNo + 1:00} カラー①", (CvsColor.ConnectColorKind)((int)__instance.slotNo * 4 + 124), MoreAccessories._self.GetPart((int)__instance.slotNo).color[0], __instance.UpdateAcsColor01, __instance.UpdateAcsColorHistory(), false);
                    }
                });
                __instance.btnAcsColor02.OnClickAsObservable().Subscribe(delegate
                {
                    if (__instance.cvsColor.isOpen && (int)__instance.cvsColor.connectColorKind == ((int)__instance.slotNo * 4 + 124 + 1))
                    {
                        __instance.cvsColor.Close();
                    }
                    else
                    {
                        __instance.cvsColor.Setup($"スロット{(int)__instance.slotNo + 1:00} カラー②", (CvsColor.ConnectColorKind)((int)__instance.slotNo * 4 + 124 + 1), MoreAccessories._self.GetPart((int)__instance.slotNo).color[1], __instance.UpdateAcsColor02, __instance.UpdateAcsColorHistory(), false);
                    }
                });
                __instance.btnAcsColor03.OnClickAsObservable().Subscribe(delegate
                {
                    if (__instance.cvsColor.isOpen && (int)__instance.cvsColor.connectColorKind == ((int)__instance.slotNo * 4 + 124 + 2))
                    {
                        __instance.cvsColor.Close();
                    }
                    else
                    {
                        __instance.cvsColor.Setup($"スロット{(int)__instance.slotNo + 1:00} カラー③", (CvsColor.ConnectColorKind)((int)__instance.slotNo * 4 + 124 + 2), MoreAccessories._self.GetPart((int)__instance.slotNo).color[2], __instance.UpdateAcsColor03, __instance.UpdateAcsColorHistory(), false);
                    }
                });
                __instance.btnAcsColor04.OnClickAsObservable().Subscribe(delegate
                {
                    if (__instance.cvsColor.isOpen && (int)__instance.cvsColor.connectColorKind == ((int)__instance.slotNo * 4 + 124 + 3))
                    {
                        __instance.cvsColor.Close();
                    }
                    else
                    {
                        __instance.cvsColor.Setup($"スロット{(int)__instance.slotNo + 1:00} カラー④", (CvsColor.ConnectColorKind)((int)__instance.slotNo * 4 + 124 + 3), MoreAccessories._self.GetPart((int)__instance.slotNo).color[3], __instance.UpdateAcsColor04, __instance.UpdateAcsColorHistory(), true);
                    }
                });
                __instance.btnInitColor.onClick.AsObservable().Subscribe(delegate
                {
                    __instance.SetDefaultColor();
#if KOIKATSU
                    if (MoreAccessories._self._hasDarkness == false)
                        __instance.UpdateAcsColorHistory();
#endif
                    __instance.UpdateCustomUI();
                });
                __instance.tglAcsMove01.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                {
                    if (__instance.cgAcsMove01)
                    {
                        bool flag = __instance.cgAcsMove01.alpha != 0f;
                        if (flag != isOn)
                        {
                            __instance.cgAcsMove01.Enable(isOn, false);
                            if (isOn)
                            {
                                __instance.tglAcsKind.isOn = false;
                                __instance.tglAcsParent.isOn = false;
                                (__instance.tglAcsMove02).isOn = false;
                            }
                        }
                    }
                });
                __instance.tglAcsMove02.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                {
                    if (__instance.cgAcsMove02)
                    {
                        bool flag = __instance.cgAcsMove02.alpha != 0f;
                        if (flag != isOn)
                        {
                            __instance.cgAcsMove02.Enable(isOn, false);
                            if (isOn)
                            {
                                __instance.tglAcsKind.isOn = false;
                                __instance.tglAcsParent.isOn = false;
                                (__instance.tglAcsMove01).isOn = false;
                            }
                        }
                    }
                });
                (from item in (__instance.tglAcsGroup).Select((tgl, idx) => new
                {
                    tgl,
                    idx
                })
                 where item.tgl != null
                 select item).ToList().ForEach(item =>
                 {
                     (from isOn in item.tgl.OnValueChangedAsObservable()
                      where isOn
                      select isOn).Subscribe(delegate
                      {
                          ChaFileAccessory.PartsInfo part = MoreAccessories._self.GetPart((int)__instance.slotNo);
                          if (((int)__instance.slotNo >= 20 || !Singleton<CustomBase>.Instance.GetUpdateCvsAccessory((int)__instance.slotNo)) && item.idx != part.hideCategory)
                          {
                              part.hideCategory = item.idx;
                              if ((int)__instance.slotNo < 20)
                                  CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo].hideCategory = item.idx;

#if KOIKATSU
                              __instance.cmpDrawCtrl.UpdateAccessoryDraw();
                              if (MoreAccessories._self._hasDarkness == false)
                                  BackwardCompatibility.CustomHistory_Instance_Add1(CustomBase.Instance.chaCtrl, null);
#endif
                          }
                      });
                 });
#if EMOTIONCREATORS
                        (from item in (__instance.tglHideTiming).Select((Toggle tgl, int idx) => new { tgl, idx })
                         where item.tgl != null
                         select item).ToList().ForEach(item =>
                         {
                             item.tgl.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                             {
                                 ChaFileAccessory.PartsInfo part = MoreAccessories._self.GetPart((int)__instance.slotNo);
                                 if ((int)__instance.slotNo >= 20 || !Singleton<CustomBase>.Instance.updateCustomUI && isOn && !Singleton<CustomBase>.Instance.GetUpdateCvsAccessory((int)__instance.slotNo) && item.idx != part.hideTiming)
                                 {
                                     part.hideTiming = item.idx;
                                     if ((int)__instance.slotNo < 20)
                                         CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo].hideTiming = item.idx;
                                 }
                             });
                         });
#endif
                if (__instance.tglDrawController01)
                {
                    (__instance.tglDrawController01).OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                    {
                        if (!Singleton<CustomBase>.Instance.updateCustomUI)
                        {
                            Singleton<CustomBase>.Instance.customSettingSave.drawController[0] = isOn;
                        }
                    });
                }
                if (__instance.tglControllerType01 != null)
                {
                    (__instance.tglControllerType01).Select((p, idx) => new
                    {
                        toggle = p,
                        index = (byte)idx
                    }).ToList().ForEach(p =>
                    {
                        p.toggle.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                        {
                            if (!Singleton<CustomBase>.Instance.updateCustomUI && isOn)
                            {
                                Singleton<CustomBase>.Instance.customSettingSave.controllerType[0] = p.index;
                                Singleton<CustomBase>.Instance.customCtrl.cmpGuid[0].SetMode(p.index);
                            }
                        });
                    });
                }
                if (__instance.sldControllerSpeed01)
                {
                    __instance.sldControllerSpeed01.onValueChanged.AsObservable().Subscribe(delegate (float val)
                    {
                        Singleton<CustomBase>.Instance.customSettingSave.controllerSpeed[0] = val;
                        Singleton<CustomBase>.Instance.customCtrl.cmpGuid[0].speedMove = val;
                    });
                    __instance.sldControllerSpeed01.OnScrollAsObservable().Subscribe(delegate (PointerEventData scl) { __instance.sldControllerSpeed01.value = Mathf.Clamp(__instance.sldControllerSpeed01.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0.01f, 1f); });
                }
                if (__instance.sldControllerScale01)
                {
                    __instance.sldControllerScale01.onValueChanged.AsObservable().Subscribe(delegate (float val)
                    {
                        Singleton<CustomBase>.Instance.customSettingSave.controllerScale[0] = val;
                        Singleton<CustomBase>.Instance.customCtrl.cmpGuid[0].scaleAxis = val;
                        Singleton<CustomBase>.Instance.customCtrl.cmpGuid[0].UpdateScale();
                    });
                    __instance.sldControllerScale01.OnScrollAsObservable().Subscribe(delegate (PointerEventData scl) { __instance.sldControllerScale01.value = Mathf.Clamp(__instance.sldControllerScale01.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0.3f, 3f); });
                }
                if (__instance.tglDrawController02)
                {
                    (__instance.tglDrawController02).OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                    {
                        if (!Singleton<CustomBase>.Instance.updateCustomUI)
                        {
                            Singleton<CustomBase>.Instance.customSettingSave.drawController[1] = isOn;
                        }
                    });
                }
                if (__instance.tglControllerType02 != null)
                {
                    (__instance.tglControllerType02).Select((p, idx) => new
                    {
                        toggle = p,
                        index = (byte)idx
                    }).ToList().ForEach(p =>
                    {
                        p.toggle.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                        {
                            if (!Singleton<CustomBase>.Instance.updateCustomUI && isOn)
                            {
                                Singleton<CustomBase>.Instance.customSettingSave.controllerType[1] = p.index;
                                Singleton<CustomBase>.Instance.customCtrl.cmpGuid[1].SetMode(p.index);
                            }
                        });
                    });
                }
                if (__instance.sldControllerSpeed02)
                {
                    __instance.sldControllerSpeed02.onValueChanged.AsObservable().Subscribe(delegate (float val)
                    {
                        Singleton<CustomBase>.Instance.customSettingSave.controllerSpeed[1] = val;
                        Singleton<CustomBase>.Instance.customCtrl.cmpGuid[1].speedMove = val;
                    });
                    __instance.sldControllerSpeed02.OnScrollAsObservable().Subscribe(delegate (PointerEventData scl) { __instance.sldControllerSpeed02.value = Mathf.Clamp(__instance.sldControllerSpeed02.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0.01f, 1f); });
                }
                if (__instance.sldControllerScale02)
                {
                    __instance.sldControllerScale02.onValueChanged.AsObservable().Subscribe(delegate (float val)
                    {
                        Singleton<CustomBase>.Instance.customSettingSave.controllerScale[1] = val;
                        Singleton<CustomBase>.Instance.customCtrl.cmpGuid[1].scaleAxis = val;
                        Singleton<CustomBase>.Instance.customCtrl.cmpGuid[1].UpdateScale();
                    });
                    __instance.sldControllerScale02.OnScrollAsObservable().Subscribe(delegate (PointerEventData scl) { __instance.sldControllerScale02.value = Mathf.Clamp(__instance.sldControllerScale02.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0.3f, 3f); });
                }
                if (MoreAccessories._self._hasDarkness)
                    __instance.tglNoShake.OnValueChangedAsObservable().Subscribe(delegate (bool isOn)
                    {
                        ChaFileAccessory.PartsInfo part = MoreAccessories._self.GetPart((int)__instance.slotNo);
                        if (!Singleton<CustomBase>.Instance.updateCustomUI && part.noShake != isOn)
                        {
                            part.noShake = isOn;
                            if ((int)__instance.slotNo < 20)
                                CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[(int)__instance.slotNo].noShake = isOn;
                            CustomBase.Instance.chaCtrl.ChangeShakeAccessory((int)__instance.slotNo);
                        }
                    });
                __instance.enabled = true;
            }
        }

        [HarmonyPatch(typeof(CvsAccessory), nameof(CvsAccessory.LateUpdate))]
        private static class CvsAccessory_LateUpdate_Patches
        {
            private static bool Prefix(CvsAccessory __instance)
            {
                if (__instance.cgSlot.alpha != 1f)
                {
                    return false;
                }
                for (int i = 0; i < 2; i++)
                {
                    if (!(null == Singleton<CustomBase>.Instance.customCtrl.cmpGuid[i]))
                    {
                        if (Singleton<CustomBase>.Instance.customCtrl.cmpGuid[i].isDrag)
                        {
                            __instance.SetAccessoryTransform(i, false);
                        }
                        else if ((__instance.isDrag)[i])
                        {
                            __instance.SetAccessoryTransform(i, true);
                        }
                        else
                        {
                            __instance.SetControllerTransform(i);
                        }
                        __instance.isDrag[i] = Singleton<CustomBase>.Instance.customCtrl.cmpGuid[i].isDrag;
                    }
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(CvsAccessoryChange), nameof(CvsAccessoryChange.Start))]
        internal static class CvsAccessoryChange_Start_Patches
        {
            private static CvsAccessoryChange _instance;
            private static void Postfix(CvsAccessoryChange __instance)
            {
                _instance = __instance;
            }

            internal static void SetSourceIndex(int index)
            {
                _instance.selSrc = index;
            }
            internal static void SetDestinationIndex(int index)
            {
                _instance.selDst = index;
            }
        }

        [HarmonyPatch(typeof(CvsAccessoryChange), nameof(CvsAccessoryChange.CalculateUI))]
        private static class CvsAccessoryChange_CalculateUI_Patches
        {
            private static void Postfix(CvsAccessoryChange __instance)
            {
                if (MoreAccessories._self._charaMakerData.nowAccessories.Count > MoreAccessories._self._additionalCharaMakerSlots.Count)
                    return;
                for (int i = 0; i < MoreAccessories._self._charaMakerData.nowAccessories.Count; i++)
                {
                    MoreAccessories.CharaMakerSlotData slot = MoreAccessories._self._additionalCharaMakerSlots[i];
                    if (slot.transferSlotObject.activeSelf)
                    {
                        ChaFileAccessory.PartsInfo part = MoreAccessories._self._charaMakerData.nowAccessories[i];
                        ListInfoBase listInfo = Singleton<CustomBase>.Instance.chaCtrl.lstCtrl.GetListInfo((ChaListDefine.CategoryNo)part.type, part.id);
                        if (listInfo == null)
                        {
                            slot.transferSourceText.text = "なし";
                            slot.transferDestinationText.text = "なし";
                        }
                        else
                        {
                            slot.transferSourceText.text = listInfo.Name;
                            slot.transferDestinationText.text = listInfo.Name;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CvsAccessoryChange), nameof(CvsAccessoryChange.CopyAcs))]
        private static class CvsAccessoryChange_CopyAcs_Patches
        {
            private static bool Prefix(CvsAccessoryChange __instance)
            {
                int selSrc = __instance.selSrc;
                int selDst = __instance.selDst;
                byte[] array = MessagePackSerializer.Serialize(MoreAccessories._self.GetPart(selSrc));
                MoreAccessories._self.SetPart(selDst, MessagePackSerializer.Deserialize<ChaFileAccessory.PartsInfo>(array));
                ChaFileAccessory.PartsInfo dstPart = MoreAccessories._self.GetPart(selDst);
                if ((__instance.tglReverse).isOn)
                {
                    string reverseParent = ChaAccessoryDefine.GetReverseParent(dstPart.parentKey);
                    if (string.Empty != reverseParent)
                    {
                        dstPart.parentKey = reverseParent;
                    }
                }
                Singleton<CustomBase>.Instance.chaCtrl.AssignCoordinate(
#if KOIKATSU
                        (ChaFileDefine.CoordinateType)Singleton<CustomBase>.Instance.chaCtrl.fileStatus.GetCoordinateType()
#endif
                        );
                Singleton<CustomBase>.Instance.chaCtrl.Reload(false, true, true, true);
                __instance.CalculateUI();

                (__instance.cmpAcsChangeSlot).UpdateSlotNames();
                CvsAccessory accessory = MoreAccessories._self.GetCvsAccessory(selDst);
                if (selDst == CustomBase.Instance.selectSlot)
                    accessory.UpdateCustomUI();
                accessory.UpdateSlotName();
#if KOIKATSU
                if (MoreAccessories._self._hasDarkness == false)
                    BackwardCompatibility.CustomHistory_Instance_Add5(Singleton<CustomBase>.Instance.chaCtrl, Singleton<CustomBase>.Instance.chaCtrl.Reload, false, true, true, true);
#endif
                return false;
            }
        }

        [HarmonyPatch(typeof(CvsAccessoryChange), nameof(CvsAccessoryChange.CopyAcsCorrect))]
        private static class CvsAccessoryChange_CopyAcsCorrect_Patches
        {
            private static bool Prefix(CvsAccessoryChange __instance, int correctNo)
            {
                int selSrc = __instance.selSrc;
                int selDst = __instance.selDst;
                ChaFileAccessory.PartsInfo srcPart = MoreAccessories._self.GetPart(selSrc);
                ChaFileAccessory.PartsInfo dstPart = MoreAccessories._self.GetPart(selDst);
                ChaFileAccessory.PartsInfo setPart = null;
                if (selDst < 20)
                    setPart = CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[selDst];
                for (int i = 0; i < 3; i++)
                {
                    Vector2 value = srcPart.addMove[correctNo, i];
                    if (selDst < 20)
                        setPart.addMove[correctNo, i] = value;
                    dstPart.addMove[correctNo, i] = value;
                }
                Singleton<CustomBase>.Instance.chaCtrl.UpdateAccessoryMoveFromInfo(selDst);
#if KOIKATSU
                if (MoreAccessories._self._hasDarkness == false)
                    BackwardCompatibility.CustomHistory_Instance_Add1(Singleton<CustomBase>.Instance.chaCtrl, Singleton<CustomBase>.Instance.chaCtrl.UpdateAccessoryMoveAllFromInfo);
#endif
                return false;
            }

        }

        [HarmonyPatch(typeof(CvsAccessoryChange), nameof(CvsAccessoryChange.CopyAcsCorrectRevLR))]
        private static class CvsAccessoryChange_CopyAcsCorrectRevLR_Patches
        {
            private static bool Prefix(CvsAccessoryChange __instance, int correctNo)
            {
                int selSrc = __instance.selSrc;
                int selDst = __instance.selDst;
                ChaFileAccessory.PartsInfo srcPart = MoreAccessories._self.GetPart(selSrc);
                ChaFileAccessory.PartsInfo dstPart = MoreAccessories._self.GetPart(selDst);
                ChaFileAccessory.PartsInfo setPart = null;
                if (selDst < 20)
                    setPart = CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[selDst];

                for (int i = 0; i < 3; i++)
                {
                    Vector3 vector = srcPart.addMove[correctNo, i];
                    if (i == 1)
                    {
                        vector.y += 180f;
                        if (vector.y >= 360f)
                        {
                            vector.y -= 360f;
                        }
                    }
                    if (selDst < 20)
                        setPart.addMove[correctNo, i] = vector;
                    dstPart.addMove[correctNo, i] = vector;
                }
                Singleton<CustomBase>.Instance.chaCtrl.UpdateAccessoryMoveFromInfo(selDst);
#if KOIKATSU
                if (MoreAccessories._self._hasDarkness == false)
                    BackwardCompatibility.CustomHistory_Instance_Add1(Singleton<CustomBase>.Instance.chaCtrl, Singleton<CustomBase>.Instance.chaCtrl.UpdateAccessoryMoveAllFromInfo);
#endif
                return false;
            }
        }

        [HarmonyPatch(typeof(CvsAccessoryChange), nameof(CvsAccessoryChange.CopyAcsCorrectRevUD))]
        private static class CvsAccessoryChange_CopyAcsCorrectRevUD_Patches
        {
            private static bool Prefix(CvsAccessoryChange __instance, int correctNo)
            {
                int selSrc = __instance.selSrc;
                int selDst = __instance.selDst;
                ChaFileAccessory.PartsInfo srcPart = MoreAccessories._self.GetPart(selSrc);
                ChaFileAccessory.PartsInfo dstPart = MoreAccessories._self.GetPart(selDst);
                ChaFileAccessory.PartsInfo setPart = null;
                if (selDst < 20)
                    setPart = CustomBase.Instance.chaCtrl.chaFile.GetCoordinate(CustomBase.Instance.chaCtrl.chaFile.status.GetCoordinateType()).accessory.parts[selDst];

                for (int i = 0; i < 3; i++)
                {
                    Vector3 vector = srcPart.addMove[correctNo, i];
                    if (i == 1)
                    {
                        vector.x += 180f;
                        if (vector.x >= 360f)
                        {
                            vector.x -= 360f;
                        }
                    }
                    if (selDst < 20)
                        setPart.addMove[correctNo, i] = vector;
                    dstPart.addMove[correctNo, i] = vector;
                }
                Singleton<CustomBase>.Instance.chaCtrl.UpdateAccessoryMoveFromInfo(selDst);
#if KOIKATSU
                if (MoreAccessories._self._hasDarkness == false)
                    BackwardCompatibility.CustomHistory_Instance_Add1(Singleton<CustomBase>.Instance.chaCtrl, Singleton<CustomBase>.Instance.chaCtrl.UpdateAccessoryMoveAllFromInfo);
#endif
                return false;
            }
        }
#if KOIKATSU
        [HarmonyPatch(typeof(CvsAccessoryCopy), nameof(CvsAccessoryCopy.ChangeDstDD))]
        private static class CvsAccessoryCopy_ChangeDstDD_Patches
        {
            private static void Postfix(CvsAccessoryCopy __instance)
            {
                List<ChaFileAccessory.PartsInfo> parts;
                TMP_Dropdown[] ddCoordeType = (__instance.ddCoordeType);
                if (MoreAccessories._self._charaMakerData.rawAccessoriesInfos.TryGetValue(ddCoordeType[0].value, out parts))
                {
                    if (MoreAccessories._self._additionalCharaMakerSlots.Count < parts.Count)
                        return;
                    for (int i = 0; i < MoreAccessories._self._additionalCharaMakerSlots.Count; i++)
                    {
                        if (i < parts.Count)
                        {
                            ChaFileAccessory.PartsInfo part = parts[i];
                            ListInfoBase listInfo = Singleton<CustomBase>.Instance.chaCtrl.lstCtrl.GetListInfo((ChaListDefine.CategoryNo)part.type, part.id);
                            MoreAccessories._self._additionalCharaMakerSlots[i].copyDestinationText.text = listInfo == null ? "なし" : listInfo.Name;
                        }
                        else
                            MoreAccessories._self._additionalCharaMakerSlots[i].copyDestinationText.text = "なし";
                    }
                }
                else
                {
                    for (int i = 0; i < MoreAccessories._self._additionalCharaMakerSlots.Count; i++)
                        MoreAccessories._self._additionalCharaMakerSlots[i].copyDestinationText.text = "なし";
                }
                int srcCount = 0;
                if (MoreAccessories._self._charaMakerData.rawAccessoriesInfos.TryGetValue(ddCoordeType[1].value, out parts))
                    srcCount = parts.Count;
                int j = 0;
                for (; j < srcCount; j++)
                    MoreAccessories._self._additionalCharaMakerSlots[j].copySlotObject.SetActive(true);
                for (; j < MoreAccessories._self._additionalCharaMakerSlots.Count; ++j)
                    MoreAccessories._self._additionalCharaMakerSlots[j].copySlotObject.SetActive(false);
            }
        }

        [HarmonyPatch(typeof(CvsAccessoryCopy), nameof(CvsAccessoryCopy.ChangeSrcDD))]
        private static class CvsAccessoryCopy_ChangeSrcDD_Patches
        {
            private static void Postfix(CvsAccessoryCopy __instance)
            {
                List<ChaFileAccessory.PartsInfo> parts;
                TMP_Dropdown[] ddCoordeType = (__instance.ddCoordeType);
                int srcCount = 0;
                if (MoreAccessories._self._charaMakerData.rawAccessoriesInfos.TryGetValue(ddCoordeType[1].value, out parts))
                {
                    if (MoreAccessories._self._additionalCharaMakerSlots.Count < parts.Count)
                        return;
                    srcCount = parts.Count;
                    for (int i = 0; i < MoreAccessories._self._additionalCharaMakerSlots.Count; i++)
                    {
                        if (i < parts.Count)
                        {
                            ChaFileAccessory.PartsInfo part = parts[i];
                            ListInfoBase listInfo = Singleton<CustomBase>.Instance.chaCtrl.lstCtrl.GetListInfo((ChaListDefine.CategoryNo)part.type, part.id);
                            MoreAccessories._self._additionalCharaMakerSlots[i].copySourceText.text = listInfo == null ? "なし" : listInfo.Name;
                        }
                        else
                            MoreAccessories._self._additionalCharaMakerSlots[i].copySourceText.text = "なし";
                    }
                }
                else
                {
                    for (int i = 0; i < MoreAccessories._self._additionalCharaMakerSlots.Count; i++)
                        MoreAccessories._self._additionalCharaMakerSlots[i].copySourceText.text = "なし";
                }
                int j = 0;
                for (; j < srcCount; j++)
                    MoreAccessories._self._additionalCharaMakerSlots[j].copySlotObject.SetActive(true);
                for (; j < MoreAccessories._self._additionalCharaMakerSlots.Count; ++j)
                    MoreAccessories._self._additionalCharaMakerSlots[j].copySlotObject.SetActive(false);
            }
        }

        [HarmonyPatch(typeof(CvsAccessoryCopy), nameof(CvsAccessoryCopy.CopyAcs))]
        private static class CvsAccessoryCopy_CopyAcs_Patches
        {
            private static void Prefix(CvsAccessoryCopy __instance)
            {
                TMP_Dropdown[] ddCoordeType = (__instance.ddCoordeType);
                List<ChaFileAccessory.PartsInfo> srcParts;
                if (MoreAccessories._self._charaMakerData.rawAccessoriesInfos.TryGetValue(ddCoordeType[1].value, out srcParts) == false)
                {
                    srcParts = new List<ChaFileAccessory.PartsInfo>();
                    MoreAccessories._self._charaMakerData.rawAccessoriesInfos.Add(ddCoordeType[1].value, srcParts);
                }
                List<ChaFileAccessory.PartsInfo> dstParts;
                if (MoreAccessories._self._charaMakerData.rawAccessoriesInfos.TryGetValue(ddCoordeType[0].value, out dstParts) == false)
                {
                    dstParts = new List<ChaFileAccessory.PartsInfo>();
                    MoreAccessories._self._charaMakerData.rawAccessoriesInfos.Add(ddCoordeType[0].value, dstParts);
                }
                for (int i = 0; i < MoreAccessories._self._additionalCharaMakerSlots.Count && i < srcParts.Count; i++)
                {
                    MoreAccessories.CharaMakerSlotData slot = MoreAccessories._self._additionalCharaMakerSlots[i];
                    if (slot.copyToggle.isOn)
                    {
                        byte[] array = MessagePackSerializer.Serialize(srcParts[i]);
                        while (i >= dstParts.Count)
                        {
                            ChaFileAccessory.PartsInfo part = new ChaFileAccessory.PartsInfo();
                            dstParts.Add(part);
                        }
                        dstParts[i] = MessagePackSerializer.Deserialize<ChaFileAccessory.PartsInfo>(array);
                    }
                }
                MoreAccessories._self.ExecuteDelayed(MoreAccessories._self.UpdateUI);
            }
        }


        [HarmonyPatch(typeof(CvsAccessoryCopy), nameof(CvsAccessory.Start))]
        private static class CvsAccessoryCopy_Start_Patches
        {
            private static void Postfix(CvsAccessoryCopy __instance)
            {
                (__instance.btnAllOn).onClick.AddListener(() =>
                {
                    foreach (MoreAccessories.CharaMakerSlotData slot in MoreAccessories._self._additionalCharaMakerSlots)
                    {
                        slot.copyToggle.isOn = true;
                    }
                });
                (__instance.btnAllOff).onClick.AddListener(() =>
                {
                    foreach (MoreAccessories.CharaMakerSlotData slot in MoreAccessories._self._additionalCharaMakerSlots)
                    {
                        slot.copyToggle.isOn = false;
                    }
                });
            }
        }
#endif
    }
}
