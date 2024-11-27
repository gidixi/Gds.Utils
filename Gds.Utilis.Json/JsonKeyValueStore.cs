using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gds.Utilis.Json
{
    public class JsonKeyValueStore
    {
        private readonly string _filePath;
        private Dictionary<string, object> _data;

        public JsonKeyValueStore(string filePath)
        {
            _filePath = filePath;
            Load();
        }

        private void Load()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                _data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
            }
            else
            {
                _data = new Dictionary<string, object>();
            }
        }

        private void Save()
        {
            string json = JsonConvert.SerializeObject(_data, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public void Set(string key, object value)
        {
            if (_data.ContainsKey(key))
            {
                _data[key] = value;
            }
            else
            {
                _data.Add(key, value);
            }
            Save();
        }

        public T Get<T>(string key)
        {
            if (_data.TryGetValue(key, out object value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            throw new KeyNotFoundException($"Key '{key}' not found in the JSON store.");
        }

        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public void Remove(string key)
        {
            if (_data.ContainsKey(key))
            {
                _data.Remove(key);
                Save();
            }
        }

        public void Clear()
        {
            _data.Clear();
            Save();
        }

        public Dictionary<string, object> GetAll()
        {
            return new Dictionary<string, object>(_data);
        }
    }

}
