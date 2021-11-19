using System.Collections;
using System.Collections.Generic;
using ChaCustom;
#if EC
using HPlay;
using ADVPart.Manipulate;
using ADVPart.Manipulate.Chara;
#endif
#if KK || KKS
#endif
using UnityEngine;

namespace MoreAccessoriesKOI
{
    /// <summary>
    /// wait for the following objects before being ready
    /// </summary>
    public class MakerMode
    {
        public MakerMode() { Plugin.StartCoroutine(WaitforMakerReady()); }
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
        internal List<CharaMakerSlotData> _additionalCharaMakerSlots = new List<CharaMakerSlotData>();
        internal bool ready;
        internal static MoreAccessories Plugin => MoreAccessories._self;

        public void UpdateMakerUI()
        {
            if (!ready) return;
            AccessoriesWindow.UpdateUI();
        }

        internal IEnumerator WaitforMakerReady()
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
            ready = true;
            UpdateMakerUI();
        }

        internal IEnumerator RefreshTogglesWaitforMakerReady(int len)
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
            if (!ready)
            {
                Plugin.StartCoroutine(RefreshTogglesWaitforMakerReady(len));
                return;
            }

            TransferWindow.RefreshToggles(len); //CharaMakerSlotData is added here 
#if KK || KKS
            CopyWindow.RefreshToggles(len);
#endif
            UpdateMakerUI();
        }

        internal void ValidatateToggles()
        {
            AccessoriesWindow.ValidatateToggles();
#if KK || KKS
            CopyWindow.ValidatateToggles();
#endif
            TransferWindow.ValidatateToggles();
        }
    }
}
