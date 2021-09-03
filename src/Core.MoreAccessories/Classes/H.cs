﻿using Cysharp.Threading.Tasks;
using MoreAccessoriesKOI.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAccessoriesKOI
{
    public class HScene
    {
        private List<List<HSceneSlotData>> _additionalHSceneSlots = new List<List<HSceneSlotData>>();
        private readonly List<ScrollRect> scrollRects = new List<ScrollRect>();
        internal List<ChaControl> _hSceneFemales;
        private List<HSprite.FemaleDressButtonCategory> _hSceneMultipleFemaleButtons;
        private HSprite _hSprite;
        private HSceneSpriteCategory _hSceneSoloFemaleAccessoryButton;

        public HScene(List<ChaControl> lstFemale, HSprite sprite)
        {
            _hSceneFemales = lstFemale;
            MoreAccessories.Print($"started H with {lstFemale.Count}");
            _hSprite = sprite;
            SpawnHUI();
        }

        private MoreAccessories Plugin => MoreAccessories._self;

        public List<ChaControl> LstFemale { get; }
        public HSprite Sprite { get; }

        internal void SpawnHUI()
        {
            for (var i = 0; i < _hSceneFemales.Count; i++)
                _additionalHSceneSlots.Add(new List<HSceneSlotData>());
            _hSceneMultipleFemaleButtons = _hSprite.lstMultipleFemaleDressButton;
            _hSceneSoloFemaleAccessoryButton = _hSprite.categoryAccessory;
            _ = MakeScrollable();
        }
#if KKS
        private async UniTask MakeScrollable()
        {
            scrollRects.Clear();
            await UniTask.WaitUntil(() => _hSceneFemales.Count > 0, PlayerLoopTiming.Update, default);
            await UniTask.DelayFrame(10);
            for (var i = 0; i < _hSceneFemales.Count; i++)
            {
                var buttonsParent = _hSceneFemales.Count == 1 ? _hSceneSoloFemaleAccessoryButton.transform : _hSceneMultipleFemaleButtons[i].accessory.transform;

                var accscroll = UIUtility.CreateScrollView($"Accessory Scroll {i}");

                accscroll.movementType = ScrollRect.MovementType.Clamped;
                accscroll.horizontal = false;
                accscroll.scrollSensitivity = 18f;
                var element = accscroll.gameObject.AddComponent<LayoutElement>();
                element.minHeight = Screen.height / 3;
                var group = accscroll.content.gameObject.AddComponent<VerticalLayoutGroup>();
                var parentGroup = buttonsParent.GetComponent<VerticalLayoutGroup>();
                group.childAlignment = parentGroup.childAlignment;
                group.childControlHeight = parentGroup.childControlHeight;
                group.childControlWidth = parentGroup.childControlWidth;
                group.childForceExpandHeight = parentGroup.childForceExpandHeight;
                group.childForceExpandWidth = parentGroup.childForceExpandWidth;
                group.spacing = parentGroup.spacing;
                accscroll.content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                for (int j = 0, n = buttonsParent.childCount; j < n; j++)
                {
                    buttonsParent.GetChild(0).SetParent(accscroll.content);
                }
                accscroll.transform.SetParent(buttonsParent, true);
                scrollRects.Add(accscroll);
            }
        }
#endif
        public void UpdateHUI()
        {
            if (_hSprite == null)
                return;
            //for (var i = 0; i < _hSceneFemales.Count; i++)
            //{
            //    var female = _hSceneFemales[i];

            //    var additionalData = _accessoriesByChar[female.chaFile];
            //    var additionalSlots = _additionalHSceneSlots[i];
            //    var buttonsParent = _hSceneFemales.Count == 1 ? _hSceneSoloFemaleAccessoryButton.transform : _hSceneMultipleFemaleButtons[i].accessory.transform;

            //    var j = 0;
            //    for (; j < additionalData.nowAccessories.Count; j++)
            //    {
            //        HSceneSlotData slot;
            //        if (j < additionalSlots.Count)
            //            slot = additionalSlots[j];
            //        else
            //        {
            //            slot = new HSceneSlotData();
            //            slot.slot = (RectTransform)Instantiate(buttonsParent.GetChild(0).gameObject).transform;
            //            slot.text = slot.slot.GetComponentInChildren<TextMeshProUGUI>(true);
            //            slot.button = slot.slot.GetComponentInChildren<Button>(true);
            //            slot.slot.SetParent(buttonsParent);
            //            slot.slot.localPosition = Vector3.zero;
            //            slot.slot.localScale = Vector3.one;
            //            var i1 = j;
            //            slot.button.onClick = new Button.ButtonClickedEvent();
            //            slot.button.onClick.AddListener(() =>
            //            {
            //                if (!Input.GetMouseButtonUp(0))
            //                    return;
            //                if (!_hSprite.IsSpriteAciotn())
            //                    return;
            //                additionalData.showAccessories[i1] = !additionalData.showAccessories[i1];
            //                Utils.Sound.Play(SystemSE.sel);
            //            });
            //            additionalSlots.Add(slot);
            //        }
            //        var objAccessory = additionalData.objAccessory[j];
            //        if (objAccessory == null)
            //            slot.slot.gameObject.SetActive(false);
            //        else
            //        {
            //            slot.slot.gameObject.SetActive(true);
            //            var component = objAccessory.GetComponent<ListInfoComponent>();
            //            slot.text.text = component.data.Name;
            //        }
            //    }

            //    for (; j < additionalSlots.Count; ++j)
            //        additionalSlots[j].slot.gameObject.SetActive(false);
            //}
        }
    }
}
