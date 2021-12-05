namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public const string versionNum = "2.0.20";
        public const string GUID = "com.joan6694.illusionplugins.moreaccessories";

        public static MoreAccessories _self;

        private const int _saveVersion = 2;
        private const string _extSaveKey = "moreAccessories";

        internal static bool _hasDarkness;
        public bool ImportingCards { get; private set; }
        internal static bool CharaMaker => _makermode != null;

        /// <summary>
        /// Only affects saving of data in old moreaccessory format
        /// </summary>
        private static bool BackwardCompatibility = true; //Do not turn back on once off.

        public static MakerMode MakerMode
        {
            get { return _makermode; }
            internal set
            {
#if KK || KKS
                _hMode = null;
#elif EC
                _playMode = null;
#endif
                _makermode = value;
            }
        }

        private static MakerMode _makermode;

#if KK || KKS
        internal static bool InH => _hMode != null;

        public static HScene HMode
        {
            get { return _hMode; }
            internal set
            {
                _makermode = null;
                _hMode = value;
            }
        }

        private static HScene _hMode;

        internal static bool InStudio => StudioMode != null;
        public static StudioClass StudioMode { get; internal set; }
#elif EC
        internal bool InPlayMode => PlayMode != null;
        public static PlayMode PlayMode
        {
            get { return _playMode; }
            internal set
            {
                MakerMode = null;
                _playMode = value;
            }
        }
        private static PlayMode _playMode;

        internal static BepInEx.Configuration.ConfigEntry<bool> SceneCreateAccessoryNames;
#endif
    }
}
