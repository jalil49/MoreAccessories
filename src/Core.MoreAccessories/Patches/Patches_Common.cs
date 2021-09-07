using ChaCustom;
using HarmonyLib;

namespace MoreAccessoriesKOI.Patches
{
    public class Common_Patches
    {
        //can't do any of this since I gotta prepatch UAR or do a different method to get the resolveinfo for old cards
        //        [HarmonyPatch(typeof(clothesFileControl), nameof(clothesFileControl.Initialize))]
        //        internal class ClothesFileControlInitialize_patch
        //        {
        //            internal static void Prefix()
        //            {
        //                MoreAccessories.ClothesFileControlLoading = true;
        //            }
        //            internal static void Postfix()
        //            {
        //                MoreAccessories.ClothesFileControlLoading = false;
        //            }
        //        }

        //        [HarmonyPatch(typeof(clothesFileControl), nameof(clothesFileControl.Initialize))]
        //        internal class CustomCharaFileInitialize_patch
        //        {
        //            internal static void Prefix()
        //            {
        //                MoreAccessories.CharaListIsLoading = true;
        //            }
        //            internal static void Postfix()
        //            {
        //                MoreAccessories.CharaListIsLoading = false;
        //            }
        //        }
        //#if KK || KKS
        //        [HarmonyPatch(typeof(CharaViewer), nameof(CharaViewer.CreateCharaList))]
        //        internal class CharaViewerStart_patch
        //        {
        //            internal static void Prefix()
        //            {
        //                MoreAccessories.Print($"CharaViewer creating list", BepInEx.Logging.LogLevel.Message);
        //                MoreAccessories.CharaListIsLoading = true;
        //            }
        //            internal static void Postfix()
        //            {
        //                MoreAccessories.CharaListIsLoading = false;
        //            }
        //        }

        //        [HarmonyPatch(typeof(SaveData.CharaData), nameof(SaveData.CharaData.SetCharFile))]
        //        internal class SetCharFilePatch
        //        {
        //            internal static void Prefix(ChaFileControl charFile)
        //            {
        //                MoreAccessories._self.OnActualCharaLoad(charFile);
        //            }
        //        }

        //#endif 
        //#if KKS
        //        [HarmonyPatch(typeof(Localize.Translate.CustomFileListSelecter), nameof(Localize.Translate.CustomFileListSelecter.Initialize))]
        //        internal class CustomFileListSelecter_patch
        //        {
        //            internal static void Prefix()
        //            {
        //                MoreAccessories.Print($"CustomFileListSelecter creating list", BepInEx.Logging.LogLevel.Message);
        //                MoreAccessories.CharaListIsLoading = true;
        //            }
        //            internal static void Postfix()
        //            {
        //                MoreAccessories.CharaListIsLoading = false;
        //            }
        //        }

        //        [HarmonyPatch(typeof(Localize.Translate.CustomFileListSelecter), nameof(Localize.Translate.CustomFileListSelecter.Awake))]
        //        internal class CustomFileListSelecterStart_patch
        //        {
        //            internal static void Postfix(Localize.Translate.CustomFileListSelecter __instance)
        //            {
        //                __instance.onEnter += (FileControl) =>
        //                  {
        //                      MoreAccessories._self.OnActualCharaLoad(FileControl);
        //                  };
        //            }
        //        }
        //#endif
    }
}
