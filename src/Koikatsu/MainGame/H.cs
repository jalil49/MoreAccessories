using MoreAccessoriesKOI.Extensions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAccessoriesKOI
{
    public class HScene
    {
        public List<ChaControl> HSceneFemales { get; private set; }

        /// <summary>
        /// only KK VR uses array
        /// usually an array of size one as a result.
        /// </summary>
        public HSprite[] HSprites { get; private set; }

        internal HScene(List<ChaControl> lstFemale, HSprite[] sprite)
        {
            HSceneFemales = lstFemale;
            HSprites = sprite;
            Plugin.ExecuteDelayed(MakeSingleScrollable, 1);
            Plugin.ExecuteDelayed(MakeMultiScrollable, 1);
        }
        private MoreAccessories Plugin => MoreAccessories._self;

        private void MakeSingleScrollable()
        {
            foreach (var sprite in HSprites)
            {
                Scrollingwork(sprite, sprite.categoryAccessory.transform, false);
            }
        }

        private void MakeMultiScrollable()
        {
            foreach (var sprite in HSprites)
            {
                foreach (var FemaleDressButton in sprite.lstMultipleFemaleDressButton)
                {
                    Scrollingwork(sprite, FemaleDressButton.accessory.transform, true);
                }
            }
        }

        public void MakeMultiScrollable(int female)//in case adding multiple heroines and default was copied
        {
            foreach (var sprite in HSprites)
            {
                Scrollingwork(sprite, sprite.lstMultipleFemaleDressButton[female].accessory.transform, true);
            }
        }

        private void Scrollingwork(HSprite sprite, Transform container, bool multi)
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
            if (multi)//multi slots are smaller than single
                recttra.offsetMax = new Vector2(175, 0);
            if (scrollrect.horizontalScrollbar != null)
                Object.Destroy(scrollrect.horizontalScrollbar.gameObject);
            Object.Destroy(scrollrect.transform.GetComponent<Image>());
            Object.Destroy(scrollrect.content.gameObject);
            Object.Destroy(scrollrect.viewport.GetComponent<Image>());//removes blank being rendered on top
            Object.Destroy(scrollrect.viewport.GetComponent<Mask>());//fixes not being able to select out of scroll,
            scrollrect.transform.SetParent(container.parent, false);
            scrollrect.content = (RectTransform)container;
            container.SetParent(scrollrect.viewport);
        }
    }
}
