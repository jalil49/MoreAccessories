using ChaCustom;
using Illusion.Extensions;
using MoreAccessoriesKOI.Extensions;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAccessoriesKOI
{
    /// <summary>
    /// Handles adjusting position of slots/windows and adds scrolling to them.
    /// </summary>
    public class Accessories
    {
        internal CustomAcsChangeSlot _customAcsChangeSlot { get; private set; }
        internal static MoreAccessories Plugin => MoreAccessories._self;
        internal GameObject scrolltemplate;
        #region Properties
        private bool Ready => MoreAccessories.MakerMode.ready;
        internal CustomAcsParentWindow ParentWin { get { return _customAcsChangeSlot.customAcsParentWin; } }
        internal CustomAcsMoveWindow[] MoveWin { get { return _customAcsChangeSlot.customAcsMoveWin; } set { _customAcsChangeSlot.customAcsMoveWin = value; } }
        internal CustomAcsSelectKind[] SelectKind { get { return _customAcsChangeSlot.customAcsSelectKind; } set { _customAcsChangeSlot.customAcsSelectKind = value; } }
        internal CvsAccessory[] CvsAccessoryArray { get { return _customAcsChangeSlot.cvsAccessory; } set { _customAcsChangeSlot.cvsAccessory = value; } }

        internal bool WindowMoved;  //wait for window to be moved to the left before allowing UpdateUI
        public static bool AddInProgress { get; private set; } //Don't allow spamming of the add buttons
        #endregion

        internal Accessories(CustomAcsChangeSlot _instance)
        {
            AddInProgress = false;//in case of something going wrong and its static
            _customAcsChangeSlot = _instance;
            PrepareScroll();
            MakeSlotsScrollable();
            Plugin.ExecuteDelayed(InitilaizeSlotNames, 5);
        }

        internal List<CharaMakerSlotData> AdditionalCharaMakerSlots { get { return MoreAccessories.MakerMode._additionalCharaMakerSlots; } set { MoreAccessories.MakerMode._additionalCharaMakerSlots = value; } }

        private ScrollRect ScrollView;
        private readonly float buttonwidth = 175f;
        private float height;
        private float _slotUIPositionY;
        private RectTransform _addButtonsGroup;
        private VerticalLayoutGroup parentGroup;
        private void PrepareScroll()
        {
            var original_scroll = GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/03_ClothesTop/tglTop/TopTop/Scroll View").GetComponent<ScrollRect>();

            scrolltemplate = DefaultControls.CreateScrollView(new DefaultControls.Resources());
            var scrollrect = scrolltemplate.GetComponent<ScrollRect>();

            scrollrect.verticalScrollbar.GetComponent<Image>().sprite = original_scroll.verticalScrollbar.GetComponent<Image>().sprite;
            scrollrect.verticalScrollbar.image.sprite = original_scroll.verticalScrollbar.image.sprite;

            scrollrect.horizontal = false;
            scrollrect.scrollSensitivity = 40f;

            scrollrect.movementType = ScrollRect.MovementType.Clamped;
            scrollrect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;

            if (scrollrect.horizontalScrollbar != null)
                Object.DestroyImmediate(scrollrect.horizontalScrollbar.gameObject);
            Object.DestroyImmediate(scrollrect.GetComponent<Image>());
        }

        internal void ValidatateToggles()
        {
            var index = _customAcsChangeSlot.GetSelectIndex();
            if (index < 0)
            {
                return;
            }
            var partcount = CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length;
            if (_customAcsChangeSlot.items[index].tglItem.isOn && _customAcsChangeSlot.items[index].cgItem.alpha < .1f)
            {
                _customAcsChangeSlot.items[index].tglItem.Set(false);
                _customAcsChangeSlot.items[0].tglItem.Set(true);
            }
#if KK || KKS
            if (index >= partcount && index < _customAcsChangeSlot.items.Length - 2)
#else
            if (index >= partcount && index < _customAcsChangeSlot.items.Length - 1)
#endif
            {
                _customAcsChangeSlot.CloseWindow();
                _customAcsChangeSlot.items[index].tglItem.Set(false);
                _customAcsChangeSlot.items[0].tglItem.Set(true);
            }
            FixWindowScroll();
        }

        private void MakeSlotsScrollable()
        {
            var container = (RectTransform)_customAcsChangeSlot.transform;

            //adjust size of all buttons (shrunk to take less screenspace/maker window wider)
            foreach (var slotTransform in container.Cast<Transform>())
            {
                var layout = slotTransform.GetComponent<LayoutElement>();
                layout.minWidth = buttonwidth;
                layout.preferredWidth = buttonwidth;
            }

            ScrollView = Object.Instantiate(scrolltemplate, container).GetComponent<ScrollRect>();
            ScrollView.name = "Slots";
            ScrollView.onValueChanged.AddListener(x =>
            {
                FixWindowScroll();
            });
            ScrollView.movementType = ScrollRect.MovementType.Clamped;
            ScrollView.horizontal = false;
            ScrollView.scrollSensitivity = 18f;
            var _charaMakerSlotTemplate = container.GetChild(0).gameObject;

            var rootCanvas = (RectTransform)_charaMakerSlotTemplate.GetComponentInParent<Canvas>().transform;
            var element = ScrollView.gameObject.AddComponent<LayoutElement>();
            height = element.minHeight = rootCanvas.rect.height / 1.298076f;
            element.minWidth = rootCanvas.rect.width * 0.35f;

            var vlg = ScrollView.content.gameObject.AddComponent<VerticalLayoutGroup>();
            parentGroup = container.GetComponent<VerticalLayoutGroup>();

            vlg.childAlignment = parentGroup.childAlignment;
            vlg.childControlHeight = parentGroup.childControlHeight;
            vlg.childControlWidth = parentGroup.childControlWidth;
            vlg.childForceExpandHeight = parentGroup.childForceExpandHeight;
            vlg.childForceExpandWidth = parentGroup.childForceExpandWidth;
            vlg.spacing = parentGroup.spacing;

            ScrollView.content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            ScrollView.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _customAcsChangeSlot.ExecuteDelayed(() =>
            {
                ScrollView.verticalScrollbar.transform.localPosition = new Vector3(-140, ScrollView.verticalScrollbar.transform.localPosition.y, ScrollView.verticalScrollbar.transform.localPosition.z);
                _slotUIPositionY = container.position.y;

            }, 15);
            for (var i = 0; i < CvsAccessoryArray.Length; i++)
            {
                var child = container.GetChild(0);
                MakeWindowScrollable(child);
                container.GetChild(0).SetParent(ScrollView.content);
            }

            ScrollView.transform.SetAsFirstSibling();
            var toggleChange = GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/04_AccessoryTop/tglChange").GetComponent<Toggle>();
            _addButtonsGroup = UIUtility.CreateNewUIObject("Add Buttons Group", ScrollView.content);
            element = _addButtonsGroup.gameObject.AddComponent<LayoutElement>();
            var childreference = ScrollView.content.GetChild(0).GetComponent<LayoutElement>();
            element.preferredWidth = buttonwidth;
            element.preferredHeight = 32;
            var textModel = toggleChange.transform.Find("imgOff").GetComponentInChildren<TextMeshProUGUI>().gameObject;

            var addOneButton = UIUtility.CreateButton("Add One Button", _addButtonsGroup, "+1");
            addOneButton.transform.SetRect(Vector2.zero, new Vector2(0.5f, 1f));
            addOneButton.colors = toggleChange.colors;
            ((Image)addOneButton.targetGraphic).sprite = toggleChange.transform.Find("imgOff").GetComponent<Image>().sprite;
            Object.Destroy(addOneButton.GetComponentInChildren<Text>().gameObject);
            var text = Object.Instantiate(textModel).GetComponent<TextMeshProUGUI>();
            text.transform.SetParent(addOneButton.transform);
            text.rectTransform.SetRect(Vector2.zero, Vector2.one, new Vector2(5f, 4f), new Vector2(-5f, -4f));
            text.text = "+1";
            addOneButton.onClick.AddListener(delegate () { AddSlot(1); });

            var addTenButton = UIUtility.CreateButton("Add Ten Button", _addButtonsGroup, "+10");
            addTenButton.transform.SetRect(new Vector2(0.5f, 0f), Vector2.one);
            addTenButton.colors = toggleChange.colors;
            ((Image)addTenButton.targetGraphic).sprite = toggleChange.transform.Find("imgOff").GetComponent<Image>().sprite;
            Object.Destroy(addTenButton.GetComponentInChildren<Text>().gameObject);
            text = Object.Instantiate(textModel).GetComponent<TextMeshProUGUI>();
            text.transform.SetParent(addTenButton.transform);
            text.rectTransform.SetRect(Vector2.zero, Vector2.one, new Vector2(5f, 4f), new Vector2(-5f, -4f));
            text.text = "+10";
            addTenButton.onClick.AddListener(delegate () { AddSlot(10); });

            LayoutRebuilder.ForceRebuildLayoutImmediate(container);
#if KK || KKS
            for (int i = 0, j = _customAcsChangeSlot.items.Length - 1; i < 2; j--, i++)
#else
            for (int i = 0, j = _customAcsChangeSlot.items.Length - 1; i < 1; j--, i++)
#endif
            {
                var data = _customAcsChangeSlot.items[j];
                var temp = i;
                var index = j;
                data.tglItem.onValueChanged.AddListener(b =>
                {
                    _customAcsChangeSlot.CloseWindow();
                    if (temp == 0)  //Transfer Window
                    {
                        if (CvsAccessoryArray.Length > 20)  //disable slot [index of transfer 22 kk/kks 21 EC] from showing
                        {
                            _customAcsChangeSlot.items[index].cgItem.Enable(false);
                        }
                        MoreAccessories.MakerMode.TransferWindow.WindowRefresh();
                    }
#if KK || KKS
                    if (temp == 1)  //Copy Window
                    {
                        if (CvsAccessoryArray.Length > 21)  //disable slot [index of copy 21 kk/kks] from showing
                        {
                            _customAcsChangeSlot.items[index].cgItem.Enable(false);
                        }
                        MoreAccessories.MakerMode.CopyWindow.WindowRefresh();
                    }
#endif
                    var t = data.cgItem.transform;
                    t.position = new Vector3(t.position.x, _slotUIPositionY);
                });
            }

            _customAcsChangeSlot.ExecuteDelayed(() =>
            {
                CvsAccessoryArray[0].UpdateCustomUI();
                CvsAccessoryArray[0].tglTakeOverParent.Set(false);
                CvsAccessoryArray[0].tglTakeOverColor.Set(false);
            }, 5);

            ScrollView.viewport.gameObject.SetActive(false);

            _customAcsChangeSlot.ExecuteDelayed(() => //Fixes problems with UI masks overlapping and creating bugs
            {
                ScrollView.viewport.gameObject.SetActive(true);
            }, 5);
        }

        private void MakeWindowScrollable(Transform slotTransform)
        {
            var listParent = slotTransform.Cast<Transform>().Where(x => x.name.EndsWith("Top")).First();

            var elements = new List<Transform>();
            foreach (Transform t in listParent)
                elements.Add(t);

            Plugin.ExecuteDelayed(delegate ()
            {
                listParent.localPosition -= new Vector3(50, 0, 0);
                WindowMoved = true;
            });

            var fitter = listParent.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scrollTransform = Object.Instantiate(scrolltemplate, listParent);
            scrollTransform.name = $"Scroll View";

            scrollTransform.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;


            var s_LE = scrollTransform.AddComponent<LayoutElement>();

            s_LE.preferredWidth = 400;
            s_LE.preferredHeight = height;

            var scroll = scrollTransform.GetComponent<ScrollRect>();
            var vlg = scroll.content.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment = parentGroup.childAlignment;
            vlg.childControlHeight = parentGroup.childControlHeight;
            vlg.childControlWidth = parentGroup.childControlWidth;
            vlg.childForceExpandHeight = parentGroup.childForceExpandHeight;
            vlg.childForceExpandWidth = parentGroup.childForceExpandWidth;
            vlg.spacing = parentGroup.spacing;

            scroll.content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            foreach (var item in elements)
            {
                var itemLayout = item.GetComponent<LayoutElement>();
                itemLayout.flexibleWidth = 1;
                item.SetParent(scroll.content);
            }

            slotTransform.SetParent(scroll.content);
        }

        internal void FixWindowScroll()
        {
            var selectedslot = _customAcsChangeSlot.GetSelectIndex();
            if (selectedslot < 0) return;
            var t = _customAcsChangeSlot.items[selectedslot].cgItem.transform;
            t.position = new Vector3(t.position.x, _slotUIPositionY);
        }

        public void UpdateUI()
        {
            if (!Ready)
            {
                return;
            }

            var count = CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length - 20;

            if (count > AdditionalCharaMakerSlots.Count)
            {
                return;
            }
            var cvscolor = CVSColor(count + 21);//do once rather than every time slots are made in case of 10 batch
            var slotindex = 0;

            for (; slotindex < count; slotindex++)
            {
                var info = AdditionalCharaMakerSlots[slotindex];
                if (info.AccessorySlot != null && slotindex < AdditionalCharaMakerSlots.Count)
                {
                    info = AdditionalCharaMakerSlots[slotindex];
                    info.AccessorySlot.SetActive(true);
                    if (slotindex + 20 == CustomBase.Instance.selectSlot)
                        Plugin.ExecuteDelayed(() => info.AccessorySlot.GetComponentInChildren<CvsAccessory>().UpdateCustomUI());
                    CvsAccessoryArray[slotindex + 20].UpdateSlotName();

                    if (info.transferSlotObject) info.transferSlotObject.SetActive(true);
#if KK || KKS
                    if (info.copySlotObject) info.copySlotObject.SetActive(true);
#endif
                }
                else
                {
                    var index = slotindex + 20;
                    var custombase = CustomBase.instance;
                    var newSlot = Object.Instantiate(ScrollView.content.GetChild(0), ScrollView.content);

                    info.AccessorySlot = newSlot.gameObject;
                    var toggle = newSlot.GetComponent<Toggle>();
                    toggle.Set(false);

                    var canvasGroup = toggle.transform.GetChild(1).GetComponentInChildren<CanvasGroup>();
                    var cvsAccessory = toggle.GetComponentInChildren<CvsAccessory>();

                    cvsAccessory.textSlotName = toggle.GetComponentInChildren<TextMeshProUGUI>();

                    CvsAccessoryArray = CvsAccessoryArray.Concat(cvsAccessory).ToArray();

                    cvsAccessory.colorKind = cvscolor;
                    foreach (var item in CvsAccessoryArray)
                    {
                        item.colorKind = cvscolor;
                    }
                    var trans = canvasGroup.transform;
                    trans.position = new Vector3(trans.position.x, _slotUIPositionY);
#if KK || KKS
                    _customAcsChangeSlot.items = _customAcsChangeSlot.items.ConcatNearEnd(new UI_ToggleGroupCtrl.ItemInfo() { tglItem = toggle, cgItem = canvasGroup });
#elif EC
                    _customAcsChangeSlot.items = _customAcsChangeSlot.items.ConcatNearEnd(new UI_ToggleGroupCtrl.ItemInfo() { tglItem = toggle, cgItem = canvasGroup }, 1);
#endif
                    foreach (var _custom in SelectKind)
                    {
                        _custom.cvsAccessory = CvsAccessoryArray;
                    }
                    foreach (var _custom in MoveWin)
                    {
                        _custom.cvsAccessory = CvsAccessoryArray;
                    }

                    ParentWin.cvsAccessory = CvsAccessoryArray;

                    canvasGroup.Enable(false, false);

                    cvsAccessory.textSlotName.text = $"スロット{index + 1:00}";
                    cvsAccessory.slotNo = (CvsAccessory.AcsSlotNo)index;
                    cvsAccessory.CalculateUI();//fixes copying data over from original slot
                    Plugin.ExecuteDelayed(cvsAccessory.CalculateUI);//fixes copying data over from original slot
                    newSlot.name = "tglSlot" + (index + 1).ToString("00");

                    custombase.actUpdateCvsAccessory = custombase.actUpdateCvsAccessory.Concat(new System.Action(cvsAccessory.UpdateCustomUI)).ToArray();
                    custombase.actUpdateAcsSlotName = custombase.actUpdateAcsSlotName.Concat(new System.Action(delegate () { Plugin.ExecuteDelayed(cvsAccessory.UpdateSlotName); })).ToArray(); //delay to avoid an error when called early due to additional patches
                    var newreactive = new BoolReactiveProperty(false);
                    custombase._updateCvsAccessory = custombase._updateCvsAccessory.Concat(newreactive).ToArray();
                    _customAcsChangeSlot.textSlotNames = _customAcsChangeSlot.textSlotNames.Concat(cvsAccessory.textSlotName).ToArray();
                    newreactive.Subscribe(delegate (bool f)
                    {
                        if (index == custombase.selectSlot)
                        {
                            custombase.actUpdateCvsAccessory[index]?.Invoke();
                        }
                        custombase.actUpdateAcsSlotName[index]?.Invoke();
                        custombase._updateCvsAccessory[index].Value = false;
                    });

                    RestoreToggle(toggle, index);

                    _addButtonsGroup.SetAsLastSibling();
                    var action = new System.Action(delegate () { cvsAccessory.Start(); });

                    //Plugin.ExecuteDelayed(action);
                    try
                    {
                        Plugin.NewSlotAdded(index, newSlot.transform);
                    }
                    catch (System.Exception ex)
                    {
                        MoreAccessories.Print(ex.ToString(), BepInEx.Logging.LogLevel.Error);
                    }
                }
            }

            MoreAccessories.MakerMode.ValidatateToggles();

            for (; slotindex < AdditionalCharaMakerSlots.Count; slotindex++)
            {
                var slot = AdditionalCharaMakerSlots[slotindex];
                if (slot.AccessorySlot) slot.AccessorySlot.SetActive(false);
                if (slot.transferSlotObject) slot.transferSlotObject.SetActive(false);
#if KK || KKS
                if (slot.copySlotObject) slot.copySlotObject.SetActive(false);
#endif
            }
            _addButtonsGroup.SetAsLastSibling();
            FixWindowScroll();
        }

        private int[,] CVSColor(int rank)
        {
            var newarray = new int[rank, 4];
            var value = 124;
            for (var i = 0; i < 20; i++)
            {
                for (var j = 0; j < 4; j++, value++)
                {
                    newarray[i, j] = value;
                }
            }
            //there is a break here with KKS since they appended to end of enum
            value = 5000;
            for (var i = 20; i < rank; i++)
            {
                for (var j = 0; j < 4; j++, value++)
                {
                    newarray[i, j] = value;
                }
            }
            return newarray;
        }

        private void RestoreToggle(Toggle toggle, int index)
        {
            toggle.OnValueChangedAsObservable().Subscribe(x =>
            {
                //code will trigger when changed to false STOP IT
                if (!x)
                {
                    return;
                }

                if (index >= CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length)
                {
                    ParentWin.ChangeSlot(index, false);
                    var arr3 = _customAcsChangeSlot.customAcsMoveWin;
                    for (var j = 0; j < arr3.Length; j++)
                    {
                        arr3[j].ChangeSlot(index, false);
                    }
                    var arr4 = _customAcsChangeSlot.customAcsSelectKind;
                    for (var j = 0; j < arr4.Length; j++)
                    {
                        arr4[j].ChangeSlot(index, false);
                    }
                    return;
                }
                var open = false;
                if (120 != CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts[index].type)
                {
                    open = true;
                }
                ParentWin.ChangeSlot(index, open);
                var array3 = _customAcsChangeSlot.customAcsMoveWin;
                for (var j = 0; j < array3.Length; j++)
                {
                    array3[j].ChangeSlot(index, open);
                }
                var array4 = _customAcsChangeSlot.customAcsSelectKind;
                for (var j = 0; j < array4.Length; j++)
                {
                    array4[j].ChangeSlot(index, open);
                }
                Singleton<CustomBase>.Instance.selectSlot = index;
                Singleton<CustomBase>.Instance.SetUpdateCvsAccessory(index, true);
                if (_customAcsChangeSlot.backIndex != index)
                {
                    _customAcsChangeSlot.ChangeColorWindow(index);
                }

                _customAcsChangeSlot.backIndex = index;
                FixWindowScroll();

                for (var i = 0; i < _customAcsChangeSlot.items.Length; i++)
                {
                    var info = _customAcsChangeSlot.items[i];
                    if (index == i && toggle.isOn)
                    {
                        info.cgItem.Enable(true, false);
                        continue;
                    }
                    info.cgItem.Enable(false, false);
                }
            });
        }

        public static void AddSlot(int num)
        {
            if (AddInProgress || !CustomBase.instance) return;
            AddInProgress = true;
            var controller = CustomBase.instance.chaCtrl;
            var nowparts = controller.nowCoordinate.accessory.parts;
#if KK || KKS
            var coordacc = controller.chaFile.coordinate[controller.chaFile.status.coordinateType].accessory;
#else
            var coordacc = controller.chaFile.coordinate.accessory;
#endif
            var newpart = new ChaFileAccessory.PartsInfo[num];
            for (var i = 0; i < num; i++)
            {
                newpart[i] = new ChaFileAccessory.PartsInfo();
            }
            coordacc.parts = controller.nowCoordinate.accessory.parts = nowparts.Concat(newpart).ToArray();
            MoreAccessories.ArraySync(controller);
            MoreAccessories.MakerMode.UpdateMakerUI();
            AddInProgress = false;
        }

        private void InitilaizeSlotNames()
        {
            foreach (var item in CvsAccessoryArray)
            {
                item.UpdateSlotName();
            }
        }
    }
}
