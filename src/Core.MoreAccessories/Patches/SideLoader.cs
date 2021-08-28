using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
#if EMOTIONCREATORS
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
#endif
using Manager;
using Sideloader.AutoResolver;
#if KOIKATSU
#endif

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public static class SideLoader_Patches
        {
            #region Sideloader
            [HarmonyPatch(typeof(UniversalAutoResolver), "IterateCardPrefixes")]
            private static class SideloaderAutoresolverHooks_IterateCardPrefixes_Patches
            {
                private static void Prefix(ChaFile file)
                {
                    if (_self._overrideCharaLoadingFilePost == null)
                        _self._overrideCharaLoadingFilePost = file;
                }

                private static void Postfix()
                {
                    _self._overrideCharaLoadingFilePost = null;
                }
            }

            [HarmonyPatch(typeof(UniversalAutoResolver), "IterateCoordinatePrefixes")]
            private static class SideloaderAutoresolverHooks_IterateCoordinatePrefixes_Patches
            {
                private static object _sideLoaderChaFileAccessoryPartsInfoProperties;
#if KOIKATSU
                [HarmonyBefore("com.deathweasel.bepinex.guidmigration")]
                private static void Prefix(ICollection<ResolveInfo> extInfo)
                {
                    if (extInfo != null)
                    {
                        var i = 0;
                        foreach (var o in extInfo)
                        {
                            var property = o.Property;
                            if (property.StartsWith("outfit.")) //Sorry to whoever reads this, I fucked up
                            {
                                var array = property.ToCharArray();
                                array[6] = array[7];
                                array[7] = '.';
                                o.Property = new string(array);
                            }
                            ++i;
                        }
                    }
                }
#endif

                private static void Postfix(object action, ChaFileCoordinate coordinate, ICollection<ResolveInfo> extInfo, string prefix)
                {
                    if (_self._inCharaMaker && _self._charaMakerData == null) return;

                    if (_sideLoaderChaFileAccessoryPartsInfoProperties == null)
                    {
#if KOIKATSU
                        _sideLoaderChaFileAccessoryPartsInfoProperties = Type.GetType($"Sideloader.AutoResolver.StructReference,KKS_Sideloader")
#elif EMOTIONCREATORS
                    _sideLoaderChaFileAccessoryPartsInfoProperties = Type.GetType($"Sideloader.AutoResolver.StructReference,EC_Sideloader")
#endif
                                                                         .GetProperty("ChaFileAccessoryPartsInfoProperties", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).GetValue(null, null);
                    }
                    WeakReference o;
                    ChaFileControl owner = null;
                    if (_self._charByCoordinate.TryGetValue(coordinate, out o) == false || o.IsAlive == false)
                    //if (_self._overrideCharaLoadingFilePost != null)
                    //    owner = _self._overrideCharaLoadingFilePost;
                    //else
                    {
                        foreach (var pair in Character.dictEntryChara)
                        {
                            if (pair.Value.nowCoordinate == coordinate)
                            {
                                owner = pair.Value.chaFile;
                                break;
                            }
#if KOIKATSU
                            foreach (var c in pair.Value.chaFile.coordinate)
#elif EMOTIONCREATORS
                        ChaFileCoordinate c = pair.Value.chaFile.coordinate;
#endif
                            {
                                if (c == coordinate)
                                {
                                    owner = pair.Value.chaFile;
                                    goto DOUBLEBREAK;
                                }
                            }
                        }
                    }
                    else
                    {
                        owner = (ChaFileControl)o.Target;
                    }
                DOUBLEBREAK:

                    if (owner == null)
                        return;
                    CharAdditionalData additionalData;
                    if (_self._accessoriesByChar.TryGetValue(owner, out additionalData) == false || additionalData.nowAccessories == null)
                        return;
                    if (string.IsNullOrEmpty(prefix))
                    {
                        for (var j = 0; j < additionalData.nowAccessories.Count; j++)
                            ((Delegate)action).DynamicInvoke(_sideLoaderChaFileAccessoryPartsInfoProperties, additionalData.nowAccessories[j], extInfo, $"{prefix}accessory{j + 20}.");
                    }
                    else
                    {
#if KOIKATSU
                        var coordId = prefix.Replace("outfit", "").Replace(".", "");
                        if (int.TryParse(coordId, out var result) == false)
                            return;
#elif EMOTIONCREATORS
                    int result = 0;
#endif
                        List<ChaFileAccessory.PartsInfo> parts;
                        if (additionalData.rawAccessoriesInfos.TryGetValue(result, out parts) == false)
                            return;
                        for (var j = 0; j < parts.Count; j++)
                            ((Delegate)action).DynamicInvoke(_sideLoaderChaFileAccessoryPartsInfoProperties, parts[j], extInfo, $"{prefix}accessory{j + 20}.");
                    }
                }
            }
            #endregion

        }
    }
}
