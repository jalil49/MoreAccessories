using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
#if EMOTIONCREATORS
using ADVPart.Manipulate.Chara;
using HEdit;
using HPlay;
#endif
using Illusion.Extensions;
using IllusionUtility.GetUtility;
using Manager;
using UnityEngine;
using System.Linq;
#if EMOTIONCREATORS
using ADVPart.Manipulate.Chara;
using HEdit;
using HPlay;
#endif
using MessagePack;
using MoreAccessoriesKOI.Extensions;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public static class ChaControl_Patches
        {
            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.GetAccessoryDefaultParentStr))]
            internal static class ChaControl_GetAccessoryDefaultParentStr_Patches
            {

                private static bool Prefix(ChaControl __instance, int slotNo, ref string __result)
                {
                    GameObject gameObject;
                    if (slotNo < 20)
                        gameObject = __instance.objAccessory[slotNo];
                    else
                        gameObject = _self._accessoriesByChar[__instance.chaFile].objAccessory[slotNo - 20];
                    if (null == gameObject)
                    {
                        __result = string.Empty;
                        return false;
                    }
                    var component = gameObject.GetComponent<ListInfoComponent>();
                    __result = component.data.GetInfo(ChaListDefine.KeyType.Parent);
                    return false;
                }
            }

            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.GetAccessoryDefaultColor))]
            internal static class ChaControl_GetAccessoryDefaultColor_Patches
            {
                private static bool Prefix(ChaControl __instance, ref Color color, int slotNo, int no, ref bool __result)
                {
                    ChaAccessoryComponent chaAccessoryComponent;
                    if (slotNo < 20)
                        chaAccessoryComponent = __instance.cusAcsCmp[slotNo];
                    else
                        chaAccessoryComponent = _self._accessoriesByChar[__instance.chaFile].cusAcsCmp[slotNo - 20];

                    if (null == chaAccessoryComponent)
                    {
                        __result = false;
                        return false;
                    }
                    if (no == 0 && chaAccessoryComponent.useColor01)
                    {
                        color = chaAccessoryComponent.defColor01;
                        __result = true;
                        return false;
                    }
                    if (no == 1 && chaAccessoryComponent.useColor02)
                    {
                        color = chaAccessoryComponent.defColor02;
                        __result = true;
                        return false;
                    }
                    if (no == 2 && chaAccessoryComponent.useColor03)
                    {
                        color = chaAccessoryComponent.defColor03;
                        __result = true;
                        return false;
                    }
                    if (no == 3 && chaAccessoryComponent.rendAlpha != null && chaAccessoryComponent.rendAlpha.Length != 0)
                    {
                        color = chaAccessoryComponent.defColor04;
                        __result = true;
                        return false;
                    }
                    __result = false;
                    return false;
                }
            }

            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.SetAccessoryPos))]
            internal static class ChaControl_SetAccessoryPos_Patches
            {
                private static bool Prefix(ChaControl __instance, int slotNo, int correctNo, float value, bool add, int flags, ref bool __result)
                {
                    GameObject gameObject;

                    if (slotNo < 20)
                        gameObject = __instance.objAcsMove[slotNo, correctNo];
                    else
                        gameObject = _self._accessoriesByChar[__instance.chaFile].objAcsMove[slotNo - 20][correctNo];

                    if (null == gameObject)
                    {
                        __result = false;
                        return false;
                    }
                    ChaFileAccessory.PartsInfo part;
                    if (slotNo < 20)
                        part = __instance.nowCoordinate.accessory.parts[slotNo];
                    else
                        part = _self._accessoriesByChar[__instance.chaFile].nowAccessories[slotNo - 20];
                    if ((flags & 1) != 0)
                    {
                        var num = float.Parse((((!add) ? 0f : part.addMove[correctNo, 0].x) + value).ToString("f1"));
                        part.addMove[correctNo, 0].x = Mathf.Clamp(num, -100f, 100f);
                    }
                    if ((flags & 2) != 0)
                    {
                        var num2 = float.Parse((((!add) ? 0f : part.addMove[correctNo, 0].y) + value).ToString("f1"));
                        part.addMove[correctNo, 0].y = Mathf.Clamp(num2, -100f, 100f);
                    }
                    if ((flags & 4) != 0)
                    {
                        var num3 = float.Parse((((!add) ? 0f : part.addMove[correctNo, 0].z) + value).ToString("f1"));
                        part.addMove[correctNo, 0].z = Mathf.Clamp(num3, -100f, 100f);
                    }
                    var categoryInfo = __instance.lstCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)part.type);
                    ListInfoBase listInfoBase = null;
                    categoryInfo.TryGetValue(part.id, out listInfoBase);
                    if (listInfoBase.GetInfoInt(ChaListDefine.KeyType.HideHair) == 1)
                    {
                        gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
                    }
                    else
                    {
                        gameObject.transform.localPosition = new Vector3(part.addMove[correctNo, 0].x * 0.01f, part.addMove[correctNo, 0].y * 0.01f, part.addMove[correctNo, 0].z * 0.01f);
                    }
                    __result = true;
                    return false;
                }
            }

            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.SetAccessoryRot))]
            internal static class ChaControl_SetAccessoryRot_Patches
            {
                private static bool Prefix(ChaControl __instance, int slotNo, int correctNo, float value, bool add, int flags, ref bool __result)
                {
                    GameObject gameObject;

                    if (slotNo < 20)
                        gameObject = __instance.objAcsMove[slotNo, correctNo];
                    else
                        gameObject = _self._accessoriesByChar[__instance.chaFile].objAcsMove[slotNo - 20][correctNo];

                    if (null == gameObject)
                    {
                        __result = false;
                        return false;
                    }
                    ChaFileAccessory.PartsInfo part;
                    if (slotNo < 20)
                        part = __instance.nowCoordinate.accessory.parts[slotNo];
                    else
                        part = _self._accessoriesByChar[__instance.chaFile].nowAccessories[slotNo - 20];
                    if ((flags & 1) != 0)
                    {
                        float num = (int)(((!add) ? 0f : part.addMove[correctNo, 1].x) + value);
                        part.addMove[correctNo, 1].x = Mathf.Repeat(num, 360f);
                    }
                    if ((flags & 2) != 0)
                    {
                        float num2 = (int)((!add ? 0f : part.addMove[correctNo, 1].y) + value);
                        part.addMove[correctNo, 1].y = Mathf.Repeat(num2, 360f);
                    }
                    if ((flags & 4) != 0)
                    {
                        float num3 = (int)(((!add) ? 0f : part.addMove[correctNo, 1].z) + value);
                        part.addMove[correctNo, 1].z = Mathf.Repeat(num3, 360f);
                    }
                    var categoryInfo = __instance.lstCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)part.type);
                    ListInfoBase listInfoBase = null;
                    categoryInfo.TryGetValue(part.id, out listInfoBase);
                    if (listInfoBase.GetInfoInt(ChaListDefine.KeyType.HideHair) == 1)
                    {
                        gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                    }
                    else
                    {
                        gameObject.transform.localEulerAngles = new Vector3(part.addMove[correctNo, 1].x, part.addMove[correctNo, 1].y, part.addMove[correctNo, 1].z);
                    }
                    __result = true;
                    return false;
                }
            }

            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.SetAccessoryScl))]
            internal static class ChaControl_SetAccessoryScl_Patches
            {
                private static bool Prefix(ChaControl __instance, int slotNo, int correctNo, float value, bool add, int flags, ref bool __result)
                {
                    GameObject gameObject;

                    if (slotNo < 20)
                        gameObject = __instance.objAcsMove[slotNo, correctNo];
                    else
                        gameObject = _self._accessoriesByChar[__instance.chaFile].objAcsMove[slotNo - 20][correctNo];

                    if (null == gameObject)
                    {
                        __result = false;
                        return false;
                    }
                    ChaFileAccessory.PartsInfo part;
                    if (slotNo < 20)
                        part = __instance.nowCoordinate.accessory.parts[slotNo];
                    else
                        part = _self._accessoriesByChar[__instance.chaFile].nowAccessories[slotNo - 20];
                    if ((flags & 1) != 0)
                    {
                        var num = float.Parse((((!add) ? 0f : part.addMove[correctNo, 2].x) + value).ToString("f2"));
                        part.addMove[correctNo, 2].x = Mathf.Clamp(num, -100f, 100f);
                    }
                    if ((flags & 2) != 0)
                    {
                        var num2 = float.Parse((((!add) ? 0f : part.addMove[correctNo, 2].y) + value).ToString("f2"));
                        part.addMove[correctNo, 2].y = Mathf.Clamp(num2, -100f, 100f);
                    }
                    if ((flags & 4) != 0)
                    {
                        var num3 = float.Parse((((!add) ? 0f : part.addMove[correctNo, 2].z) + value).ToString("f2"));
                        part.addMove[correctNo, 2].z = Mathf.Clamp(num3, -100f, 100f);
                    }
                    var categoryInfo = __instance.lstCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)part.type);
                    ListInfoBase listInfoBase = null;
                    categoryInfo.TryGetValue(part.id, out listInfoBase);
                    if (listInfoBase.GetInfoInt(ChaListDefine.KeyType.HideHair) == 1)
                    {
                        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                    else
                    {
                        gameObject.transform.localScale = new Vector3(part.addMove[correctNo, 2].x, part.addMove[correctNo, 2].y, part.addMove[correctNo, 2].z);
                    }
                    __result = true;
                    return false;
                }
            }

            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.UpdateAccessoryMoveFromInfo))]
            internal static class ChaControl_UpdateAccessoryMoveFromInfo_Patches
            {
                private static bool Prefix(ChaControl __instance, int slotNo, ref bool __result)
                {
                    ChaFileAccessory.PartsInfo part;
                    CharAdditionalData data = null;
                    if (slotNo < 20)
                        part = __instance.nowCoordinate.accessory.parts[slotNo];
                    else
                    {
                        data = _self._accessoriesByChar[__instance.chaFile];
                        part = data.nowAccessories[slotNo - 20];
                    }
                    var categoryInfo = __instance.lstCtrl.GetCategoryInfo((ChaListDefine.CategoryNo)part.type);
                    ListInfoBase listInfoBase = null;
                    categoryInfo.TryGetValue(part.id, out listInfoBase);
                    if (listInfoBase.GetInfoInt(ChaListDefine.KeyType.HideHair) == 1)
                    {
                        for (var i = 0; i < 2; i++)
                        {
                            GameObject gameObject;
                            if (slotNo < 20)
                                gameObject = __instance.objAcsMove[slotNo, i];
                            else
                                gameObject = data.objAcsMove[slotNo - 20][i];
                            if (!(null == gameObject))
                            {
                                gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
                                gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                                gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                            }
                        }
                    }
                    else
                    {
                        for (var j = 0; j < 2; j++)
                        {
                            GameObject gameObject2;
                            if (slotNo < 20)
                                gameObject2 = __instance.objAcsMove[slotNo, j];
                            else
                                gameObject2 = data.objAcsMove[slotNo - 20][j];
                            if (!(null == gameObject2))
                            {
                                gameObject2.transform.localPosition = new Vector3(part.addMove[j, 0].x * 0.01f, part.addMove[j, 0].y * 0.01f, part.addMove[j, 0].z * 0.01f);
                                gameObject2.transform.localEulerAngles = new Vector3(part.addMove[j, 1].x, part.addMove[j, 1].y, part.addMove[j, 1].z);
                                gameObject2.transform.localScale = new Vector3(part.addMove[j, 2].x, part.addMove[j, 2].y, part.addMove[j, 2].z);
                            }
                        }
                    }
                    return true;
                }
            }

            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.ChangeAccessoryColor))]
            internal static class ChaControl_ChangeAccessoryColor_Patches
            {
                private static bool Prefix(ChaControl __instance, int slotNo, ref bool __result)
                {
                    ChaAccessoryComponent chaAccessoryComponent;
                    if (slotNo < 20)
                        chaAccessoryComponent = __instance.cusAcsCmp[slotNo];
                    else
                        chaAccessoryComponent = _self._accessoriesByChar[__instance.chaFile].cusAcsCmp[slotNo - 20];
                    if (null == chaAccessoryComponent)
                    {
                        __result = false;
                        return false;
                    }
                    ChaFileAccessory.PartsInfo partsInfo;
                    if (slotNo < 20)
                        partsInfo = __instance.nowCoordinate.accessory.parts[slotNo];
                    else
                        partsInfo = _self._accessoriesByChar[__instance.chaFile].nowAccessories[slotNo - 20];

                    if (chaAccessoryComponent.rendNormal != null)
                    {
                        foreach (var renderer in chaAccessoryComponent.rendNormal)
                        {
                            if (chaAccessoryComponent.useColor01)
                            {
                                renderer.material.SetColor(ChaShader._Color, partsInfo.color[0]);
                            }
                            if (chaAccessoryComponent.useColor02)
                            {
                                renderer.material.SetColor(ChaShader._Color2, partsInfo.color[1]);
                            }
                            if (chaAccessoryComponent.useColor03)
                            {
                                renderer.material.SetColor(ChaShader._Color3, partsInfo.color[2]);
                            }
                        }
                    }
                    if (chaAccessoryComponent.rendAlpha != null)
                    {
                        foreach (var renderer2 in chaAccessoryComponent.rendAlpha)
                        {
                            renderer2.material.SetColor(ChaShader._Color4, partsInfo.color[3]);
                            renderer2.gameObject.SetActiveIfDifferent(partsInfo.color[3].a != 0f);
                        }
                    }
                    if (chaAccessoryComponent.rendHair != null)
                    {
                        var startColor = __instance.fileHair.parts[0].startColor;
                        foreach (var renderer3 in chaAccessoryComponent.rendHair)
                        {
                            renderer3.material.SetColor(ChaShader._Color, startColor);
                            renderer3.material.SetColor(ChaShader._Color2, startColor);
                            renderer3.material.SetColor(ChaShader._Color3, startColor);
                        }
                    }
                    __result = true;
                    return false;
                }
            }


            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.ChangeAccessoryParent))]
            internal static class ChaControl_ChangeAccessoryParent_Patches
            {
                private static bool Prefix(ChaControl __instance, int slotNo, string parentStr, ref bool __result)
                {
                    GameObject gameObject;
                    CharAdditionalData additionalData = null;
                    if (slotNo < 20)
                        gameObject = __instance.objAccessory[slotNo];
                    else
                    {
                        additionalData = _self._accessoriesByChar[__instance.chaFile];
                        gameObject = additionalData.objAccessory[slotNo - 20];

                    }
                    if (null == gameObject)
                    {
                        __result = false;
                        return false;
                    }
                    if ("none" == parentStr)
                    {
                        gameObject.transform.SetParent(null, false);
                        __result = true;
                        return false;
                    }
                    var component = gameObject.GetComponent<ListInfoComponent>();
                    var data = component.data;
                    if ("0" == data.GetInfo(ChaListDefine.KeyType.Parent))
                    {
                        __result = false;
                        return false;
                    }
                    try
                    {
                        var key = (ChaReference.RefObjKey)Enum.Parse(typeof(ChaReference.RefObjKey), parentStr);
                        var referenceInfo = __instance.GetReferenceInfo(key);
                        if (null == referenceInfo)
                        {
                            return false;
                        }
                        gameObject.transform.SetParent(referenceInfo.transform, false);

                        if (slotNo < 20)
                        {
                            __instance.nowCoordinate.accessory.parts[slotNo].parentKey = parentStr;
                            __instance.nowCoordinate.accessory.parts[slotNo].partsOfHead = ChaAccessoryDefine.CheckPartsOfHead(parentStr);
                        }
                        else
                        {
                            additionalData.nowAccessories[slotNo - 20].parentKey = parentStr;
                            additionalData.nowAccessories[slotNo - 20].partsOfHead = ChaAccessoryDefine.CheckPartsOfHead(parentStr);
                        }
                    }
                    catch (ArgumentException)
                    {
                        __result = false;
                        return false;
                    }
                    __result = true;
                    return true;
                }
            }

            internal static class ChaControl_ChangeAccessory_Patches
            {
                private static MethodInfo _loadCharaFbxData;
#if KOIKATSU
                private static readonly object[] _params = new object[10];
#elif EMOTIONCREATORS
        private static readonly object[] _params = new object[8];
#endif

                internal static void ManualPatch(Harmony harmony)
                {
                    foreach (var info in typeof(ChaControl).GetMethods(BindingFlags.Instance | BindingFlags.Public))
                    {
                        switch (info.Name)
                        {
                            case "ChangeAccessory":
                                if (info.GetParameters().Length < 3)
                                    harmony.Patch(info, new HarmonyMethod(typeof(ChaControl_ChangeAccessory_Patches).GetMethod(nameof(GroupUpdatePrefix), BindingFlags.NonPublic | BindingFlags.Static)), null);
                                else
                                    harmony.Patch(info, new HarmonyMethod(typeof(ChaControl_ChangeAccessory_Patches).GetMethod(nameof(IndividualPrefix), BindingFlags.NonPublic | BindingFlags.Static)), null);
                                break;
                            case "ChangeAccessoryAsync":
                                if (info.GetParameters().Length == 1)
                                    harmony.Patch(info, new HarmonyMethod(typeof(ChaControl_ChangeAccessory_Patches).GetMethod(nameof(GroupPrefix), BindingFlags.NonPublic | BindingFlags.Static)), null);
                                break;
                        }
                    }
                }

                private static void GroupPrefix(ChaControl __instance, bool forceChange)
                {
                    CharAdditionalData data;
                    if (_self._accessoriesByChar.TryGetValue(__instance.chaFile, out data) == false)
                        return;
                    int i;
                    for (i = 0; i < data.nowAccessories.Count; i++)
                    {
                        var part = data.nowAccessories[i];
                        ChangeAccessory(__instance, i, part.type, part.id, part.parentKey, forceChange, true);
                    }
                    for (; i < data.objAccessory.Count; i++)
                    {
                        CleanRemainingAccessory(__instance, data, i);
                    }
                }

                private static void GroupUpdatePrefix(ChaControl __instance, bool forceChange, bool update = true)
                {
                    CharAdditionalData data;
                    if (_self._accessoriesByChar.TryGetValue(__instance.chaFile, out data) == false)
                        return;
                    int i;
                    for (i = 0; i < data.nowAccessories.Count; i++)
                    {
                        var part = data.nowAccessories[i];
                        ChangeAccessory(__instance, i, part.type, part.id, part.parentKey, forceChange, update);
                    }
                    for (; i < data.objAccessory.Count; i++)
                    {
                        CleanRemainingAccessory(__instance, data, i);
                    }
                }

                private static bool IndividualPrefix(ChaControl __instance, int slotNo, int type, int id, string parentKey, bool forceChange = false)
                {
                    if (slotNo >= 20)
                    {
                        ChangeAccessory(__instance, slotNo - 20, type, id, parentKey, forceChange, false);
                        return false;
                    }
                    return true;
                }

                private static void ChangeAccessory(ChaControl instance, int slotNo, int type, int id, string parentKey, bool forceChange = false, bool update = true)
                {
                    ListInfoBase lib = null;
                    var load = true;
                    var release = true;
                    bool typerelease;
                    if (Game.isAddH)
                    {
                        typerelease = (120 == type || !MathfEx.RangeEqualOn(121, type, 130));
                    }
                    else
                    {
                        typerelease = (120 == type || !MathfEx.RangeEqualOn(121, type, 129));
                    }
                    var data = _self._accessoriesByChar[instance.chaFile];

                    if (typerelease)
                    {
                        release = true;
                        load = false;
                    }
                    else
                    {
                        if (id == -1)
                        {
                            release = false;
                            load = false;
                        }
                        var num = (data.infoAccessory[slotNo] != null) ? data.infoAccessory[slotNo].Category : -1;
                        var num2 = (data.infoAccessory[slotNo] != null) ? data.infoAccessory[slotNo].Id : -1;
                        if (!forceChange && null != data.objAccessory[slotNo] && type == num && id == num2)
                        {
                            load = false;
                            release = false;
                        }
                        if (-1 != id)
                        {
                            if (!instance.lstCtrl.ContainsCategoryInfo((ChaListDefine.CategoryNo)type))
                            {
                                release = true;
                                load = false;
                            }
                            else
                            {
                                lib = instance.lstCtrl.GetInfo((ChaListDefine.CategoryNo)type, id);
                                if (lib == null)
                                {
                                    release = true;
                                    load = false;
                                }
                                else if (!instance.hiPoly)
                                {
                                    var flag4 = true;
                                    if (123 == type && 1 == lib.Kind)
                                    {
                                        flag4 = false;
                                    }
                                    if (122 == type && 1 == lib.GetInfoInt(ChaListDefine.KeyType.HideHair))
                                    {
                                        flag4 = false;
                                    }
                                    if (Manager.Config.EtcData.loadHeadAccessory && 122 == type && 1 == lib.Kind)
                                    {
                                        flag4 = false;
                                    }
                                    if (Manager.Config.EtcData.loadAllAccessory)
                                    {
                                        flag4 = false;
                                    }
                                    if (flag4)
                                    {
                                        release = true;
                                        load = false;
                                    }
                                }
                            }
                        }
                    }
                    if (release)
                    {
                        if (!load)
                        {
                            data.nowAccessories[slotNo].MemberInit();
                            data.nowAccessories[slotNo].type = 120;
                        }
                        if (data.objAccessory[slotNo])
                        {
                            instance.SafeDestroy(data.objAccessory[slotNo]);
                            data.objAccessory[slotNo] = null;
                            data.infoAccessory[slotNo] = null;
                            data.cusAcsCmp[slotNo] = null;
                            for (var i = 0; i < 2; i++)
                            {
                                data.objAcsMove[slotNo][i] = null;
                            }
                        }
                    }
                    if (load)
                    {
                        byte weight = 0;
                        Transform trfParent = null;
                        if ("null" == lib.GetInfo(ChaListDefine.KeyType.Parent))
                        {
                            weight = 2;
                            trfParent = instance.objTop.transform;
                        }
                        if (_loadCharaFbxData == null)
                            _loadCharaFbxData = instance.GetType().GetMethod("LoadCharaFbxData", AccessTools.all);
#if KOIKATSU

                        _params[0] = new Action<ListInfoBase>(delegate (ListInfoBase l) { data.infoAccessory[slotNo] = l; });
                        _params[1] = true;
                        _params[2] = type;
                        _params[3] = id;
                        _params[4] = "ca_slot" + (slotNo + 20).ToString("00");
                        _params[5] = false;
                        _params[6] = weight;
                        _params[7] = trfParent;
                        _params[8] = -1;
                        _params[9] = false;
#elif EMOTIONCREATORS
                _params[0] = type;
                _params[1] = id;
                _params[2] = "ca_slot" + (slotNo + 20).ToString("00");
                _params[3] = false;
                _params[4] = weight;
                _params[5] = trfParent;
                _params[6] = -1;
                _params[7] = false;
#endif

                        data.objAccessory[slotNo] = (GameObject)_loadCharaFbxData.Invoke(instance, _params); // I'm doing this the reflection way in order to be compatible with other plugins (like RimRemover)
                        if (data.objAccessory[slotNo])
                        {
                            var component = data.objAccessory[slotNo].GetComponent<ListInfoComponent>();
                            lib = (data.infoAccessory[slotNo] = component.data);
                            data.cusAcsCmp[slotNo] = data.objAccessory[slotNo].GetComponent<ChaAccessoryComponent>();
                            data.nowAccessories[slotNo].type = type;
                            data.nowAccessories[slotNo].id = lib.Id;
                            data.objAcsMove[slotNo][0] = data.objAccessory[slotNo].transform.FindLoop("N_move");
                            data.objAcsMove[slotNo][1] = data.objAccessory[slotNo].transform.FindLoop("N_move2");
                        }
                    }
                    if (data.objAccessory[slotNo])
                    {
                        if (instance.loadWithDefaultColorAndPtn)
                        {
                            SetAccessoryDefaultColor(instance, slotNo);
                        }
                        instance.ChangeAccessoryColor(slotNo + 20);
                        if (string.Empty == parentKey)
                        {
                            parentKey = lib.GetInfo(ChaListDefine.KeyType.Parent);
                        }
                        instance.ChangeAccessoryParent(slotNo + 20, parentKey);
                        instance.UpdateAccessoryMoveFromInfo(slotNo + 20);
                        data.nowAccessories[slotNo].partsOfHead = ChaAccessoryDefine.CheckPartsOfHead(parentKey);
#if KOIKATSU
                        if (!instance.hiPoly && !Manager.Config.EtcData.loadAllAccessory)
                        {
                            var componentsInChildren = data.objAccessory[slotNo].GetComponentsInChildren<DynamicBone>(true);
                            foreach (var dynamicBone in componentsInChildren)
                            {
                                dynamicBone.enabled = false;
                            }
                        }
#endif
                        if (_self._hasDarkness)
                            instance.ChangeShakeAccessory(slotNo + 20);
                    }
                    instance.SetHideHairAccessory();
                }

                private static void CleanRemainingAccessory(ChaControl instance, CharAdditionalData data, int slotNo)
                {
                    if (slotNo < data.nowAccessories.Count)
                    {
                        data.nowAccessories[slotNo].MemberInit();
                        data.nowAccessories[slotNo].type = 120;
                    }
                    if (data.objAccessory[slotNo])
                    {
                        instance.SafeDestroy(data.objAccessory[slotNo]);
                        data.objAccessory[slotNo] = null;
                        data.infoAccessory[slotNo] = null;
                        data.cusAcsCmp[slotNo] = null;
                        for (var i = 0; i < 2; i++)
                        {
                            data.objAcsMove[slotNo][i] = null;
                        }
                        if (_self._hasDarkness)
                            instance.ChangeShakeAccessory(slotNo);
                    }
                    instance.SetHideHairAccessory();
                }

                private static void SetAccessoryDefaultColor(ChaControl instance, int slotNo)
                {
                    var data = _self._accessoriesByChar[instance.chaFile];
                    var chaAccessoryComponent = data.cusAcsCmp[slotNo];
                    if (null == chaAccessoryComponent)
                    {
                        return;
                    }
                    if (chaAccessoryComponent.useColor01)
                    {
                        data.nowAccessories[slotNo].color[0] = chaAccessoryComponent.defColor01;
                    }
                    if (chaAccessoryComponent.useColor02)
                    {
                        data.nowAccessories[slotNo].color[1] = chaAccessoryComponent.defColor02;
                    }
                    if (chaAccessoryComponent.useColor03)
                    {
                        data.nowAccessories[slotNo].color[2] = chaAccessoryComponent.defColor03;
                    }
                    if (chaAccessoryComponent.rendAlpha != null && chaAccessoryComponent.rendAlpha.Length != 0)
                    {
                        data.nowAccessories[slotNo].color[3] = chaAccessoryComponent.defColor04;
                    }
                }
            }
