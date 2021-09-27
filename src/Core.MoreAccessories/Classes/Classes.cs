using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
#if EC
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
#endif

using System.Linq;

namespace MoreAccessoriesKOI
{
    #region Private Types
    public class CharAdditionalData
    {
        public CharAdditionalData() { }

#if KK || KKS
        public CharAdditionalData(ChaControl chactrl)
        {
            nowAccessories = chactrl.nowCoordinate.accessory.parts.Skip(20).ToList();
            for (var i = 0; i < chactrl.chaFile.coordinate.Length; i++)
            {
                rawAccessoriesInfos[i] = chactrl.chaFile.coordinate[i].accessory.parts.Skip(20).ToList();
            }
        }
        public CharAdditionalData(ChaFile file)
        {
            for (var i = 0; i < file.coordinate.Length; i++)
            {
                rawAccessoriesInfos[i] = file.coordinate[i].accessory.parts.Skip(20).ToList();
            }
        }
#elif EC
        public CharAdditionalData(ChaControl chactrl)
        {
            nowAccessories = chactrl.nowCoordinate.accessory.parts.Skip(20).ToList();
            rawAccessoriesInfos[0] = chactrl.chaFile.coordinate.accessory.parts.Skip(20).ToList();
        }
        public CharAdditionalData(ChaFile file)
        {
            rawAccessoriesInfos[0] = file.coordinate.accessory.parts.Skip(20).ToList();
        }
#endif
        public CharAdditionalData(ChaFileAccessory.PartsInfo[] parts)
        {
            nowAccessories = parts.Skip(20).ToList();
        }

        public List<ChaFileAccessory.PartsInfo> nowAccessories = new List<ChaFileAccessory.PartsInfo>();
        internal List<bool> showAccessories = new List<bool>();

#if EC
        public List<int> advState = new List<int>();
#endif
        public Dictionary<int, List<ChaFileAccessory.PartsInfo>> rawAccessoriesInfos = new Dictionary<int, List<ChaFileAccessory.PartsInfo>>();
    }
    public class CharaMakerSlotData
    {
        public GameObject AccessorySlot;
#if KK || KKS
        public GameObject copySlotObject;
#endif
        public GameObject transferSlotObject;
    }

#if KK || KKS
    public class StudioSlotData
    {
        public RectTransform slot;
        public Text name;
        public Button onButton;
        public Button offButton;
    }
#elif EC
    public class PlaySceneSlotData
    {
        public RectTransform slot;
        public TextMeshProUGUI text;
        public Button button;
    }

    public class ADVSceneSlotData
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
