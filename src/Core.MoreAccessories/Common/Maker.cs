using System;
using System.Collections.Generic;
using System.Reflection;
using ChaCustom;
using HarmonyLib;
#if EMOTIONCREATORS
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
#endif
using Illusion.Extensions;
#if KOIKATSU
#endif
using TMPro;
using MoreAccessoriesKOI.Extensions;
using UnityEngine;
using UnityEngine.UI;

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

        private void UpdateMakerUI()
        {
            if (_customAcsChangeSlot == null || _charaMakerData == null)
                return;

            var count = _charaMakerData != null ? (_charaMakerData.nowAccessories != null ? _charaMakerData.nowAccessories.Count : 0) : 0;
            int i;
            for (i = 0; i < count; i++)
            {
                CharaMakerSlotData info;
                if (i < _additionalCharaMakerSlots.Count)
                {
                    info = _additionalCharaMakerSlots[i];
                    info.toggle.gameObject.SetActive(true);
                    if (i + 20 == CustomBase.Instance.selectSlot)
                        info.cvsAccessory.UpdateCustomUI();

                    info.transferSlotObject.SetActive(true);
                }
                else
                {
                    var newSlot = Instantiate(_charaMakerSlotTemplate, _charaMakerScrollView.content);
                    info = new CharaMakerSlotData();
                    info.toggle = newSlot.GetComponent<Toggle>();
                    info.text = info.toggle.GetComponentInChildren<TextMeshProUGUI>();
                    info.canvasGroup = info.toggle.transform.GetChild(1).GetComponent<CanvasGroup>();
                    info.cvsAccessory = info.toggle.GetComponentInChildren<CvsAccessory>();
                    info.toggle.onValueChanged = new Toggle.ToggleEvent();
                    info.toggle.isOn = false;
                    var index = i + 20;
                    info.toggle.onValueChanged.AddListener(b =>
                    {
                        AccessorySlotToggleCallback(index, info.toggle);
                        AccessorySlotCanvasGroupCallback(index, info.toggle, info.canvasGroup);
                    });
                    info.text.text = $"スロット{index + 1:00}";
                    info.cvsAccessory.slotNo = (CvsAccessory.AcsSlotNo)index;
                    newSlot.name = "tglSlot" + (index + 1).ToString("00");
                    info.canvasGroup.Enable(false, false);

#if KOIKATSU
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

                    _additionalCharaMakerSlots.Add(info);
                    info.cvsAccessory.UpdateCustomUI();

                    if (onCharaMakerSlotAdded != null)
                        onCharaMakerSlotAdded(index, newSlot.transform);
                }
                info.cvsAccessory.UpdateSlotName();

            }
            for (; i < _additionalCharaMakerSlots.Count; i++)
            {
                var slot = _additionalCharaMakerSlots[i];
                slot.toggle.gameObject.SetActive(false);
                slot.toggle.isOn = false;
                slot.transferSlotObject.SetActive(false);
            }
            _addButtonsGroup.SetAsLastSibling();
        }

        internal void SpawnMakerUI()
        {
            var container = (RectTransform)GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/04_AccessoryTop").transform;
            _charaMakerScrollView = UIUtility.CreateScrollView("Slots", container);
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

            this.ExecuteDelayed(() =>
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
                container.GetChild(0).SetParent(_charaMakerScrollView.content);

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
            addOneButton.onClick.AddListener(AddSlot);

            var addTenButton = UIUtility.CreateButton("Add Ten Button", _addButtonsGroup, "+10");
            addTenButton.transform.SetRect(new Vector2(0.5f, 0f), Vector2.one);
            addTenButton.colors = toggleChange.colors;
            ((Image)addTenButton.targetGraphic).sprite = toggleChange.transform.Find("imgOff").GetComponent<Image>().sprite;
            Destroy(addTenButton.GetComponentInChildren<Text>().gameObject);
            text = Instantiate(textModel).GetComponent<TextMeshProUGUI>();
            text.transform.SetParent(addTenButton.transform);
            text.rectTransform.SetRect(Vector2.zero, Vector2.one, new Vector2(5f, 4f), new Vector2(-5f, -4f));
            text.text = "+10";
            addTenButton.onClick.AddListener(AddTenSlot);
            LayoutRebuilder.ForceRebuildLayoutImmediate(container);

            for (var i = 0; i < _customAcsChangeSlot.items.Length; i++)
            {
                var info = _customAcsChangeSlot.items[i];
                info.tglItem.onValueChanged = new Toggle.ToggleEvent();
                if (i < 20)
                {
                    var i1 = i;
                    info.tglItem.onValueChanged.AddListener(b =>
                    {
                        AccessorySlotToggleCallback(i1, info.tglItem);
                        AccessorySlotCanvasGroupCallback(i1, info.tglItem, info.cgItem);
                    });
                }
                else if (i == 20)
                {
                    info.tglItem.onValueChanged.AddListener(b =>
                    {
                        if (info.tglItem.isOn)
                        {
                            _customAcsChangeSlot.CloseWindow();
#if KOIKATSU
                            CustomBase.Instance.updateCvsAccessoryCopy = true;
#endif
                        }
                        AccessorySlotCanvasGroupCallback(-1, info.tglItem, info.cgItem);
                    });
                    ((RectTransform)info.cgItem.transform).anchoredPosition += new Vector2(0f, 40f);
                }
                else if (i == 21)
                {
                    info.tglItem.onValueChanged.AddListener(b =>
                    {
                        if (info.tglItem.isOn)
                        {
                            _customAcsChangeSlot.CloseWindow();
                            Singleton<CustomBase>.Instance.updateCvsAccessoryChange = true;
                        }
                        AccessorySlotCanvasGroupCallback(-2, info.tglItem, info.cgItem);
                    });
                    ((RectTransform)info.cgItem.transform).anchoredPosition += new Vector2(0f, 40f);
                }
            }

            this.ExecuteDelayed(() =>
            {
                _cvsAccessory[0].UpdateCustomUI();
                _cvsAccessory[0].tglTakeOverParent.isOn = false;
                _cvsAccessory[0].tglTakeOverColor.isOn = false;
            }, 5);

            RectTransform content;
#if KOIKATSU
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

            this.ExecuteDelayed(() => //Fixes problems with UI masks overlapping and creating bugs
            {
                _charaMakerScrollView.viewport.gameObject.SetActive(true);
            }, 5);
            this.ExecuteDelayed(() =>
            {
                UpdateMakerUI();
                CustomBase.Instance.updateCustomUI = true;
            }, 2);
        }

        private void AddSlot()
        {
            if (_self._accessoriesByChar.TryGetValue(CustomBase.Instance.chaCtrl.chaFile, out _charaMakerData) == false)
            {
                _charaMakerData = new CharAdditionalData();
                _accessoriesByChar.Add(CustomBase.Instance.chaCtrl.chaFile, _charaMakerData);
            }
            if (_charaMakerData.nowAccessories == null)
            {
                _charaMakerData.nowAccessories = new List<ChaFileAccessory.PartsInfo>();
                _charaMakerData.rawAccessoriesInfos.Add(CustomBase.Instance.chaCtrl.fileStatus.GetCoordinateType(), _charaMakerData.nowAccessories);
            }
            var partsInfo = new ChaFileAccessory.PartsInfo();
            _charaMakerData.nowAccessories.Add(partsInfo);
            while (_charaMakerData.infoAccessory.Count < _charaMakerData.nowAccessories.Count)
                _charaMakerData.infoAccessory.Add(null);
            while (_charaMakerData.objAccessory.Count < _charaMakerData.nowAccessories.Count)
                _charaMakerData.objAccessory.Add(null);
            while (_charaMakerData.objAcsMove.Count < _charaMakerData.nowAccessories.Count)
                _charaMakerData.objAcsMove.Add(new GameObject[2]);
            while (_charaMakerData.cusAcsCmp.Count < _charaMakerData.nowAccessories.Count)
                _charaMakerData.cusAcsCmp.Add(null);
            while (_charaMakerData.showAccessories.Count < _charaMakerData.nowAccessories.Count)
                _charaMakerData.showAccessories.Add(true);
            UpdateMakerUI();
        }

        private void AddTenSlot()
        {
            if (_self._accessoriesByChar.TryGetValue(CustomBase.Instance.chaCtrl.chaFile, out _charaMakerData) == false)
            {
                _charaMakerData = new CharAdditionalData();
                _accessoriesByChar.Add(CustomBase.Instance.chaCtrl.chaFile, _charaMakerData);
            }
            if (_charaMakerData.nowAccessories == null)
            {
                _charaMakerData.nowAccessories = new List<ChaFileAccessory.PartsInfo>();
                _charaMakerData.rawAccessoriesInfos.Add(CustomBase.Instance.chaCtrl.fileStatus.GetCoordinateType(), _charaMakerData.nowAccessories);
            }
            for (var i = 0; i < 10; i++)
            {
                var partsInfo = new ChaFileAccessory.PartsInfo();
                _charaMakerData.nowAccessories.Add(partsInfo);
            }
            while (_charaMakerData.infoAccessory.Count < _charaMakerData.nowAccessories.Count)
                _charaMakerData.infoAccessory.Add(null);
            while (_charaMakerData.objAccessory.Count < _charaMakerData.nowAccessories.Count)
                _charaMakerData.objAccessory.Add(null);
            while (_charaMakerData.objAcsMove.Count < _charaMakerData.nowAccessories.Count)
                _charaMakerData.objAcsMove.Add(new GameObject[2]);
            while (_charaMakerData.cusAcsCmp.Count < _charaMakerData.nowAccessories.Count)
                _charaMakerData.cusAcsCmp.Add(null);
            while (_charaMakerData.showAccessories.Count < _charaMakerData.nowAccessories.Count)
                _charaMakerData.showAccessories.Add(true);
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

        private void AccessorySlotToggleCallback(int index, Toggle toggle)
        {
            if (toggle.isOn)
            {
                CustomBase.Instance.selectSlot = index;
                var open = GetPart(index).type != 120;
                _customAcsParentWin.ChangeSlot(index, open);
                foreach (var customAcsMoveWindow in _customAcsMoveWin)
                    customAcsMoveWindow.ChangeSlot(index, open);
                foreach (var customAcsSelectKind in _customAcsSelectKind)
                    customAcsSelectKind.ChangeSlot(index, open);

                Singleton<CustomBase>.Instance.selectSlot = index;
                if (index < 20)
                    Singleton<CustomBase>.Instance.SetUpdateCvsAccessory(index, true);
                else
                {
                    var accessory = GetCvsAccessory(index);
                    if (index == CustomBase.Instance.selectSlot)
                        accessory.UpdateCustomUI();
                    accessory.UpdateSlotName();
                }
                if (_customAcsChangeSlot.backIndex != index)
                    _customAcsChangeSlot.ChangeColorWindow(index);
                _customAcsChangeSlot.SetPrivate("backIndex", index);
            }
        }

        private void AccessorySlotCanvasGroupCallback(int index, Toggle toggle, CanvasGroup canvasGroup)
        {
            for (var i = 0; i < _customAcsChangeSlot.items.Length; i++)
            {
                var info = _customAcsChangeSlot.items[i];
                if (info.cgItem != null)
                    info.cgItem.Enable(false, false);
            }
            for (var i = 0; i < _additionalCharaMakerSlots.Count; i++)
            {
                var info = _additionalCharaMakerSlots[i];
                if (info.canvasGroup != null)
                    info.canvasGroup.Enable(false, false);
            }
            if (toggle.isOn && canvasGroup)
                canvasGroup.Enable(true, false);
        }

        internal ChaFileAccessory.PartsInfo GetPart(int index)
        {
            if (index < 20)
                return CustomBase.Instance.chaCtrl.nowCoordinate.accessory.parts[index];
            return _charaMakerData.nowAccessories[index - 20];
        }

        internal void SetPart(int index, ChaFileAccessory.PartsInfo value)
        {
            if (index < 20)
                CustomBase.Instance.chaCtrl.nowCoordinate.accessory.parts[index] = value;
            else
                _charaMakerData.nowAccessories[index - 20] = value;
        }

        internal int GetPartsLength()
        {
            return _charaMakerData.nowAccessories.Count + 20;
        }

        internal CvsAccessory GetCvsAccessory(int index)
        {
            if (index < 20)
                return _cvsAccessory[index];
            return _additionalCharaMakerSlots[index - 20].cvsAccessory;
        }
    }
}
