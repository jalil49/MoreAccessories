using HarmonyLib;
using Sideloader.AutoResolver;
using System.Collections.Generic;

#if KKS || EC
using ExtensibleSaveFormat;
using MessagePack;
using System.Linq;
#endif

namespace MoreAccessoriesKOI.Patches
{
    public static class SideLoader_Patches
    {
#if !EC
        [HarmonyPatch(typeof(UniversalAutoResolver), "IterateCoordinatePrefixes")]
        private static class SideloaderAutoresolverHooks_IterateCoordinatePrefixes_Patches
        {
            [HarmonyBefore("com.deathweasel.bepinex.guidmigration")]
            private static void Prefix(ICollection<ResolveInfo> extInfo) => Outfit_Error_Fix(extInfo);
        }
#endif

#if KKS || EC
        [HarmonyPatch(typeof(ExtendedSave), "CardImportEvent")]
        private static class SideloaderAutoresolverHooks_Import_Patches
        {
            private static void Prefix(Dictionary<string, PluginData> data)
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
