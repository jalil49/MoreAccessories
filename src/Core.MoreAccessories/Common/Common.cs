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
            foreach (var item in controller.chaFile.coordinate)
            {
                len = Math.Max(len, item.accessory.parts.Length);
            }
#endif
            var show = controller.fileStatus.showAccessory;
            var obj = controller.objAccessory;
            var objmove = controller.objAcsMove;
            var cusAcsCmp = controller.cusAcsCmp;
            var hideHairAcs = controller.hideHairAcs;
            var listinfo = controller.infoAccessory;

            var delta = len - show.Length;
            if (delta > 0)
            {
                var newarray = new bool[delta];
                for (var i = 0; i < delta; i++)
                {
                    newarray[i] = true;
                }
                controller.fileStatus.showAccessory = controller.fileStatus.showAccessory.Concat(newarray).ToArray();
            }
            controller.objAccessory = obj.ArrayExpansion(len - obj.Length);
            controller.cusAcsCmp = cusAcsCmp.ArrayExpansion(len - cusAcsCmp.Length);
            controller.hideHairAcs = hideHairAcs.ArrayExpansion(len - hideHairAcs.Length);
            delta = parts.Length - listinfo.Length;
            if (delta > 0)
            {
                controller.infoAccessory = listinfo.Concat(new ListInfoBase[delta]).ToArray();
            }
            var movelen = objmove.GetLength(0);
            delta = len - movelen;
            if (delta > 0)
            {
                var newarray = new GameObject[len, 2];
                for (var i = 0; i < movelen; i++)
                {
                    for (var j = 0; j < 2; j++)
                    {
                        newarray[i, j] = objmove[i, j];
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
    }
}
