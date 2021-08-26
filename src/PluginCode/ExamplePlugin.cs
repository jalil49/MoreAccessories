using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using KKAPI;

namespace PluginCode
{
    [BepInPlugin(GUID, PluginName, Version)]
    // Tell BepInEx that this plugin needs KKAPI of at least the specified version.
    // If not found, this plugi will not be loaded and a warning will be shown.
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    public class ExamplePlugin : BaseUnityPlugin
    {
        /// <summary>
        /// Human-readable name of the plugin. In general, it should be short and concise.
        /// This is the name that is shown to the users who run BepInEx and to modders that inspect BepInEx logs. 
        /// </summary>
        public const string PluginName = "BepInEx Plugin";

        /// <summary>
        /// Unique ID of the plugin. Will be used as the default config file name.
        /// This must be a unique string that contains only characters a-z, 0-9 underscores (_) and dots (.)
        /// When creating Harmony patches or any persisting data, it's best to use this ID for easier identification.
        /// </summary>
        public const string GUID = "org.pluginid";

        /// <summary>
        /// Version of the plugin. Must be in form <major>.<minor>.<build>.<revision>.
        /// Major and minor versions are mandatory, but build and revision can be left unspecified.
        /// </summary>
        public const string Version = "1.0.0";

        internal static new ManualLogSource Logger;

        private ConfigEntry<bool> _exampleConfigEntry;

        private void Awake()
        {
            Logger = base.Logger;

            _exampleConfigEntry = Config.Bind("General", "Enable this plugin", true, "If false, this plugin will do nothing");

            if (_exampleConfigEntry.Value)
            {
                Harmony.CreateAndPatchAll(typeof(Hooks), GUID);
                //CharacterApi.RegisterExtraBehaviour<MyCustomController>(GUID);
            }
        }

        private static class Hooks
        {
            // [HarmonyPrefix]
            // [HarmonyPatch(typeof(SomeClass), nameof(SomeClass.SomeInstanceMethod))]
            // private static void SomeMethodPrefix(SomeClass __instance, int someParameter, ref int __result)
            // {
            //     ...
            // }
        }
    }
}
