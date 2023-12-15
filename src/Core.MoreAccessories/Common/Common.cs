using System;
using System.Linq;
using MoreAccessoriesKOI.Patches;
using UnityEngine;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public static void ManuallyUpdateUI() => _self.UpdateUI();

        /// <summary>
        /// Sync the Arrays of the ChaControl so that the now coordinate size is reflected in relevant arrays.
        /// In maker: syncs to the largest array size
        /// </summary>
        /// <param name="controller"> The Chacontrol to be adjusted</param>
        public static void ArraySync(ChaControl controller)
        {
            Common_Patches.Seal(false);
            if (controller.nowCoordinate == null)
            {
                return;
            }

            var maxAccessoryLength = MaxAccessoryCount(controller);
            UpdateShowAccessoryArray(controller, maxAccessoryLength);

            UpdateObjectAccessoryArray(controller, maxAccessoryLength);

            UpdateChaAccessoryComponentArray(controller, maxAccessoryLength);

            UpdateHideHairAccessoriesArray(controller, maxAccessoryLength);

            UpdateInfoAccessoryArray(controller, maxAccessoryLength);

            UpdateObjAcsMoveArray(controller, maxAccessoryLength);

            if (CharaMaker)
            {
                var accessoryParts = controller.nowCoordinate.accessory.parts;
                var currentCoordinateLength = accessoryParts.Length;
                for (var i = 0; i < currentCoordinateLength; i++)
                {
                    var part = accessoryParts[i];
                    if (part.type != 120)
                    {
#if KKS
                        controller.infoAccessory[i] = controller.lstCtrl.GetInfo((ChaListDefine.CategoryNo)part.type, part.id);
#else
                        controller.infoAccessory[i] = controller.lstCtrl.GetListInfo((ChaListDefine.CategoryNo)part.type, part.id);
#endif
                    }
                }

                var delta = maxAccessoryLength - accessoryParts.Length;
                if (delta > 0)
                {
                    var partsArray = new ChaFileAccessory.PartsInfo[delta];
                    for (var i = 0; i < delta; i++)
                    {
                        partsArray[i] = new ChaFileAccessory.PartsInfo();
                    }

                    controller.nowCoordinate.accessory.parts = accessoryParts.Concat(partsArray).ToArray();
                }
                else if (delta < 0)
                {
                    controller.nowCoordinate.accessory.parts = accessoryParts.Take(maxAccessoryLength).ToArray();
                }

#if KK || KKS
                foreach (var item in controller.chaFile.coordinate)
                {
                    delta = maxAccessoryLength - item.accessory.parts.Length;
                    if (delta > 0)
                    {
                        var array = new ChaFileAccessory.PartsInfo[delta];
                        for (var i = 0; i < delta; i++)
                        {
                            array[i] = new ChaFileAccessory.PartsInfo();
                        }

                        item.accessory.parts = item.accessory.parts.Concat(array).ToArray();
                    }
                    else if (delta < 0)
                    {
                        item.accessory.parts = item.accessory.parts.Take(maxAccessoryLength).ToArray();
                    }
                }
#elif EC
                delta = maxAccessoryLength - controller.chaFile.coordinate.accessory.parts.Length;
                if (delta > 0)
                {
                    var array = new ChaFileAccessory.PartsInfo[delta];
                    for (var i = 0; i < delta; i++) { array[i] = new ChaFileAccessory.PartsInfo(); }
                    controller.chaFile.coordinate.accessory.parts = controller.chaFile.coordinate.accessory.parts.Concat(array).ToArray();
                }
                else if (delta < 0)
                {
                    controller.chaFile.coordinate.accessory.parts = controller.chaFile.coordinate.accessory.parts.Take(maxAccessoryLength).ToArray();
                }
#endif
                MakerMode.RefreshToggles(maxAccessoryLength);
            }

            Common_Patches.Seal(true);
        }

        internal static int MaxAccessoryCount(ChaControl controller)
        {
            var maxAccessoryLength = controller.nowCoordinate.accessory.parts.Length;

#if KK || KKS
            if (CharaMaker)
            {
                foreach (var item in controller.chaFile.coordinate)
                {
                    maxAccessoryLength = System.Math.Max(maxAccessoryLength, item.accessory.parts.Length);
                }
            }
#endif
            return maxAccessoryLength;
        }

        private static void UpdateShowAccessoryArray(ChaControl controller, int len)
        {
            var delta = len - controller.fileStatus.showAccessory.Length;
            if (delta > 0)
            {
                var newArray = new bool[delta];
                for (var i = 0; i < delta; i++)
                {
                    newArray[i] = true;
                }

                controller.fileStatus.showAccessory = controller.fileStatus.showAccessory.Concat(newArray).ToArray();
            }
            else if (delta < 0)
            {
                controller.fileStatus.showAccessory = controller.fileStatus.showAccessory.Take(len).ToArray();
            }
        }

        private static void UpdateObjectAccessoryArray(ChaControl controller, int len)
        {
            var delta = len - controller.objAccessory.Length;
            if (delta > 0)
            {
                controller.objAccessory = controller.objAccessory.Concat(new GameObject[delta]).ToArray();
            }
            else if (delta < 0)
            {
                for (var i = len; i < controller.objAccessory.Length; i++)
                {
                    var obj = controller.objAccessory[i];
                    if (obj)
                    {
                        controller.SafeDestroy(obj);
                    }
                }

                controller.objAccessory = controller.objAccessory.Take(len).ToArray();
            }
        }

        private static void UpdateChaAccessoryComponentArray(ChaControl controller, int len)
        {
            var delta = len - controller.cusAcsCmp.Length;
            if (delta > 0)
            {
                controller.cusAcsCmp = controller.cusAcsCmp.Concat(new ChaAccessoryComponent[delta]).ToArray();
            }
            else if (delta < 0)
            {
                controller.cusAcsCmp = controller.cusAcsCmp.Take(len).ToArray();
            }
        }

        private static void UpdateHideHairAccessoriesArray(ChaControl controller, int len)
        {
            var delta = len - controller.hideHairAcs.Length;
            if (delta > 0)
            {
                controller.hideHairAcs = controller.hideHairAcs.Concat(new bool[delta]).ToArray();
            }
            else if (delta < 0)
            {
                controller.hideHairAcs = controller.hideHairAcs.Take(len).ToArray();
            }
        }

        private static void UpdateInfoAccessoryArray(ChaControl controller, int len)
        {
            var delta = len - controller.infoAccessory.Length;
            if (delta > 0)
            {
                controller.infoAccessory = controller.infoAccessory.Concat(new ListInfoBase[delta]).ToArray();
            }
            else if (delta < 0)
            {
                controller.infoAccessory = controller.infoAccessory.Take(len).ToArray();
            }
        }

        private static void UpdateObjAcsMoveArray(ChaControl controller, int len)
        {
            var moveLength = controller.objAcsMove.GetLength(0);
            var delta = len - moveLength;
            if (delta > 0)
            {
                var newArray = new GameObject[len, 2];
                for (var i = 0; i < moveLength; i++)
                {
                    for (var j = 0; j < 2; j++)
                    {
                        newArray[i, j] = controller.objAcsMove[i, j];
                    }
                }

                controller.objAcsMove = newArray;
            }
            else if (delta < 0)
            {
                var newArray = new GameObject[len, 2];
                for (var i = 0; i < len; i++)
                {
                    for (var j = 0; j < 2; j++)
                    {
                        newArray[i, j] = controller.objAcsMove[i, j];
                    }
                }

                controller.objAcsMove = newArray;
            }
        }

        /// <summary>
        /// Sync the show accessory array with parts array.
        /// </summary>
        /// <param name="file"></param>
        internal static void ArraySync(ChaFile file)
        {
            Common_Patches.Seal(false);
#if KK || KKS
            var maxAccessoryLength = file.coordinate[file.status.coordinateType].accessory.parts.Length;
#elif EC
            var maxAccessoryLength = file.coordinate.accessory.parts.Length;
#endif
            //Print($"Max size in sync is {len}");
            var delta = maxAccessoryLength - file.status.showAccessory.Length;
            if (delta > 0)
            {
                var newArray = new bool[delta];
                for (var i = 0; i < delta; i++)
                {
                    newArray[i] = true;
                }

                file.status.showAccessory = file.status.showAccessory.Concat(newArray).ToArray();
            }
            else if (delta < 0)
            {
                file.status.showAccessory = file.status.showAccessory.Take(maxAccessoryLength).ToArray();
            }

            Common_Patches.Seal(true);
        }

        internal static void NowCoordinateTrimAndSync(ChaControl controller)
        {
            if (controller == null) return;

            var index = Math.Max(20, Array.FindLastIndex(controller.nowCoordinate.accessory.parts, x => x.type != 120) + 1);
            controller.nowCoordinate.accessory.parts = controller.nowCoordinate.accessory.parts.Take(index).ToArray();
            ArraySync(controller);
        }

        /// <summary>
        /// Turns off BackwardsCompatibility save;
        /// </summary>
        public static void TurnOffBackwardsCompatibility()
        {
            _backwardCompatibility = false;
        }
    }
}