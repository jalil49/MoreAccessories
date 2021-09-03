using ChaCustom;
using HarmonyLib;
using Illusion.Extensions;
using Localize.Translate;
using MoreAccessoriesKOI.Extensions;
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
        internal GameObject scrolltemplate;
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
            PrepareScroll();
            MakeSlotsScrollable();
        }

        internal List<CharaMakerSlotData> AdditionalCharaMakerSlots { get { return Plugin.MakerMode._additionalCharaMakerSlots; } set { Plugin.MakerMode._additionalCharaMakerSlots = value; } }

        private ScrollRect ScrollView;
        private float buttonwidth = 175;
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

        private void MakeSlotsScrollable()
        {

            var container = (RectTransform)GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/04_AccessoryTop").transform;


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
                Plugin.ExecuteDelayed(FixWindowScroll);
            });
            ScrollView.movementType = ScrollRect.MovementType.Clamped;
            ScrollView.horizontal = false;
            ScrollView.scrollSensitivity = 18f;
            ScrollView.verticalScrollbar.transform.position -= new Vector3(400, 0, 0);
            ScrollView.transform.position -= new Vector3(50, 0, 0);
            //if (ScrollView.verticalScrollbar != null)
            //    Object.Destroy(ScrollView.verticalScrollbar.gameObject);
            //Object.Destroy(ScrollView.content.GetComponent<Image>());
            var _charaMakerSlotTemplate = container.GetChild(0).gameObject;

            var rootCanvas = (RectTransform)_charaMakerSlotTemplate.GetComponentInParent<Canvas>().transform;
            var element = ScrollView.gameObject.AddComponent<LayoutElement>();
            height = element.minHeight = rootCanvas.rect.height / 1.298076f;
            element.minWidth = 680f; //Because trying to get the value dynamically fails for some reason so fuck it.
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
                _slotUIPositionY = _charaMakerSlotTemplate.transform.parent.position.y;
            }, 15);

            var kkus = System.Type.GetType("HSUS.HSUS,KKUS");
            if (kkus != null)
            {
                var self = kkus.GetField("_self", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                var scale = Traverse.Create(self).Field("_gameUIScale").GetValue<float>();
                //element.minHeight = element.minHeight / scale + 160f * (1f - scale);
            }

            for (var i = 0; i < 20; i++)
            {
                var child = container.GetChild(0);
                MakeWindowScrollable(child);
                container.GetChild(0).SetParent(ScrollView.content);
            }


            ScrollView.transform.SetAsFirstSibling();
            var toggleChange = GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/04_AccessoryTop/tglChange").GetComponent<Toggle>();
            _addButtonsGroup = UIUtility.CreateNewUIObject("Add Buttons Group", ScrollView.content);
            element = _addButtonsGroup.gameObject.AddComponent<LayoutElement>();
            element.preferredWidth = buttonwidth;
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
        }

        internal void FixWindowScroll()
        {
            var t = _customAcsChangeSlot.items[CustomBase.Instance.selectSlot].cgItem.transform;
            t.position = new Vector3(t.position.x, _slotUIPositionY);
        }

        private void MakeWindowScrollable(Transform slotTransform)
        {
            var listParent = slotTransform.Cast<Transform>().Where(x => x.name.EndsWith("Top")).First();
            //Object.Destroy(listParent.GetComponent<Image>());//Destroy image that contains scrollbar
            var elements = new List<Transform>();
            foreach (Transform t in listParent)
                elements.Add(t);

            listParent.position -= new Vector3(30, 0, 0);

            var fitter = listParent.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scrollTransform = Object.Instantiate(scrolltemplate);
            scrollTransform.name = $"{slotTransform.name}ScrollView";
            scrollTransform.transform.SetParent(listParent, false);

            scrollTransform.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;


            var s_LE = scrollTransform.AddComponent<LayoutElement>();

            s_LE.preferredWidth = 400;
            s_LE.preferredHeight = height;
            //s_LE.flexibleHeight = 1;

            var scroll = scrollTransform.GetComponent<ScrollRect>();
            scroll.viewport.transform.position -= new Vector3(300, 0, 0);
            var vlg = scroll.content.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment = parentGroup.childAlignment;
            vlg.childControlHeight = parentGroup.childControlHeight;
            vlg.childControlWidth = parentGroup.childControlWidth;
            vlg.childForceExpandHeight = parentGroup.childForceExpandHeight;
            vlg.childForceExpandWidth = parentGroup.childForceExpandWidth;
            vlg.spacing = parentGroup.spacing;

            //var vlg2 = scroll.viewport.gameObject.AddComponent<VerticalLayoutGroup>();
            //vlg2.childAlignment = parentGroup.childAlignment;
            //vlg2.childControlHeight = parentGroup.childControlHeight;
            //vlg2.childControlWidth = parentGroup.childControlWidth;
            //vlg2.childForceExpandHeight = parentGroup.childForceExpandHeight;
            //vlg2.childForceExpandWidth = parentGroup.childForceExpandWidth;
            //vlg2.spacing = parentGroup.spacing;


            scroll.content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            //scroll.viewport.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;


            foreach (var item in elements)
                item.SetParent(scroll.content);
            slotTransform.SetParent(scroll.content);
        }

        public void UpdateUI()
        {
            if (CustomBase.instance.chaCtrl == null)
            {
                return;
            }
            var count = CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length - 20;
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
                    info.transferSlotObject.SetActive(true);
                    info.copySlotObject.SetActive(true);
                }
                else
                {
                    var index = slotindex + 20;
                    var custombase = CustomBase.instance;
                    var newSlot = Object.Instantiate(ScrollView.content.GetChild(0), ScrollView.content);

                    info.AccessorySlot = newSlot.gameObject;
                    var toggle = newSlot.GetComponent<Toggle>();
                    var canvasGroup = toggle.transform.GetChild(1).GetComponentInChildren<CanvasGroup>();
                    var cvsAccessory = toggle.GetComponentInChildren<CvsAccessory>();
                    cvsAccessory.textSlotName = toggle.GetComponentInChildren<TextMeshProUGUI>();

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

                    ParentWin.cvsAccessory = CvsAccessoryArray;

                    toggle.isOn = false;
                    canvasGroup.Enable(false, false);

                    RestoreToggle(toggle, index);

                    cvsAccessory.textSlotName.text = $"スロット{index + 1:00}";
                    cvsAccessory.slotNo = (CvsAccessory.AcsSlotNo)index;
                    newSlot.name = "tglSlot" + (index + 1).ToString("00");

                    custombase.actUpdateCvsAccessory = custombase.actUpdateCvsAccessory.Concat(new System.Action(cvsAccessory.UpdateCustomUI)).ToArray();
                    custombase.actUpdateAcsSlotName = custombase.actUpdateAcsSlotName.Concat(new System.Action(cvsAccessory.UpdateSlotName)).ToArray();
                    var newreactive = new BoolReactiveProperty(false);
                    custombase._updateCvsAccessory = custombase._updateCvsAccessory.Concat(newreactive).ToArray();
                    newreactive.Subscribe(delegate (bool f)
                    {
                        if (index == custombase.selectSlot)
                        {
                            custombase.actUpdateCvsAccessory[index]?.Invoke();
                        }
                        custombase.actUpdateAcsSlotName[index]?.Invoke();
                        custombase._updateCvsAccessory[index].Value = false;
                    });

                    _addButtonsGroup.SetAsLastSibling();
                    cvsAccessory.Start();
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

            for (; slotindex < AdditionalCharaMakerSlots.Count; slotindex++)
            {
                var slot = AdditionalCharaMakerSlots[slotindex];
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
    }
}
