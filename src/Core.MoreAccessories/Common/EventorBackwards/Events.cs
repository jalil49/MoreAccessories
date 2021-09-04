using System;
using UnityEngine;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        /// <summary>
        /// Fires when a new accessory UI slot is created in the maker.
        /// </summary>
#pragma warning disable IDE1006 // Naming Styles
        public event Action<int, Transform> onCharaMakerSlotAdded;
#pragma warning restore IDE1006 // Naming Styles

        internal void NewSlotAdded(int index, Transform slot)
        {
            onCharaMakerSlotAdded?.Invoke(index, slot);
        }
    }
}
