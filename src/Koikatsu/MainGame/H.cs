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
            scrollrect.name = "AccessoryScroll";
            scrollrect.verticalScrollbar.GetComponent<Image>().sprite = original_scroll.verticalScrollbar.GetComponent<Image>().sprite;
            scrollrect.verticalScrollbar.image.sprite = original_scroll.verticalScrollbar.image.sprite;

            scrollrect.horizontal = false;
            scrollrect.scrollSensitivity = 40f;

            scrollrect.movementType = ScrollRect.MovementType.Clamped;
            scrollrect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrolltemplate.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var element = scrolltemplate.AddComponent<LayoutElement>();
            element.preferredHeight = Screen.height / 4;

            var recttra = (RectTransform)scrolltemplate.transform;

            recttra.pivot = new Vector2(0, 1);//set pivot to Left,Top so that recttransform grows downwards on larger resolutions
            recttra.anchorMax = new Vector2(0, 0.50f);//0.5f Anchor so that the scroll bar aligns well enough
            if (scrollrect.horizontalScrollbar != null)
                Object.Destroy(scrollrect.horizontalScrollbar.gameObject);
            Object.Destroy(scrollrect.transform.GetComponent<Image>());

            scrollrect.transform.SetParent(container.parent, false);
            scrollrect.content = (RectTransform)container;
            container.SetParent(scrollrect.content);
        }
    }
}
