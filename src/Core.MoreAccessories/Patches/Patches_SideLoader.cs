using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Sideloader.AutoResolver;
#if KKS || EC
using ExtensibleSaveFormat;
using MessagePack;
using System.Linq;
#endif

#pragma warning disable IDE0051 // Remove unused private members

namespace MoreAccessoriesKOI.Patches
{
    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Harmony Patches - Used Externally")]
    [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Harmony Patches - Used Externally")]
    public static class SideLoader_Patches
    {
#if !EC
        [HarmonyPatch(typeof(UniversalAutoResolver), "IterateCoordinatePrefixes")]
        private static class SideLoaderBugFix
        {
            [HarmonyBefore("com.deathweasel.bepinex.guidmigration")]
            private static void Prefix(ICollection<ResolveInfo> extInfo) => Outfit_Error_Fix(extInfo);
        }
#endif

#if KKS || EC
        [HarmonyPatch(typeof(ExtendedSave), "CardImportEvent")]
        private static class SideloaderImport_Patch
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
                        pluginData.data["info"] = extInfo.Select(MessagePackSerializer.Serialize).ToArray();
                    }
                }
            }
        }
#endif
        internal static void Outfit_Error_Fix(ICollection<ResolveInfo> extInfo)
        {
            if (extInfo != null)
            {
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
                }
            }
        }
    }
}
