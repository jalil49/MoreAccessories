using ChaCustom;
using HarmonyLib;
using Illusion.Extensions;
using MoreAccessoriesKOI.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAccessoriesKOI
{
    public class Accessories
    {
        internal CustomAcsChangeSlot _customAcsChangeSlot { get; private set; }

        internal static MoreAccessories Plugin => MoreAccessories._self;

        #region Properties
        internal CustomAcsParentWindow ParentWin { get { return _customAcsChangeSlot.customAcsParentWin; } set { _customAcsChangeSlot.customAcsParentWin = value; } }
        internal CustomAcsMoveWindow[] MoveWin { get { return _customAcsChangeSlot.customAcsMoveWin; } set { _customAcsChangeSlot.customAcsMoveWin = value; } }
        internal CustomAcsSelectKind[] SelectKind { get { return _customAcsChangeSlot.customAcsSelectKind; } set { _customAcsChangeSlot.customAcsSelectKind = value; } }
        internal CvsAccessory[] CvsAccessoryArray { get { return _customAcsChangeSlot.cvsAccessory; } set { _customAcsChangeSlot.cvsAccessory = value; } }

        public static bool AddInProgress { get; internal set; }
        #endregion

        public Accessories(CustomAcsChangeSlot _instance)
        {
            _customAcsChangeSlot = _instance;
            MakeSlotsScrollable();
        }

        internal List<CharaMakerSlotData> AdditionalCharaMakerSlots { get { return Plugin.MakerMode._additionalCharaMakerSlots; } set { Plugin.MakerMode._additionalCharaMakerSlots = value; } }

        private ScrollRect ScrollView;

        private float _slotUIPositionY;
        private RectTransform _addButtonsGroup;

        private void MakeSlotsScrollable()
        {
            var container = (RectTransform)GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/04_AccessoryTop").transform;
            Sprite scroll_bar_area_sprite = null;
            Sprite scroll_bar_handle_sprite = null;
            Image content_image = null;
#if KK || KKS
            var original_scroll = GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/03_ClothesTop/tglTop/TopTop/Scroll View").transform.GetComponent<ScrollRect>();
#if KKS
            content_image = original_scroll.content.GetComponent<Image>();
#endif
            scroll_bar_area_sprite = original_scroll.verticalScrollbar.GetComponent<Image>().sprite;
            scroll_bar_handle_sprite = original_scroll.verticalScrollbar.image.sprite;
#if KK
            var root = container.parent.parent.parent;
            root.Find("tglCopy").GetComponent<LayoutElement>().minWidth = 200f;
            root.Find("tglChange").GetComponent<LayoutElement>().minWidth = 200f;
#endif
            foreach (var slotTransform in container.Cast<Transform>())
            {
                var layout = slotTransform.GetComponent<LayoutElement>();
                layout.minWidth = 200f;
                layout.preferredWidth = 200f;
            }
#endif
            ScrollView = UIUtility.CreateScrollView("Slots", container);
            ScrollView.onValueChanged.AddListener(x =>
            {
                FixWindowScroll();
                Plugin.ExecuteDelayed(FixWindowScroll);
            });
            ScrollView.movementType = ScrollRect.MovementType.Clamped;
            ScrollView.horizontal = false;
            ScrollView.scrollSensitivity = 18f;
            if (ScrollView.horizontalScrollbar != null)
                Object.Destroy(ScrollView.horizontalScrollbar.gameObject);
            if (ScrollView.verticalScrollbar != null)
                Object.Destroy(ScrollView.verticalScrollbar.gameObject);
            Object.Destroy(ScrollView.GetComponent<Image>());
            var _charaMakerSlotTemplate = container.GetChild(0).gameObject;

            var rootCanvas = (RectTransform)_charaMakerSlotTemplate.GetComponentInParent<Canvas>().transform;
            var element = ScrollView.gameObject.AddComponent<LayoutElement>();
            element.minHeight = rootCanvas.rect.height / 1.298076f;
            element.minWidth = 622f; //Because trying to get the value dynamically fails for some reason so fuck it.
            var group = ScrollView.content.gameObject.AddComponent<VerticalLayoutGroup>();
            var parentGroup = container.GetComponent<VerticalLayoutGroup>();
            group.childAlignment = parentGroup.childAlignment;
            group.childControlHeight = parentGroup.childControlHeight;
            group.childControlWidth = parentGroup.childControlWidth;
            group.childForceExpandHeight = parentGroup.childForceExpandHeight;
            group.childForceExpandWidth = parentGroup.childForceExpandWidth;
            group.spacing = parentGroup.spacing;
            ScrollView.content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _customAcsChangeSlot.ExecuteDelayed(() =>
            {
                _slotUIPositionY = _charaMakerSlotTemplate.transform.parent.position.y;
            }, 15);

            var kkus = System.Type.GetType("HSUS.HSUS,KKUS");
            if (kkus != null)
            {
                var self = kkus.GetField("_self", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                var scale = Traverse.Create(self).Field("_gameUIScale").GetValue<float>();
                element.minHeight = element.minHeight / scale + 160f * (1f - scale);
            }

            for (var i = 0; i < 20; i++)
            {
                var child = container.GetChild(0);
                //MakeWindowScrollable(child, content_image, scroll_bar_area_sprite, scroll_bar_handle_sprite);
                container.GetChild(0).SetParent(ScrollView.content);
            }

            ScrollView.transform.SetAsFirstSibling();
            var toggleChange = GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/04_AccessoryTop/tglChange").GetComponent<Toggle>();
            _addButtonsGroup = UIUtility.CreateNewUIObject("Add Buttons Group", ScrollView.content);
            element = _addButtonsGroup.gameObject.AddComponent<LayoutElement>();
            element.preferredWidth = 224f;
            element.preferredHeight = 32f;
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

            for (int i = 0, j = _customAcsChangeSlot.items.Length - 1; i < 2; j--, i++)
            {
                var data = _customAcsChangeSlot.items[j];
                data.tglItem.onValueChanged.AddListener(b =>
                {
                    var t = data.cgItem.transform;
                    t.position = new Vector3(t.position.x, _slotUIPositionY);
                });
            }

            _customAcsChangeSlot.ExecuteDelayed(() =>
            {
                CvsAccessoryArray[0].UpdateCustomUI();
                CvsAccessoryArray[0].tglTakeOverParent.isOn = false;
                CvsAccessoryArray[0].tglTakeOverColor.isOn = false;
            }, 5);


            ScrollView.viewport.gameObject.SetActive(false);

            _customAcsChangeSlot.ExecuteDelayed(() => //Fixes problems with UI masks overlapping and creating bugs
            {
                ScrollView.viewport.gameObject.SetActive(true);
            }, 5);
            _customAcsChangeSlot.ExecuteDelayed(() =>
            {
                Plugin.MakerMode.UpdateMakerUI();
                CustomBase.Instance.updateCustomUI = true;
            }, 5);

        }

        internal void FixWindowScroll()
        {
            var t = _customAcsChangeSlot.items[CustomBase.Instance.selectSlot].cgItem.transform;
            t.position = new Vector3(t.position.x, _slotUIPositionY);
        }

        internal void MakeWindowScrollable(Transform slotTransform, Image content_image, Sprite scroll_bar_area_sprite, Sprite scroll_bar_handle_sprite)
        {
#if KK || KKS
            var listParent = slotTransform.Cast<Transform>().Where(x => x.name.EndsWith("Top")).First();
#if KKS
            Object.DestroyImmediate(listParent.GetComponent<Image>());//Destroy image that contains scrollbar
#endif
            var elements = new List<Transform>();
            foreach (Transform t in listParent)
                elements.Add(t);

            var fitter = listParent.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scrollTransform = DefaultControls.CreateScrollView(new DefaultControls.Resources());
            scrollTransform.name = $"{slotTransform.name}ScrollView";
            scrollTransform.transform.SetParent(listParent.transform, false);
            listParent.transform.position -= new Vector3(30, 0, 0);
            var scroll = scrollTransform.GetComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.scrollSensitivity = 40f;

            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scroll.verticalScrollbar.image.sprite = scroll_bar_handle_sprite;
            scroll.verticalScrollbar.GetComponent<Image>().sprite = scroll_bar_area_sprite;

#if KKS
            //Add image that doesn't contain scroll bar
            var image = scroll.content.gameObject.AddComponent<Image>();
            image.sprite = content_image.sprite;
            image.type = content_image.type;
#endif
            Object.DestroyImmediate(scroll.horizontalScrollbar.gameObject);
            var content = scroll.content.transform;
            Object.Destroy(scroll.GetComponent<Image>());

            var s_LE = scroll.gameObject.AddComponent<LayoutElement>();
#if KK
                var height = (GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/04_AccessoryTop/Slots").transform as RectTransform).rect.height;
                var width = 400f;
#else
            var height = 875f;   //Slots from KK doesn't exist
            var width = 400f;
#endif
            s_LE.preferredHeight = height;
            s_LE.preferredWidth = width;

            var vlg = scroll.content.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            scroll.content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            foreach (var item in elements)
                item.SetParent(scroll.content);
            slotTransform.SetParent(scroll.content);
#endif

        }

        public void UpdateUI()
        {
            var count = CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length - 20;
            var i = 0;
            for (; i < count; i++)
            {
                var info = AdditionalCharaMakerSlots[i];
                if (info.AccessorySlot != null && i < AdditionalCharaMakerSlots.Count)
                {
                    info = AdditionalCharaMakerSlots[i];
                    info.AccessorySlot.SetActive(true);
                    //if (i + 20 == CustomBase.Instance.selectSlot)
                    //    Plugin.ExecuteDelayed(() => info.cvsAccessory.UpdateCustomUI());
                    info.transferSlotObject.SetActive(true);
                    info.copySlotObject.SetActive(true);
                }
                else
                {
                    var index = i + 20;
                    Plugin.Print($"1");
                    MoreAccessories.ArrayExpansion(ref CustomBase.instance.actUpdateCvsAccessory);
                    MoreAccessories.ArrayExpansion(ref CustomBase.instance.actUpdateAcsSlotName);
                    var custombase = CustomBase.instance;

                    var reactive = custombase._updateCvsAccessory = custombase._updateCvsAccessory.Concat(new BoolReactiveProperty(false)).ToArray();
                    Plugin.Print($"2");

                    var newSlot = Object.Instantiate(ScrollView.content.GetChild(0), ScrollView.content);

                    info = new CharaMakerSlotData { AccessorySlot = newSlot.gameObject, };
                    var toggle = newSlot.GetComponent<Toggle>();
                    var text = toggle.GetComponentInChildren<TextMeshProUGUI>();
                    var canvasGroup = toggle.transform.GetChild(1).GetComponent<CanvasGroup>();
                    var cvsAccessory = toggle.GetComponentInChildren<CvsAccessory>();
                    Plugin.Print($"3");

                    toggle.onValueChanged = new Toggle.ToggleEvent();

                    Plugin.Print($"4");

                    CvsAccessoryArray = CvsAccessoryArray.Concat(cvsAccessory).ToArray();

                    var uigroups = _customAcsChangeSlot.items = _customAcsChangeSlot.items.ConcatNearEnd(new UI_ToggleGroupCtrl.ItemInfo() { tglItem = toggle, cgItem = canvasGroup });
                    foreach (var _custom in SelectKind)
                    {
                        _custom.cvsAccessory = CvsAccessoryArray;
                    }
                    foreach (var _custom in MoveWin)
                    {
                        _custom.cvsAccessory = CvsAccessoryArray;
                    }
                    Plugin.Print($"5");

                    ParentWin.cvsAccessory = CvsAccessoryArray;

                    //toggle.onValueChanged = new Toggle.ToggleEvent();
                    toggle.isOn = false;
                    canvasGroup.Enable(false, false);

                    RestoreToggle(toggle, index);
                    Plugin.Print($"6");

                    text.text = $"スロット{index + 1:00}";
                    cvsAccessory.slotNo = (CvsAccessory.AcsSlotNo)index;
                    newSlot.name = "tglSlot" + (index + 1).ToString("00");

                    // _ = cvsAccessory.Start();

                    _addButtonsGroup.SetAsLastSibling();
                    Plugin.Print($"7");

                    Plugin.ExecuteDelayed(() =>
                    {
                        cvsAccessory.UpdateCustomUI();
                    }, 5);


                    reactive[index].Subscribe(x =>
                    {

                        if (custombase.selectSlot == index)
                            custombase.actUpdateCvsAccessory[index]();
                    });

                    Plugin.NewSlotAdded(index, newSlot.transform);
                }
            }

            for (; i < AdditionalCharaMakerSlots.Count; i++)
            {
                var slot = AdditionalCharaMakerSlots[i];
                slot.AccessorySlot.SetActive(false);
                slot.transferSlotObject.SetActive(false);
                slot.copySlotObject.SetActive(false);
            }
            _addButtonsGroup.SetAsLastSibling();
        }

        private void RestoreToggle(Toggle toggle, int index)
        {
            toggle.onValueChanged.AddListener(x =>
            {
                if (!x) return;

                var open = false;
                if (120 != CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts[index].type)
                {
                    open = true;
                }
                if (CustomBase.instance.chaCtrl.hideHairAcs[index])
                {
                    open = false;
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

        private static void AddSlot(int num)
        {
            if (AddInProgress) return;
            AddInProgress = true;
            var controller = CustomBase.instance.chaCtrl;
            var nowparts = controller.nowCoordinate.accessory.parts;
            var coordacc = controller.chaFile.coordinate[controller.chaFile.status.coordinateType].accessory;
            var newpart = new ChaFileAccessory.PartsInfo[num];
            for (var i = 0; i < num; i++)
            {
                newpart[i] = new ChaFileAccessory.PartsInfo();
            }
            coordacc.parts = controller.nowCoordinate.accessory.parts = nowparts.Concat(newpart).ToArray();
            MoreAccessories.ArraySync(controller);
            Plugin.MakerMode.UpdateMakerUI();
            AddInProgress = false;
        }

        //private void AccessorySlotToggleCallback(int index, Toggle toggle)
        //{
        //    if (toggle.isOn)
        //    {
        //        CustomBase.Instance.selectSlot = index;
        //        var open = GetPart(index).type != 120;
        //        _customAcsParentWin.ChangeSlot(index, open);
        //        foreach (var customAcsMoveWindow in _customAcsMoveWin)
        //            customAcsMoveWindow.ChangeSlot(index, open);
        //        foreach (var customAcsSelectKind in _customAcsSelectKind)
        //            customAcsSelectKind.ChangeSlot(index, open);

        //        Singleton<CustomBase>.Instance.selectSlot = index;
        //        //if (index < 20)
        //        Singleton<CustomBase>.Instance.SetUpdateCvsAccessory(index, true);
        //        //else
        //        //{
        //        //    var accessory = GetCvsAccessory(index);
        //        //    if (index == CustomBase.Instance.selectSlot)
        //        //        accessory.UpdateCustomUI();
        //        //    accessory.UpdateSlotName();
        //        //}
        //if (_customAcsChangeSlot.backIndex != index)
        //    _customAcsChangeSlot.ChangeColorWindow(index);
        //        _customAcsChangeSlot.backIndex = index;
        //    }
        //}

        //private void AccessorySlotCanvasGroupCallback(Toggle toggle, CanvasGroup canvasGroup)
        //{
        //    for (var i = 0; i < _customAcsChangeSlot.items.Length; i++)
        //    {
        //        var info = _customAcsChangeSlot.items[i];
        //        if (info.cgItem != null)
        //            info.cgItem.Enable(false, false);
        //    }
        //    if (toggle.isOn && canvasGroup)
        //        canvasGroup.Enable(true, false);
        //}
    }
}
