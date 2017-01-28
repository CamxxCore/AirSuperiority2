using System.Linq;
using System.Collections.Generic;
using AirSuperiority.Core.IO;

namespace AirSuperiority.ScriptBase
{
    public static class Resources
    {
        public static readonly string BaseDirectory = @"scripts\AirSuperiority\";

        private static readonly string metdataPath = BaseDirectory + "assets.xml";

        private static Dictionary<string, IEnumerable<XMLAttributesCollection>> metadata;

        static Resources()
        {
            metadata = new Dictionary<string, IEnumerable<XMLAttributesCollection>>();
        }

        /// <summary>
        /// Get an metadata asset by name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetMetaEntry<T>(string sectionName) where T : XMLSimpleMetadata, new()
        {
            IEnumerable<XMLAttributesCollection> xml;

            if (!metadata.TryGetValue(sectionName, out xml))
            {
                xml = XMLSimpleParser.GetNestedAttributes(metdataPath, sectionName);

                metadata.Add(sectionName, xml);
            }

            foreach (XMLAttributesCollection collection in xml)
            {
                T entry = new T();

                entry.ParseAttributes(collection);

                yield return entry;
            }
        }

        /// <summary>
        /// Reload all known metadata assets.
        /// </summary>
        public static void ReloadMetaAssets()
        {
            foreach (string key in metadata.Keys.ToList())
            {
                metadata[key] = XMLSimpleParser.GetNestedAttributes(metdataPath, key);
            }
        }
    }
}
