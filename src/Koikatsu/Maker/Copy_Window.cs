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
    /// <summary>
    /// CopyWindow add scrolling and slots
    /// If Unwanted dependency hook CvsAccessoryCopy.Start for reference 
    /// </summary>
    public class Copy_Window
    {
        public CvsAccessoryCopy CopyWindow { get; private set; }
        internal MoreAccessories Plugin => MoreAccessories._self;
        internal List<CharaMakerSlotData> AdditionalCharaMakerSlots { get { return MoreAccessories.MakerMode._additionalCharaMakerSlots; } set { MoreAccessories.MakerMode._additionalCharaMakerSlots = value; } }

        internal Copy_Window(CvsAccessoryCopy _instance)
        {
            CopyWindow = _instance;
            MakeScrollable();
            CopyWindow.btnAllOn.onClick = new Button.ButtonClickedEvent();
            CopyWindow.btnAllOn.onClick.AddListener(SelectAllToggles);
        }

        private ScrollRect ScrollView;

        internal void RefreshToggles(int length)
        {
            Plugin.ExecuteDelayed(WindowRefresh);

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
                tglarray[i].Set(false);
                tglarray[i].graphic.raycastTarget = true;
                tglarray[i].transform.GetComponentInChildren<TextMeshProUGUI>(true).text = index.ToString("00");
                copyToggle.name = "kind" + (index - 1).ToString("00");
                srcarray[i] = copyToggle.transform.Find("srcText00").GetComponent<TextMeshProUGUI>();
                srcarray[i].name = $"srcText{i:00}";
                dstarray[i] = copyToggle.transform.Find("dstText00").GetComponent<TextMeshProUGUI>();
                dstarray[i].name = $"dstText{i:00}";
                var info = AdditionalCharaMakerSlots[index - 21];//21 since index starts at 1
                info.copySlotObject = copyToggle;
            }

            CopyWindow.tglKind = CopyWindow.tglKind.Concat(tglarray).ToArray();
            CopyWindow.textSrc = CopyWindow.textSrc.Concat(srcarray).ToArray();
            CopyWindow.textDst = CopyWindow.textDst.Concat(dstarray).ToArray();
        }

        internal void ValidatateToggles()
        {
            for (var i = CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length; i < CopyWindow.tglKind.Length; i++)
            {
                CopyWindow.tglKind[i].Set(false);
            }
        }

        private void SelectAllToggles()
        {
            var array = CopyWindow.tglKind;
            var partscount = CopyWindow.accessory.parts.Length;
            for (var i = 0; i < partscount; i++)
            {
                array[i].Set(true);
            }
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

            Plugin.ExecuteDelayed(delegate ()
            {
                CopyWindow.transform.localPosition -= new Vector3(50, 0, 0);
            });
        }

        internal void WindowRefresh()
        {
            CopyWindow.UpdateCustomUI();
            ValidatateToggles();
        }
    }
}
