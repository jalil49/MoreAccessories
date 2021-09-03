using System;
using System.Linq;
using ChaCustom;
#if EC
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
#endif
using Illusion.Extensions;
#if KKS
#endif
using UnityEngine;
using UniRx;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        internal static void ArrayExpansion<T>(ref T[] array, int count = 1)
        {
            if (count < 1) return;
            array = array.Concat(new T[count]).ToArray();
        }

        public static void ArraySync(ChaControl controller)
        {
            var nowcoordinatevalid = controller.nowCoordinate != null;
            var parts = nowcoordinatevalid ? controller.nowCoordinate.accessory.parts : new ChaFileAccessory.PartsInfo[20];
            var len = parts.Length;
            var nowlength = len;
            foreach (var item in controller.chaFile.coordinate)
            {
                len = Math.Max(len, item.accessory.parts.Length);
            }
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
            if (nowcoordinatevalid && delta > 0)
            {
                var newarray = new ListInfoBase[delta];
                for (var i = 0; i < delta; i++)
                {
                    var part = parts[listinfo.Length + i];
                    if (part.type != 120)
                    {
                        newarray[i] = controller.lstCtrl.GetInfo((ChaListDefine.CategoryNo)part.type, part.id);
                        continue;
                    }
                }
                controller.infoAccessory = listinfo.Concat(newarray).ToArray();
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
                delta = len - nowlength;
                if (delta > 0)
                {
                    var partsarray = new ChaFileAccessory.PartsInfo[delta];
                    for (var i = 0; i < delta; i++) { partsarray[i] = new ChaFileAccessory.PartsInfo(); }
                    controller.nowCoordinate.accessory.parts = controller.nowCoordinate.accessory.parts.Concat(partsarray).ToArray();
                }
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
                _self.MakerMode.RefreshToggles(len);
            }
        }

        internal int GetSelectedMakerIndex()
        {
            for (var i = 0; MakerMode.AccessoriesWindow != null && i < MakerMode.AccessoriesWindow._customAcsChangeSlot.items.Length; i++)
            {
                var info = MakerMode.AccessoriesWindow._customAcsChangeSlot.items[i];
                if (info.tglItem.isOn)
                    return i;
            }
            return -1;
        }

        internal ChaFileAccessory.PartsInfo GetPart(int index)
        {
            return CustomBase.Instance.chaCtrl.nowCoordinate.accessory.parts[index];
        }

        internal CvsAccessory GetCvsAccessory(int index)
        {
            return MakerMode.AccessoriesWindow.CvsAccessoryArray[index];
        }
    }
}
