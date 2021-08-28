using System;
using UnityEngine;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        #region Events
        /// <summary>
        /// Fires when a new accessory UI slot is created in the maker.
        /// </summary>
        public event Action<int, Transform> onCharaMakerSlotAdded;
        #endregion
    }
}
