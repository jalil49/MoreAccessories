using System.Collections.Generic;
using ChaCustom;
#if EMOTIONCREATORS
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
#endif
using MessagePack;
#if KOIKATSU
#endif
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        #region Private Types
        public class CharAdditionalData
        {
            public List<ChaFileAccessory.PartsInfo> nowAccessories;
            public readonly List<ListInfoBase> infoAccessory = new List<ListInfoBase>();
            public readonly List<GameObject> objAccessory = new List<GameObject>();
            public readonly List<GameObject[]> objAcsMove = new List<GameObject[]>();
            public readonly List<ChaAccessoryComponent> cusAcsCmp = new List<ChaAccessoryComponent>();

#if EMOTIONCREATORS
            public List<int> advState = new List<int>();
#endif
            public List<bool> showAccessories = new List<bool>();
            public readonly Dictionary<int, List<ChaFileAccessory.PartsInfo>> rawAccessoriesInfos = new Dictionary<int, List<ChaFileAccessory.PartsInfo>>();

            public void LoadFrom(CharAdditionalData other)
            {
#if EMOTIONCREATORS
                advState.Clear();
#endif
                showAccessories.Clear();
                rawAccessoriesInfos.Clear();
                infoAccessory.Clear();
                foreach (var o in objAccessory)
                {
                    if (o != null)
                        Destroy(o);
                }
                objAccessory.Clear();
                objAcsMove.Clear();
                foreach (var c in cusAcsCmp)
                {
                    if (c != null)
                        Destroy(c);
                }
                cusAcsCmp.Clear();


#if EMOTIONCREATORS
                advState.AddRange(other.advState);
#endif
                showAccessories.AddRange(other.showAccessories);

                foreach (var coordPair in other.rawAccessoriesInfos)
                {
                    var parts = new List<ChaFileAccessory.PartsInfo>();
                    foreach (var part in coordPair.Value)
                        parts.Add(MessagePackSerializer.Deserialize<ChaFileAccessory.PartsInfo>(MessagePackSerializer.Serialize(part)));
                    if (coordPair.Value == other.nowAccessories)
                        nowAccessories = parts;
                    rawAccessoriesInfos.Add(coordPair.Key, parts);
                }

                infoAccessory.AddRange(new ListInfoBase[other.infoAccessory.Count]);
                objAccessory.AddRange(new GameObject[other.objAccessory.Count]);
                while (objAcsMove.Count < other.objAcsMove.Count)
                    objAcsMove.Add(new GameObject[2]);
                cusAcsCmp.AddRange(new ChaAccessoryComponent[other.cusAcsCmp.Count]);
            }
        }

        internal class CharaMakerSlotData
        {
            public Toggle toggle;
            public CanvasGroup canvasGroup;
            public TextMeshProUGUI text;
            public CvsAccessory cvsAccessory;

#if KOIKATSU
            public GameObject copySlotObject;
            public Toggle copyToggle;
            public TextMeshProUGUI copySourceText;
            public TextMeshProUGUI copyDestinationText;
#endif

            public GameObject transferSlotObject;
            public Toggle transferSourceToggle;
            public Toggle transferDestinationToggle;
            public TextMeshProUGUI transferSourceText;
            public TextMeshProUGUI transferDestinationText;
        }

#if KOIKATSU
        private class StudioSlotData
        {
            public RectTransform slot;
            public Text name;
            public Button onButton;
            public Button offButton;
        }

        private class HSceneSlotData
        {
            public RectTransform slot;
            public TextMeshProUGUI text;
            public Button button;
        }
#elif EMOTIONCREATORS
        private class PlaySceneSlotData
        {
            public RectTransform slot;
            public TextMeshProUGUI text;
            public Button button;
        }

        private class ADVSceneSlotData
        {
            public RectTransform slot;
            public TextMeshProUGUI text;
            public Toggle keep;
            public Toggle wear;
            public Toggle takeOff;
        }
#endif
        #endregion
    }
}
