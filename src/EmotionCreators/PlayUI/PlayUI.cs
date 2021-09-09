using ADVPart.Manipulate.Chara;
using HPlay;
using MoreAccessoriesKOI.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAccessoriesKOI
{
    public class PlayMode
    {
        internal PlayMode(HPlayHPartAccessoryCategoryUI hPlayH)
        {
            _playUI = hPlayH;
            SpawnPlayUI();
        }

        private readonly List<PlaySceneSlotData> _additionalPlaySceneSlots = new List<PlaySceneSlotData>();
        private RectTransform _playButtonTemplate;
        private HPlayHPartAccessoryCategoryUI _playUI;
        private Coroutine _updatePlayUIHandler;
        private MoreAccessories Plugin => MoreAccessories._self;

        //CharaUICtrl
        internal void SpawnPlayUI()
        {
            var buttons = _playUI.accessoryCategoryUIs;
            _playButtonTemplate = (RectTransform)buttons[0].btn.transform;
            _playButtonTemplate.GetComponentInChildren<TextMeshProUGUI>().fontMaterial = new Material(_playButtonTemplate.GetComponentInChildren<TextMeshProUGUI>().fontMaterial);
            var index = _playButtonTemplate.parent.GetSiblingIndex();

            var scrollView = UIUtility.CreateScrollView("ScrollView", _playButtonTemplate.parent.parent);
            scrollView.transform.SetSiblingIndex(index);
            scrollView.transform.SetRect(_playButtonTemplate.parent);
            ((RectTransform)scrollView.transform).offsetMax = new Vector2(_playButtonTemplate.offsetMin.x + 192f, -88f);
            ((RectTransform)scrollView.transform).offsetMin = new Vector2(_playButtonTemplate.offsetMin.x, -640f - 88f);
            scrollView.viewport.GetComponent<Image>().sprite = null;
            scrollView.movementType = ScrollRect.MovementType.Clamped;
            scrollView.horizontal = false;
            scrollView.scrollSensitivity = 18f;
            if (scrollView.horizontalScrollbar != null)
                Object.Destroy(scrollView.horizontalScrollbar.gameObject);
            if (scrollView.verticalScrollbar != null)
                Object.Destroy(scrollView.verticalScrollbar.gameObject);
            Object.Destroy(scrollView.GetComponent<Image>());
            Object.Destroy(scrollView.content.gameObject);
            scrollView.content = (RectTransform)_playButtonTemplate.parent;
            _playButtonTemplate.parent.SetParent(scrollView.viewport, false);
            _playButtonTemplate.parent.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            ((RectTransform)_playButtonTemplate.parent).anchoredPosition = Vector2.zero;
            _playButtonTemplate.parent.GetComponent<VerticalLayoutGroup>().padding = new RectOffset(0, 0, 0, 0);
            foreach (var b in buttons)
                ((RectTransform)b.btn.transform).anchoredPosition = Vector2.zero;
        }

        internal void UpdatePlayUI()
        {
            if (_playUI == null || _playButtonTemplate == null || _playUI.selectChara == null)
                return;
            //if (_updatePlayUIHandler == null)
            //    _updatePlayUIHandler = Plugin.StartCoroutine(UpdatePlayUI_Routine());
            var parts = _playUI.selectChara.nowCoordinate.accessory.parts;
            var count = parts.Length - 20;
            var j = 0;
            for (; j < count; j++)
            {
                PlaySceneSlotData slot;
                if (j < _additionalPlaySceneSlots.Count)
                    slot = _additionalPlaySceneSlots[j];
                else
                {
                    slot = new PlaySceneSlotData();
                    slot.slot = (RectTransform)Object.Instantiate(_playButtonTemplate.gameObject).transform;
                    slot.text = slot.slot.GetComponentInChildren<TextMeshProUGUI>(true);
                    slot.text.fontMaterial = new Material(slot.text.fontMaterial);
                    slot.button = slot.slot.GetComponentInChildren<Button>(true);
                    slot.slot.SetParent(_playButtonTemplate.parent);
                    slot.slot.localPosition = Vector3.zero;
                    slot.slot.localScale = Vector3.one;
                    var i1 = j;
                    slot.button.onClick = new Button.ButtonClickedEvent();
                    slot.button.onClick.AddListener(() =>
                    {
                        _playUI.selectChara.chaFile.status.showAccessory[i1] = !_playUI.selectChara.chaFile.status.showAccessory[i1];
                    });
                    _additionalPlaySceneSlots.Add(slot);
                }
                var objAccessory = _playUI.selectChara.objAccessory[j + 20];
                if (objAccessory == null)
                    slot.slot.gameObject.SetActive(false);
                else
                {
                    slot.slot.gameObject.SetActive(true);
                    var component = objAccessory.GetComponent<ListInfoComponent>();
                    slot.text.text = component.data.Name;
                }
            }

            for (; j < _additionalPlaySceneSlots.Count; ++j)
                _additionalPlaySceneSlots[j].slot.gameObject.SetActive(false);
        }

        // So, this thing is actually a coroutine because if I don't do the following, TextMeshPro start disappearing because their material is destroyed.
        // IDK, fuck you unity I guess
        private IEnumerator UpdatePlayUI_Routine()
        {
            while (_playButtonTemplate.gameObject.activeInHierarchy == false)
                yield return null;
            //var character = _playUI.selectChara;

            //CharAdditionalData additionalData = _accessoriesByChar[character.chaFile];
            //int j;
            //for (j = 0; j < additionalData.nowAccessories.Count; j++)
            //{
            //    PlaySceneSlotData slot;
            //    if (j < _additionalPlaySceneSlots.Count)
            //        slot = _additionalPlaySceneSlots[j];
            //    else
            //    {
            //        slot = new PlaySceneSlotData();
            //        slot.slot = (RectTransform)Instantiate(_playButtonTemplate.gameObject).transform;
            //        slot.text = slot.slot.GetComponentInChildren<TextMeshProUGUI>(true);
            //        slot.text.fontMaterial = new Material(slot.text.fontMaterial);
            //        slot.button = slot.slot.GetComponentInChildren<Button>(true);
            //        slot.slot.SetParent(_playButtonTemplate.parent);
            //        slot.slot.localPosition = Vector3.zero;
            //        slot.slot.localScale = Vector3.one;
            //        var i1 = j;
            //        slot.button.onClick = new Button.ButtonClickedEvent();
            //        slot.button.onClick.AddListener(() =>
            //        {
            //            additionalData.showAccessories[i1] = !additionalData.showAccessories[i1];
            //        });
            //        _additionalPlaySceneSlots.Add(slot);
            //    }
            //    GameObject objAccessory = additionalData.objAccessory[j];
            //    if (objAccessory == null)
            //        slot.slot.gameObject.SetActive(false);
            //    else
            //    {
            //        slot.slot.gameObject.SetActive(true);
            //        var component = objAccessory.GetComponent<ListInfoComponent>();
            //        slot.text.text = component.data.Name;
            //    }
            //}

            //for (; j < _additionalPlaySceneSlots.Count; ++j)
            //    _additionalPlaySceneSlots[j].slot.gameObject.SetActive(false);
            //_updatePlayUIHandler = null;
        }

    }
}
