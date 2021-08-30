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
    }
}