#if KOIKATSU
            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.ChangeCoordinateType), new[] { typeof(ChaFileDefine.CoordinateType), typeof(bool) })]
            internal static class ChaControl_ChangeCoordinateType_Patches
            {
                private static void Prefix(ChaControl __instance, ChaFileDefine.CoordinateType type)
                {
                    CharAdditionalData data;
                    List<ChaFileAccessory.PartsInfo> accessories;
                    if (_self._accessoriesByChar.TryGetValue(__instance.chaFile, out data) == false)
                    {
                        data = new CharAdditionalData();
                        _self._accessoriesByChar.Add(__instance.chaFile, data);
                    }
                    if (data.rawAccessoriesInfos.TryGetValue((int)type, out accessories) == false)
                    {
                        accessories = new List<ChaFileAccessory.PartsInfo>();
                        data.rawAccessoriesInfos.Add((int)type, accessories);
                    }
                    data.nowAccessories = accessories;
                    while (data.infoAccessory.Count < data.nowAccessories.Count)
                        data.infoAccessory.Add(null);
                    while (data.objAccessory.Count < data.nowAccessories.Count)
                        data.objAccessory.Add(null);
                    while (data.objAcsMove.Count < data.nowAccessories.Count)
                        data.objAcsMove.Add(new GameObject[2]);
                    while (data.cusAcsCmp.Count < data.nowAccessories.Count)
                        data.cusAcsCmp.Add(null);
                    while (data.showAccessories.Count < data.nowAccessories.Count)
                        data.showAccessories.Add(true);
                    _self.ExecuteDelayed(_self.OnCoordTypeChange);
                }
            }
