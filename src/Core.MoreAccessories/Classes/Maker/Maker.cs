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
        public MakerMode()
        {
            CustomBase.Instance.selectSlot = 0;
        }

        public Accessories AccessoriesWindow;
        public List<UI_RaycastCtrl> RaycastCtrls = new List<UI_RaycastCtrl>();
        internal List<CharaMakerSlotData> _additionalCharaMakerSlots = new List<CharaMakerSlotData>();

        public Copy_Window CopyWindow { get; internal set; }
        internal Transfer_Window TransferWindow { get; set; }

        public void UpdateMakerUI()
        {
            AccessoriesWindow.UpdateUI();
        }

        internal void RefreshToggles(int len)
        {
            TransferWindow.RefreshToggles(len); //CharaMakerSlotData is added here 
            CopyWindow.RefreshToggles(len);
        }
    }

    public class CharaMakerSlotData
    {
        public GameObject AccessorySlot;
#if KK || KKS
        public GameObject copySlotObject;
#endif
        public GameObject transferSlotObject;
    }
}
