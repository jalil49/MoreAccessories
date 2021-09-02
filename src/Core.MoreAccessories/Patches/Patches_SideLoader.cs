using HarmonyLib;
using Sideloader.AutoResolver;
using System.Collections.Generic;

namespace MoreAccessoriesKOI.Patches
{
    public static class SideLoader_Patches
    {
        [HarmonyPatch(typeof(UniversalAutoResolver), "IterateCoordinatePrefixes")]
        private static class SideloaderAutoresolverHooks_IterateCoordinatePrefixes_Patches
        {
            [HarmonyBefore("com.deathweasel.bepinex.guidmigration")]
            internal static void Prefix(ICollection<ResolveInfo> extInfo)
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


            //            private static void Postfix(object action, ChaFileCoordinate coordinate, object extInfo, string prefix)
            //            {

            //                ChaFileControl owner = null;



            //                if (owner == null)
            //                    return;

            //                if (string.IsNullOrEmpty(prefix))
            //                {
            //                    for (var j = 0; j < additionalData.nowAccessories.Count; j++)
            //                        ((Delegate)action).DynamicInvoke(_sideLoaderChaFileAccessoryPartsInfoProperties, additionalData.nowAccessories[j], extInfo, $"{prefix}accessory{j + 20}.");
            //                }
            //                else
            //                {
            //#if KK
            //                                string coordId = prefix.Replace("outfit", "").Replace(".", "");
            //                                if (int.TryParse(coordId, out int result) == false)
            //                                    return;
            //#elif EC
            //                                int result = 0;
            //#endif
            //                    List<ChaFileAccessory.PartsInfo> parts;
            //                    if (additionalData.rawAccessoriesInfos.TryGetValue(result, out parts) == false)
            //                        return;
            //                    for (var j = 0; j < parts.Count; j++)
            //                        ((Delegate)action).DynamicInvoke(_sideLoaderChaFileAccessoryPartsInfoProperties, parts[j], extInfo, $"{prefix}accessory{j + 20}.");
            //                }
            //            }

        }


        //var uarHooks = typeof(Sideloader.AutoResolver.UniversalAutoResolver).GetNestedType("Hooks", AccessTools.all);
        //harmony.Patch(uarHooks.GetMethod("ExtendedCardLoad", AccessTools.all), new HarmonyMethod(typeof(MoreAccessories), nameof(OnActualCharaLoad)));
        //harmony.Patch(uarHooks.GetMethod("ExtendedCoordinateLoad", AccessTools.all), new HarmonyMethod(typeof(MoreAccessories), nameof(OnActualCoordLoad)));
        //[HarmonyPatch("Sideloader.AutoResolver.UniversalAutoResolver+Hooks, KKS_Sideloader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "ExtendedCardLoad")]
        //internal static void CharaLoadPostFix(ChaFile file)
        //{
        //    MoreAccessories._self.OnActualCharaLoad(file);
        //}
        //[HarmonyPatch("Sideloader.AutoResolver.UniversalAutoResolver+Hooks", "ExtendedCoordinateLoad")]
        //internal static void CoordinateLoadPostFix(ChaFileCoordinate file)
        //{
        //    MoreAccessories._self.OnActualCoordLoad(file);
        //}

    }
}
