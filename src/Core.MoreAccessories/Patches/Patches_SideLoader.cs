using ExtensibleSaveFormat;
using HarmonyLib;
using MessagePack;
using Sideloader.AutoResolver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoreAccessoriesKOI.Patches
{
    public static class SideLoader_Patches
    {
        [HarmonyPatch(typeof(UniversalAutoResolver), "IterateCoordinatePrefixes")]
        private static class SideloaderAutoresolverHooks_IterateCoordinatePrefixes_Patches
        {
            private static object _sideLoaderChaFileAccessoryPartsInfoProperties;

            [HarmonyBefore("com.deathweasel.bepinex.guidmigration")]
            internal static void Prefix(ICollection<ResolveInfo> extInfo) => Outfit_Error_Fix(extInfo);
            internal static void Postfix(object action, ChaFileCoordinate coordinate, object extInfo, string prefix)
            {
                var additionalData = MoreAccessories.PreviousMigratedData;

                if (additionalData == null) return;

                if (_sideLoaderChaFileAccessoryPartsInfoProperties == null)
                {
#if KK
                    _sideLoaderChaFileAccessoryPartsInfoProperties = Type.GetType($"Sideloader.AutoResolver.StructReference,Sideloader")
#elif KKS
                    _sideLoaderChaFileAccessoryPartsInfoProperties = Type.GetType($"Sideloader.AutoResolver.StructReference,KKS_Sideloader")
#elif EC
                    _sideLoaderChaFileAccessoryPartsInfoProperties = Type.GetType($"Sideloader.AutoResolver.StructReference,EC_Sideloader")
#endif
                                                                         .GetProperty("ChaFileAccessoryPartsInfoProperties", AccessTools.all).GetValue(null, null);
                }


                if (string.IsNullOrEmpty(prefix))
                {
                    for (var j = 0; j < additionalData.nowAccessories.Count; j++)
                        ((Delegate)action).DynamicInvoke(_sideLoaderChaFileAccessoryPartsInfoProperties, additionalData.nowAccessories[j], extInfo, $"{prefix}accessory{j + 20}.");
                }
                else
                {
                    var coordId = prefix.Replace("outfit", "").Replace(".", "");
                    if (int.TryParse(coordId, out var result) == false)
                        return;
                    if (additionalData.rawAccessoriesInfos.TryGetValue(result, out var parts) == false)
                        return;
                    for (var j = 0; j < parts.Count; j++)
                        ((Delegate)action).DynamicInvoke(_sideLoaderChaFileAccessoryPartsInfoProperties, parts[j], extInfo, $"{prefix}accessory{j + 20}.");
                }
            }
        }
#if KKS || EC
        [HarmonyPatch(typeof(ExtendedSave), "CardImportEvent")]
        private static class SideloaderAutoresolverHooks_Import_Patches
        {
            internal static void Prefix(Dictionary<string, PluginData> data)
            {
                if (data.TryGetValue("com.bepis.sideloader.universalautoresolver", out var pluginData))
                {
                    if (pluginData != null && pluginData.data.ContainsKey("info"))
                    {
                        var tmpExtInfo = (object[])pluginData.data["info"];
                        var extInfo = tmpExtInfo.Select(x => MessagePackSerializer.Deserialize<ResolveInfo>((byte[])x)).ToList();
                        Outfit_Error_Fix(extInfo);
                        var serial = extInfo.Select(x => MessagePackSerializer.Serialize(x)).ToArray();
                        pluginData.data["info"] = serial;
                    }
                }
            }
        }
#endif
        internal static void Outfit_Error_Fix(ICollection<ResolveInfo> extInfo)
        {
            if (extInfo != null)
            {
                var i = 0;
                foreach (var o in extInfo)
                {
                    var property = o.Property;
                    if (property.StartsWith("outfit.")) //Sorry to whoever reads this, I(joan?) fucked up
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
    }
}
