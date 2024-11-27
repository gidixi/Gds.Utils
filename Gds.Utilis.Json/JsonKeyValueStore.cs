using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Gds.Utilis.Json
{
    /// <summary>
    /// Classe che implementa un archivio chiave-valore persistente utilizzando un file JSON.
    /// </summary>
    public class JsonKeyValueStore
    {
        private readonly string _filePath;
        private Dictionary<string, object> _data;

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="JsonKeyValueStore"/> con il percorso del file specificato.
        /// </summary>
        /// <param name="filePath">Il percorso del file JSON in cui verranno memorizzati i dati.</param>
        public JsonKeyValueStore(string filePath)
        {
            _filePath = filePath;
            Load();
        }

        /// <summary>
        /// Carica i dati dal file JSON. Se il file non esiste, inizializza un nuovo dizionario vuoto.
        /// </summary>
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

        /// <summary>
        /// Salva i dati correnti nel file JSON.
        /// </summary>
        private void Save()
        {
            string json = JsonConvert.SerializeObject(_data, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        /// <summary>
        /// Imposta un valore per una chiave specificata. Se la chiave esiste già, il valore viene aggiornato.
        /// </summary>
        /// <param name="key">La chiave per cui impostare il valore.</param>
        /// <param name="value">Il valore da impostare.</param>
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

        /// <summary>
        /// Ottiene il valore associato alla chiave specificata.
        /// </summary>
        /// <typeparam name="T">Il tipo di valore da restituire.</typeparam>
        /// <param name="key">La chiave per cui ottenere il valore.</param>
        /// <returns>Il valore associato alla chiave, convertito nel tipo specificato.</returns>
        /// <exception cref="KeyNotFoundException">Generata se la chiave non esiste nell'archivio.</exception>
        public T Get<T>(string key)
        {
            if (_data.TryGetValue(key, out object value))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            throw new KeyNotFoundException($"Key '{key}' not found in the JSON store.");
        }

        /// <summary>
        /// Verifica se una chiave esiste nell'archivio.
        /// </summary>
        /// <param name="key">La chiave da cercare.</param>
        /// <returns>True se la chiave esiste, altrimenti False.</returns>
        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        /// <summary>
        /// Rimuove una chiave e il suo valore associato dall'archivio.
        /// </summary>
        /// <param name="key">La chiave da rimuovere.</param>
        public void Remove(string key)
        {
            if (_data.ContainsKey(key))
            {
                _data.Remove(key);
                Save();
            }
        }

        /// <summary>
        /// Rimuove tutte le chiavi e i valori dall'archivio.
        /// </summary>
        public void Clear()
        {
            _data.Clear();
            Save();
        }

        /// <summary>
        /// Ottiene una copia di tutte le chiavi e i valori nell'archivio.
        /// </summary>
        /// <returns>Un dizionario contenente tutte le chiavi e i valori.</returns>
        public Dictionary<string, object> GetAll()
        {
            return new Dictionary<string, object>(_data);
        }
    }
}
