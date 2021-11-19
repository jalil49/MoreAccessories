using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ExtensibleSaveFormat;
using MoreAccessoriesKOI.Extensions;
using System.Linq;
using System.Xml;

#if EC
using TMPro;
#endif


namespace MoreAccessoriesKOI
{
    #region Private Types
    public class CharAdditionalData
    {
        public CharAdditionalData() { }

        public CharAdditionalData(PluginData pluginData)
        {
            XmlNode node = null;
            if (pluginData.data.TryGetValue("additionalAccessories", out var xmlData) && xmlData != null)
            {
                var doc = new XmlDocument();
                doc.LoadXml((string)xmlData);
                node = doc.FirstChild;
            }
            if (node != null)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    switch (childNode.Name)
                    {
                        case "accessorySet":
                            var coordinateType = XmlConvert.ToInt32(childNode.Attributes["type"].Value);
                            List<ChaFileAccessory.PartsInfo> parts;

                            if (rawAccessoriesInfos.TryGetValue(coordinateType, out parts) == false)
                            {
                                parts = new List<ChaFileAccessory.PartsInfo>();
                                rawAccessoriesInfos.Add(coordinateType, parts);
                            }

                            foreach (XmlNode accessoryNode in childNode.ChildNodes)
                            {
                                var part = new ChaFileAccessory.PartsInfo { type = XmlConvert.ToInt32(accessoryNode.Attributes["type"].Value) };
                                if (part.type != 120)
                                {
                                    part.id = XmlConvert.ToInt32(accessoryNode.Attributes["id"].Value);
                                    part.parentKey = accessoryNode.Attributes["parentKey"].Value;

                                    for (var i = 0; i < 2; i++)
                                    {
                                        for (var j = 0; j < 3; j++)
                                        {
                                            part.addMove[i, j] = new Vector3
                                            {
                                                x = XmlConvert.ToSingle(accessoryNode.Attributes[$"addMove{i}{j}x"].Value),
                                                y = XmlConvert.ToSingle(accessoryNode.Attributes[$"addMove{i}{j}y"].Value),
                                                z = XmlConvert.ToSingle(accessoryNode.Attributes[$"addMove{i}{j}z"].Value)
                                            };
                                        }
                                    }
                                    for (var i = 0; i < 4; i++)
                                    {
                                        part.color[i] = new Color
                                        {
                                            r = XmlConvert.ToSingle(accessoryNode.Attributes[$"color{i}r"].Value),
                                            g = XmlConvert.ToSingle(accessoryNode.Attributes[$"color{i}g"].Value),
                                            b = XmlConvert.ToSingle(accessoryNode.Attributes[$"color{i}b"].Value),
                                            a = XmlConvert.ToSingle(accessoryNode.Attributes[$"color{i}a"].Value)
                                        };
                                    }
                                    part.hideCategory = XmlConvert.ToInt32(accessoryNode.Attributes["hideCategory"].Value);
#if EC
                                    if (accessoryNode.Attributes["hideTiming"] != null)
                                        part.hideTiming = XmlConvert.ToInt32(accessoryNode.Attributes["hideTiming"].Value);
#endif
                                    if (MoreAccessories._hasDarkness)
                                        part.SetPrivateProperty("noShake", accessoryNode.Attributes["noShake"] != null && XmlConvert.ToBoolean(accessoryNode.Attributes["noShake"].Value));
                                }
                                parts.Add(part);
                            }
                            break;
#if KK || KKS
                        case "visibility":
                            if (MoreAccessories.InStudio)
                            {
                                showAccessories = new List<bool>();
                                foreach (XmlNode grandChildNode in childNode.ChildNodes)
                                    showAccessories.Add(grandChildNode.Attributes?["value"] == null || XmlConvert.ToBoolean(grandChildNode.Attributes["value"].Value));
                            }
                            break;
#endif
                        default: break;
                    }
                }
            }
        }


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
            if (MoreAccessories.InStudio)
                showAccessories = file.status.showAccessory.Skip(20).ToList();
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