#endif

            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.UpdateVisible))]
            internal static class ChaControl_UpdateVisible_Patches
            {
                private static void Postfix(ChaControl __instance)
                {
                    CharAdditionalData data;
                    if (_self._accessoriesByChar.TryGetValue(__instance.chaFile, out data) == false)
                        return;

                    var flag2 = true;
                    if (Scene.NowSceneNames.Any(s => s == "H"))
                    {
                        flag2 = (__instance.sex != 0);
                    }

#if EMOTIONCREATORS
            bool[] array10 = new bool[]
            {
                true,
                __instance.objClothes[0] && __instance.objClothes[0].activeSelf,
                __instance.objClothes[1] && __instance.objClothes[1].activeSelf,
                __instance.objClothes[2] && __instance.objClothes[2].activeSelf,
                __instance.objClothes[3] && __instance.objClothes[3].activeSelf,
                __instance.objClothes[4] && __instance.objClothes[4].activeSelf,
                __instance.objClothes[5] && __instance.objClothes[5].activeSelf,
                __instance.objClothes[6] && __instance.objClothes[6].activeSelf,
                __instance.objClothes[7] && __instance.objClothes[7].activeSelf
            };
            bool[] array11 = new bool[]
            {
                true,
                false,
                false,
                false,
                false,
                true,
                true,
                true,
                true
            };
            array11[1] = (__instance.fileStatus.clothesState[0] != 1);
            array11[2] = (__instance.fileStatus.clothesState[1] != 1);
            array11[3] = (__instance.fileStatus.clothesState[2] != 1);
            array11[4] = (__instance.fileStatus.clothesState[3] != 1 && __instance.fileStatus.clothesState[3] != 2);
            bool[] array12 = array11;

            bool flag3 = false;
            if (Singleton<Scene>.Instance.NowSceneNames.Any(s => s == "HPlayScene") && Singleton<HPlayData>.IsInstance() && Singleton<HPlayData>.Instance.basePart != null && Singleton<HPlayData>.Instance.basePart.kind == 0)
            {
                flag3 = (__instance.sex == 0 && __instance.fileStatus.visibleSimple);
            }
#endif

                    for (var i = 0; i < data.nowAccessories.Count; i++)
                    {
                        var objAccessory = data.objAccessory[i];
#if KOIKATSU
                        if (objAccessory == null)
                            continue;

                        var flag9 = false;
                        if (!__instance.fileStatus.visibleHeadAlways && data.nowAccessories[i].partsOfHead)
                        {
                            flag9 = true;
                        }
                        if (!__instance.fileStatus.visibleBodyAlways || !flag2)
                        {
                            flag9 = true;
                        }

                        objAccessory.SetActive(__instance.visibleAll &&
                                               data.showAccessories[i] &&
                                               __instance.fileStatus.visibleSimple == false &&
                                               !flag9);
#elif EMOTIONCREATORS
                ChaFileAccessory.PartsInfo part = data.nowAccessories[i];

                bool flag10 = array10[part.hideCategory];
                if (flag10 && part.hideTiming == 0)
                {
                    flag10 = array12[part.hideCategory];
                }
                bool flag11 = false;
                if (!__instance.fileStatus.visibleHeadAlways && part.partsOfHead)
                {
                    flag11 = true;
                }
                if (!__instance.fileStatus.visibleBodyAlways || !flag2)
                {
                    flag11 = true;
                }
                if (YS_Assist.SetActiveControl(objAccessory, __instance.visibleAll, data.showAccessories[i], flag10, !flag3, !flag11))
                {
                    __instance.updateShape = true;
                }
#endif
                    }
                }
            }

            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.SetAccessoryStateAll))]
            internal static class ChaControl_SetAccessoryStateAll_Patches
            {
                private static void Postfix(ChaControl __instance, bool show)
                {
                    CharAdditionalData data;
                    if (_self._accessoriesByChar.TryGetValue(__instance.chaFile, out data) == false)
                        return;
                    for (var i = 0; i < data.nowAccessories.Count; i++)
                        data.showAccessories[i] = show;
                }
            }

