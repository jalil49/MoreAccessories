using System.Collections.Generic;
using ChaCustom;
using UnityEngine;
using UnityEngine.UI;
using BepInEx.Logging;

#if EC
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
#endif
#if KK || KKS
using Studio;
#endif

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public const string versionNum = "2.0.0";
        public const string GUID = "com.joan6694.illusionplugins.moreaccessories";

        public static MoreAccessories _self;

        #region Private Variables
        private const int _saveVersion = 2;
        private const string _extSaveKey = "moreAccessories";

        internal bool _hasDarkness;
        internal bool _isParty = false;
        internal static ManualLogSource LogSource;
        internal static bool CharaMaker => _self.MakerMode != null;
        internal bool _loadAdditionalAccessories = true;
        private CustomFileWindow _loadCoordinatesWindow;

        public MakerMode MakerMode { get; private set; }
        public StudioClass StudioMode { get; private set; }
#if KK || KKS
        internal static bool _inH => _self.HMode != null;
        public HScene HMode;
#elif EC
        private bool _inPlay;
        private readonly List<PlaySceneSlotData> _additionalPlaySceneSlots = new List<PlaySceneSlotData>();
        private RectTransform _playButtonTemplate;
        private HPlayHPartAccessoryCategoryUI _playUI;
        private Coroutine _updatePlayUIHandler;

        private AccessoryUICtrl _advUI;
        private readonly List<ADVSceneSlotData> _additionalADVSceneSlots = new List<ADVSceneSlotData>();
        private RectTransform _advToggleTemplate;
#endif
        #endregion
    }
}
