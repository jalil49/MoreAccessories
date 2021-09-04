using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
#if EC
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
#endif
#if KK || KKS
#endif

using System.Linq;

namespace MoreAccessoriesKOI
{
    #region Private Types
    public class CharAdditionalData
    {
        public CharAdditionalData() { }

#if KKS || KK
        public CharAdditionalData(ChaControl chactrl)
        {
            nowAccessories = chactrl.nowCoordinate.accessory.parts.ToList();
            nowAccessories.RemoveRange(0, 20);
            for (var i = 0; i < chactrl.chaFile.coordinate.Length; i++)
            {
                rawAccessoriesInfos[i] = chactrl.chaFile.coordinate[i].accessory.parts.ToList();
                rawAccessoriesInfos[i].RemoveRange(0, 20);
            }
        }
#elif EC
        public CharAdditionalData(ChaControl chactrl)
        {
            nowAccessories = chactrl.nowCoordinate.accessory.parts.ToList();
            nowAccessories.RemoveRange(0, 20);

            rawAccessoriesInfos[0] = chactrl.chaFile.coordinate.accessory.parts.ToList();
            rawAccessoriesInfos[0].RemoveRange(0, 20);
        }
#endif
        public CharAdditionalData(ChaFileAccessory.PartsInfo[] parts)
        {
            nowAccessories = parts.ToList();
            nowAccessories.RemoveRange(0, 20);
        }

        public List<ChaFileAccessory.PartsInfo> nowAccessories = new List<ChaFileAccessory.PartsInfo>();
        internal List<bool> showAccessories = new List<bool>();

#if EC
        public List<int> advState = new List<int>();
#endif
        public readonly Dictionary<int, List<ChaFileAccessory.PartsInfo>> rawAccessoriesInfos = new Dictionary<int, List<ChaFileAccessory.PartsInfo>>();
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
