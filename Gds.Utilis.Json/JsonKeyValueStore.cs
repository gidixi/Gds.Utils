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
        private readonly ILogger<JsonKeyValueStore> _logger;

        /// <summary>
        /// Inizializza una nuova istanza di <see cref="JsonKeyValueStore"/> con il percorso del file specificato.
        /// </summary>
        /// <param name="filePath">Il percorso del file JSON in cui verranno memorizzati i dati.</param>
        /// <param name="logger">Il logger per registrare le operazioni.</param>
        public JsonKeyValueStore(string filePath, ILogger<JsonKeyValueStore> logger)
        {
            _filePath = filePath;
            _logger = logger;
            Load();
        }

        /// <summary>
        /// Carica i dati dal file JSON. Se il file non esiste, inizializza un nuovo dizionario vuoto.
        /// </summary>
        private void Load()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    _data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
                    _logger.LogInformation("Loaded data from file: {FilePath}", _filePath);
                }
                else
                {
                    _data = new Dictionary<string, object>();
                    _logger.LogInformation("File does not exist. Initialized new data dictionary.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load data from file: {FilePath}", _filePath);
                throw;
            }
        }

        /// <summary>
        /// Salva i dati correnti nel file JSON.
        /// </summary>
        private void Save()
        {
            try
            {
                string json = JsonConvert.SerializeObject(_data, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(_filePath, json);
                _logger.LogInformation("Saved data to file: {FilePath}", _filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save data to file: {FilePath}", _filePath);
                throw;
            }
        }

        /// <summary>
        /// Imposta un valore per una chiave specificata. Se la chiave esiste già, il valore viene aggiornato.
        /// </summary>
        /// <param name="key">La chiave per cui impostare il valore.</param>
        /// <param name="value">Il valore da impostare.</param>
        public void Set(string key, object value)
        {
            try
            {
                if (_data.ContainsKey(key))
                {
                    _data[key] = value;
                    _logger.LogInformation("Updated value for key: {Key}", key);
                }
                else
                {
                    _data.Add(key, value);
                    _logger.LogInformation("Added new key-value pair: {Key} - {Value}", key, value);
                }
                Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set value for key: {Key}", key);
                throw;
            }
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
            try
            {
                if (_data.TryGetValue(key, out object value))
                {
                    _logger.LogInformation("Retrieved value for key: {Key}", key);
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                throw new KeyNotFoundException($"Key '{key}' not found in the JSON store.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get value for key: {Key}", key);
                throw;
            }
        }

        /// <summary>
        /// Verifica se una chiave esiste nell'archivio.
        /// </summary>
        /// <param name="key">La chiave da cercare.</param>
        /// <returns>True se la chiave esiste, altrimenti False.</returns>
        public bool ContainsKey(string key)
        {
            try
            {
                bool exists = _data.ContainsKey(key);
                _logger.LogInformation("Checked existence of key: {Key} - Result: {Exists}", key, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check existence of key: {Key}", key);
                throw;
            }
        }

        /// <summary>
        /// Rimuove una chiave e il suo valore associato dall'archivio.
        /// </summary>
        /// <param name="key">La chiave da rimuovere.</param>
        public void Remove(string key)
        {
            try
            {
                if (_data.ContainsKey(key))
                {
                    _data.Remove(key);
                    Save();
                    _logger.LogInformation("Removed key: {Key}", key);
                }
                else
                {
                    _logger.LogWarning("Key not found for removal: {Key}", key);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove key: {Key}", key);
                throw;
            }
        }

        /// <summary>
        /// Rimuove tutte le chiavi e i valori dall'archivio.
        /// </summary>
        public void Clear()
        {
            try
            {
                _data.Clear();
                Save();
                _logger.LogInformation("Cleared all keys and values from the JSON store.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear all keys and values from the JSON store.");
                throw;
            }
        }

        /// <summary>
        /// Ottiene una copia di tutte le chiavi e i valori nell'archivio.
        /// </summary>
        /// <returns>Un dizionario contenente tutte le chiavi e i valori.</returns>
        public Dictionary<string, object> GetAll()
        {
            try
            {
                _logger.LogInformation("Retrieved all keys and values from the JSON store.");
                return new Dictionary<string, object>(_data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve all keys and values from the JSON store.");
                throw;
            }
        }
    }
}
