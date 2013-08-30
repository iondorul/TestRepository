using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;
using System.IO;

namespace avt.ActionForm.Core.Config
{
    public class ItemsFromXmlConfig<T>
        where T : IConfigItem, new()
    {
        List<T> items;
        public List<T> Items { get { return items; } }

        Dictionary<string, T> itemsHash;
        public Dictionary<string, T> ItemsHash { get { return itemsHash; } }

        private ItemsFromXmlConfig()
        {
            items = new List<T>();
            itemsHash = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        }

        public static ItemsFromXmlConfig<T> GetConfig(string configFolder)
        {
            lock (typeof(ItemsFromXmlConfig<T>)) {
                string cacheKey = string.Format("avt.ItemsFromXmlConfig.{0}", configFolder.GetHashCode());
                if (HttpRuntime.Cache[cacheKey] == null) {

                    ItemsFromXmlConfig<T> config = new ItemsFromXmlConfig<T>();

                    // load default first - check Defaut.config first since this extension is protected by IIS
                    if (File.Exists(Path.Combine(configFolder, "Defaults.config"))) {
                        LoadFromFile(Path.Combine(configFolder, "Defaults.config"), ref config.items, ref config.itemsHash);
                    } else {
                        LoadFromFile(Path.Combine(configFolder, "Defaults.xml"), ref config.items, ref config.itemsHash);
                    }

                    foreach (string configFile in Directory.GetFiles(configFolder)) {

                        if (Path.GetFileName(configFile)[0] == '.' || System.IO.Path.GetFileName(configFile) == "Defaults.xml" || System.IO.Path.GetFileName(configFile) == "Defaults.config")
                            continue;

                        LoadFromFile(configFile, ref config.items, ref config.itemsHash);
                    }

                    HttpRuntime.Cache.Add(
                        cacheKey,
                        config,
                        new System.Web.Caching.CacheDependency(new string[] { configFolder }, new string[] { }),
                        DateTime.Now.AddDays(1),
                        System.Web.Caching.Cache.NoSlidingExpiration,
                        System.Web.Caching.CacheItemPriority.Normal,
                        null
                    );

                    return config;
                } else {
                    
                    return (ItemsFromXmlConfig<T>) HttpRuntime.Cache[cacheKey];
                }
            }
        }

        static void LoadFromFile(string configFile, ref List<T> items, ref Dictionary<string, T> itemsHash)
        {
            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.Load(configFile);

            foreach (XmlNode node in xmlConfig.DocumentElement.ChildNodes) {
                if (node is XmlElement && (node.Attributes["sample"] == null || node.Attributes["sample"].Value != "true")) {
                    T item = new T();
                    item.LoadFromXml((XmlElement)node);
                    items.Add(item);
                    itemsHash[item.GetKey()] = item;
                }
            }
        }
    }
}
