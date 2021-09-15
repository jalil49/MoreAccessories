using System;
using System.Collections.Generic;
using System.Linq;

namespace MoreAccessoriesKOI
{
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
            var indexlist = new List<int>();
            for (int i = 0, n = list.Count; i < n; i++)
            {
                if (match(list[i]))
                {
                    indexlist.Add(i);
                }
            }
            return indexlist;
        }

        public static List<int> FindAllIndex<T>(this T[] list, Func<T, bool> match)
        {
            var indexlist = new List<int>();
            for (int i = 0, n = list.Length; i < n; i++)
            {
                if (match(list[i]))
                {
                    indexlist.Add(i);
                }
            }
            return indexlist;
        }

        public static T[] ConcatNearEnd<T>(this T[] array, T second, int subtractindex = 2)
        {
            var list = array.ToList();
            list.Insert(array.Length - subtractindex, second);
            return list.ToArray();
        }
    }
}
