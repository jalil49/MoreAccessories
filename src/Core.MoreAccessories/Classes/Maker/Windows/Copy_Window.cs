#if KK || KKS
using ChaCustom;
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
    public class Copy_Window
    {
        public CvsAccessoryCopy CopyWindow { get; private set; }
        internal MoreAccessories Plugin => MoreAccessories._self;

        internal List<CharaMakerSlotData> AdditionalCharaMakerSlots { get { return MoreAccessories.MakerMode._additionalCharaMakerSlots; } set { MoreAccessories.MakerMode._additionalCharaMakerSlots = value; } }

        public Copy_Window(CvsAccessoryCopy _instance)
        {
            CopyWindow = _instance;
            MakeScrollable();
        }

        private ScrollRect ScrollView;

        public void RefreshToggles(int length)
        {
            var windowlength = CopyWindow.tglKind.Length;
            var delta = length - windowlength;
            if (delta < 1) return;

            var index = 1;
            foreach (var item in CopyWindow.tglKind)
            {
                item.GetComponentInChildren<TextMeshProUGUI>(true).text = index.ToString("00");
                index++;
            }

            var gameobject = ScrollView.content.GetChild(0).gameObject;
            var tglarray = new Toggle[delta];
            var srcarray = new TextMeshProUGUI[delta];
            var dstarray = new TextMeshProUGUI[delta];
            for (var i = 0; i < delta; i++, index++)
            {
                var copyToggle = Object.Instantiate(gameobject, ScrollView.content);
                tglarray[i] = copyToggle.GetComponentInChildren<Toggle>();
                tglarray[i].graphic.raycastTarget = true;
                tglarray[i].transform.GetComponentInChildren<TextMeshProUGUI>(true).text = index.ToString("00");
                copyToggle.name = "Slot" + index.ToString("00");
                srcarray[i] = copyToggle.transform.Find("srcText00").GetComponent<TextMeshProUGUI>();
                dstarray[i] = copyToggle.transform.Find("dstText00").GetComponent<TextMeshProUGUI>();
                var info = AdditionalCharaMakerSlots[index - 21];//21 since index starts at 1
                info.copySlotObject = copyToggle;
            }
            CopyWindow.tglKind = CopyWindow.tglKind.Concat(tglarray).ToArray();
            CopyWindow.textSrc = CopyWindow.textSrc.Concat(srcarray).ToArray();
            CopyWindow.textDst = CopyWindow.textDst.Concat(dstarray).ToArray();
        }

        private void MakeScrollable()
        {
            var container = (RectTransform)GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/04_AccessoryTop/tglCopy/CopyTop/rect").transform;
            ScrollView = UIUtility.CreateScrollView("CopySlots", container);
            ScrollView.movementType = ScrollRect.MovementType.Clamped;
            ScrollView.horizontal = false;
            ScrollView.scrollSensitivity = 18f;
            if (ScrollView.horizontalScrollbar != null)
                Object.Destroy(ScrollView.horizontalScrollbar.gameObject);
            if (ScrollView.verticalScrollbar != null)
                Object.Destroy(ScrollView.verticalScrollbar.gameObject);
            Object.Destroy(ScrollView.GetComponent<Image>());

            var content = (RectTransform)container.Find("grpClothes");
            ScrollView.transform.SetRect(content);
            content.SetParent(ScrollView.viewport);
            Object.Destroy(ScrollView.content.gameObject);
            ScrollView.content = content;
            ScrollView.transform.SetAsFirstSibling();
            ScrollView.transform.SetRect(new Vector2(0f, 1f), Vector2.one, new Vector2(16f, -570f), new Vector2(-16f, -80f));
        }
        internal void WindowRefresh()
        {
            CopyWindow.UpdateCustomUI();
        }
    }
}
#endif