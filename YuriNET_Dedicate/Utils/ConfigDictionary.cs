namespace YuriNET.Dedicated.Utils {
    [System.Xml.Serialization.XmlRoot("Config")]
    public class ConfigDictionary<TKey, TValue> : SerializableDictionary<TKey, TValue> {
        protected override string itemName {
            get { return "Item"; }
        }

        protected override string keyName {
            get { return "Key"; }
        }

        protected override string valueName {
            get { return "Value"; }
        }
    }
}
