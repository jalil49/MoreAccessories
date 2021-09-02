using HarmonyLib;
using Sideloader.AutoResolver;
using System.Collections.Generic;

namespace MoreAccessoriesKOI.Patches
{
    public static class SideLoader_Patches
    {
        #region Sideloader
#if KK || KKS
        [HarmonyPatch(typeof(UniversalAutoResolver), "IterateCoordinatePrefixes")]
        private static class SideloaderAutoresolverHooks_IterateCoordinatePrefixes_Patches
        {
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
        }
#endif
        #endregion
    }
}
