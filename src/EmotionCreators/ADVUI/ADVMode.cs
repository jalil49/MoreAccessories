#if false
using ADVPart.Manipulate.Chara;
using UnityEngine;

namespace MoreAccessoriesKOI
{
    public class ADVMode
    {
        public AccessoryUICtrl _advUI { get; private set; }
        public RectTransform _advToggleTemplate { get; private set; }

        public ADVMode(AccessoryUICtrl ui)
        {
            _advUI = ui;
            _advToggleTemplate = (RectTransform)ui.toggles[19].toggles[0].transform.parent.parent;
            SpawnADVUI();
        }

        internal void SpawnADVUI()
        {

            var buttons = _advUI.buttonALL.buttons;
            for (var i = 0; i < buttons.Length; i++)
            {
                var b = buttons[i];
                var i1 = i;
                b.onClick.AddListener(() =>
                {
                    //CharAdditionalData ad = _accessoriesByChar[_advUI.chaControl.chaFile];
                    //for (var j = 0; j < ad.advState.Count; j++)
                    //    ad.advState[j] = i1 - 1;
                    UpdateADVUI();
                });
            }
        }
        internal void UpdateADVUI()
        {
            if (_advUI == null)
                return;

            //CharAdditionalData additionalData = _accessoriesByChar[_advUI.chaControl.chaFile];
            //var i = 0;
            //for (; i < additionalData.nowAccessories.Count; ++i)
            //{
            //    ADVSceneSlotData slot;
            //    if (i < _additionalADVSceneSlots.Count)
            //        slot = _additionalADVSceneSlots[i];
            //    else
            //    {
            //        slot = new ADVSceneSlotData();
            //        slot.slot = (RectTransform)Instantiate(_advToggleTemplate.gameObject).transform;
            //        slot.slot.SetParent(_advToggleTemplate.parent);
            //        slot.slot.localPosition = Vector3.zero;
            //        slot.slot.localRotation = Quaternion.identity;
            //        slot.slot.localScale = Vector3.one;
            //        slot.text = slot.slot.Find("TextMeshPro").GetComponent<TextMeshProUGUI>();
            //        slot.keep = slot.slot.Find("Root/Button -1").GetComponent<Toggle>();
            //        slot.wear = slot.slot.Find("Root/Button 0").GetComponent<Toggle>();
            //        slot.takeOff = slot.slot.Find("Root/Button 1").GetComponent<Toggle>();
            //        slot.text.text = "スロット" + (21 + i);

            //        slot.keep.onValueChanged = new Toggle.ToggleEvent();
            //        var i1 = i;
            //        slot.keep.onValueChanged.AddListener(b =>
            //        {
            //            CharAdditionalData ad = _accessoriesByChar[_advUI.chaControl.chaFile];
            //            ad.advState[i1] = -1;
            //        });
            //        slot.wear.onValueChanged = new Toggle.ToggleEvent();
            //        slot.wear.onValueChanged.AddListener(b =>
            //        {
            //            CharAdditionalData ad = _accessoriesByChar[_advUI.chaControl.chaFile];
            //            ad.advState[i1] = 0;
            //            _advUI.chaControl.SetAccessoryState(i1 + 20, true);
            //        });
            //        slot.takeOff.onValueChanged = new Toggle.ToggleEvent();
            //        slot.takeOff.onValueChanged.AddListener(b =>
            //        {
            //            CharAdditionalData ad = _accessoriesByChar[_advUI.chaControl.chaFile];
            //            ad.advState[i1] = 1;
            //            _advUI.chaControl.SetAccessoryState(i1 + 20, false);
            //        });

            //        _additionalADVSceneSlots.Add(slot);
            //    }
            //    slot.slot.gameObject.SetActive(true);
            //    slot.keep.SetIsOnNoCallback(additionalData.advState[i] == -1);
            //    slot.keep.interactable = additionalData.objAccessory[i] != null;
            //    slot.wear.SetIsOnNoCallback(additionalData.advState[i] == 0);
            //    slot.wear.interactable = additionalData.objAccessory[i] != null;
            //    slot.takeOff.SetIsOnNoCallback(additionalData.advState[i] == 1);
            //    slot.takeOff.interactable = additionalData.objAccessory[i] != null;
            //}
            //for (; i < _additionalADVSceneSlots.Count; i++)
            //    _additionalADVSceneSlots[i].slot.gameObject.SetActive(false);
            //var parent = (RectTransform)_advToggleTemplate.parent.parent;
            //parent.offsetMin = new Vector2(0, parent.offsetMax.y - 66 - 34 * (additionalData.nowAccessories.Count + 21));
            //ExecuteDelayed(() =>
            //{
            //    //Fuck you I'm going to bed
            //    LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_advToggleTemplate.parent.parent);
            //    LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_advToggleTemplate.parent.parent.parent);
            //});
        }

    }
}
#endif