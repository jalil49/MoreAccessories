using BepInEx.Logging;

#if EC
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
using System.Collections.Generic;
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

        internal static bool _hasDarkness;
        internal static bool _isParty = false;
        internal static ManualLogSource LogSource;
        internal static bool CharaMaker => MakerMode != null;
        internal bool _loadAdditionalAccessories = true;

        public bool ImportingCards { get; private set; } = true;
        public static bool ClothesFileControlLoading { get; internal set; }
        public static bool CharaListIsLoading { get; internal set; }

        public static MakerMode MakerMode { get; private set; }
#if KK || KKS
        internal static bool InH => HMode != null;
        internal static bool InStudio => StudioMode != null;
        public static HScene HMode;

        public static StudioClass StudioMode { get; private set; }
#elif EC
        private bool _inPlay;
        private readonly List<PlaySceneSlotData> _additionalPlaySceneSlots = new List<PlaySceneSlotData>();
        private RectTransform _playButtonTemplate;
        private HPlayHPartAccessoryCategoryUI _playUI;
        private Coroutine _updatePlayUIHandler;

        private AccessoryUICtrl _advUI;
        public readonly List<ADVSceneSlotData> _additionalADVSceneSlots = new List<ADVSceneSlotData>();
        private RectTransform _advToggleTemplate;
#endif
    }
}
