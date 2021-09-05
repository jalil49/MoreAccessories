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
    public class MakerMode
    {
        public MakerMode() { Plugin.StartCoroutine(WaitforMakerReady()); }
        internal static MoreAccessories Plugin => MoreAccessories._self;
        public Accessories AccessoriesWindow;
        internal List<CharaMakerSlotData> _additionalCharaMakerSlots = new List<CharaMakerSlotData>();
#if KK || KKS
        public Copy_Window CopyWindow { get; internal set; }
#endif
        internal Transfer_Window TransferWindow { get; set; }

        public void UpdateMakerUI()
        {
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

                return false;
            });
            UpdateMakerUI();
        }

        internal void RefreshToggles(int len)
        {
            TransferWindow.RefreshToggles(len); //CharaMakerSlotData is added here 
#if KK || KKS
            CopyWindow.RefreshToggles(len);
#endif
            UpdateMakerUI();
        }
    }
}
