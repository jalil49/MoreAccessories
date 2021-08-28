using System;
using System.Collections.Generic;
using ChaCustom;
#if EMOTIONCREATORS
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
#endif
#if KOIKATSU
using Studio;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public const string versionNum = "1.1.0";
        public const string GUID = "com.joan6694.illusionplugins.moreaccessories";

        #region Private Variables
        public static MoreAccessories _self; //Not internal because other plugins might access this
        private const int _saveVersion = 1;
        private const string _extSaveKey = "moreAccessories";
        private GameObject _charaMakerSlotTemplate;
        private ScrollRect _charaMakerScrollView;
        internal CustomAcsChangeSlot _customAcsChangeSlot;
        internal CustomAcsParentWindow _customAcsParentWin;
        internal CustomAcsMoveWindow[] _customAcsMoveWin;
        internal CustomAcsSelectKind[] _customAcsSelectKind;
        internal CvsAccessory[] _cvsAccessory;
        internal List<CharaMakerSlotData> _additionalCharaMakerSlots = new List<CharaMakerSlotData>();
        public readonly WeakKeyDictionary<ChaFile, CharAdditionalData> _accessoriesByChar = new WeakKeyDictionary<ChaFile, CharAdditionalData>(); //Sorry Prefer this to be public 
        public readonly WeakKeyDictionary<ChaFileCoordinate, WeakReference> _charByCoordinate = new WeakKeyDictionary<ChaFileCoordinate, WeakReference>();
        public CharAdditionalData _charaMakerData = null;
        private float _slotUIPositionY;
        internal bool _hasDarkness;
        internal bool _isParty = false;

        private bool _inCharaMaker = false;
        private RectTransform _addButtonsGroup;
#if KOIKATSU
        private ScrollRect _charaMakerCopyScrollView;
        private GameObject _copySlotTemplate;
#endif
        private ScrollRect _charaMakerTransferScrollView;
        private GameObject _transferSlotTemplate;
        private List<UI_RaycastCtrl> _raycastCtrls = new List<UI_RaycastCtrl>();
        private ChaFile _overrideCharaLoadingFilePre;
        private ChaFile _overrideCharaLoadingFilePost;
        private bool _loadAdditionalAccessories = true;
        private CustomFileWindow _loadCoordinatesWindow;

#if KOIKATSU
        private bool _inH;
        internal List<ChaControl> _hSceneFemales;
        private List<HSprite.FemaleDressButtonCategory> _hSceneMultipleFemaleButtons;
        private List<List<HSceneSlotData>> _additionalHSceneSlots = new List<List<HSceneSlotData>>();
        private HSprite _hSprite;
        private HSceneSpriteCategory _hSceneSoloFemaleAccessoryButton;

        private StudioSlotData _studioToggleAll;
        private RectTransform _studioToggleTemplate;
        private bool _inStudio;
        private OCIChar _selectedStudioCharacter;
        private readonly List<StudioSlotData> _additionalStudioSlots = new List<StudioSlotData>();
        private StudioSlotData _studioToggleMain;
        private StudioSlotData _studioToggleSub;
#elif EMOTIONCREATORS
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
