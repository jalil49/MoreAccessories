using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using BepInEx;
using BepInEx.Logging;
using ExtensibleSaveFormat;
using HarmonyLib;
using Manager;
#if KK || KKS
using Studio;
#endif
using MoreAccessoriesKOI.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

namespace MoreAccessoriesKOI
{
    [BepInPlugin(GUID: GUID, Name: "MoreAccessories", Version: versionNum)]
    [BepInDependency(ExtendedSave.GUID)]
    [BepInDependency(Sideloader.Sideloader.GUID)]
    public partial class MoreAccessories : BaseUnityPlugin
    {
        #region Unity Methods
        private void Awake()
        {
            _self = this;
#if KKS
            ExtendedSave.CardBeingImported += MultiCoord_CardBeingImported;
#elif EC
            ExtendedSave.CardBeingImported += SingleCoord_CardBeingImported; ;
#endif
            SceneManager.sceneLoaded += LevelLoaded;

            _hasDarkness = typeof(ChaControl).GetMethod("ChangeShakeAccessory", AccessTools.all) != null;
            var harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            var uarHooks = typeof(Sideloader.AutoResolver.UniversalAutoResolver).GetNestedType("Hooks", AccessTools.all);
            harmony.Patch(uarHooks.GetMethod("ExtendedCardLoad", AccessTools.all), new HarmonyMethod(typeof(MoreAccessories), nameof(UAR_ExtendedCardLoad_Prefix)));
            harmony.Patch(uarHooks.GetMethod("ExtendedCoordinateLoad", AccessTools.all), new HarmonyMethod(typeof(MoreAccessories), nameof(UAR_ExtendedCoordLoad_Prefix)));
            SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
#if DEBUG
            foreach (var item in harmony.GetPatchedMethods())
            {
                Print($"{item.ReflectedType}.{item.Name}");
            }
#endif
            ExtendedSave.CardBeingSaved += OnActualCharaSave;
            ExtendedSave.CoordinateBeingSaved += OnActualCoordSave;
        }

        private void SceneManager_sceneUnloaded(Scene arg0)
        {
            switch (arg0.name)
            {
                case "CustomScene":
                    MakerMode = null;
                    break;
                default:
                    break;
            }
        }

        internal static void Print(string text, LogLevel logLevel = LogLevel.Warning)
        {
            _self.Logger.Log(logLevel, text);
        }

        public bool InFreeHSelect { get; private set; }
        public bool AtMenu { get; private set; }

#if KKS
        private void MultiCoord_CardBeingImported(Dictionary<string, PluginData> importedExtendedData, Dictionary<int, int?> coordinateMapping)
        {
            if (!importedExtendedData.TryGetValue(_extSaveKey, out var pluginData) || pluginData == null || !pluginData.data.TryGetValue("additionalAccessories", out var xmlData) || xmlData == null) return; //new version doesn't have anything but version number

            var data = new CharAdditionalData();
            var doc = new XmlDocument();
            doc.LoadXml((string)xmlData);
            var node = doc.FirstChild;

            foreach (XmlNode childNode in node.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "accessorySet":
                        var coordinateType = XmlConvert.ToInt32(childNode.Attributes["type"].Value);
                        List<ChaFileAccessory.PartsInfo> parts;

                        if (data.rawAccessoriesInfos.TryGetValue(coordinateType, out parts) == false)
                        {
                            parts = new List<ChaFileAccessory.PartsInfo>();
                            data.rawAccessoriesInfos.Add(coordinateType, parts);
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
                                if (_hasDarkness)
                                    part.noShake = accessoryNode.Attributes["noShake"] != null && XmlConvert.ToBoolean(accessoryNode.Attributes["noShake"].Value);
                            }
                            parts.Add(part);
                        }
                        break;
                    default: break;
                }
            }

