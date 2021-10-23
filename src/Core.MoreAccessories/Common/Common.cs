using System;
using System.Linq;
#if EC
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
#endif
using Illusion.Extensions;
using UnityEngine;
using UniRx;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        public static void ArraySync(ChaControl controller)
        {
            //Print($"Syncng {controller.chaFile.parameter.fullname}");
            if (controller.nowCoordinate == null)
            {
                return;
            }
            var parts = controller.nowCoordinate.accessory.parts;
            var len = parts.Length;
            var nowlength = len;
#if KK || KKS
            //Print($"Nowlength is {nowlength}");
            if (CharaMaker)
            {
                foreach (var item in controller.chaFile.coordinate)
                {
                    len = Math.Max(len, item.accessory.parts.Length);
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
                var newarray = new GameObject[delta];
                controller.objAccessory = controller.objAccessory.Concat(newarray).ToArray();
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
                var newarray = new ChaAccessoryComponent[delta];
                controller.cusAcsCmp = controller.cusAcsCmp.Concat(newarray).ToArray();
            }
            else if (delta < 0)
            {
                controller.cusAcsCmp = controller.cusAcsCmp.Take(len).ToArray();
            }

            delta = len - controller.hideHairAcs.Length;
            if (delta > 0)
            {
                var newarray = new bool[delta];
                controller.hideHairAcs = controller.hideHairAcs.Concat(newarray).ToArray();
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
        }
        internal static void ArraySync(ChaFile file)
        {
            var len = 20;
#if KK || KKS
            //Print($"Nowlength is {nowlength}");
            foreach (var item in file.coordinate)
            {
                len = Math.Max(len, item.accessory.parts.Length);
                //Print($"coordinate length is {item.accessory.parts.Length}");
            }
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
        }

        public static void TurnOffBackwardsCompatibility()
        {
            BackwardCompatibility = false;
        }
    }
}
