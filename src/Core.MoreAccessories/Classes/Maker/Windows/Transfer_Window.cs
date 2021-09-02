using ChaCustom;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using Illusion.Extensions;
using MoreAccessoriesKOI.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAccessoriesKOI
{
    public class Transfer_Window
    {
        public CvsAccessoryChange ChangeWindow { get; private set; }
        public MoreAccessories Plugin => MoreAccessories._self;

        internal List<CharaMakerSlotData> AdditionalCharaMakerSlots { get { return Plugin.MakerMode._additionalCharaMakerSlots; } set { Plugin.MakerMode._additionalCharaMakerSlots = value; } }

        public Transfer_Window(CvsAccessoryChange _instance)
        {
            ChangeWindow = _instance;
            MakeScrollable();
        }

        private ScrollRect ScrollView;

        internal void SetSourceIndex(int index)
        {
            ChangeWindow.selSrc = index;
        }

        internal void SetDestinationIndex(int index)
        {
            ChangeWindow.selDst = index;
        }

        public void MakeScrollable()
        {
            var container = (RectTransform)GameObject.Find("CustomScene/CustomRoot/FrontUIGroup/CustomUIGroup/CvsMenuTree/04_AccessoryTop/tglChange/ChangeTop/rect").transform;
            ScrollView = UIUtility.CreateScrollView("Slots", container);
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
            Plugin.MakerMode.RaycastCtrls.Add(container.parent.GetComponent<UI_RaycastCtrl>());
            ScrollView.transform.SetAsFirstSibling();
            ScrollView.transform.SetRect(new Vector2(0f, 1f), Vector2.one, new Vector2(16f, -530f), new Vector2(-16f, -48f));
        }

        internal void RefreshToggles(int length)
        {
            var windowlength = ChangeWindow.tglSrcKind.Length;
            var delta = length - windowlength;
            if (delta < 1) return;

            var index = 1;
            foreach (var item in ChangeWindow.tglSrcKind)
            {
                item.GetComponentInChildren<TextMeshProUGUI>(true).text = index.ToString("00");
                index++;
            }

            var gameobject = ScrollView.content.GetChild(0);
            var tglSrcKindarray = new Toggle[delta];
            var tglDstKindarray = new Toggle[delta];
            var srcarray = new TextMeshProUGUI[delta];
            var dstarray = new TextMeshProUGUI[delta];
            for (var i = 0; i < delta; i++, index++)
            {
                var Transfer = Object.Instantiate(gameobject, ScrollView.content);
                tglSrcKindarray[i] = Transfer.GetChild(1).GetComponentInChildren<Toggle>();
                srcarray[i] = tglSrcKindarray[i].GetComponent<TextMeshProUGUI>();

                tglDstKindarray[i] = Transfer.GetChild(2).GetComponentInChildren<Toggle>();
                dstarray[i] = tglDstKindarray[i].GetComponent<TextMeshProUGUI>();

                Transfer.name = "Slot" + index.ToString("00");

                var info = new CharaMakerSlotData { transferSlotObject = Transfer.gameObject };
                AdditionalCharaMakerSlots.Add(info);
            }
            ChangeWindow.tglSrcKind = ChangeWindow.tglSrcKind.Concat(tglSrcKindarray).ToArray();
            ChangeWindow.tglDstKind = ChangeWindow.tglDstKind.Concat(tglDstKindarray).ToArray();
            ChangeWindow.textSrc = ChangeWindow.textSrc.Concat(srcarray).ToArray();
            ChangeWindow.textDst = ChangeWindow.textDst.Concat(dstarray).ToArray();
        }

        public void Something()
        {
            var index = 0;
            var transferSlotObject = Object.Instantiate(ScrollView.content.GetChild(0).gameObject, ScrollView.content);
            var transferSourceToggle = transferSlotObject.transform.GetChild(1).GetComponentInChildren<Toggle>();
            var transferDestinationToggle = transferSlotObject.transform.GetChild(2).GetComponentInChildren<Toggle>();
            var transferSourceText = transferSourceToggle.GetComponentInChildren<TextMeshProUGUI>();
            var transferDestinationText = transferDestinationToggle.GetComponentInChildren<TextMeshProUGUI>();
            transferSlotObject.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = (index + 1).ToString("00");
            transferSourceText.text = "なし";
            transferDestinationText.text = "なし";
            transferSlotObject.name = "kind" + index.ToString("00");
            transferSourceToggle.onValueChanged = new Toggle.ToggleEvent();
            transferSourceToggle.onValueChanged.AddListener((b) =>
            {
                if (transferSourceToggle.isOn)
                    SetSourceIndex(index);
            });
            transferDestinationToggle.onValueChanged = new Toggle.ToggleEvent();
            transferDestinationToggle.onValueChanged.AddListener((b) =>
            {
                if (transferDestinationToggle.isOn)
                    SetDestinationIndex(index);
            });
            transferSourceToggle.isOn = false;
            transferDestinationToggle.isOn = false;
            transferSourceToggle.graphic.raycastTarget = true;
            transferDestinationToggle.graphic.raycastTarget = true;
        }
    }
}
