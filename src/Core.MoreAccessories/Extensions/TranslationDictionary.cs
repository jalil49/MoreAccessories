using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace MoreAccessoriesKOI
{
    public class TranslationDictionary<T> where T : struct, IConvertible
    {
        private readonly Dictionary<T, string> _strings = new Dictionary<T, string>();
        public TranslationDictionary(string resourceDictionary)
        {
            if (typeof(T).IsEnum == false)
                throw new ArgumentException("T must be an enumerated type");
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceDictionary))
            {
                var doc = new XmlDocument();
                doc.Load(stream);
                var enumType = typeof(T);
                foreach (XmlNode node in doc.FirstChild)
                {
                    try
                    {
                        switch (node.Name)
                        {
                            // Might have different types of nodes in the future
                            case "string":
                                var key = node.Attributes["key"].Value;
                                var value = node.Attributes["value"].Value;
                                var e = (T)Enum.Parse(enumType, key);
                                _strings.Add(e, value);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
#if !KK
                        Debug.LogWarning("Could not parse translation string " + node.OuterXml + " in " + resourceDictionary + "\n" + e);
#endif
                    }
                }
            }
        }

        public string GetString(T key)
        {
            if (_strings.TryGetValue(key, out var res) == false)
            {
                res = "";
#if !KK
                Debug.LogError("Could not find string " + key + " in translation dictionary.");
#endif
            }
            return res;
        }
    }
}
