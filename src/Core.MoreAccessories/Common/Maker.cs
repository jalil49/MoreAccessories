using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using ChaCustom;
using HarmonyLib;
#if EC
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
#endif
using Illusion.Extensions;
#if KKS
using Cysharp.Threading.Tasks;
#endif
using TMPro;
using MoreAccessoriesKOI.Extensions;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        /// <summary>
        /// Get the total of accessory UI element in the chara maker (vanilla + additional).
        /// </summary>
        /// <returns></returns>
        public int GetCvsAccessoryCount()
        {
            if (_inCharaMaker)
                return _additionalCharaMakerSlots.Count + 20;
            return 0;
        }

        private async UniTask UpdateMakerUI()
        {
            if (_customAcsChangeSlot == null || CustomBase.instance.chaCtrl == null)
                return;

            var count = CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length - 20;
            var i = 0;
            for (; i < count; i++)
            {
                _self.Logger.LogWarning($"index {i + 20} {i}/{count - 1}");

                CharaMakerSlotData info;
                if (i < _additionalCharaMakerSlots.Count)
                {
                    info = _additionalCharaMakerSlots[i];
                    info.toggle.gameObject.SetActive(true);
                    if (i + 20 == CustomBase.Instance.selectSlot)
                        _self.ExecuteDelayed(() => info.cvsAccessory.UpdateCustomUI());

                    info.transferSlotObject.SetActive(true);
                }
                else
                {
                    var index = i + 20;

                    ArrayExpansion(ref CustomBase.instance.actUpdateCvsAccessory);
                    ArrayExpansion(ref CustomBase.instance.actUpdateAcsSlotName);
                    var custombase = CustomBase.instance;

                    var reactive = custombase._updateCvsAccessory = custombase._updateCvsAccessory.Concat(new BoolReactiveProperty(false)).ToArray();
                    _self.Logger.LogWarning("t-3");

                    var newSlot = Instantiate(_charaMakerSlotTemplate.transform, _charaMakerScrollView.content);
                    info = new CharaMakerSlotData { toggle = newSlot.GetComponent<Toggle>(), };
                    _self.Logger.LogWarning("t-2");

                    info.text = info.toggle.GetComponentInChildren<TextMeshProUGUI>();
                    info.canvasGroup = info.toggle.transform.GetChild(1).GetComponent<CanvasGroup>();
                    info.cvsAccessory = info.toggle.GetComponentInChildren<CvsAccessory>();

                    info.toggle.onValueChanged = new Toggle.ToggleEvent();
                    info.toggle.isOn = false;

                    _self.Logger.LogWarning($"ddAcsType is null? {info.cvsAccessory.ddAcsType == null}  {info.cvsAccessory.ddAcsType.value} {info.cvsAccessory.ddAcsType.value - 1}");
                    _self.Logger.LogWarning($"tglAcsKind is null? {info.cvsAccessory.tglAcsKind == null} {info.cvsAccessory.tglAcsKind.isOn}");
                    _self.Logger.LogWarning($"cgAccessoryWin is null? {info.cvsAccessory.cgAccessoryWin == null} {info.cvsAccessory.cgAccessoryWin.Any(x => x == null)} {info.cvsAccessory.cgAccessoryWin.Length}");

                    _self.Logger.LogWarning($"tglAcsKind is null? {info.cvsAccessory.tglAcsKind == null} {info.cvsAccessory.tglAcsKind.isOn}");
                    _self.Logger.LogWarning($"tglAcsKind is null? {info.cvsAccessory.tglAcsKind == null} {info.cvsAccessory.tglAcsKind.isOn}");
                    _self.Logger.LogWarning($"cgAccessoryWin is null? {info.cvsAccessory.cgAccessoryWin == null} {info.cvsAccessory.cgAccessoryWin.Any(x => x == null)} {info.cvsAccessory.cgAccessoryWin.Length}");

                    _self.Logger.LogWarning($"cgAcsParent is null? {info.cvsAccessory.cgAcsParent == null}");
                    _self.Logger.LogWarning($"cusAcsParentWin is null? {info.cvsAccessory.cusAcsParentWin == null}");
                    _self.Logger.LogWarning($"textAcsParent is null? {info.cvsAccessory.textAcsParent == null}");
                    _self.Logger.LogWarning($"customAccessory is null? {info.cvsAccessory.customAccessory == null} {info.cvsAccessory.customAccessory.Any(x => x == null)} {info.cvsAccessory.customAccessory.Length}");


                    var localPosition = info.canvasGroup.transform.localPosition;
                    localPosition.y = 40f * index;
                    info.canvasGroup.transform.localPosition = localPosition;

                    var wait = -1;
                    var etst = info.cvsAccessory.Start();
                    await UniTask.WaitUntil(() =>
                    {
                        wait++;
                        return etst.Status.IsCompleted();
                    });

                    CvsAccessoryArray = CvsAccessoryArray.Concat(info.cvsAccessory).ToArray();
                    _self.Logger.LogWarning($"t-1 {info.cvsAccessory.customAccessory == null}");


                    CustomAcsMoveWin = CustomAcsMoveWin.Concat(gameObject.AddComponent<CustomAcsMoveWindow>()).ToArray();

                    var uigroups = _customAcsChangeSlot.items = _customAcsChangeSlot.items.ConcatNearEnd(new UI_ToggleGroupCtrl.ItemInfo() { tglItem = info.toggle, cgItem = info.canvasGroup });

                    _self.Logger.LogWarning($"is toggle null? {uigroups.ElementAt(index).tglItem == null} is canvas null? {uigroups.ElementAt(index).cgItem}");

                    _self.Logger.LogWarning("t0");

                    foreach (var _custom in CustomAcsSelectKind)
                    {
                        _custom.cvsAccessory = CvsAccessoryArray;
                    }
                    foreach (var _custom in CustomAcsMoveWin)
                    {
                        _custom.cvsAccessory = CvsAccessoryArray;
                    }

                    CustomAcsParentWin.cvsAccessory = CvsAccessoryArray;
                    _self.Logger.LogWarning("t1");

                    //info.toggle.onValueChanged = new Toggle.ToggleEvent();
                    info.toggle.isOn = false;
                    info.canvasGroup.Enable(false, false);

                    RestoreToggle(info.toggle, index);




                    //_self.Logger.LogWarning($"waited {wait} frames(?) task completed successfully? {etst.Status.IsCompletedSuccessfully()}");

                    _self.Logger.LogWarning("t2");
                    //info.toggle.onValueChanged.AddListener(b =>
                    //{
                    //    AccessorySlotToggleCallback(index, info.toggle);
                    //    AccessorySlotCanvasGroupCallback(index, info.toggle, info.canvasGroup);
                    //});

                    info.text.text = $"スロット{index + 1:00}";
                    info.cvsAccessory.slotNo = (CvsAccessory.AcsSlotNo)index;
                    newSlot.name = "tglSlot" + (index + 1).ToString("00");
                    _self.Logger.LogWarning("t3");
#if KK || KKS
                    info.copySlotObject = Instantiate(_copySlotTemplate, _charaMakerCopyScrollView.content);
                    info.copyToggle = info.copySlotObject.GetComponentInChildren<Toggle>();
                    info.copySourceText = info.copySlotObject.transform.Find("srcText00").GetComponent<TextMeshProUGUI>();
                    info.copyDestinationText = info.copySlotObject.transform.Find("dstText00").GetComponent<TextMeshProUGUI>();
                    info.copyToggle.GetComponentInChildren<TextMeshProUGUI>().text = (index + 1).ToString("00");
                    info.copySourceText.text = "なし";
                    info.copyDestinationText.text = "なし";
                    info.copyToggle.onValueChanged = new Toggle.ToggleEvent();
                    info.copyToggle.isOn = false;
                    info.copyToggle.interactable = true;
                    info.copySlotObject.name = "kind" + index.ToString("00");
                    info.copyToggle.graphic.raycastTarget = true;
#endif
                    _self.Logger.LogWarning("t4");
                    info.transferSlotObject = Instantiate(_transferSlotTemplate, _charaMakerTransferScrollView.content);
                    info.transferSourceToggle = info.transferSlotObject.transform.GetChild(1).GetComponentInChildren<Toggle>();
                    info.transferDestinationToggle = info.transferSlotObject.transform.GetChild(2).GetComponentInChildren<Toggle>();
                    info.transferSourceText = info.transferSourceToggle.GetComponentInChildren<TextMeshProUGUI>();
                    info.transferDestinationText = info.transferDestinationToggle.GetComponentInChildren<TextMeshProUGUI>();
                    info.transferSlotObject.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = (index + 1).ToString("00");
                    info.transferSourceText.text = "なし";
                    info.transferDestinationText.text = "なし";
                    info.transferSlotObject.name = "kind" + index.ToString("00");
                    info.transferSourceToggle.onValueChanged = new Toggle.ToggleEvent();
                    info.transferSourceToggle.onValueChanged.AddListener((b) =>
                    {
                        if (info.transferSourceToggle.isOn)
                            CvsAccessory_Patches.CvsAccessoryChange_Start_Patches.SetSourceIndex(index);
                    });
                    info.transferDestinationToggle.onValueChanged = new Toggle.ToggleEvent();
                    info.transferDestinationToggle.onValueChanged.AddListener((b) =>
                    {
                        if (info.transferDestinationToggle.isOn)
                            CvsAccessory_Patches.CvsAccessoryChange_Start_Patches.SetDestinationIndex(index);
                    });
                    info.transferSourceToggle.isOn = false;
                    info.transferDestinationToggle.isOn = false;
                    info.transferSourceToggle.graphic.raycastTarget = true;
                    info.transferDestinationToggle.graphic.raycastTarget = true;
                    _self.Logger.LogWarning("t5");
                    _additionalCharaMakerSlots.Add(info);
                    _self.Logger.LogWarning("t6");
                    _addButtonsGroup.SetAsLastSibling();

                    _self.Logger.LogWarning("t7");
                    _self.ExecuteDelayed(() =>
                    {
                        var test = info.cvsAccessory.accessory.parts.Length;
                        _self.Logger.LogWarning($"index {_customAcsChangeSlot.GetSelectIndex()}, part length {test}");
                        info.cvsAccessory.UpdateCustomUI();
                    }, 5);
                    _self.Logger.LogWarning("t8");

                    reactive[index].Subscribe(x =>
                    {
                        _self.Logger.LogWarning("selectSlot pass");
                        if (custombase.selectSlot == index)
                            custombase.actUpdateCvsAccessory[index]();
                    });
                    _self.Logger.LogWarning("t8");
                    //onCharaMakerSlotAdded?.Invoke(index, newSlot.transform);

                    this.ExecuteDelayed(() =>
                    {
                        //selectKind.Initialize();

                    }, 5);

                    _self.Logger.LogWarning($"ddAcsType is null? {info.cvsAccessory.ddAcsType == null}  {info.cvsAccessory.ddAcsType.value} {info.cvsAccessory.ddAcsType.value - 1}");
                    _self.Logger.LogWarning($"tglAcsKind is null? {info.cvsAccessory.tglAcsKind == null} {info.cvsAccessory.tglAcsKind.isOn}");
                    _self.Logger.LogWarning($"cgAccessoryWin is null? {info.cvsAccessory.cgAccessoryWin == null} {info.cvsAccessory.cgAccessoryWin.Any(x => x == null)} {info.cvsAccessory.cgAccessoryWin.Length}");

                    _self.Logger.LogWarning($"cgAcsParent is null? {info.cvsAccessory.cgAcsParent == null}");
                    _self.Logger.LogWarning($"cusAcsParentWin is null? {info.cvsAccessory.cusAcsParentWin == null}");
                    _self.Logger.LogWarning($"textAcsParent is null? {info.cvsAccessory.textAcsParent == null}");
                    _self.Logger.LogWarning($"customAccessory is null? {info.cvsAccessory.customAccessory == null} {info.cvsAccessory.customAccessory.Any(x => x == null)} {info.cvsAccessory.customAccessory.Length}");

                }
                info.cvsAccessory.UpdateSlotName();
            }
            _self.Logger.LogWarning("t7");
            for (; i < _additionalCharaMakerSlots.Count; i++)
            {
                var slot = _additionalCharaMakerSlots[i];
                slot.toggle.gameObject.SetActive(false);
                slot.toggle.isOn = false;
                slot.transferSlotObject.SetActive(false);
            }
            _addButtonsGroup.SetAsLastSibling();
            _self.Logger.LogWarning("finished maker");
        }

        private void RestoreToggle(Toggle toggle, int index)
        {
            toggle.onValueChanged.AddListener(x =>
            {
                if (!x) return;
                _self.Logger.LogWarning(index);

                var open = false;
                if (120 != CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts[index].type)
                {
                    open = true;
                }
                if (CustomBase.instance.chaCtrl.hideHairAcs[index])
                {
                    open = false;
                }
                CustomAcsParentWin.ChangeSlot(index, open);
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

        internal void SpawnMakerUI()
        {
            _self.Logger.LogWarning("SpawnMakerUI");
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
            _charaMakerScrollView = UIUtility.CreateScrollView("Slots", container);
            _charaMakerScrollView.onValueChanged.AddListener(x =>
            {
                FixWindowScroll();
                this.ExecuteDelayed(FixWindowScroll);
            });
            _charaMakerScrollView.movementType = ScrollRect.MovementType.Clamped;
            _charaMakerScrollView.horizontal = false;
            _charaMakerScrollView.scrollSensitivity = 18f;
            if (_charaMakerScrollView.horizontalScrollbar != null)
                Destroy(_charaMakerScrollView.horizontalScrollbar.gameObject);
            if (_charaMakerScrollView.verticalScrollbar != null)
                Destroy(_charaMakerScrollView.verticalScrollbar.gameObject);
            Destroy(_charaMakerScrollView.GetComponent<Image>());
            _charaMakerSlotTemplate = container.GetChild(0).gameObject;

            var rootCanvas = ((RectTransform)_charaMakerSlotTemplate.GetComponentInParent<Canvas>().transform);
            var element = _charaMakerScrollView.gameObject.AddComponent<LayoutElement>();
            element.minHeight = rootCanvas.rect.height / 1.298076f;
            element.minWidth = 622f; //Because trying to get the value dynamically fails for some reason so fuck it.
            var group = _charaMakerScrollView.content.gameObject.AddComponent<VerticalLayoutGroup>();
            var parentGroup = container.GetComponent<VerticalLayoutGroup>();
            group.childAlignment = parentGroup.childAlignment;
            group.childControlHeight = parentGroup.childControlHeight;
            group.childControlWidth = parentGroup.childControlWidth;
            group.childForceExpandHeight = parentGroup.childForceExpandHeight;
            group.childForceExpandWidth = parentGroup.childForceExpandWidth;
            group.spacing = parentGroup.spacing;
            _charaMakerScrollView.content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _customAcsChangeSlot.ExecuteDelayed(() =>
            {
                _slotUIPositionY = _charaMakerSlotTemplate.transform.parent.position.y;
            }, 15);

            var kkus = Type.GetType("HSUS.HSUS,KKUS");
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
                container.GetChild(0).SetParent(_charaMakerScrollView.content);
            }

            _charaMakerScrollView.transform.SetAsFirstSibling();
            var toggleChange = GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/04_AccessoryTop/tglChange").GetComponent<Toggle>();
            _addButtonsGroup = UIUtility.CreateNewUIObject("Add Buttons Group", _charaMakerScrollView.content);
            element = _addButtonsGroup.gameObject.AddComponent<LayoutElement>();
            element.preferredWidth = 224f;
            element.preferredHeight = 32f;
            var textModel = toggleChange.transform.Find("imgOff").GetComponentInChildren<TextMeshProUGUI>().gameObject;

            var addOneButton = UIUtility.CreateButton("Add One Button", _addButtonsGroup, "+1");
            addOneButton.transform.SetRect(Vector2.zero, new Vector2(0.5f, 1f));
            addOneButton.colors = toggleChange.colors;
            ((Image)addOneButton.targetGraphic).sprite = toggleChange.transform.Find("imgOff").GetComponent<Image>().sprite;
            Destroy(addOneButton.GetComponentInChildren<Text>().gameObject);
            var text = Instantiate(textModel).GetComponent<TextMeshProUGUI>();
            text.transform.SetParent(addOneButton.transform);
            text.rectTransform.SetRect(Vector2.zero, Vector2.one, new Vector2(5f, 4f), new Vector2(-5f, -4f));
            text.text = "+1";
            addOneButton.onClick.AddListener(delegate () { AddSlot(1); });

            var addTenButton = UIUtility.CreateButton("Add Ten Button", _addButtonsGroup, "+10");
            addTenButton.transform.SetRect(new Vector2(0.5f, 0f), Vector2.one);
            addTenButton.colors = toggleChange.colors;
            ((Image)addTenButton.targetGraphic).sprite = toggleChange.transform.Find("imgOff").GetComponent<Image>().sprite;
            Destroy(addTenButton.GetComponentInChildren<Text>().gameObject);
            text = Instantiate(textModel).GetComponent<TextMeshProUGUI>();
            text.transform.SetParent(addTenButton.transform);
            text.rectTransform.SetRect(Vector2.zero, Vector2.one, new Vector2(5f, 4f), new Vector2(-5f, -4f));
            text.text = "+10";
            addTenButton.onClick.AddListener(delegate () { AddSlot(10); });
            LayoutRebuilder.ForceRebuildLayoutImmediate(container);

            for (var i = 0; i < _customAcsChangeSlot.items.Length; i++)
            {
                FixWindowScroll();
            }

            //            for (var i = 0; i < _customAcsChangeSlot.items.Length; i++)
            //            {
            //                var info = _customAcsChangeSlot.items[i];
            //                info.tglItem.onValueChanged = new Toggle.ToggleEvent();
            //                if (i < 20)
            //                {
            //                    var i1 = i;
            //                    info.tglItem.onValueChanged.AddListener(b =>
            //                    {
            //                        AccessorySlotToggleCallback(i1, info.tglItem);
            //                        AccessorySlotCanvasGroupCallback(info.tglItem, info.cgItem);
            //                    });
            //                }
            //                else if (i == 20)
            //                {
            //                    info.tglItem.onValueChanged.AddListener(b =>
            //                    {
            //                        if (info.tglItem.isOn)
            //                        {
            //                            _customAcsChangeSlot.CloseWindow();
            //#if KK || KKS
            //                            CustomBase.Instance.updateCvsAccessoryCopy = true;
            //#endif
            //                        }
            //                        AccessorySlotCanvasGroupCallback(info.tglItem, info.cgItem);
            //                    });
            //                    ((RectTransform)info.cgItem.transform).anchoredPosition += new Vector2(0f, 40f);
            //                }
            //                else if (i == 21)
            //                {
            //                    info.tglItem.onValueChanged.AddListener(b =>
            //                    {
            //                        if (info.tglItem.isOn)
            //                        {
            //                            _customAcsChangeSlot.CloseWindow();
            //                            Singleton<CustomBase>.Instance.updateCvsAccessoryChange = true;
            //                        }
            //                        AccessorySlotCanvasGroupCallback(info.tglItem, info.cgItem);
            //                    });
            //                    ((RectTransform)info.cgItem.transform).anchoredPosition += new Vector2(0f, 40f);
            //                }
            //            }

            _customAcsChangeSlot.ExecuteDelayed(() =>
            {
                CvsAccessoryArray[0].UpdateCustomUI();
                CvsAccessoryArray[0].tglTakeOverParent.isOn = false;
                CvsAccessoryArray[0].tglTakeOverColor.isOn = false;
            }, 5);

            RectTransform content;
#if KK || KKS
            container = (RectTransform)GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/04_AccessoryTop/tglCopy/CopyTop/rect").transform;
            _charaMakerCopyScrollView = UIUtility.CreateScrollView("Slots", container);
            _charaMakerCopyScrollView.movementType = ScrollRect.MovementType.Clamped;
            _charaMakerCopyScrollView.horizontal = false;
            _charaMakerCopyScrollView.scrollSensitivity = 18f;
            if (_charaMakerCopyScrollView.horizontalScrollbar != null)
                Destroy(_charaMakerCopyScrollView.horizontalScrollbar.gameObject);
            if (_charaMakerCopyScrollView.verticalScrollbar != null)
                Destroy(_charaMakerCopyScrollView.verticalScrollbar.gameObject);
            Destroy(_charaMakerCopyScrollView.GetComponent<Image>());

            content = (RectTransform)container.Find("grpClothes");
            _charaMakerCopyScrollView.transform.SetRect(content);
            content.SetParent(_charaMakerCopyScrollView.viewport);
            Destroy(_charaMakerCopyScrollView.content.gameObject);
            _charaMakerCopyScrollView.content = content;
            _copySlotTemplate = _charaMakerCopyScrollView.content.GetChild(0).gameObject;
            _raycastCtrls.Add(container.parent.GetComponent<UI_RaycastCtrl>());
            _charaMakerCopyScrollView.transform.SetAsFirstSibling();
            _charaMakerCopyScrollView.transform.SetRect(new Vector2(0f, 1f), Vector2.one, new Vector2(16f, -570f), new Vector2(-16f, -80f));
#endif

            container = (RectTransform)GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/04_AccessoryTop/tglChange/ChangeTop/rect").transform;
            _charaMakerTransferScrollView = UIUtility.CreateScrollView("Slots", container);
            _charaMakerTransferScrollView.movementType = ScrollRect.MovementType.Clamped;
            _charaMakerTransferScrollView.horizontal = false;
            _charaMakerTransferScrollView.scrollSensitivity = 18f;
            if (_charaMakerTransferScrollView.horizontalScrollbar != null)
                Destroy(_charaMakerTransferScrollView.horizontalScrollbar.gameObject);
            if (_charaMakerTransferScrollView.verticalScrollbar != null)
                Destroy(_charaMakerTransferScrollView.verticalScrollbar.gameObject);
            Destroy(_charaMakerTransferScrollView.GetComponent<Image>());
            content = (RectTransform)container.Find("grpClothes");
            _charaMakerTransferScrollView.transform.SetRect(content);
            content.SetParent(_charaMakerTransferScrollView.viewport);
            Destroy(_charaMakerTransferScrollView.content.gameObject);
            _charaMakerTransferScrollView.content = content;
            _transferSlotTemplate = _charaMakerTransferScrollView.content.GetChild(0).gameObject;
            _raycastCtrls.Add(container.parent.GetComponent<UI_RaycastCtrl>());
            _charaMakerTransferScrollView.transform.SetAsFirstSibling();
            _charaMakerTransferScrollView.transform.SetRect(new Vector2(0f, 1f), Vector2.one, new Vector2(16f, -530f), new Vector2(-16f, -48f));

            _charaMakerScrollView.viewport.gameObject.SetActive(false);

            _customAcsChangeSlot.ExecuteDelayed(() => //Fixes problems with UI masks overlapping and creating bugs
            {
                _charaMakerScrollView.viewport.gameObject.SetActive(true);
            }, 5);
            _customAcsChangeSlot.ExecuteDelayed(() =>
            {
                UpdateMakerUI();
                CustomBase.Instance.updateCustomUI = true;
            }, 2);
        }

        private void FixWindowScroll()
        {
            var t = _customAcsChangeSlot.items[CustomBase.Instance.selectSlot].cgItem.transform;
            t.position = new Vector3(t.position.x, _slotUIPositionY);
        }

        private void ArrayExpansion<T>(ref T[] array, int count = 1)
        {
            if (count < 1) return;
            array = array.Concat(new T[count]).ToArray();
        }

        internal void MakeWindowScrollable(Transform slotTransform, Image content_image, Sprite scroll_bar_area_sprite, Sprite scroll_bar_handle_sprite)
        {
#if KK || KKS
            var listParent = slotTransform.Cast<Transform>().Where(x => x.name.EndsWith("Top")).First();
#if KKS
            GameObject.DestroyImmediate(listParent.GetComponent<Image>());//Destroy image that contains scrollbar
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
            UnityEngine.Object.DestroyImmediate(scroll.horizontalScrollbar.gameObject);
            var content = scroll.content.transform;
            UnityEngine.Object.Destroy(scroll.GetComponent<Image>());

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

        private void ArraySync(ChaControl controller)
        {
            var len = controller.nowCoordinate.accessory.parts.Length;

            var show = controller.fileStatus.showAccessory;
            var obj = controller.objAccessory;
            var objmove = controller.objAcsMove;
            var cusAcsCmp = controller.cusAcsCmp;
            var hideHairAcs = controller.hideHairAcs;

            Logger.LogWarning($"syns show count {show.Length }");
            var delta = len - show.Length;
            controller.fileStatus.showAccessory = show.ArrayExpansion(delta);
            for (var i = 0; i < delta; i++)
            {
                controller.fileStatus.showAccessory[show.Length - 1 - i] = true;
            }
            Logger.LogWarning($"finish show count {show.Length} {controller.fileStatus.showAccessory.Length}");

            controller.objAccessory = obj.ArrayExpansion(len - obj.Length);
            controller.cusAcsCmp = cusAcsCmp.ArrayExpansion(len - cusAcsCmp.Length);
            controller.hideHairAcs = hideHairAcs.ArrayExpansion(len - hideHairAcs.Length);

            var movelen = objmove.GetLength(0);
            var count = len - movelen;
            if (count > 0)
            {
                var newarray = new GameObject[len, 2];
                for (var i = 0; i < movelen; i++)
                {
                    for (var j = 0; j < 2; j++)
                    {
                        newarray[i, j] = objmove[i, j];
                    }
                }
                controller.objAcsMove = newarray;
            }

        }

        private void AddSlot(int num)
        {
            var controller = CustomBase.instance.chaCtrl;
            var nowparts = controller.nowCoordinate.accessory.parts;
            var coordacc = controller.chaFile.coordinate[controller.chaFile.status.coordinateType].accessory;
            var coordparts = coordacc.parts;
            var newpart = new ChaFileAccessory.PartsInfo[num];
            for (var i = 0; i < num; i++)
            {
                newpart[i] = new ChaFileAccessory.PartsInfo();
            }
            coordacc.parts = controller.nowCoordinate.accessory.parts = nowparts.Concat(newpart).ToArray();
            ArraySync(controller);

            UpdateMakerUI();
        }

        internal int GetSelectedMakerIndex()
        {
            for (var i = 0; _customAcsChangeSlot != null && i < _customAcsChangeSlot.items.Length; i++)
            {
                var info = _customAcsChangeSlot.items[i];
                if (info.tglItem.isOn)
                    return i;
            }
            for (var i = 0; i < _additionalCharaMakerSlots.Count; i++)
            {
                var slot = _additionalCharaMakerSlots[i];
                if (slot.toggle.isOn)
                    return i + 20;
            }
            return -1;
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

        internal ChaFileAccessory.PartsInfo GetPart(int index)
        {
            return CustomBase.Instance.chaCtrl.nowCoordinate.accessory.parts[index];
        }

        internal CvsAccessory GetCvsAccessory(int index)
        {
            return CvsAccessoryArray[index];
        }
    }
}