            var dict = data.rawAccessoriesInfos;
            var transferdict = new Dictionary<int, List<ChaFileAccessory.PartsInfo>>();
            foreach (var item in coordinateMapping)
            {
                if (!dict.TryGetValue(item.Key, out var list) || list == null || !item.Value.HasValue) continue;
                transferdict[item.Value.Value] = list;
            }
            data.rawAccessoriesInfos = transferdict;
            using (var stringWriter = new StringWriter())
            using (var xmlWriter = new XmlTextWriter(stringWriter))
            {
                var maxCount = 0;
                xmlWriter.WriteStartElement("additionalAccessories");
                xmlWriter.WriteAttributeString("version", versionNum);
                foreach (var pair in data.rawAccessoriesInfos)
                {
                    if (pair.Value.Count == 0)
                        continue;
                    xmlWriter.WriteStartElement("accessorySet");
                    xmlWriter.WriteAttributeString("type", XmlConvert.ToString(pair.Key));
                    if (maxCount < pair.Value.Count)
                        maxCount = pair.Value.Count;

                    for (var index = 0; index < pair.Value.Count; index++)
                    {
                        var part = pair.Value[index];
                        xmlWriter.WriteStartElement("accessory");
                        xmlWriter.WriteAttributeString("type", XmlConvert.ToString(part.type));

                        if (part.type != 120)
                        {
                            xmlWriter.WriteAttributeString("id", XmlConvert.ToString(part.id));
                            xmlWriter.WriteAttributeString("parentKey", part.parentKey);

                            for (var i = 0; i < 2; i++)
                            {
                                for (var j = 0; j < 3; j++)
                                {
                                    var v = part.addMove[i, j];
                                    xmlWriter.WriteAttributeString($"addMove{i}{j}x", XmlConvert.ToString(v.x));
                                    xmlWriter.WriteAttributeString($"addMove{i}{j}y", XmlConvert.ToString(v.y));
                                    xmlWriter.WriteAttributeString($"addMove{i}{j}z", XmlConvert.ToString(v.z));
                                }
                            }
                            for (var i = 0; i < 4; i++)
                            {
                                var c = part.color[i];
                                xmlWriter.WriteAttributeString($"color{i}r", XmlConvert.ToString(c.r));
                                xmlWriter.WriteAttributeString($"color{i}g", XmlConvert.ToString(c.g));
                                xmlWriter.WriteAttributeString($"color{i}b", XmlConvert.ToString(c.b));
                                xmlWriter.WriteAttributeString($"color{i}a", XmlConvert.ToString(c.a));
                            }
                            xmlWriter.WriteAttributeString("hideCategory", XmlConvert.ToString(part.hideCategory));
                            if (_hasDarkness)
                                xmlWriter.WriteAttributeString("noShake", XmlConvert.ToString(part.noShake));
                        }
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                }

#if KK || KKS
                if (StudioMode != null)
                {
                    xmlWriter.WriteStartElement("visibility");
                    for (var i = 0; i < maxCount && i < data.showAccessories.Count; i++)
                    {
                        xmlWriter.WriteStartElement("visible");
                        xmlWriter.WriteAttributeString("value", XmlConvert.ToString(data.showAccessories[i]));
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                }
#endif

                xmlWriter.WriteEndElement();

                pluginData.version = 1;//use old version cause it is
                pluginData.data["additionalAccessories"] = stringWriter.ToString();
            }
        }
#elif EC
        private void SingleCoord_CardBeingImported(Dictionary<string, PluginData> importedExtendedData)
        {
            if (!importedExtendedData.TryGetValue(_extSaveKey, out var pluginData) || pluginData == null || !pluginData.data.TryGetValue("additionalAccessories", out var xmlData)) return; //new version doesn't have anything but version number

            var data = new CharAdditionalData();
            var doc = new XmlDocument();
            doc.LoadXml((string)xmlData);
            var node = doc.FirstChild;

            foreach (XmlNode childNode in node.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "accessorySet":
                        var coordinateType = XmlConvert.ToInt32(childNode.Attributes["type"].Value);
                        List<ChaFileAccessory.PartsInfo> parts;

                        if (data.rawAccessoriesInfos.TryGetValue(coordinateType, out parts) == false)
                        {
                            parts = new List<ChaFileAccessory.PartsInfo>();
                            data.rawAccessoriesInfos.Add(coordinateType, parts);
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
                                if (accessoryNode.Attributes["hideTiming"] != null)
                                    part.hideTiming = XmlConvert.ToInt32(accessoryNode.Attributes["hideTiming"].Value);
                                if (_hasDarkness)
                                    part.noShake = accessoryNode.Attributes["noShake"] != null && XmlConvert.ToBoolean(accessoryNode.Attributes["noShake"].Value);
                            }
                            parts.Add(part);
                        }
                        break;
                    default: break;
                }
            }