#if KOIKATSU
            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.SetAccessoryStateCategory))]
            internal static class ChaControl_SetAccessoryStateCategory_Patches
            {
                private static void Postfix(ChaControl __instance, int cateNo, bool show)
                {
                    CharAdditionalData data;
                    if (_self._accessoriesByChar.TryGetValue(__instance.chaFile, out data) == false)
                        return;
                    for (var i = 0; i < data.nowAccessories.Count; i++)
                        if (data.nowAccessories[i].hideCategory == cateNo)
                            data.showAccessories[i] = show;
                }
            }

            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.GetAccessoryCategoryCount))]
            internal static class ChaControl_GetAccessoryCategoryCount_Patches
            {
                private static void Postfix(ChaControl __instance, int cateNo, ref int __result)
                {
                    if (__result == -1)
                        return;
                    CharAdditionalData data;
                    if (_self._accessoriesByChar.TryGetValue(__instance.chaFile, out data) == false)
                        return;
                    foreach (var part in data.nowAccessories)
                    {
                        if (part.hideCategory == cateNo)
                            __result++;
                    }
                }
            }
#endif

            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.SetAccessoryState))]
            internal static class ChaControl_SetAccessoryState_Patches
            {
                private static void Postfix(ChaControl __instance, int slotNo, bool show)
                {
                    if (slotNo < 20)
                        return;
                    CharAdditionalData data;
                    if (_self._accessoriesByChar.TryGetValue(__instance.chaFile, out data) == false)
                        return;
                    data.showAccessories[slotNo - 20] = show;
                }
            }

            [HarmonyPatch(typeof(ChaControl), nameof(ChaControl.ChangeShakeAccessory))]
            internal static class ChaControl_ChangeShakeAccessory_Patches
            {
                private static bool Prepare()
                {
                    return (_self._hasDarkness);

                }
                private static bool Prefix(ChaControl __instance, int slotNo)
                {
                    if (slotNo < 20)
                        return true;
                    CharAdditionalData data;
                    if (_self._accessoriesByChar.TryGetValue(__instance.chaFile, out data) == false)
                        return false;
                    var obj = data.objAccessory[slotNo - 20];
                    if (obj != null)
                    {
                        var componentsInChildren = obj.GetComponentsInChildren<DynamicBone>(true);
                        var noShake = data.nowAccessories[slotNo - 20].noShake;
                        foreach (var dynamicBone in componentsInChildren)
                        {
                            dynamicBone.enabled = !noShake;
                        }
                    }
                    return false;
                }
            }

            [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.CopyAll))]
            internal static class ChaFile_CopyAll_Patches
            {
                private static void Postfix(ChaFile __instance, ChaFile _chafile)
                {
                    CharAdditionalData sourceData;
                    if (_self._accessoriesByChar.TryGetValue(_chafile, out sourceData) == false)
                        return;
                    CharAdditionalData destinationData;
                    if (_self._accessoriesByChar.TryGetValue(__instance, out destinationData))
                    {
                        foreach (var pair in destinationData.rawAccessoriesInfos)
                        {
                            if (pair.Value != null)
                                pair.Value.Clear();
                        }
                    }
                    else
                    {
                        destinationData = new CharAdditionalData();
                        _self._accessoriesByChar.Add(__instance, destinationData);
                    }
                    foreach (var sourcePair in sourceData.rawAccessoriesInfos)
                    {
                        if (sourcePair.Value == null || sourcePair.Value.Count == 0)
                            continue;
                        List<ChaFileAccessory.PartsInfo> destinationParts;
                        if (destinationData.rawAccessoriesInfos.TryGetValue(sourcePair.Key, out destinationParts) == false)
                        {
                            destinationParts = new List<ChaFileAccessory.PartsInfo>();
                            destinationData.rawAccessoriesInfos.Add(sourcePair.Key, destinationParts);
                        }
                        foreach (var sourcePart in sourcePair.Value)
                        {
                            {
                                var bytes = MessagePackSerializer.Serialize(sourcePart);
                                destinationParts.Add(MessagePackSerializer.Deserialize<ChaFileAccessory.PartsInfo>(bytes));
                            }
                        }
                    }
                    if (destinationData.rawAccessoriesInfos.TryGetValue(_chafile.status.GetCoordinateType(), out destinationData.nowAccessories) == false)
                    {
                        destinationData.nowAccessories = new List<ChaFileAccessory.PartsInfo>();
                        destinationData.rawAccessoriesInfos.Add(_chafile.status.GetCoordinateType(), destinationData.nowAccessories);
                    }
                    while (destinationData.infoAccessory.Count < destinationData.nowAccessories.Count)
                        destinationData.infoAccessory.Add(null);
                    while (destinationData.objAccessory.Count < destinationData.nowAccessories.Count)
                        destinationData.objAccessory.Add(null);
                    while (destinationData.objAcsMove.Count < destinationData.nowAccessories.Count)
                        destinationData.objAcsMove.Add(new GameObject[2]);
                    while (destinationData.cusAcsCmp.Count < destinationData.nowAccessories.Count)
                        destinationData.cusAcsCmp.Add(null);
                    while (destinationData.showAccessories.Count < destinationData.nowAccessories.Count)
                        destinationData.showAccessories.Add(true);
                }
            }

#if false
    [HarmonyPatch(typeof(CustomAcsParentWindow), nameof(CustomAcsParentWindow.Awake))]
    internal static class CustomAcsParentWindow_Awake_Patches
    {
        private static void Postfix(CustomAcsParentWindow __instance)
        {
            _self._cvsAccessory = __instance.cvsAccessory;
        }
    }
#endif


        }
    }
}
