using BepInEx.Configuration;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        internal const string VersionNum = "2.0.22";
        private const string Guid = "com.joan6694.illusionplugins.moreaccessories";
        
        // ReSharper disable once InconsistentNaming - Used By Coordinate load options
        public static MoreAccessories _self;

        private const int SaveVersion = 2;
        private const string ExtSaveKey = "moreAccessories";

        internal static bool HasDarkness;
        private bool ImportingCards { get; set; }
        internal static bool CharaMaker => _makerMode != null;

        /// <summary>
        /// Only affects saving of data in old more accessory format
        /// </summary>
        private static bool _backwardCompatibility = true; //Do not turn back on once off.

        public static MakerMode MakerMode
        {
            get { return _makerMode; }
            private set
            {
#if KK || KKS
                _hMode = null;
#elif EC
                _playMode = null;
#endif
                _makerMode = value;
            }
        }

        private static MakerMode _makerMode;

#if KK || KKS
        private static bool InH => _hMode != null;

        public static HScene HMode
        {
            get => _hMode;
            internal set
            {
                _makerMode = null;
                _hMode = value;
            }
        }

        private static HScene _hMode;

        internal static bool InStudio => StudioMode != null;
        private static StudioClass StudioMode { get; set; }
#elif EC
        internal bool InPlayMode => PlayMode != null;
        public static PlayMode PlayMode
        {
            get => _playMode;
            internal set
            {
                MakerMode = null;
                _playMode = value;
            }
        }
        private static PlayMode _playMode;

        internal static ConfigEntry<bool> SceneCreateAccessoryNames;
#endif
    }
}
