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
            var nowcoordinatevalid = controller.nowCoordinate != null;
            var parts = nowcoordinatevalid ? controller.nowCoordinate.accessory.parts : new ChaFileAccessory.PartsInfo[20];
            var len = parts.Length;
            var nowlength = len;
#if KK || KKS
            //Print($"Nowlength is {nowlength}");

            foreach (var item in controller.chaFile.coordinate)
            {
                len = Math.Max(len, item.accessory.parts.Length);
                //Print($"coordinate length is {item.accessory.parts.Length}");
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
                var newarray = new bool[len];
                Array.Copy(controller.fileStatus.showAccessory, newarray, len);
                controller.fileStatus.showAccessory = newarray;
            }


            delta = len - controller.objAccessory.Length;
            if (delta > 0)
            {
                var newarray = new GameObject[delta];
                controller.objAccessory = controller.objAccessory.Concat(newarray).ToArray();
            }
            else if (delta < 0)
            {
                var newarray = new GameObject[len];
                for (var i = len; i < controller.objAccessory.Length; i++)
                {
                    var obj = controller.objAccessory[i];
                    if (obj)
                    {
                        controller.SafeDestroy(obj);
                    }
                }
                Array.Copy(controller.objAccessory, newarray, len);
                controller.objAccessory = newarray;
            }

            delta = len - controller.cusAcsCmp.Length;
            if (delta > 0)
            {
                var newarray = new ChaAccessoryComponent[delta];
                controller.cusAcsCmp = controller.cusAcsCmp.Concat(newarray).ToArray();
            }
            else if (delta < 0)
            {
                var newarray = new ChaAccessoryComponent[len];
                Array.Copy(controller.cusAcsCmp, newarray, len);
                controller.cusAcsCmp = newarray;
            }

            delta = len - controller.hideHairAcs.Length;
            if (delta > 0)
            {
                var newarray = new bool[delta];
                controller.hideHairAcs = controller.hideHairAcs.Concat(newarray).ToArray();
            }
            else if (delta < 0)
            {
                var newarray = new bool[len];
                Array.Copy(controller.hideHairAcs, newarray, len);
                controller.hideHairAcs = newarray;
            }


            delta = len - controller.infoAccessory.Length;
            if (delta > 0)
            {
                controller.infoAccessory = controller.infoAccessory.Concat(new ListInfoBase[delta]).ToArray();
            }
            else if (delta < 0)
            {
                var newarray = new ListInfoBase[len];
                Array.Copy(controller.infoAccessory, newarray, len);
                controller.infoAccessory = newarray;
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
                    var newarray = new ChaFileAccessory.PartsInfo[delta];
                    Array.Copy(controller.nowCoordinate.accessory.parts, newarray, len);
                    controller.nowCoordinate.accessory.parts = newarray;
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
                }
#endif
                MoreAccessories.MakerMode.RefreshToggles(len);
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
                var newarray = new bool[len];
                Array.Copy(file.status.showAccessory, newarray, len);
                file.status.showAccessory = newarray;
            }
        }
    }
}
