using ChaCustom;
using System;
using System.Linq;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        [Obsolete("Data is now stored directly on Chacontrol.cusAcsCmp")]
        public ChaAccessoryComponent GetChaAccessoryComponent(ChaControl character, int index)
        {
            return character.cusAcsCmp[index];
        }

        [Obsolete("Data is now stored directly on Chacontrol.cusAcsCmp")]
        public int GetChaAccessoryComponentIndex(ChaControl character, ChaAccessoryComponent component)
        {
            return character.cusAcsCmp.ToList().IndexOf(component); ;
        }

        [Obsolete("Data is now stored directly on ChaFileAccessory.Parts")]
        internal int GetPartsLength()
        {
            return CustomBase.instance.chaCtrl.nowCoordinate.accessory.parts.Length;
        }

        [Obsolete("No Purpose")]
        public int GetCvsAccessoryCount()
        {
            if (CharaMaker && (MakerMode == null || MakerMode.AccessoriesWindow == null)) return 20;

            if (CharaMaker) return MakerMode.AccessoriesWindow.AdditionalCharaMakerSlots.Count + 20;
            return 0;
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
