using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using YuriNET.CoreServer;
using YuriNET.Dedicated.CoreServer;
using YuriNET.Utils;

namespace YuriNET.Dedicated.Utils {
    public static class ClientCollection {
        private static readonly Logger logger = Logger.getInstance(typeof(ClientCollection));
        private static ConfigDictionary<int, Client> variables = new ConfigDictionary<int, Client>();

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
            ConfigDictionary<int, Client> loadedConfig = null;

            string path = string.Format("{0}\\{1}", Info.getAppFolder(), "clients.xml");
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
                    XmlSerializer xs = new XmlSerializer(typeof(ConfigDictionary<int, Client>));
                    loadedConfig = xs.Deserialize(new StringReader(configXML)) as ConfigDictionary<int, Client>;
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
            string path = string.Format("{0}\\{1}", Info.getAppFolder(), "clients.xml");
            logger.info("Saving configuration : {0}...", path);
            StringBuilder sb = new StringBuilder();

            XmlTextWriter xtw = new XmlTextWriter(new StringWriter(sb)) { Formatting = Formatting.Indented };
            xtw.WriteRaw("");

            logger.debug("Serializing...");
            XmlSerializer xs = new XmlSerializer(typeof(ConfigDictionary<int, Client>));
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

        public static Client Get(int key) {
            Client value;
            if (variables.TryGetValue(key, out value)) {
                logger.debug("Get client : " + key);
                return value;
            } else {
                logger.debug("Get client : " + key + ", not found.");
                return null;
            }
        }
        
        public static void Set(int key, Client value) {
            logger.debug("Set client : " + key);
            variables[key] = value;
        }

        public static void Delete(int key) {
            logger.debug("Deleting client : " + key);
            if (variables.ContainsKey(key)) {
                variables.Remove(key);
                logger.debug("Deleted client " + key);
            }
        }

        public static ConfigDictionary<int, Client> GetCollection() {
            return variables;
        }
    }
}
