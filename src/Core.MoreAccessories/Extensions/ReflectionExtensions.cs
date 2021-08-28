using System;
using System.Collections.Generic;
using System.Reflection;

namespace MoreAccessoriesKOI.Extensions
{
    internal static class ReflectionExtensions
    {
        private struct MemberKey
        {
            public readonly Type type;
            public readonly string name;
            private readonly int _hashCode;

            public MemberKey(Type inType, string inName)
            {
                type = inType;
                name = inName;
                _hashCode = type.GetHashCode() ^ name.GetHashCode();
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }
        }

        private static readonly Dictionary<MemberKey, FieldInfo> _fieldCache = new Dictionary<MemberKey, FieldInfo>();
        private static readonly Dictionary<MemberKey, PropertyInfo> _propertyCache = new Dictionary<MemberKey, PropertyInfo>();

        public static void SetPrivateExplicit<T>(this T self, string name, object value)
        {
            var key = new MemberKey(typeof(T), name);
            FieldInfo info;
            if (_fieldCache.TryGetValue(key, out info) == false)
            {
                info = key.type.GetField(key.name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                _fieldCache.Add(key, info);
            }
            info.SetValue(self, value);
        }
        public static void SetPrivate(this object self, string name, object value)
        {
            var key = new MemberKey(self.GetType(), name);
            FieldInfo info;
            if (_fieldCache.TryGetValue(key, out info) == false)
            {
                info = key.type.GetField(key.name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                _fieldCache.Add(key, info);
            }
            info.SetValue(self, value);
        }
        public static object GetPrivateExplicit<T>(this T self, string name)
        {
            var key = new MemberKey(typeof(T), name);
            FieldInfo info;
            if (_fieldCache.TryGetValue(key, out info) == false)
            {
                info = key.type.GetField(key.name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                _fieldCache.Add(key, info);
            }
            return info.GetValue(self);
        }
        public static object GetPrivate(this object self, string name)
        {
            var key = new MemberKey(self.GetType(), name);
            FieldInfo info;
            if (_fieldCache.TryGetValue(key, out info) == false)
            {
                info = key.type.GetField(key.name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                _fieldCache.Add(key, info);
            }
            return info.GetValue(self);
        }

        public static void SetPrivateProperty(this object self, string name, object value)
        {
            var key = new MemberKey(self.GetType(), name);
            PropertyInfo info;
            if (_propertyCache.TryGetValue(key, out info) == false)
            {
                info = key.type.GetProperty(key.name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                _propertyCache.Add(key, info);
            }
            info.SetValue(self, value, null);
        }

        public static object GetPrivateProperty(this object self, string name)
        {
            var key = new MemberKey(self.GetType(), name);
            PropertyInfo info;
            if (_propertyCache.TryGetValue(key, out info) == false)
            {
                info = key.type.GetProperty(key.name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                _propertyCache.Add(key, info);
            }
            return info.GetValue(self, null);
        }

        //Static versions
        public static void SetPrivate(this Type self, string name, object value)
        {
            var key = new MemberKey(self, name);
            FieldInfo info;
            if (_fieldCache.TryGetValue(key, out info) == false)
            {
                info = key.type.GetField(key.name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                _fieldCache.Add(key, info);
            }
            info.SetValue(null, value);
        }

        public static object GetPrivate(this Type self, string name)
        {
            var key = new MemberKey(self, name);
            FieldInfo info;
            if (_fieldCache.TryGetValue(key, out info) == false)
            {
                info = key.type.GetField(key.name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                _fieldCache.Add(key, info);
            }
            return info.GetValue(null);
        }

        public static void SetPrivateProperty(this Type self, string name, object value)
        {
            var key = new MemberKey(self, name);
            PropertyInfo info;
            if (_propertyCache.TryGetValue(key, out info) == false)
            {
                info = key.type.GetProperty(key.name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                _propertyCache.Add(key, info);
            }
            info.SetValue(null, value, null);
        }

        public static object GetPrivateProperty(this Type self, string name)
        {
            var key = new MemberKey(self, name);
            PropertyInfo info;
            if (_propertyCache.TryGetValue(key, out info) == false)
            {
                info = key.type.GetProperty(key.name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                _propertyCache.Add(key, info);
            }
            return info.GetValue(null, null);
        }

        public static object CallPrivate(this object self, string name, params object[] p)
        {
            return self.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy).Invoke(self, p);
        }

        public static object CallPrivate(this Type self, string name, params object[] p)
        {
            return self.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).Invoke(null, p);
        }

        public static void LoadWith<T>(this T to, T from)
        {
            var fields = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            foreach (var fi in fields)
            {
                if (fi.FieldType.IsArray)
                {
                    var arr = (Array)fi.GetValue(from);
                    var arr2 = Array.CreateInstance(fi.FieldType.GetElementType(), arr.Length);
                    for (var i = 0; i < arr.Length; i++)
                        arr2.SetValue(arr.GetValue(i), i);
                }
                else
                    fi.SetValue(to, fi.GetValue(from));
            }
        }

        public static MethodInfo GetCoroutineMethod(this Type objectType, string name)
        {
            Type t = null;
            name = "+<" + name + ">";
            foreach (var type in objectType.GetNestedTypes(BindingFlags.NonPublic))
            {
                if (type.FullName.Contains(name))
                {
                    t = type;
                    break;
                }
            }

            if (t != null)
                return t.GetMethod("MoveNext", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return null;
        }

        public static byte[] GetResource(this Assembly a, string resourceName)
        {
            using (var stream = a.GetManifestResourceStream(resourceName))
            {
                var arr = new byte[stream.Length];
                stream.Read(arr, 0, arr.Length);
                return arr;
            }
        }
    }
}