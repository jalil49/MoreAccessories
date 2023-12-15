#pragma warning disable CS0618 // Type or member is obsolete
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml;
using ExtensibleSaveFormat;
using JetBrains.Annotations;
using MoreAccessoriesKOI.Extensions;
using UnityEngine;
using UnityEngine.UI;
#if EC
using TMPro;
#pragma warning disable CS0618 // Type or member is obsolete
#endif

namespace MoreAccessoriesKOI
{
    public class CharAdditionalData
    {
        public CharAdditionalData() { }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
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

                            if (rawAccessoriesInfos.TryGetValue(coordinateType, out var parts) == false)
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
                                    if (MoreAccessories.HasDarkness)
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
        public CharAdditionalData(ChaControl chaControl)
        {
            nowAccessories = chaControl.nowCoordinate.accessory.parts.Skip(20).ToList();
            for (var i = 0; i < chaControl.chaFile.coordinate.Length; i++)
            {
                rawAccessoriesInfos[i] = chaControl.chaFile.coordinate[i].accessory.parts.Skip(20).ToList();
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
        [Obsolete("Directly on character")]
        public CharAdditionalData(ChaControl chaControl)
        {
            nowAccessories = chaControl.nowCoordinate.accessory.parts.Skip(20).ToList();
            rawAccessoriesInfos[0] = chaControl.chaFile.coordinate.accessory.parts.Skip(20).ToList();
        }

        [Obsolete("Directly on character")]
        public CharAdditionalData(ChaFile file)
        {
            rawAccessoriesInfos[0] = file.coordinate.accessory.parts.Skip(20).ToList();
        }
#endif
        [Obsolete("Directly on character", true)]
        public CharAdditionalData(ChaFileAccessory.PartsInfo[] parts)
        {
            nowAccessories = parts.Skip(20).ToList();
        }

        [Obsolete("Directly on character")] [PublicAPI]
        // ReSharper disable once InconsistentNaming
        public List<ChaFileAccessory.PartsInfo> nowAccessories = new List<ChaFileAccessory.PartsInfo>();

        [PublicAPI] [Obsolete("Directly on character")]
        // ReSharper disable once InconsistentNaming
        internal List<bool> showAccessories = new List<bool>();

#if EC
        [PublicAPI]
        // ReSharper disable once InconsistentNaming
        public List<int> advState = new List<int>();
#endif
        // ReSharper disable once InconsistentNaming
        [Obsolete("Unused directly stored on Accessory.parts")] [PublicAPI]
        public Dictionary<int, List<ChaFileAccessory.PartsInfo>> rawAccessoriesInfos = new Dictionary<int, List<ChaFileAccessory.PartsInfo>>();
    }

    [PublicAPI]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class CharaMakerSlotData
    {
        public GameObject AccessorySlot;
#if KK || KKS
        public GameObject copySlotObject;
#endif
        public GameObject transferSlotObject;
    }

#if KK || KKS
    [PublicAPI]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class StudioSlotData
    {
        public RectTransform slot;
        public Text name;
        public Button onButton;
        public Button offButton;
    }
#elif EC
    [PublicAPI]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PlaySceneSlotData
    {
        public RectTransform slot;
        public TextMeshProUGUI text;
        public Button button;
    }

    [PublicAPI]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ADVSceneSlotData
    {
        public RectTransform slot;
        public TextMeshProUGUI text;
        public Toggle keep;
        public Toggle wear;
        public Toggle takeOff;
    }
#endif
}