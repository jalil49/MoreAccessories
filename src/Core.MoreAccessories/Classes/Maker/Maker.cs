using System.Collections;
using System.Collections.Generic;
using ChaCustom;
using UnityEngine;
#if EC
#endif
#if KK || KKS
#endif

namespace MoreAccessoriesKOI
{
    /// <summary>
    /// wait for the following objects before being ready
    /// </summary>
    public class MakerMode
    {
        public MakerMode() { Plugin.StartCoroutine(WaitForMakerReady()); }
        public Accessories AccessoriesWindow;
#if KK || KKS
        public Copy_Window CopyWindow { get; internal set; }
#endif
        public Transfer_Window TransferWindow { get; set; }

        /// <summary>
        /// Accessible through their own arrays
        /// AccessorySlot:= CustomAcsChangeSlot.Items
        /// copySlotObject:= CvsAccessoryChange.tglSrcKind, CvsAccessoryChange.tglDstKind, CvsAccessoryChange.textSrc, CvsAccessoryChange.textDst,
        /// transferSlotObject:= CustomAcsChangeSlot.Items
        /// </summary>
        internal List<CharaMakerSlotData> AdditionalCharaMakerSlots = new List<CharaMakerSlotData>();
        internal bool Ready;
        internal static MoreAccessories Plugin => MoreAccessories._self;

        public void UpdateMakerUI()
        {
            if (!Ready) return;
            AccessoriesWindow.UpdateUI();
        }

        internal IEnumerator WaitForMakerReady()
        {
            yield return new WaitWhile(() =>
            {
                if (TransferWindow == null || AccessoriesWindow == null) return true;
#if KK || KKS
                if (CopyWindow == null) return true;
#endif
                if (CustomBase.Instance.chaCtrl == null) return true;

                if (!AccessoriesWindow.WindowMoved) return true;

                return false;
            });
            Ready = true;
            UpdateMakerUI();
        }

        internal IEnumerator RefreshTogglesWaitForMakerReady(int len)
        {
            yield return new WaitWhile(() =>
            {
                if (TransferWindow == null || AccessoriesWindow == null) return true;

#if KK || KKS
                if (CopyWindow == null) return true;
#endif
                if (CustomBase.Instance.chaCtrl == null) return true;

                return false;
            });
            TransferWindow.RefreshToggles(len); //CharaMakerSlotData is added here 
#if KK || KKS
            CopyWindow.RefreshToggles(len);
#endif
            UpdateMakerUI();
        }


        internal void RefreshToggles(int len)
        {
            if (!Ready)
            {
                Plugin.StartCoroutine(RefreshTogglesWaitForMakerReady(len));
                return;
            }

            TransferWindow.RefreshToggles(len); //CharaMakerSlotData is added here 
#if KK || KKS
            CopyWindow.RefreshToggles(len);
#endif
            UpdateMakerUI();
        }

        internal void ValidateToggles()
        {
            AccessoriesWindow.ValidateToggles();
#if KK || KKS
            CopyWindow.ValidateToggles();
#endif
            TransferWindow.ValidateToggles();
        }
    }
}
