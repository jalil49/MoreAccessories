using BepInEx.Logging;
using System.Collections.Generic;
#if EC
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
using UnityEngine;
#endif
#if KK || KKS
#endif

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public const string versionNum = "2.0.0";
        public const string GUID = "com.joan6694.illusionplugins.moreaccessories";

        public static MoreAccessories _self;

        private const int _saveVersion = 2;
        private const string _extSaveKey = "moreAccessories";
        internal static CharAdditionalData PreviousMigratedData;

        internal static bool _hasDarkness;
        public bool ImportingCards { get; private set; } = true;
        internal static bool CharaMaker => MakerMode != null;

        public static MakerMode MakerMode { get; internal set; }
#if KK || KKS
        internal static bool InH => HMode != null;
        public static HScene HMode { get; internal set; }

        internal static bool InStudio => StudioMode != null;
        public static StudioClass StudioMode { get; internal set; }
#elif EC
        internal bool InPlayMode => PlayMode != null;
        public static PlayMode PlayMode;
        internal bool InADVMode => ADVMode != null;
        public static ADVMode ADVMode;
#endif
    }
}
