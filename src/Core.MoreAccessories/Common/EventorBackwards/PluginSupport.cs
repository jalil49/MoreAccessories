﻿using System;
using System.Linq;
using ChaCustom;
using JetBrains.Annotations;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        [PublicAPI]
        [Obsolete("Data is now stored directly on Chacontrol.cusAcsCmp")]
        public ChaAccessoryComponent GetChaAccessoryComponent(ChaControl character, int index)
        {
            if (index >= character.cusAcsCmp.Length) return null;
            return character.cusAcsCmp[index];
        }

        [PublicAPI]
        [Obsolete("Data is now stored directly on Chacontrol.cusAcsCmp Array.IndexOf(Chacontrol.cusAcsCmp, component)")]
        public int GetChaAccessoryComponentIndex(ChaControl character, ChaAccessoryComponent component)
        {
            return character.cusAcsCmp.ToList().IndexOf(component);
        }

        [PublicAPI]
        [Obsolete("Data is now stored directly on ChaFileAccessory.Parts")]
        internal int GetPartsLength()
        {
            return CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length;
        }

        [PublicAPI]
        [Obsolete("No Purpose")]
        public int GetCvsAccessoryCount()
        {
            if (CharaMaker && MakerMode?.AccessoriesWindow == null) return 20;

            if (CharaMaker) return MakerMode.AccessoriesWindow.AdditionalCharaMakerSlots.Count + 20;
            return 0;
        }

        [PublicAPI]
        internal int GetSelectedMakerIndex()
        {
            for (var i = 0; MakerMode.AccessoriesWindow != null && i < MakerMode.AccessoriesWindow.AccessoryTab.items.Length; i++)
            {
                var info = MakerMode.AccessoriesWindow.AccessoryTab.items[i];
                if (info.tglItem.isOn)
                    return i;
            }

            return -1;
        }

        [PublicAPI]
        internal ChaFileAccessory.PartsInfo GetPart(int index)
        {
            return CustomBase.Instance.chaCtrl.nowCoordinate.accessory.parts[index];
        }

        public CvsAccessory GetCvsAccessory(int index)
        {
            return MakerMode.AccessoriesWindow.CvsAccessoryArray[index];
        }
    }
}