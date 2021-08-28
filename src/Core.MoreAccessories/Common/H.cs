using System.Collections.Generic;
#if EMOTIONCREATORS
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
#endif
using Illusion.Game;
#if KOIKATSU
#endif
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        internal void SpawnHUI(List<ChaControl> females, HSprite hSprite)
        {
            _hSceneFemales = females;
            _additionalHSceneSlots = new List<List<HSceneSlotData>>();
            for (var i = 0; i < 2; i++)
                _additionalHSceneSlots.Add(new List<HSceneSlotData>());
            _hSprite = hSprite;
            _hSceneMultipleFemaleButtons = _hSprite.lstMultipleFemaleDressButton;
            _hSceneSoloFemaleAccessoryButton = _hSprite.categoryAccessory;
            UpdateHUI();
        }

        private void UpdateHUI()
        {
            if (_hSprite == null)
                return;
            for (var i = 0; i < _hSceneFemales.Count; i++)
            {
                var female = _hSceneFemales[i];

                var additionalData = _accessoriesByChar[female.chaFile];
                int j;
                var additionalSlots = _additionalHSceneSlots[i];
                var buttonsParent = _hSceneFemales.Count == 1 ? _hSceneSoloFemaleAccessoryButton.transform : _hSceneMultipleFemaleButtons[i].accessory.transform;
                for (j = 0; j < additionalData.nowAccessories.Count; j++)
                {
                    HSceneSlotData slot;
                    if (j < additionalSlots.Count)
                        slot = additionalSlots[j];
                    else
                    {
                        slot = new HSceneSlotData();
                        slot.slot = (RectTransform)Instantiate(buttonsParent.GetChild(0).gameObject).transform;
                        slot.text = slot.slot.GetComponentInChildren<TextMeshProUGUI>(true);
                        slot.button = slot.slot.GetComponentInChildren<Button>(true);
                        slot.slot.SetParent(buttonsParent);
                        slot.slot.localPosition = Vector3.zero;
                        slot.slot.localScale = Vector3.one;
                        var i1 = j;
                        slot.button.onClick = new Button.ButtonClickedEvent();
                        slot.button.onClick.AddListener(() =>
                        {
                            if (!Input.GetMouseButtonUp(0))
                                return;
                            if (!_hSprite.IsSpriteAciotn())
                                return;
                            additionalData.showAccessories[i1] = !additionalData.showAccessories[i1];
                            Utils.Sound.Play(SystemSE.sel);
                        });
                        additionalSlots.Add(slot);
                    }
                    var objAccessory = additionalData.objAccessory[j];
                    if (objAccessory == null)
                        slot.slot.gameObject.SetActive(false);
                    else
                    {
                        slot.slot.gameObject.SetActive(true);
                        var component = objAccessory.GetComponent<ListInfoComponent>();
                        slot.text.text = component.data.Name;
                    }
                }

                for (; j < additionalSlots.Count; ++j)
                    additionalSlots[j].slot.gameObject.SetActive(false);
            }
        }
    }
}
