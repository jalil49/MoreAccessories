using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
#if EMOTIONCREATORS
using ADVPart.Manipulate.Chara;
using HEdit;
using HPlay;
#endif

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public static class ChaFile_Patches
        {
            [HarmonyPatch(typeof(ChaFileControl), MethodType.Constructor)]
            internal static class ChaFileControl_Ctor_Patches
            {
                private static void Postfix(ChaFileControl __instance)
                {
#if KOIKATSU
                    foreach (var coordinate in __instance.coordinate)
                        _self._charByCoordinate[coordinate] = new WeakReference(__instance);
#elif EMOTIONCREATORS
            _self._charByCoordinate[__instance.coordinate] = new WeakReference(__instance);
#endif
                }
            }

            [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.CopyCoordinate))]
            internal static class ChaFile_CopyCoordinate_Patches
            {
                private static void Postfix(ChaFile __instance,
#if KOIKATSU
                                    ChaFileCoordinate[] _coordinate
#elif EMOTIONCREATORS
                                    ChaFileCoordinate _coordinate
#endif
                )
                {
                    ChaFileControl sourceFile;
                    WeakReference r;
#if KOIKATSU
                    if (_self._charByCoordinate.TryGetValue(_coordinate[0], out r) == false || r.IsAlive == false)
#elif EMOTIONCREATORS
            if (_self._charByCoordinate.TryGetValue(_coordinate, out r) == false || r.IsAlive == false)
#endif
                        return;
                    else
                        sourceFile = (ChaFileControl)r.Target;
                    if (__instance == sourceFile)
                        return;

                    CharAdditionalData sourceData;
                    if (_self._accessoriesByChar.TryGetValue(sourceFile, out sourceData) == false)
                    {
                        sourceData = new CharAdditionalData();
                        _self._accessoriesByChar.Add(sourceFile, sourceData);
                    }
                    CharAdditionalData destinationData;
                    if (_self._accessoriesByChar.TryGetValue(__instance, out destinationData) == false)
                    {
                        destinationData = new CharAdditionalData();
                        _self._accessoriesByChar.Add(__instance, destinationData);
                    }
                    destinationData.LoadFrom(sourceData);
                }
            }


            [HarmonyPatch(typeof(ChaFileControl), nameof(ChaFileControl.LoadFileLimited), new[] { typeof(string), typeof(byte), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool), typeof(bool) })]
            private static class ChaFileControl_LoadFileLimited_Patches
            {
                private static void Prefix(ChaFileControl __instance, bool coordinate = true)
                {
                    if (_self._inCharaMaker && _self._customAcsChangeSlot != null)
                    {
                        if (_self._overrideCharaLoadingFilePost == null)
                            _self._overrideCharaLoadingFilePost = __instance;
                        _self._loadAdditionalAccessories = coordinate;
                    }
                }

                private static void Postfix()
                {
                    _self._overrideCharaLoadingFilePost = null;
                    _self._loadAdditionalAccessories = true;
                }
            }

            [HarmonyPatch(typeof(ChaFileControl), nameof(ChaFileControl.LoadCharaFile), typeof(BinaryReader), typeof(bool), typeof(bool))]
            private static class ChaFileControl_LoadCharaFile_Patches
            {
                private static void Prefix(ChaFileControl __instance)
                {
                    if (_self._overrideCharaLoadingFilePost == null)
                        _self._overrideCharaLoadingFilePost = __instance;
                }
                private static void Postfix()
                {
                    _self._overrideCharaLoadingFilePost = null;
                }
            }

            [HarmonyPatch(typeof(ChaFile), nameof(ChaFile.CopyAll))]
            private static class ChaFile_CopyAll_Patches
            {
                private static void Prefix(ChaFile __instance, ChaFile _chafile)
                {
                    if (__instance == _chafile)
                        return;
                    if (_self._accessoriesByChar.TryGetValue(_chafile, out var srcData) == false)
                    {
                        srcData = new CharAdditionalData();
                        _self._accessoriesByChar.Add(_chafile, srcData);
                    }
                    if (_self._accessoriesByChar.TryGetValue(__instance, out var dstData) == false)
                    {
                        dstData = new CharAdditionalData();
                        _self._accessoriesByChar.Add(__instance, dstData);
                    }
                    dstData.LoadFrom(srcData);

                    if (dstData.rawAccessoriesInfos.TryGetValue(__instance.status.GetCoordinateType(), out dstData.nowAccessories) == false)
                    {
                        dstData.nowAccessories = new List<ChaFileAccessory.PartsInfo>();
                        dstData.rawAccessoriesInfos.Add(__instance.status.GetCoordinateType(), dstData.nowAccessories);
                    }
                }
            }

        }
    }
}
