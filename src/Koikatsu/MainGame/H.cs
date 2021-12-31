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

            //grab size of offsetmax
            var offsetMax = ((RectTransform)sprite.categoryAccessory.lstButton[0].transform).offsetMax;
            if (multi)//multi slots are smaller than single
                offsetMax = ((RectTransform)sprite.lstMultipleFemaleDressButton[0].accessory.lstButton[0].transform).offsetMax;
            offsetMax.x += 10;//add to adjust scrollbar position
            var element = scrolltemplate.AddComponent<LayoutElement>();
            element.preferredHeight = Screen.height / 3;//set height to 1/3 of screen height to display more accessories since i fixed having to show full list
            element.preferredWidth = offsetMax.x;//width of scroll

            scrollrect.content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var recttra = (RectTransform)scrolltemplate.transform;
            recttra.pivot = new Vector2(0, 1);//set pivot to Left,Top so that recttransform grows downwards on larger resolutions
            recttra.anchorMin = Vector2.zero;//starts at 0.5,0.5 for some reason
            recttra.anchorMax = new Vector2(0, 0.50f);//0.5f Anchor so that the scroll bar aligns well enough
            recttra.offsetMax = offsetMax;
            if (scrollrect.horizontalScrollbar != null)
                Object.Destroy(scrollrect.horizontalScrollbar.gameObject);
            Object.Destroy(scrollrect.content.gameObject);
            Object.Destroy(scrollrect.transform.GetComponent<Image>());//destroy white slightly transparent background
            scrollrect.transform.SetParent(container.parent, false);
            scrollrect.content = (RectTransform)container;
            container.SetParent(scrollrect.viewport);
        }
    }
}
