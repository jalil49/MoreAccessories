using HPlay;
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
    public class PlayMode
    {
        internal PlayMode(HPlayHPartAccessoryCategoryUI hPlayH)
        {
            _playUI = hPlayH;
            SpawnPlayUI();
        }

        private readonly List<PlaySceneSlotData> _additionalPlaySceneSlots = new List<PlaySceneSlotData>();
        private RectTransform _playButtonTemplate;
        public readonly HPlayHPartAccessoryCategoryUI _playUI;
        private ScrollRect scrollView;
        //CharaUICtrl
        private void SpawnPlayUI()
        {
            var buttons = _playUI.accessoryCategoryUIs;
            _playButtonTemplate = (RectTransform)buttons[0].btn.transform;
            _playButtonTemplate.GetComponentInChildren<TextMeshProUGUI>().fontMaterial = new Material(_playButtonTemplate.GetComponentInChildren<TextMeshProUGUI>().fontMaterial);
            var index = _playButtonTemplate.parent.GetSiblingIndex();

            scrollView = UIUtility.CreateScrollView("ScrollView", _playButtonTemplate.parent.parent);
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

            var parts = _playUI.selectChara.nowCoordinate.accessory.parts;
            var count = parts.Length - 20;
            var j = 0;
            for (; j < count; j++)
            {
                if (j < _additionalPlaySceneSlots.Count)
                    continue;

                var slot = new PlaySceneSlotData
                {
                    slot = (RectTransform)Object.Instantiate(_playButtonTemplate.gameObject).transform
                };
                slot.text = slot.slot.GetComponentInChildren<TextMeshProUGUI>(true);
                slot.text.fontMaterial = new Material(slot.text.fontMaterial);
                slot.button = slot.slot.GetComponentInChildren<Button>(true);
                slot.slot.SetParent(_playButtonTemplate.parent);
                slot.slot.localPosition = Vector3.zero;
                slot.slot.localScale = Vector3.one;
                var i1 = j + 20;
                slot.button.onClick = new Button.ButtonClickedEvent();
                slot.button.onClick.AddListener(() =>
                {
                    _playUI.selectChara.chaFile.status.showAccessory[i1] = !_playUI.selectChara.chaFile.status.showAccessory[i1];
                });

                //modify original array
                _playUI.accessoryCategoryUIs = _playUI.accessoryCategoryUIs.Concat(new HPlayHPartUI.SelectUITextMesh
                {
                    btn = slot.button,
                    text = slot.text
                }).ToArray();

                _additionalPlaySceneSlots.Add(slot);
            }
        }

        //control when scrollview is active so scrollview does not block other menus
        internal void SetScrollViewActive(bool active)
        {
            scrollView.gameObject.SetActive(active);
        }
    }
}
