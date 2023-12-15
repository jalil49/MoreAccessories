using System.Collections.Generic;
using System.Linq;
using ChaCustom;
using Illusion.Extensions;
using MoreAccessoriesKOI.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAccessoriesKOI
{
    /// <summary>
    /// Transfer Window Patching, add scrolling and additional slots
    /// If Unwanted dependency hook CvsAccessoryChange.Start for reference 
    /// </summary>
    public class Transfer_Window
    {
        public  CvsAccessoryChange ChangeWindow { get; }

        private ScrollRect _scrollView;

        internal List<CharaMakerSlotData> AdditionalCharaMakerSlots
        {
            get => MoreAccessories.MakerMode.AdditionalCharaMakerSlots;
            set => MoreAccessories.MakerMode.AdditionalCharaMakerSlots = value;
        }

        internal Transfer_Window(CvsAccessoryChange instance)
        {
            ChangeWindow = instance;
            MakeScrollable();
        }


        private void MakeScrollable()
        {
            var container = (RectTransform)GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/04_AccessoryTop/tglChange/ChangeTop/rect").transform;
            _scrollView = UIUtility.CreateScrollView("kind", container);
            _scrollView.movementType = ScrollRect.MovementType.Clamped;
            _scrollView.horizontal = false;
            _scrollView.scrollSensitivity = 18f;

            MoreAccessories._self.ExecuteDelayed(delegate { ChangeWindow.transform.localPosition -= new Vector3(50, 0, 0); });

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
            _scrollView.transform.SetRect(new Vector2(0f, 1f), Vector2.one, new Vector2(16f, -530f), new Vector2(-16f, -48f));
        }

        internal void RefreshToggles(int maxLength)
        {
            MoreAccessories._self.ExecuteDelayed(WindowRefresh);

            var delta = maxLength - ChangeWindow.tglSrcKind.Length;
            if (delta < 1) return;

            var index = 1;
            foreach (var item in _scrollView.content.Children())
            {
                item.GetComponentInChildren<TextMeshProUGUI>(true).text = index.ToString("00");
                index++;
            }

            var gameObject = _scrollView.content.GetChild(0);
            var tglSrcKindArray = new Toggle[delta];
            var tglDstKindArray = new Toggle[delta];
            var srcArray = new TextMeshProUGUI[delta];
            var dstArray = new TextMeshProUGUI[delta];

            //OnValueChangedAsObservable overwrites the selected slot save original value
            var originalSelDst = ChangeWindow.selDst;
            var originalSelSrc = ChangeWindow.selSrc;

            for (var i = 0; i < delta; i++, index++)
            {
                var transfer = Object.Instantiate(gameObject, _scrollView.content);
                transfer.GetComponentInChildren<TextMeshProUGUI>().text = index.ToString("00");
                var srcToggle = tglSrcKindArray[i] = transfer.GetChild(1).GetComponentInChildren<Toggle>();
                var tempIndex = index - 1;
                srcToggle.Set(false);
                srcToggle.onValueChanged = new Toggle.ToggleEvent();
                srcToggle.OnValueChangedAsObservable().Subscribe(delegate { ChangeWindow.selSrc = tempIndex; });
                srcArray[i] = srcToggle.GetComponentInChildren<TextMeshProUGUI>();

                var dstToggle = tglDstKindArray[i] = transfer.GetChild(2).GetComponentInChildren<Toggle>();
                dstToggle.Set(false);
                dstToggle.onValueChanged = new Toggle.ToggleEvent();
                dstToggle.OnValueChangedAsObservable().Subscribe(delegate { ChangeWindow.selDst = tempIndex; });
                dstArray[i] = dstToggle.GetComponentInChildren<TextMeshProUGUI>();

                transfer.name = $"kind{tempIndex}";

                srcToggle.graphic.raycastTarget = true;
                dstToggle.graphic.raycastTarget = true;

                AdditionalCharaMakerSlots.Add(new CharaMakerSlotData { transferSlotObject = transfer.gameObject });
            }

            ChangeWindow.selDst = originalSelDst;
            ChangeWindow.selSrc = originalSelSrc;

            ChangeWindow.tglSrcKind = ChangeWindow.tglSrcKind.Concat(tglSrcKindArray).ToArray();
            ChangeWindow.tglDstKind = ChangeWindow.tglDstKind.Concat(tglDstKindArray).ToArray();
            ChangeWindow.textSrc = ChangeWindow.textSrc.Concat(srcArray).ToArray();
            ChangeWindow.textDst = ChangeWindow.textDst.Concat(dstArray).ToArray();
        }

        internal void WindowRefresh()
        {
            ChangeWindow.UpdateCustomUI();
            ValidateToggles();
        }

        internal void ValidateToggles()
        {
            var partsCount = CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length;
            if (ChangeWindow.selSrc >= partsCount)
            {
                ChangeWindow.tglSrcKind[ChangeWindow.selSrc].Set(false);
                ChangeWindow.tglSrcKind[0].Set(true);
                ChangeWindow.selSrc = 0;
            }

            if (ChangeWindow.selDst >= partsCount)
            {
                ChangeWindow.tglDstKind[ChangeWindow.selDst].Set(false);
                ChangeWindow.tglDstKind[0].Set(true);
                ChangeWindow.selDst = 0;
            }
        }
    }
}