            var dict = data.rawAccessoriesInfos;
            var transferdict = new Dictionary<int, List<ChaFileAccessory.PartsInfo>>();
            if (dict.TryGetValue(0, out var list))
            {
                transferdict[0] = list;
            }
            data.rawAccessoriesInfos = transferdict;
            using (var stringWriter = new StringWriter())
            using (var xmlWriter = new XmlTextWriter(stringWriter))
            {
                var maxCount = 0;
                xmlWriter.WriteStartElement("additionalAccessories");
                xmlWriter.WriteAttributeString("version", versionNum);
                foreach (var pair in data.rawAccessoriesInfos)
                {
                    if (pair.Value.Count == 0)
                        continue;
                    xmlWriter.WriteStartElement("accessorySet");
                    xmlWriter.WriteAttributeString("type", XmlConvert.ToString(pair.Key));
                    if (maxCount < pair.Value.Count)
                        maxCount = pair.Value.Count;

                    for (var index = 0; index < pair.Value.Count; index++)
                    {
                        var part = pair.Value[index];
                        xmlWriter.WriteStartElement("accessory");
                        xmlWriter.WriteAttributeString("type", XmlConvert.ToString(part.type));

                        if (part.type != 120)
                        {
                            xmlWriter.WriteAttributeString("id", XmlConvert.ToString(part.id));
                            xmlWriter.WriteAttributeString("parentKey", part.parentKey);

                            for (var i = 0; i < 2; i++)
                            {
                                for (var j = 0; j < 3; j++)
                                {
                                    var v = part.addMove[i, j];
                                    xmlWriter.WriteAttributeString($"addMove{i}{j}x", XmlConvert.ToString(v.x));
                                    xmlWriter.WriteAttributeString($"addMove{i}{j}y", XmlConvert.ToString(v.y));
                                    xmlWriter.WriteAttributeString($"addMove{i}{j}z", XmlConvert.ToString(v.z));
                                }
                            }
                            for (var i = 0; i < 4; i++)
                            {
                                var c = part.color[i];
                                xmlWriter.WriteAttributeString($"color{i}r", XmlConvert.ToString(c.r));
                                xmlWriter.WriteAttributeString($"color{i}g", XmlConvert.ToString(c.g));
                                xmlWriter.WriteAttributeString($"color{i}b", XmlConvert.ToString(c.b));
                                xmlWriter.WriteAttributeString($"color{i}a", XmlConvert.ToString(c.a));
                            }
                            xmlWriter.WriteAttributeString("hideCategory", XmlConvert.ToString(part.hideCategory));
                            xmlWriter.WriteAttributeString("hideTiming", XmlConvert.ToString(part.hideTiming));
                            if (_hasDarkness)
                                xmlWriter.WriteAttributeString("noShake", XmlConvert.ToString(part.noShake));
                        }
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                }
                xmlWriter.WriteEndElement();

                pluginData.version = 1;
                pluginData.data["additionalAccessories"] = stringWriter.ToString();
            }

        }
#endif
        private void LevelLoaded(Scene scene, LoadSceneMode loadMode)
        {
            var instudio = false;

#if KK || KKS
            instudio = Application.productName.StartsWith("CharaStudio");
#endif
#if DEBUG
            Print($"loadmode {loadMode} index {scene.buildIndex} ");
#endif
            switch (loadMode)
            {
                case LoadSceneMode.Single:
                    if (!instudio)
                    {
                        MakerMode = null;
                        ImportingCards = false;
                        InFreeHSelect = false;
                        AtMenu = false;
#if KK || KKS
                        HMode = null;
#elif EC
                        PlayMode = null;
#endif
                        switch (scene.buildIndex)
                        {
                            case 1: //converted
                                ImportingCards = true;
                                break;

                            //Chara maker
#if KK
                            case 2:
#elif KKS || EC
                            case 3: //sunshine uses 3 for chara
#endif
                                MakerMode = new MakerMode();
                                break;

#if KK || KKS
                            case 7: //Hscenes
                                break;


                            case 11: //freeh select
                                InFreeHSelect = true;
                                break;
                            case 4: //menu
                                AtMenu = true;
                                break;
                            default:
                                break;
#endif
                        }
                    }
#if KK || KKS
                    else
                    {
#if KK
                        if (scene.buildIndex == 1) //Studio
#elif KKS
                        if (scene.buildIndex == 2) //Studio
#endif
                        {
                            StudioMode = new StudioClass();
                        }
                        else
                            StudioMode = null;
                    }
#endif
                    break;
#if !EC
                case LoadSceneMode.Additive:

#if KKS
                    if (Game.initialized && scene.buildIndex == 3) //Class chara maker
#elif KK
                    if (Game.IsInstance() && scene.buildIndex == 2) //Class chara maker
#endif
                    {
                        MakerMode = new MakerMode();
                    }
                    else
                    {
                        MakerMode = null;
                    }
                    break;
#endif
            }
        }

