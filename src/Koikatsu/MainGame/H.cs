using System.Collections.Generic;
using MoreAccessoriesKOI.Extensions;
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
        public HSprite[] HSprites { get; }

        internal HScene(List<ChaControl> lstFemale, HSprite[] sprite)
        {
            HSceneFemales = lstFemale;
            HSprites = sprite;
            var moreAccessories = MoreAccessories._self;
            moreAccessories.ExecuteDelayed(MakeSingleScrollable);
            moreAccessories.ExecuteDelayed(MakeMultiScrollable);
        }

        private void MakeSingleScrollable()
        {
            foreach (var sprite in HSprites)
            {
                ScrollingWork(sprite, sprite.categoryAccessory.transform, false);
            }
        }

        private void MakeMultiScrollable()
        {
            foreach (var sprite in HSprites)
            {
                foreach (var femaleDressButton in sprite.lstMultipleFemaleDressButton)
                {
                    ScrollingWork(sprite, femaleDressButton.accessory.transform, true);
                }
            }
        }

        public void MakeMultiScrollable(int female)//in case adding multiple heroines and default was copied
        {
            foreach (var sprite in HSprites)
            {
                ScrollingWork(sprite, sprite.lstMultipleFemaleDressButton[female].accessory.transform, true);
            }
        }

        private static void ScrollingWork(HSprite sprite, Transform container, bool multi)
        {
            var originalScroll = sprite.clothCusutomCtrl.transform.GetComponentInChildren<ScrollRect>();

            var scrollTemplate = DefaultControls.CreateScrollView(new DefaultControls.Resources());
            var scrollRect = scrollTemplate.GetComponent<ScrollRect>();
            scrollRect.name = "AccessoryScroll";
            scrollRect.verticalScrollbar.GetComponent<Image>().sprite = originalScroll.verticalScrollbar.GetComponent<Image>().sprite;
            scrollRect.verticalScrollbar.image.sprite = originalScroll.verticalScrollbar.image.sprite;

            scrollRect.horizontal = false;
            scrollRect.scrollSensitivity = 40f;

            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollTemplate.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            //grab size of offset max
            var offsetMax = ((RectTransform)sprite.categoryAccessory.lstButton[0].transform).offsetMax;
            if (multi)//multi slots are smaller than single
                offsetMax = ((RectTransform)sprite.lstMultipleFemaleDressButton[0].accessory.lstButton[0].transform).offsetMax;
            offsetMax.x += 10;//add to adjust scrollbar position
            var element = scrollTemplate.AddComponent<LayoutElement>();
            element.preferredHeight = Screen.height / 3f;//set height to 1/3 of screen height to display more accessories since i fixed having to show full list
            element.preferredWidth = offsetMax.x;//width of scroll

            scrollRect.content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var rectTransform = (RectTransform)scrollTemplate.transform;
            rectTransform.pivot = new Vector2(0, 1);//set pivot to Left,Top so that rect transform grows downwards on larger resolutions
            rectTransform.anchorMin = Vector2.zero;//starts at 0.5,0.5 for some reason
            rectTransform.anchorMax = new Vector2(0, 0.50f);//0.5f Anchor so that the scroll bar aligns well enough
            rectTransform.offsetMax = offsetMax;
            if (scrollRect.horizontalScrollbar != null)
                Object.Destroy(scrollRect.horizontalScrollbar.gameObject);
            Object.Destroy(scrollRect.content.gameObject);
            Object.Destroy(scrollRect.transform.GetComponent<Image>());//destroy white slightly transparent background
            scrollRect.transform.SetParent(container.parent, false);
            scrollRect.content = (RectTransform)container;
            container.SetParent(scrollRect.viewport);
        }
    }
}
