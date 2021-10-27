namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public const string versionNum = "2.0.17.1";
        public const string GUID = "com.joan6694.illusionplugins.moreaccessories";

        public static MoreAccessories _self;

        private const int _saveVersion = 2;
        private const string _extSaveKey = "moreAccessories";

        internal static bool _hasDarkness;
        public bool ImportingCards { get; private set; }
        internal static bool CharaMaker => MakerMode != null;

        private static bool BackwardCompatibility = true; //Do not turn back on once off.

        public static MakerMode MakerMode { get; internal set; }
#if KK || KKS
        internal static bool InH => HMode != null;
        public static HScene HMode { get; internal set; }

        internal static bool InStudio => StudioMode != null;
        public static StudioClass StudioMode { get; internal set; }
#elif EC
        internal bool InPlayMode => PlayMode != null;
        public static PlayMode PlayMode;
#endif
    }
}
