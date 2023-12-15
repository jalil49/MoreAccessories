using System.Collections.Generic;
using System.Linq;
using ChaCustom;
using MoreAccessoriesKOI.Extensions;
using TMPro;
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
        public CvsAccessoryCopy CopyWindow { get; }
        private ScrollRect _scrollView;

        internal List<CharaMakerSlotData> AdditionalCharaMakerSlots
        {
            get => MoreAccessories.MakerMode.AdditionalCharaMakerSlots;
            set => MoreAccessories.MakerMode.AdditionalCharaMakerSlots = value;
        }

        internal Copy_Window(CvsAccessoryCopy instance)
        {
            CopyWindow = instance;
            MakeScrollable();
            CopyWindow.btnAllOn.onClick = new Button.ButtonClickedEvent();
            CopyWindow.btnAllOn.onClick.AddListener(SelectAllToggles);
        }


        internal void RefreshToggles(int length)
        {
            MoreAccessories._self.ExecuteDelayed(WindowRefresh);

            var delta = length - CopyWindow.tglKind.Length;
            if (delta < 1) return;

            var index = 1;
            foreach (var item in CopyWindow.tglKind)
            {
                item.GetComponentInChildren<TextMeshProUGUI>(true).text = index.ToString("00");
                index++;
            }

            var gameObject = _scrollView.content.GetChild(0).gameObject;
            var tglArray = new Toggle[delta];
            var srcArray = new TextMeshProUGUI[delta];
            var dstArray = new TextMeshProUGUI[delta];

            for (var i = 0; i < delta; i++, index++)
            {
                var copyToggle = Object.Instantiate(gameObject, _scrollView.content);
                tglArray[i] = copyToggle.GetComponentInChildren<Toggle>();
                tglArray[i].Set(false);
                tglArray[i].graphic.raycastTarget = true;
                tglArray[i].transform.GetComponentInChildren<TextMeshProUGUI>(true).text = index.ToString("00");
                copyToggle.name = "kind" + (index - 1).ToString("00");
                srcArray[i] = copyToggle.transform.Find("srcText00").GetComponent<TextMeshProUGUI>();
                srcArray[i].name = $"srcText{i:00}";
                dstArray[i] = copyToggle.transform.Find("dstText00").GetComponent<TextMeshProUGUI>();
                dstArray[i].name = $"dstText{i:00}";
                AdditionalCharaMakerSlots[index - 21].copySlotObject = copyToggle;
            }

            CopyWindow.tglKind = CopyWindow.tglKind.Concat(tglArray).ToArray();
            CopyWindow.textSrc = CopyWindow.textSrc.Concat(srcArray).ToArray();
            CopyWindow.textDst = CopyWindow.textDst.Concat(dstArray).ToArray();
        }

        internal void ValidateToggles()
        {
            for (var i = CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length; i < CopyWindow.tglKind.Length; i++)
            {
                CopyWindow.tglKind[i].Set(false);
            }
        }

        private void SelectAllToggles()
        {
            var array = CopyWindow.tglKind;
            var partsCount = CopyWindow.accessory.parts.Length;
            for (var i = 0; i < partsCount; i++)
            {
                array[i].Set(true);
            }
        }

        private void MakeScrollable()
        {
            var container = (RectTransform)GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/04_AccessoryTop/tglCopy/CopyTop/rect").transform;
            _scrollView = UIUtility.CreateScrollView("CopySlots", container);
            _scrollView.movementType = ScrollRect.MovementType.Clamped;
            _scrollView.horizontal = false;
            _scrollView.scrollSensitivity = 18f;
            if (_scrollView.horizontalScrollbar != null)
                Object.Destroy(_scrollView.horizontalScrollbar.gameObject);
            if (_scrollView.verticalScrollbar != null)
                Object.Destroy(_scrollView.verticalScrollbar.gameObject);
            Object.Destroy(_scrollView.GetComponent<Image>());

            var content = (RectTransform)container.Find("grpClothes");
            _scrollView.transform.SetRect(content);
            content.SetParent(_scrollView.viewport);
            Object.Destroy(_scrollView.content.gameObject);
            _scrollView.content = content;
            _scrollView.transform.SetAsFirstSibling();
            _scrollView.transform.SetRect(new Vector2(0f, 1f), Vector2.one, new Vector2(16f, -570f), new Vector2(-16f, -80f));

            MoreAccessories._self.ExecuteDelayed(delegate { CopyWindow.transform.localPosition -= new Vector3(50, 0, 0); });
        }

        internal void WindowRefresh()
        {
            CopyWindow.UpdateCustomUI();
            ValidateToggles();
        }
    }
}