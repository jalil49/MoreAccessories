using System.Collections.Generic;
using ChaCustom;
#if EC
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
#endif
#if KK || KKS
using Studio;
#endif
using UnityEngine;
using UnityEngine.UI;

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


        private GameObject _charaMakerSlotTemplate;
        private ScrollRect _charaMakerScrollView;
        internal CustomAcsChangeSlot _customAcsChangeSlot;

        internal List<CharaMakerSlotData> _additionalCharaMakerSlots = new List<CharaMakerSlotData>();
        private float _slotUIPositionY;
        internal bool _hasDarkness;
        internal bool _isParty = false;

        private bool _inCharaMaker = false;
        private RectTransform _addButtonsGroup;
#if KK || KKS
        private ScrollRect _charaMakerCopyScrollView;
        private GameObject _copySlotTemplate;
#endif
        private ScrollRect _charaMakerTransferScrollView;
        private GameObject _transferSlotTemplate;
        private List<UI_RaycastCtrl> _raycastCtrls = new List<UI_RaycastCtrl>();
        private bool _loadAdditionalAccessories = true;
        private CustomFileWindow _loadCoordinatesWindow;

#if KK || KKS
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

        #region Properties
        internal CustomAcsParentWindow ParentWin { get { return _customAcsChangeSlot.customAcsParentWin; } set { _customAcsChangeSlot.customAcsParentWin = value; } }
        internal CustomAcsMoveWindow[] MoveWin { get { return _customAcsChangeSlot.customAcsMoveWin; } set { _customAcsChangeSlot.customAcsMoveWin = value; } }
        internal CustomAcsSelectKind[] SelectKind { get { return _customAcsChangeSlot.customAcsSelectKind; } set { _customAcsChangeSlot.customAcsSelectKind = value; } }
        internal CvsAccessory[] CvsAccessoryArray { get { return _customAcsChangeSlot.cvsAccessory; } set { _customAcsChangeSlot.cvsAccessory = value; } }
        #endregion
    }
}
