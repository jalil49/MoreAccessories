﻿using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MoreAccessoriesKOI.Extensions
{
    internal static class TransformExtensions
    {
        public static string GetPathFrom(this Transform self, Transform root, bool includeRoot = false)
        {
            if (self == root)
                return "";
            var self2 = self;
            var path = new StringBuilder(self2.name);
            self2 = self2.parent;
            while (self2 != root)
            {
                path.Insert(0, "/");
                path.Insert(0, self2.name);
                self2 = self2.parent;
            }
            if (self2 != null && includeRoot)
            {
                path.Insert(0, "/");
                path.Insert(0, root.name);
            }
            return path.ToString();
        }

        public static bool IsChildOf(this Transform self, string parent)
        {
            while (self != null)
            {
                if (self.name.Equals(parent))
                    return true;
                self = self.parent;
            }
            return false;
        }

        public static string GetPathFrom(this Transform self, string root, bool includeRoot = false)
        {
            if (self.name.Equals(root))
                return "";
            var self2 = self;
            var path = new StringBuilder(self2.name);
            self2 = self2.parent;
            while (self2 != null && self2.name.Equals(root) == false)
            {
                path.Insert(0, "/");
                path.Insert(0, self2.name);
                self2 = self2.parent;
            }
            if (self2 != null && includeRoot)
            {
                path.Insert(0, "/");
                path.Insert(0, root);
            }
            return path.ToString();
        }

        public static List<int> GetListPathFrom(this Transform self, Transform root)
        {
            var path = new List<int>();
            var self2 = self;
            while (self2 != root)
            {
                path.Add(self2.GetSiblingIndex());
                self2 = self2.parent;
            }
            path.Reverse();
            return path;
        }

        public static Transform Find(this Transform self, List<int> path)
        {
            var self2 = self;
            for (var i = 0; i < path.Count; i++)
                self2 = self2.GetChild(path[i]);
            return self2;
        }

        public static Transform FindDescendant(this Transform self, string name)
        {
            if (self.name.Equals(name))
                return self;
            foreach (Transform t in self)
            {
                var res = t.FindDescendant(name);
                if (res != null)
                    return res;
            }
            return null;
        }

        public static Transform GetFirstLeaf(this Transform self)
        {
            while (self.childCount != 0)
                self = self.GetChild(0);
            return self;
        }

        public static Transform GetDeepestLeaf(this Transform self)
        {
            var d = -1;
            Transform res = null;
            foreach (Transform transform in self)
            {

                var resT = GetDeepestLeaf(transform, 0, out var resD);
                if (resD > d)
                {
                    d = resD;
                    res = resT;
                }
            }
            return res;
        }

        private static Transform GetDeepestLeaf(Transform t, int depth, out int resultDepth)
        {
            if (t.childCount == 0)
            {
                resultDepth = depth;
                return t;
            }
            Transform res = null;
            var d = 0;
            foreach (Transform child in t)
            {
                var resT = GetDeepestLeaf(child, depth + 1, out var resD);
                if (resD > d)
                {
                    d = resD;
                    res = resT;
                }
            }
            resultDepth = d;
            return res;
        }

#if !AI && !HS2
        public static IEnumerable<Transform> Children(this Transform self)
        {
            foreach (Transform t in self)
                yield return t;
        }
#endif
    }
}