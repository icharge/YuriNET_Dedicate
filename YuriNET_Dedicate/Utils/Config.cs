using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using YuriNET.Dedicated.CoreServer;
using YuriNET.Utils;

namespace YuriNET.Dedicated.Utils {
    namespace GoldStar.Lib.Helper {
        public static class Config {
            private static readonly Logger logger = Logger.getInstance(typeof (Config));
            private static ConfigDictionary<string, string> variables = new ConfigDictionary<string, string>();

            public static void Init() {
                variables.Clear();
                logger.info("Initializing configuration...");

                // Initialize default fields
                // Encrypt or Decrypt use GEncryption
                // EX : 
                //   - Set -> Config.Set("test_encode", GEncryption.Encrypt("Password"));
                //   - Load -> GEncryption.Decrypt(Config.Get("test_encode"));
                // Return type is string.


                logger.info("Configuration is initialized.");
            }

            public static bool Load() {
                //logger.debug("Decode : " + GEncryption.Decrypt(Get<string>("test_encode")));

                // initial temp config
                ConfigDictionary<string, string> loadedConfig = null;

                string path = string.Format("{0}\\{1}", Info.getAppFolder(), "config.xml");
                logger.info("Loading configuration : {0}...", path);
                string configXML;
                if (File.Exists(path)) {
                    try {
                        configXML = File.ReadAllText(path);
                        if (string.IsNullOrEmpty(configXML)) {
                            logger.warn("Invalid config file.");
                            return false;
                        }
                        logger.debug("Deserializing...");
                        XmlSerializer xs = new XmlSerializer(typeof(ConfigDictionary<string, string>));
                        loadedConfig = xs.Deserialize(new StringReader(configXML)) as ConfigDictionary<string, string>;
                        logger.info("Configuration is loaded.");

                        loadedConfig.ToList().ForEach(kvp => {
                            if (!variables.ContainsKey(kvp.Key))
                                variables.Add(kvp.Key, kvp.Value);
                            else
                                variables[kvp.Key] = kvp.Value;
                        });
                        logger.info("Configuration is merged from file.");
                        return true;
                    } catch (IOException) {
                        return false;
                    }
                } else
                    return Save();
            }

            public static bool Save() {
                // XML Serialize
                string path = string.Format("{0}\\{1}", Info.getAppFolder(), "config.xml");
                logger.info("Saving configuration : {0}...", path);
                StringBuilder sb = new StringBuilder();

                XmlTextWriter xtw = new XmlTextWriter(new StringWriter(sb)) { Formatting = Formatting.Indented };
                xtw.WriteRaw("");

                logger.debug("Serializing...");
                XmlSerializer xs = new XmlSerializer(typeof(ConfigDictionary<string, string>));
                xs.Serialize(xtw, variables);

                //logger.debug("XML : " + sb.ToString());
                try {
                    File.WriteAllText(path, sb.ToString());
                    logger.info("Saved configuration.");
                    return true;
                } catch (IOException) {
                    return false;
                }
            }

            public static string Get(string key) {
                return Get<string>(key);
            }

            public static T Get<T>(string key) where T : IConvertible {
                string value;
                if (variables.TryGetValue(key, out value)) {
                    return (T)Convert.ChangeType(value, typeof(T));
                } else {
                    return default(T);
                }
            }

            public static bool Is(string key) {
                string value;
                if (variables.TryGetValue(key, out value)) {
                    return value.ToLower() == "true" ? true : false;
                } else {
                    return false;
                }
            }

            public static void Set(string key, string value) {
                if (null == value)
                    value = "";
                variables[key] = value;
            }

            public static void Set(string key, bool value) {
                variables[key] = value.ToString();
            }

            public static void Delete(string key) {
                if (variables.ContainsKey(key)) {
                    variables.Remove(key);
                }
            }
        }
    }
}
