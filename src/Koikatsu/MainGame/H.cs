using MoreAccessoriesKOI.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAccessoriesKOI
{
    public class HScene
    {
        internal List<ChaControl> _hSceneFemales;
        private readonly HSprite[] _hSprite;

        public HScene(List<ChaControl> lstFemale, HSprite[] sprite)
        {
            _hSceneFemales = lstFemale;
            _hSprite = sprite;
            Plugin.ExecuteDelayed(MakeSingleScrollable, 1);
            Plugin.ExecuteDelayed(MakeMultiScrollable, 1);
        }

        private MoreAccessories Plugin => MoreAccessories._self;

        public List<ChaControl> LstFemale { get; }
        public HSprite Sprite { get; }

        private void MakeSingleScrollable()
        {
            foreach (var sprite in _hSprite)
            {
                Scrollingwork(sprite, sprite.categoryAccessory.transform);
            }
        }
        private void MakeMultiScrollable()
        {
            foreach (var sprite in _hSprite)
            {
                foreach (var FemaleDressButton in sprite.lstMultipleFemaleDressButton)
                {
                    Scrollingwork(sprite, FemaleDressButton.accessory.transform);
                }
            }
        }
        public void MakeMultiScrollable(int female)//in case adding multiple heroines
        {
            foreach (var sprite in _hSprite)
            {
                Scrollingwork(sprite, sprite.lstMultipleFemaleDressButton[female].accessory.transform);
            }
        }
        private void Scrollingwork(HSprite sprite, Transform container)//Its a miracle that this works
        {
            var original_scroll = sprite.clothCusutomCtrl.transform.GetComponentInChildren<ScrollRect>();

            var scrolltemplate = DefaultControls.CreateScrollView(new DefaultControls.Resources());
            var scrollrect = scrolltemplate.GetComponent<ScrollRect>();

            scrollrect.verticalScrollbar.GetComponent<Image>().sprite = original_scroll.verticalScrollbar.GetComponent<Image>().sprite;
            scrollrect.verticalScrollbar.image.sprite = original_scroll.verticalScrollbar.image.sprite;

            scrollrect.horizontal = false;
            scrollrect.scrollSensitivity = 40f;

            scrollrect.movementType = ScrollRect.MovementType.Clamped;
            scrollrect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrolltemplate.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var element = scrolltemplate.AddComponent<LayoutElement>();
            element.preferredHeight = element.minHeight = Screen.height / 4;
            element.preferredWidth = element.minWidth = Screen.width / 10;
            if (scrollrect.horizontalScrollbar != null)
                Object.DestroyImmediate(scrollrect.horizontalScrollbar.gameObject);
            //scrollrect.rectTransform.localPosition -= new Vector3(0, 100, 0);
            //if (scrollrect.verticalScrollbar != null)
            //    Object.DestroyImmediate(scrollrect.verticalScrollbar.gameObject);
            Object.DestroyImmediate(scrollrect.transform.GetComponent<Image>());

            var vlg = scrollrect.content.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.childForceExpandHeight = true;

            scrollrect.transform.SetRect(container);
            //Object.Destroy(scrollrect.content.gameObject);
            scrollrect.content = (RectTransform)container;
            scrollrect.transform.SetParent(container.parent, false);
        }
        internal void UpdateHUI()
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
