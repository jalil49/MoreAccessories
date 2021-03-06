using Illusion.Extensions;
using System.Linq;
using UniRx;
using UnityEngine;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {

        public static void ManuallyUpdateUI() => _self.UpdateUI();

        /// <summary>
        /// Sync the Arrays of the ChaControl so that the nowcoordinate size is reflected in revelent arrays.
        /// In maker: syncs to the largest array size
        /// </summary>
        /// <param name="controller"> The Chacontrol to be adjusted</param>
        public static void ArraySync(ChaControl controller)
        {
            Patches.Common_Patches.Seal(false);
            if (controller.nowCoordinate == null)
            {
                return;
            }
            var parts = controller.nowCoordinate.accessory.parts;
            var len = parts.Length;
            var nowlength = len;
            //Print($"Syncng {controller.chaFile.parameter.fullname} to {len}");

#if KK || KKS
            //Print($"Nowlength is {nowlength}");
            if (CharaMaker)
            {
                foreach (var item in controller.chaFile.coordinate)
                {
                    len = System.Math.Max(len, item.accessory.parts.Length);
                    //Print($"coordinate length is {item.accessory.parts.Length}");
                }
            }
#endif
            //Print($"Max size in sync is {len}");
            var delta = len - controller.fileStatus.showAccessory.Length;
            if (delta > 0)
            {
                var newarray = new bool[delta];
                for (var i = 0; i < delta; i++)
                {
                    newarray[i] = true;
                }
                controller.fileStatus.showAccessory = controller.fileStatus.showAccessory.Concat(newarray).ToArray();
            }
            else if (delta < 0)
            {
                controller.fileStatus.showAccessory = controller.fileStatus.showAccessory.Take(len).ToArray();
            }


            delta = len - controller.objAccessory.Length;
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

            delta = len - controller.cusAcsCmp.Length;
            if (delta > 0)
            {
                controller.cusAcsCmp = controller.cusAcsCmp.Concat(new ChaAccessoryComponent[delta]).ToArray();
            }
            else if (delta < 0)
            {
                controller.cusAcsCmp = controller.cusAcsCmp.Take(len).ToArray();
            }

            delta = len - controller.hideHairAcs.Length;
            if (delta > 0)
            {
                controller.hideHairAcs = controller.hideHairAcs.Concat(new bool[delta]).ToArray();
            }
            else if (delta < 0)
            {
                controller.hideHairAcs = controller.hideHairAcs.Take(len).ToArray();
            }

            delta = len - controller.infoAccessory.Length;
            if (delta > 0)
            {
                controller.infoAccessory = controller.infoAccessory.Concat(new ListInfoBase[delta]).ToArray();
            }
            else if (delta < 0)
            {
                controller.infoAccessory = controller.infoAccessory.Take(len).ToArray();
            }

            var movelen = controller.objAcsMove.GetLength(0);
            delta = len - movelen;
            if (delta > 0)
            {
                var newarray = new GameObject[len, 2];
                for (var i = 0; i < movelen; i++)
                {
                    for (var j = 0; j < 2; j++)
                    {
                        newarray[i, j] = controller.objAcsMove[i, j];
                    }
                }
                controller.objAcsMove = newarray;
            }
            else if (delta < 0)
            {
                var newarray = new GameObject[len, 2];
                for (var i = 0; i < len; i++)
                {
                    for (var j = 0; j < 2; j++)
                    {
                        newarray[i, j] = controller.objAcsMove[i, j];
                    }
                }
                controller.objAcsMove = newarray;
            }

            if (CharaMaker)
            {
                for (var i = 0; i < nowlength; i++)
                {
                    var part = parts[i];
                    if (part.type != 120)
                    {
#if KKS
                        controller.infoAccessory[i] = controller.lstCtrl.GetInfo((ChaListDefine.CategoryNo)part.type, part.id);
#else
                        controller.infoAccessory[i] = controller.lstCtrl.GetListInfo((ChaListDefine.CategoryNo)part.type, part.id);
#endif
                        continue;
                    }
                }
                delta = len - nowlength;
                if (delta > 0)
                {
                    var partsarray = new ChaFileAccessory.PartsInfo[delta];
                    for (var i = 0; i < delta; i++) { partsarray[i] = new ChaFileAccessory.PartsInfo(); }
                    controller.nowCoordinate.accessory.parts = controller.nowCoordinate.accessory.parts.Concat(partsarray).ToArray();
                }
                else if (delta < 0)
                {
                    controller.nowCoordinate.accessory.parts = controller.nowCoordinate.accessory.parts.Take(len).ToArray();
                }

#if KK || KKS
                foreach (var item in controller.chaFile.coordinate)
                {
                    delta = len - item.accessory.parts.Length;
                    if (delta > 0)
                    {
                        var array = new ChaFileAccessory.PartsInfo[delta];
                        for (var i = 0; i < delta; i++) { array[i] = new ChaFileAccessory.PartsInfo(); }
                        item.accessory.parts = item.accessory.parts.Concat(array).ToArray();
                    }
                    else if (delta < 0)
                    {
                        item.accessory.parts = item.accessory.parts.Take(len).ToArray();
                    }
                }
#elif EC
                delta = len - controller.chaFile.coordinate.accessory.parts.Length;
                if (delta > 0)
                {
                    var array = new ChaFileAccessory.PartsInfo[delta];
                    for (var i = 0; i < delta; i++) { array[i] = new ChaFileAccessory.PartsInfo(); }
                    controller.chaFile.coordinate.accessory.parts = controller.chaFile.coordinate.accessory.parts.Concat(array).ToArray();
                }
                else if (delta < 0)
                {
                    controller.chaFile.coordinate.accessory.parts = controller.chaFile.coordinate.accessory.parts.Take(len).ToArray();
                }
#endif
                MakerMode.RefreshToggles(len);
            }
            Patches.Common_Patches.Seal(true);
        }

        /// <summary>
        /// Sync the showaccessory array with parts array.
        /// </summary>
        /// <param name="file"></param>
        internal static void ArraySync(ChaFile file)
        {
            Patches.Common_Patches.Seal(false);
            var len = 20;
#if KK || KKS
            len = file.coordinate[file.status.coordinateType].accessory.parts.Length;
#elif EC
            len = file.coordinate.accessory.parts.Length;
#endif
            //Print($"Max size in sync is {len}");
            var delta = len - file.status.showAccessory.Length;
            if (delta > 0)
            {
                var newarray = new bool[delta];
                for (var i = 0; i < delta; i++)
                {
                    newarray[i] = true;
                }
                file.status.showAccessory = file.status.showAccessory.Concat(newarray).ToArray();
            }
            else if (delta < 0)
            {
                file.status.showAccessory = file.status.showAccessory.Take(len).ToArray();
            }

            Patches.Common_Patches.Seal(true);
        }

        internal static void NowCoordinateTrimAndSync(ChaControl controller)
        {
            if (controller == null) return;

            var index = System.Array.FindLastIndex(controller.nowCoordinate.accessory.parts, x => x.type != 120) + 1;
            if (index < 20) index = 20;
            controller.nowCoordinate.accessory.parts = controller.nowCoordinate.accessory.parts.Take(index).ToArray();
            ArraySync(controller);
        }

        /// <summary>
        /// Turns off BackwardsCompatibility save;
        /// </summary>
        public static void TurnOffBackwardsCompatibility()
        {
            BackwardCompatibility = false;
        }
    }
}
