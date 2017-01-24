using System.Linq;
using System.Collections.Generic;
using AirSuperiority.Core.IO;

namespace AirSuperiority.ScriptBase
{
    public static class Resources
    {
        public static readonly string BaseDirectory = @"scripts\AirSuperiority\";

        private static readonly string baseMetadataPath = BaseDirectory + "assets.xml";

        private static Dictionary<string, IEnumerable<XMLAttributesCollection>> assets;

        static Resources()
        {
            assets = new Dictionary<string, IEnumerable<XMLAttributesCollection>>();
        }

        /// <summary>
        /// Gets an asset by name. Instaniate it locally if it doesn't already exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetByName<T>(string sectionName) where T : XMLSimpleMetadata, new()
        {
            IEnumerable<XMLAttributesCollection> xml;

            if (!assets.TryGetValue(sectionName, out xml))
            {
                xml = XMLSimpleParser.GetNestedAttributes(baseMetadataPath, sectionName);
                assets.Add(sectionName, xml);
            }

            foreach (XMLAttributesCollection collection in xml)
            {
                T metadata = new T();

                metadata.ParseAttributes(collection);

                yield return metadata;
            }
        }

        /// <summary>
        /// Reload all known local assets.
        /// </summary>
        public static void ReloadAssets()
        {
            foreach (string key in assets.Keys.ToList())
            {
                assets[key] = XMLSimpleParser.GetNestedAttributes(baseMetadataPath, key);
            }
        }
    }
}
