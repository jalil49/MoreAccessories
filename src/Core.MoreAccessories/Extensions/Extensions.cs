using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MoreAccessoriesKOI
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Remenant")]
    [SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Remenant")]
    public static class Extension
    {
        public static ChaFileCoordinate GetCoordinate(this ChaFile self, int index)
        {
#if KK || KKS
            return self.coordinate[index];
#elif EC
            return self.coordinate;
#endif
        }

        public static int GetCoordinateType(this ChaFileStatus self)
        {
#if KK || KKS
            return self.coordinateType;
#elif EC
            return 0;
#endif
        }

        public static List<int> FindAllIndex<T>(this List<T> list, Func<T, bool> match)
        {
            var indexList = new List<int>();
            for (int i = 0, n = list.Count; i < n; i++)
            {
                if (match(list[i]))
                {
                    indexList.Add(i);
                }
            }
            return indexList;
        }

        public static List<int> FindAllIndex<T>(this T[] list, Func<T, bool> match)
        {
            var indexList = new List<int>();
            for (int i = 0, n = list.Length; i < n; i++)
            {
                if (match(list[i]))
                {
                    indexList.Add(i);
                }
            }
            return indexList;
        }

        public static T[] ConcatNearEnd<T>(this T[] array, T second, int subtractIndex = 2)
        {
            var list = array.ToList();
            list.Insert(array.Length - subtractIndex, second);
            return list.ToArray();
        }
    }
}