        internal void Update()
        {
#if KK || KKS
            if (InStudio)
            {
                var treeNodeObject = Studio.Studio.Instance.treeNodeCtrl.selectNode;
                if (treeNodeObject != null)
                {
                    if (Studio.Studio.Instance.dicInfo.TryGetValue(treeNodeObject, out var info))
                    {
                        var selected = info as OCIChar;
                        if (selected != StudioMode._selectedStudioCharacter)
                        {
                            StudioMode._selectedStudioCharacter = selected;
                            StudioMode.UpdateStudioUI();
                        }
                    }
                }
            }
#endif
        }

        //private void LateUpdate()
        //{
        //    if (_inCharaMaker && _customAcsChangeSlot != null)
        //    {
        //        Transform t;
        //        if (CustomBase.Instance.selectSlot < 20)
        //            t = _customAcsChangeSlot.items[CustomBase.Instance.selectSlot].cgItem.transform;
        //        else
        //            t = _additionalCharaMakerSlots[CustomBase.Instance.selectSlot - 20].canvasGroup.transform;
        //        t.position = new Vector3(t.position.x, _slotUIPositionY);
        //    }
        //}
        #endregion

        #region Private Methods
        internal void UpdateUI()
        {
            if (MakerMode != null)
                MakerMode.UpdateMakerUI();
#if KK || KKS
            else if (StudioMode != null)
                StudioMode.UpdateStudioUI();
            else if (InH)
                this.ExecuteDelayed(HMode.UpdateHUI);
#elif EC
            else if (InPlayMode)
                PlayMode.UpdatePlayUI();
#endif
        }

        #endregion

        private static void UAR_ExtendedCardLoad_Prefix(ChaFile file)
        {
            _self.OnActualCharaLoad(file);
        }

        private static void UAR_ExtendedCoordLoad_Prefix(ChaFileCoordinate file)
        {
            _self.OnActualCoordLoad(file);
        }


