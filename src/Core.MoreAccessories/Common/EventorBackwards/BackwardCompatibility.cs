using System;
using System.Diagnostics.CodeAnalysis;
using ChaCustom;
using HarmonyLib;
using MoreAccessoriesKOI.Extensions;
using UnityEngine;

namespace MoreAccessoriesKOI
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class BackwardCompatibility
    {
        private static object _customHistoryInstance;
        private static Action<ChaControl, Func<bool>> _customHistoryAdd1;
        private static Action<ChaControl, Func<bool, bool>, bool> _customHistoryAdd2;
        private static Action<ChaControl, Func<bool, bool, bool>, bool, bool> _customHistoryAdd3;
        private static Extensions.Action<ChaControl, Func<bool, bool, bool, bool, bool>, bool, bool, bool, bool> _customHistoryAdd5;

        private static void CheckInstance()
        {
            if (_customHistoryInstance == null)
            {
                var t = Type.GetType("ChaCustom.CustomHistory,Assembly-CSharp.dll");
                _customHistoryInstance = t.GetPrivateProperty("Instance");
                _customHistoryAdd1 = (Action<ChaControl, Func<bool>>)Delegate.CreateDelegate(typeof(Action<ChaControl, Func<bool>>), _customHistoryInstance, _customHistoryInstance.GetType().GetMethod("Add1", AccessTools.all));
                _customHistoryAdd2 = (Action<ChaControl, Func<bool, bool>, bool>)Delegate.CreateDelegate(typeof(Action<ChaControl, Func<bool, bool>, bool>), _customHistoryInstance,
                    _customHistoryInstance.GetType().GetMethod("Add2", AccessTools.all));
                _customHistoryAdd3 = (Action<ChaControl, Func<bool, bool, bool>, bool, bool>)Delegate.CreateDelegate(typeof(Action<ChaControl, Func<bool, bool, bool>, bool, bool>), _customHistoryInstance,
                    _customHistoryInstance.GetType().GetMethod("Add3", AccessTools.all));
                _customHistoryAdd5 = (Extensions.Action<ChaControl, Func<bool, bool, bool, bool, bool>, bool, bool, bool, bool>)Delegate.CreateDelegate(
                    typeof(Extensions.Action<ChaControl, Func<bool, bool, bool, bool, bool>, bool, bool, bool, bool>), _customHistoryInstance, _customHistoryInstance.GetType().GetMethod("Add5", AccessTools.all));
            }
        }

        public static void CustomHistory_Instance_Add1(ChaControl instanceChaCtrl, Func<bool> updateAccessoryMoveAllFromInfo)
        {
            CheckInstance();
            _customHistoryAdd1(instanceChaCtrl, updateAccessoryMoveAllFromInfo);
        }

        public static void CustomHistory_Instance_Add2(ChaControl instanceChaCtrl, Func<bool, bool> funcUpdateAcsColor, bool b)
        {
            CheckInstance();
            _customHistoryAdd2(instanceChaCtrl, funcUpdateAcsColor, b);
        }

        internal static void CustomHistory_Instance_Add3(ChaControl instanceChaCtrl, Func<bool, bool, bool> funcUpdateAccessory, bool b, bool b1)
        {
            CheckInstance();
            _customHistoryAdd3(instanceChaCtrl, funcUpdateAccessory, b, b1);
        }

        internal static void CustomHistory_Instance_Add5(ChaControl chaCtrl, Func<bool, bool, bool, bool, bool> reload, bool v1, bool v2, bool v3, bool v4)
        {
            CheckInstance();
            _customHistoryAdd5(chaCtrl, reload, v1, v2, v3, v4);
        }


        internal static void Setup(this CvsColor self, string winTitle, CvsColor.ConnectColorKind kind, Color color, Action<Color> actUpdateColor, Action actUpdateHistory, bool useAlpha)
        {
            var cvsColorSetup = self.GetType().GetMethod("Setup", AccessTools.all);
            cvsColorSetup.Invoke(self, MoreAccessories.HasDarkness ? new object[] { winTitle, kind, color, actUpdateColor, useAlpha } : new object[] { winTitle, kind, color, actUpdateColor, actUpdateHistory, useAlpha });
        }

        internal static Action UpdateAcsColorHistory(this CvsAccessory __instance)
        {
            if (MoreAccessories.HasDarkness)
                return null;
            var methodInfo = __instance.GetType().GetMethod("UpdateAcsColorHistory", AccessTools.all);
            if (methodInfo != null)
                return (Action)Delegate.CreateDelegate(typeof(Action), __instance, methodInfo);
            return null;
        }

        internal static void UpdateAcsMoveHistory(this CvsAccessory self)
        {
            var cvsAccessoryUpdateAcsMoveHistory = self.GetType().GetMethod("UpdateAcsMoveHistory", AccessTools.all);
            cvsAccessoryUpdateAcsMoveHistory.Invoke(self, null);
        }
    }
}