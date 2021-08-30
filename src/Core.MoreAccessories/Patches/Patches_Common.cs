using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MoreAccessoriesKOI
{
    public partial class MoreAccessories
    {
        private static MethodBase GetMethod(Type type, string method, Type[] Param = null)
        {
            if (Param == null)
                return type.GetMethod(method, AccessTools.all);
            return type.GetMethod(method, Param);
        }
    }
}