        #region Saves
        internal void OnActualCharaLoad(ChaFile file)
        {
            PreviousMigratedData = null;
#if KK || KKS
            if (file.coordinate.Any(x => x.accessory.parts.Length > 20))
            {
                //Print($"{file.parameter.fullname} has an array larger than 20");
                return;
            }
#else
            if (file.coordinate.accessory.parts.Length > 20)
                return;
#endif
            // Print($"Loading Data for {file.parameter.fullname}");
            var pluginData = ExtendedSave.GetExtendedDataById(file, _extSaveKey);
            if (pluginData == null)
            {
                //Print("Plugin Data Null", LogLevel.Error);
                return;
            }
            PreviousMigratedData = new CharAdditionalData();

            XmlNode node = null;
            if (pluginData != null && pluginData.data.TryGetValue("additionalAccessories", out var xmlData))
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

                            if (PreviousMigratedData.rawAccessoriesInfos.TryGetValue(coordinateType, out parts) == false)
                            {
                                parts = new List<ChaFileAccessory.PartsInfo>();
                                PreviousMigratedData.rawAccessoriesInfos.Add(coordinateType, parts);
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
                                    if (_hasDarkness)
                                        part.SetPrivateProperty("noShake", accessoryNode.Attributes["noShake"] != null && XmlConvert.ToBoolean(accessoryNode.Attributes["noShake"].Value));
                                }
                                parts.Add(part);
                            }
                            break;
                        default: break;
                    }
                }
            }

            //Print($"Plugin Data has {PreviousMigratedData.rawAccessoriesInfos.Count} and version {pluginData.version}", LogLevel.Error);

            foreach (var item in PreviousMigratedData.rawAccessoriesInfos)
            {
                //Print($"raw data has key {item.Key}");
#if KK || KKS
                if (!(item.Key < file.coordinate.Length)) continue;

                var accessory = file.coordinate[item.Key].accessory;
#else
                var accessory = file.coordinate.accessory;
#endif
                accessory.parts = accessory.parts.Concat(item.Value).ToArray();
                //Print($"Settings coordinate {item.Key}");
            }


            if (
#if KK || KKS
                    InH ||
#endif
                    CharaMaker
            )
                this.ExecuteDelayed(UpdateUI);
            else
                UpdateUI();
        }

        private void OnActualCharaSave(ChaFile file)
        {
            if (ImportingCards) return;
            var pluginData = new PluginData { version = _saveVersion };
#if false

            var data = new CharAdditionalData(file);
            using (var stringWriter = new StringWriter())
            using (var xmlWriter = new XmlTextWriter(stringWriter))
            {
                var maxCount = 0;
                xmlWriter.WriteStartElement("additionalAccessories");
                xmlWriter.WriteAttributeString("version", versionNum);
                foreach (var pair in data.rawAccessoriesInfos)
                {
                    if (pair.Value.Count == 0)
                        continue;
                    xmlWriter.WriteStartElement("accessorySet");
                    xmlWriter.WriteAttributeString("type", XmlConvert.ToString(pair.Key));
                    if (maxCount < pair.Value.Count)
                        maxCount = pair.Value.Count;

                    for (var index = 0; index < pair.Value.Count; index++)
                    {
                        var part = pair.Value[index];
                        xmlWriter.WriteStartElement("accessory");
                        xmlWriter.WriteAttributeString("type", XmlConvert.ToString(part.type));

                        if (part.type != 120)
                        {
                            xmlWriter.WriteAttributeString("id", XmlConvert.ToString(part.id));
                            xmlWriter.WriteAttributeString("parentKey", part.parentKey);

                            for (var i = 0; i < 2; i++)
                            {
                                for (var j = 0; j < 3; j++)
                                {
                                    var v = part.addMove[i, j];
                                    xmlWriter.WriteAttributeString($"addMove{i}{j}x", XmlConvert.ToString(v.x));
                                    xmlWriter.WriteAttributeString($"addMove{i}{j}y", XmlConvert.ToString(v.y));
                                    xmlWriter.WriteAttributeString($"addMove{i}{j}z", XmlConvert.ToString(v.z));
                                }
                            }
                            for (var i = 0; i < 4; i++)
                            {
                                var c = part.color[i];
                                xmlWriter.WriteAttributeString($"color{i}r", XmlConvert.ToString(c.r));
                                xmlWriter.WriteAttributeString($"color{i}g", XmlConvert.ToString(c.g));
                                xmlWriter.WriteAttributeString($"color{i}b", XmlConvert.ToString(c.b));
                                xmlWriter.WriteAttributeString($"color{i}a", XmlConvert.ToString(c.a));
                            }
                            xmlWriter.WriteAttributeString("hideCategory", XmlConvert.ToString(part.hideCategory));
#if EC
                            xmlWriter.WriteAttributeString("hideTiming", XmlConvert.ToString(part.hideTiming));
#endif
                            if (_hasDarkness)
                                xmlWriter.WriteAttributeString("noShake", XmlConvert.ToString(part.noShake));
                        }
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();

                }

#if KK || KKS
                if (StudioMode != null)
                {
                    xmlWriter.WriteStartElement("visibility");
                    for (var i = 0; i < maxCount && i < data.showAccessories.Count; i++)
                    {
                        xmlWriter.WriteStartElement("visible");
                        xmlWriter.WriteAttributeString("value", XmlConvert.ToString(data.showAccessories[i]));
                        xmlWriter.WriteEndElement();
                    }
                    xmlWriter.WriteEndElement();
                }
#endif

                xmlWriter.WriteEndElement();

                pluginData.data.Add("additionalAccessories", stringWriter.ToString());
                ExtendedSave.SetExtendedDataById(file, _extSaveKey, pluginData);
            }
#else
            ExtendedSave.SetExtendedDataById(file, _extSaveKey, pluginData);
#endif
        }

        internal void OnActualCoordLoad(ChaFileCoordinate file)
        {
            PreviousMigratedData = null;

            var pluginData = ExtendedSave.GetExtendedDataById(file, _extSaveKey);

            if (file.accessory.parts.Length > 20) //escape data is already saved directly on card 
            {
                return;
            }

            PreviousMigratedData = new CharAdditionalData();

            XmlNode node = null;
            if (pluginData != null && pluginData.data.TryGetValue("additionalAccessories", out var xmlData))
            {
                var doc = new XmlDocument();
                doc.LoadXml((string)xmlData);
                node = doc.FirstChild;
            }
            if (node != null)
            {
                foreach (XmlNode accessoryNode in node.ChildNodes)
                {
                    var part = new ChaFileAccessory.PartsInfo
                    {
                        type = XmlConvert.ToInt32(accessoryNode.Attributes["type"].Value)
                    };
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
                        if (_hasDarkness)
                            part.SetPrivateProperty("noShake", accessoryNode.Attributes["noShake"] != null && XmlConvert.ToBoolean(accessoryNode.Attributes["noShake"].Value));
                    }
                    PreviousMigratedData.nowAccessories.Add(part);
                }
            }

            file.accessory.parts = file.accessory.parts.Concat(PreviousMigratedData.nowAccessories).ToArray();

            if (
#if KK || KKS
                    InH ||
#endif
                    CharaMaker
            )
                this.ExecuteDelayed(UpdateUI);
            else
                UpdateUI();
        }

        private void OnActualCoordSave(ChaFileCoordinate file)
        {
#if false
            var data = new CharAdditionalData(CustomBase.instance.chaCtrl);

            using (var stringWriter = new StringWriter())
            using (var xmlWriter = new XmlTextWriter(stringWriter))
            {
                xmlWriter.WriteStartElement("additionalAccessories");
                xmlWriter.WriteAttributeString("version", versionNum);
                foreach (var part in data.nowAccessories)
                {
                    xmlWriter.WriteStartElement("accessory");
                    xmlWriter.WriteAttributeString("type", XmlConvert.ToString(part.type));
                    if (part.type != 120)
                    {
                        xmlWriter.WriteAttributeString("id", XmlConvert.ToString(part.id));
                        xmlWriter.WriteAttributeString("parentKey", part.parentKey);
                        for (var i = 0; i < 2; i++)
                        {
                            for (var j = 0; j < 3; j++)
                            {
                                var v = part.addMove[i, j];
                                xmlWriter.WriteAttributeString($"addMove{i}{j}x", XmlConvert.ToString(v.x));
                                xmlWriter.WriteAttributeString($"addMove{i}{j}y", XmlConvert.ToString(v.y));
                                xmlWriter.WriteAttributeString($"addMove{i}{j}z", XmlConvert.ToString(v.z));
                            }
                        }
                        for (var i = 0; i < 4; i++)
                        {
                            var c = part.color[i];
                            xmlWriter.WriteAttributeString($"color{i}r", XmlConvert.ToString(c.r));
                            xmlWriter.WriteAttributeString($"color{i}g", XmlConvert.ToString(c.g));
                            xmlWriter.WriteAttributeString($"color{i}b", XmlConvert.ToString(c.b));
                            xmlWriter.WriteAttributeString($"color{i}a", XmlConvert.ToString(c.a));
                        }
                        xmlWriter.WriteAttributeString("hideCategory", XmlConvert.ToString(part.hideCategory));
#if EC
                        xmlWriter.WriteAttributeString("hideTiming", XmlConvert.ToString(part.hideTiming));
#endif
                        if (_hasDarkness)
                            xmlWriter.WriteAttributeString("noShake", XmlConvert.ToString(part.noShake));
                    }
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();

                var pluginData = new PluginData { version = _saveVersion };
                pluginData.data.Add("additionalAccessories", stringWriter.ToString());
                ExtendedSave.SetExtendedDataById(file, _extSaveKey, pluginData);
            }
#endif
            var pluginData = new PluginData
            {
                version = _saveVersion
            };
            ExtendedSave.SetExtendedDataById(file, _extSaveKey, pluginData);
        }
        #endregion
    }
}
